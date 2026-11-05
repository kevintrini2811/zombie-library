using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppReloaded.Input;
using Il2CppSource.Binders;
using Il2CppTMPro;
using ReplantedModUi;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ZombieLibrary;

internal static class LibraryAlmanac
{
    private static readonly Dictionary<int, View> views = new();
    internal static void Attach(AlmanacZombieBinder binder)
    {
        var root = binder.transform;
        while (root && !root.name.StartsWith("P_Almanac_Zombies", StringComparison.Ordinal)) root = root.parent;
        if (!root || views.ContainsKey(root.GetInstanceID())) return;
        View? view = null;
        try { view = new View(root); view.Build(); views.Add(root.GetInstanceID(), view); }
        catch (Exception error) { view?.Dispose(); LibraryMod.Instance.LoggerInstance.Error($"Zombie almanac: {error}"); }
    }
    internal static void Update()
    {
        foreach (var (id, view) in views.ToArray())
        {
            if (!view.Root) { view.Dispose(); views.Remove(id); }
            else if (!view.Root.gameObject.activeInHierarchy) view.Close(false);
            else view.Update();
        }
    }
    internal static void Dispose() { foreach (var view in views.Values) view.Dispose(); views.Clear(); }

    // One owner coordinates all tabs. Native content is restored once when
    // switching back, closing the panel or losing the scene.
    private sealed class View : IDisposable
    {
        private const int Columns = 5, PageSize = 25;
        private sealed record Entry(ZombieDefinition Definition, bool Duck);
        internal readonly Transform Root;
        private readonly List<GameObject> owned = new();
        private readonly List<(GameObject Object, bool Active)> hidden = new();
        private readonly List<(ModCatalog Mod, Button Tab)> tabs = new();
        private readonly List<(Button Button, Image Background, Image Portrait, Image Border)> cards = new();
        private readonly List<Image> cardDucks = new();
        private readonly List<Sprite> artwork = new();
        private Transform panel = null!;
        private GameObject staging = null!, page = null!;
        private Button previous = null!, next = null!, back = null!, close = null!;
        private TMP_Text title = null!, description = null!, header = null!, count = null!;
        private Image portrait = null!, duckPortrait = null!;
        private ScrollRect scroll = null!;
        private BasicNavigationLayer? navigation;
        private Selectable? originalTarget;
        private Navigation originalBack = null!, originalClose = null!;
        private Entry[] entries = Array.Empty<Entry>();
        private ModCatalog? current;
        private int pageIndex, selected;
        private bool german;
        private Sprite? duck;
        private Sprite cardBackground = null!, cardBorder = null!;
        internal View(Transform root) { Root = root; german = NativeLocalization.German; }
        private GameObject Own(GameObject value) { owned.Add(value); return value; }
        private static Transform Required(Transform parent, string path) => parent.Find(path)
            ?? throw new InvalidDataException($"Missing native zombie almanac element: {path}");
        private static RectTransform Rect(Transform transform) => transform.Cast<RectTransform>();
        private static void Place(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size, Vector2 pivot)
        { rect.anchorMin = rect.anchorMax = anchor; rect.anchoredPosition = position; rect.sizeDelta = size; rect.pivot = pivot; }
        private static void Stretch(RectTransform rect)
        { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        private GameObject Clone(GameObject source, Transform parent, string name) =>
            Own(NativeUi.Clone(source, staging.transform, parent, name));
        private Sprite LoadArtwork(string name)
        {
            using var stream = typeof(LibraryAlmanac).Assembly.GetManifestResourceStream("ZombieLibrary.Assets." + name)
                ?? throw new InvalidDataException("Missing zombie almanac artwork: " + name);
            using var bytes = new MemoryStream(); stream.CopyTo(bytes);
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
            if (!ImageConversion.LoadImage(texture, bytes.ToArray(), true))
            { Object.Destroy(texture); throw new InvalidDataException("Invalid zombie almanac artwork: " + name); }
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100);
            sprite.hideFlags = HideFlags.HideAndDontSave; artwork.Add(sprite); return sprite;
        }
        private TMP_Text Label(GameObject source, Transform parent, string name)
        {
            var clone = Clone(source, parent, name);
            var label = clone.GetComponent<TMP_Text>(); label.raycastTarget = false; clone.SetActive(true); return label;
        }
        private Button Button(GameObject source, Transform parent, string name, Action action)
        {
            var clone = Clone(source, parent, name);
            var button = clone.GetComponent<Button>(); button.enabled = button.interactable = true;
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener((UnityAction)(() => {
                try { action(); }
                catch (Exception error) { Close(true); LibraryMod.Instance.LoggerInstance.Error($"Zombie almanac: {error}"); }
            }));
            if (button.targetGraphic != null) button.targetGraphic.raycastTarget = true;
            clone.SetActive(true); return button;
        }
        private Button TextButton(GameObject source, Transform parent, string name, string text, Action action)
        {
            var button = Button(source, parent, name, action);
            foreach (var fitter in button.GetComponentsInChildren<ContentSizeFitter>(true)) fitter.enabled = false;
            var layout = Required(button.transform, "Layout");
            foreach (var group in layout.GetComponents<LayoutGroup>()) group.enabled = false;
            Stretch(Rect(layout));
            var background = Required(layout, "Background").GetComponent<Image>();
            Stretch(background.rectTransform); background.enabled = background.raycastTarget = true;
            background.gameObject.SetActive(true); button.targetGraphic = background;
            var label = Required(layout, "ButtonText").GetComponent<TMP_Text>(); Stretch(label.rectTransform);
            label.enableAutoSizing = true; label.fontSizeMin = 26; label.fontSizeMax = 50;
            label.alignment = TextAlignmentOptions.Center; label.text = text; label.raycastTarget = false;
            return button;
        }
        internal void Build()
        {
            panel = Required(Root, "Canvas/Layout/Center/Panel");
            back = Required(panel, "P_BasicButton03-Back").GetComponent<Button>();
            close = Required(panel, "P_BasicButton03-Close").GetComponent<Button>();
            originalBack = back.navigation; originalClose = close.navigation;
            navigation = Root.GetComponent<BasicNavigationLayer>();
            if (navigation != null) originalTarget = navigation.m_targetSelectable;
            staging = Own(new GameObject("ZombieLibraryStaging")); staging.SetActive(false); staging.transform.SetParent(panel, false);
            page = Own(new GameObject("ZombieLibraryPage")); page.SetActive(false);
            var pageRect = page.AddComponent<RectTransform>(); pageRect.SetParent(panel, false); Stretch(pageRect);
            var nativeGrid = Required(panel, "ItemGrid");
            var nativeCard = Required(nativeGrid, "P_Almanac_ZombieItem");
            var nativeDetail = Required(panel, "SelectedItem");
            var name = Required(nativeDetail, "SelectedItemName").gameObject;
            var grid = Own(new GameObject("ZombieLibraryGrid")).AddComponent<RectTransform>(); grid.SetParent(pageRect, false);
            var sourceRect = Rect(nativeGrid);
            Place(grid, sourceRect.anchorMin, sourceRect.anchoredPosition, sourceRect.sizeDelta, sourceRect.pivot);
            var layout = grid.gameObject.AddComponent<GridLayoutGroup>();
            var nativeLayout = nativeGrid.GetComponent<GridLayoutGroup>();
            layout.cellSize = nativeLayout.cellSize; layout.spacing = nativeLayout.spacing;
            layout.padding = nativeLayout.padding; layout.childAlignment = nativeLayout.childAlignment;
            layout.startCorner = nativeLayout.startCorner; layout.startAxis = nativeLayout.startAxis;
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount; layout.constraintCount = Columns;
            cardBackground = LoadArtwork("zombie-card-background.png");
            cardBorder = LoadArtwork("zombie-card-border.png");
            duck = LoadArtwork("duck.png");
            for (int i = 0; i < PageSize; i++)
            {
                int slot = i;
                var button = Button(nativeCard.gameObject, grid, $"ZombieLibraryCard{i}", () => Select(pageIndex * PageSize + slot));
                Required(button.transform, "Locked").gameObject.SetActive(false);
                var background = Required(button.transform, "Background").GetComponent<Image>();
                background.sprite = cardBackground; background.color = Color.white; background.raycastTarget = true;
                var border = Required(button.transform, "Border").GetComponent<Image>();
                border.sprite = cardBorder; border.color = Color.white; border.raycastTarget = false;
                border.gameObject.SetActive(true);
                var over = Required(button.transform, "OverState").GetComponent<Image>();
                over.gameObject.SetActive(false);
                // The prefab targets the thumbnail: its native ColorBlock dims
                // idle cards and brightens hover/selection without covering art.
                button.targetGraphic = background; button.transition = Selectable.Transition.ColorTint;
                background.CrossFadeColor(button.colors.normalColor * button.colors.colorMultiplier, 0, true, true);
                var image = Required(button.transform, "Portrait").GetComponent<Image>(); image.preserveAspect = true;
                image.gameObject.SetActive(true); image.raycastTarget = false;
                var cardDuckRect = Own(new GameObject("NativeDuckPortrait")).AddComponent<RectTransform>();
                cardDuckRect.SetParent(image.transform, false);
                Place(cardDuckRect, new Vector2(.5f, .3f), Vector2.zero, new Vector2(145, 78), new Vector2(.5f, .5f));
                var duckImage = cardDuckRect.gameObject.AddComponent<Image>();
                duckImage.preserveAspect = true; duckImage.raycastTarget = false; cardDucks.Add(duckImage);
                cards.Add((button, background, image, border));
            }
            var detail = Clone(nativeDetail.gameObject, pageRect, "ZombieLibraryDetail").transform;
            Required(detail, "SelectedItemRenderPortrait").gameObject.SetActive(false);
            portrait = Required(detail, "SelectedItemPortrait").GetComponent<Image>(); portrait.preserveAspect = true;
            Place(portrait.rectTransform, new Vector2(.5f, 1), new Vector2(0, -75), new Vector2(620, 620), new Vector2(.5f, 1));
            portrait.gameObject.SetActive(true);
            var duckRect = Own(new GameObject("NativeDuckPortrait")).AddComponent<RectTransform>();
            duckRect.SetParent(portrait.transform, false);
            Place(duckRect, new Vector2(.5f, .30f), Vector2.zero,
                new Vector2(portrait.rectTransform.rect.width * .7f, portrait.rectTransform.rect.height * .3f), new Vector2(.5f, .5f));
            duckPortrait = duckRect.gameObject.AddComponent<Image>(); duckPortrait.preserveAspect = true; duckPortrait.raycastTarget = false;
            title = Required(detail, "SelectedItemName").GetComponent<TMP_Text>();
            scroll = Required(detail, "Scroll View").GetComponent<ScrollRect>(); scroll.enabled = true;
            scroll.horizontal = false; scroll.vertical = true;
            var gamepadScroll = scroll.GetComponent<GamepadScroll>(); if (gamepadScroll != null) gamepadScroll.enabled = true;
            var content = Required(scroll.transform, "Viewport/Content");
            description = Required(content, "SelectedItemInfoLabel").GetComponent<TMP_Text>();
            description.enableAutoSizing = false; description.fontSize = description.fontSizeMax;
            description.alignment = TextAlignmentOptions.TopLeft; description.raycastTarget = false;
            scroll.content = Rect(content); detail.gameObject.SetActive(true);
            header = Label(Required(panel, "HeaderText").gameObject, pageRect, "ZombieLibraryHeader");
            count = Label(name, pageRect, "ZombieLibraryPageCount");
            Place(count.rectTransform, Vector2.zero, new Vector2(1000, 280), new Vector2(690, 94), Vector2.zero);
            previous = TextButton(back.gameObject, pageRect, "PreviousZombies", "", () => ShowPage(pageIndex - 1));
            next = TextButton(back.gameObject, pageRect, "NextZombies", "", () => ShowPage(pageIndex + 1));
            Place(Rect(previous.transform), Vector2.zero, new Vector2(550, 280), new Vector2(430, 94), Vector2.zero);
            Place(Rect(next.transform), Vector2.zero, new Vector2(1710, 280), new Vector2(430, 94), Vector2.zero);
            foreach (string path in new[] { "ItemGrid", "SelectedItem", "P_ControllerPrompt_Scroll", "HeaderText" })
            { var obj = Required(panel, path).gameObject; hidden.Add((obj, obj.activeSelf)); }
            AddTabs();
        }
        private void AddTabs()
        {
            foreach (var mod in Zombies.Catalogs.Where(mod => !tabs.Any(item => item.Mod == mod)))
            {
                var tab = TextButton(back.gameObject, panel, "ZombieLibraryTab_" + mod.Id, mod.Name, () => Open(mod));
                tabs.Add((mod, tab));
            }
            // Wrap rather than letting further mods put tabs outside the panel.
            var rect = Rect(back.transform);
            for (int i = 0; i < tabs.Count; i++)
                Place(Rect(tabs[i].Tab.transform), rect.anchorMin,
                    rect.anchoredPosition + new Vector2(rect.rect.width + 40 + i % 4 * 440, i / 4 * 100),
                    new Vector2(420, rect.sizeDelta.y), rect.pivot);
            LinkNavigation();
        }
        private EntryText Text(Entry entry) => german ? entry.Definition.German : entry.Definition.English;
        private string Name(Entry entry) => Text(entry).Name + (entry.Duck ? german ? " (Gummiente)" : " (Ducky Tube)" : "");
        private Sprite? Portrait(Entry entry) => entry.Duck && entry.Definition.DuckPortrait != null
            ? entry.Definition.DuckPortrait(german) : entry.Definition.Portrait?.Invoke(german);
        private void RefreshCard(int slot, Entry entry)
        {
            var card = cards[slot];
            var thumbnail = entry.Definition.Thumbnail?.Invoke(german, entry.Duck);
            card.Background.sprite = thumbnail ?? cardBackground;
            card.Border.gameObject.SetActive(thumbnail == null);
            card.Portrait.gameObject.SetActive(thumbnail == null);
            card.Portrait.sprite = thumbnail == null ? Portrait(entry) : null;
            cardDucks[slot].sprite = duck;
            cardDucks[slot].enabled = thumbnail == null && entry.Duck && entry.Definition.DuckPortrait == null;
            if (thumbnail == null)
            {
                // Unframed mod artwork uses separate images inside the same card.
                // Follow the button's native transition, including its fade time.
                var tint = card.Background.canvasRenderer.GetColor();
                card.Border.canvasRenderer.SetColor(tint);
                card.Portrait.canvasRenderer.SetColor(tint);
                cardDucks[slot].canvasRenderer.SetColor(tint);
            }
        }
        internal void Update()
        {
            if (tabs.Count != Zombies.Catalogs.Count) AddTabs();
            bool changed = german != NativeLocalization.German; german = NativeLocalization.German;
            if (!page.activeSelf) return;
            if (changed) { ShowPage(pageIndex); return; }
            // Borrowed native/addressable sprites can become ready asynchronously.
            int start = pageIndex * PageSize;
            for (int i = 0; i < cards.Count && start + i < entries.Length; i++) RefreshCard(i, entries[start + i]);
            if (entries.Length > 0) portrait.sprite = Portrait(entries[selected]);
        }
        private void Open(ModCatalog mod)
        {
            if (current == mod && page.activeSelf) { Close(true); return; }
            if (!page.activeSelf)
                for (int i = 0; i < hidden.Count; i++)
                { var obj = hidden[i].Object; hidden[i] = (obj, obj.activeSelf); obj.SetActive(false); }
            current = mod; german = NativeLocalization.German;
            entries = (mod == Zombotany.Catalog ? Zombotany.Entries : Zombies.All).Where(item => item.Mod == mod).SelectMany(item => item.DuckOnly
                ? new[] { new Entry(item, true) } : item.RubberDuck
                ? new[] { new Entry(item, false), new Entry(item, true) } : new[] { new Entry(item, false) }).Where(item => Sightings.Contains(item.Definition, item.Duck)).ToArray();
            page.SetActive(true); ShowPage(0);
        }
        private void ShowPage(int index)
        {
            pageIndex = Math.Clamp(index, 0, Math.Max(0, (entries.Length - 1) / PageSize));
            int start = pageIndex * PageSize, length = Math.Min(PageSize, entries.Length - start);
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i]; card.Button.gameObject.SetActive(i < length); if (i >= length) continue;
                RefreshCard(i, entries[start + i]);
                var nav = card.Button.navigation; nav.mode = Navigation.Mode.Explicit;
                nav.selectOnLeft = i % Columns == 0 ? ActiveTab : cards[i - 1].Button;
                nav.selectOnRight = i % Columns == Columns - 1 || i + 1 == length ? ActiveTab : cards[i + 1].Button;
                nav.selectOnUp = i < Columns ? ActiveTab : cards[i - Columns].Button;
                nav.selectOnDown = i + Columns < length ? cards[i + Columns].Button : next;
                card.Button.navigation = nav;
            }
            header.text = current!.Name; count.text = $"{pageIndex + 1} / {Math.Max(1, (entries.Length + PageSize - 1) / PageSize)}";
            previous.GetComponentInChildren<TMP_Text>(true).text = german ? "Zurück" : "Previous";
            next.GetComponentInChildren<TMP_Text>(true).text = german ? "Weiter" : "Next";
            if (length > 0) Select(start);
            else { title.text = current.Name; description.text = german ? "Noch keine Zombies gesichtet." : "No zombies encountered yet."; portrait.sprite = null; duckPortrait.enabled = false; }
            LinkNavigation(); Focus(length > 0 ? cards[0].Button : ActiveTab);
        }
        private Button ActiveTab => tabs.First(item => item.Mod == current).Tab;
        private void Select(int index)
        {
            selected = index; var entry = entries[index]; var text = Text(entry);
            title.text = Name(entry); portrait.sprite = Portrait(entry);
            duckPortrait.sprite = duck; duckPortrait.enabled = entry.Duck && entry.Definition.DuckPortrait == null;
            description.text = $"<color=#28325a>{text.Info}</color>\n\n<color=#cc241d>{text.Flavor}</color>"
                + (entry.Duck ? german ? "\n\nGummiente: Kann im Pool schwimmen. Fähigkeiten bleiben erhalten." : "\n\nDucky tube: Can swim in the pool. Retains its abilities." : "");
            float width = description.rectTransform.rect.width;
            float height = description.GetPreferredValues(description.text, width, float.PositiveInfinity).y + 35;
            description.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.StopMovement(); scroll.verticalNormalizedPosition = 1;
        }
        private static void Focus(Selectable? target)
        { if (target != null && target && InputNavigationManager.Instance != null) InputNavigationManager.Instance.RefreshSelection(target); }
        private void LinkNavigation()
        {
            if (tabs.Count == 0) return;
            var target = page.activeSelf ? entries.Length > 0 ? cards[0].Button : ActiveTab : originalTarget;
            if (navigation != null && navigation) navigation.m_targetSelectable = target;
            var links = new[] { back }.Concat(tabs.Select(item => item.Tab)).Append(close).ToArray();
            for (int i = 0; i < links.Length; i++)
            {
                var nav = links[i].navigation; nav.mode = Navigation.Mode.Explicit;
                nav.selectOnLeft = links[(i + links.Length - 1) % links.Length];
                nav.selectOnRight = links[(i + 1) % links.Length]; nav.selectOnUp = target; links[i].navigation = nav;
            }
            foreach (var button in new[] { previous, next })
            {
                var nav = button.navigation; nav.mode = Navigation.Mode.Explicit;
                nav.selectOnUp = target; nav.selectOnDown = page.activeSelf ? ActiveTab : back;
                nav.selectOnLeft = previous; nav.selectOnRight = next; button.navigation = nav;
            }
            foreach (var (mod, tab) in tabs)
                tab.GetComponentInChildren<TMP_Text>(true).text = page.activeSelf && mod == current ? "Zombies" : mod.Name;
        }
        internal void Close(bool focus)
        {
            if (page == null || !page || !page.activeSelf) return;
            page.SetActive(false);
            foreach (var (obj, active) in hidden) if (obj) obj.SetActive(active);
            if (Root && panel) LinkNavigation();
            if (focus && Root && Root.gameObject.activeInHierarchy) Focus(ActiveTab);
        }
        public void Dispose()
        {
            Close(false);
            if (back) back.navigation = originalBack;
            if (close) close.navigation = originalClose;
            if (navigation != null && navigation) navigation.m_targetSelectable = originalTarget;
            foreach (var obj in owned.AsEnumerable().Reverse()) if (obj) Object.Destroy(obj);
            owned.Clear();
            foreach (var sprite in artwork) if (sprite) { Object.Destroy(sprite.texture); Object.Destroy(sprite); }
            artwork.Clear();
        }
    }
}

[HarmonyPatch(typeof(AlmanacZombieBinder), nameof(AlmanacZombieBinder.Bind))]
internal static class LibraryAlmanacBindPatch
{ private static void Postfix(AlmanacZombieBinder __instance) => LibraryAlmanac.Attach(__instance); }
