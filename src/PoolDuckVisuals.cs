using HarmonyLib;
using Il2CppReloaded.Gameplay;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace ZombieLibrary;

// The tube is exported from Replanted's Zombie/duckytube Spine attachment.
// Gameplay and the saved entry/exit height remain entirely native.
public static class PoolDuckVisuals
{
    private sealed class View : IDisposable
    {
        internal readonly Zombie Zombie;
        internal readonly uint Id;
        internal readonly IntPtr Controller;
        internal readonly SpriteRenderer Renderer;
        private readonly Il2CppSpine.Unity.SkeletonAnimation skin;
        private readonly Il2CppSpine.Slot slot;
        private readonly Il2CppSpine.RegionAttachment? nativeDuck;
        private readonly PoolSpriteClip clip = new();
        private readonly Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<float> vertices;
        private readonly Vector2 center, size;
        private PoolArmLayers? arms;

        internal View(Zombie zombie, Il2CppSpine.Unity.SkeletonAnimation skin, Il2CppSpine.Slot slot,
            Il2CppSpine.RegionAttachment? duck)
        {
            Zombie = zombie; Id = zombie.DataID; Controller = zombie.mController.Pointer;
            this.skin = skin; this.slot = slot; nativeDuck = duck;
            int count = slot.Attachment?.TryCast<Il2CppSpine.MeshAttachment>()?.WorldVerticesLength ?? 8;
            vertices = new(Math.Max(count, 8));
            if (duck == null && Zombies.Find(zombie)?.PlacePoolDuck == null)
            {
                if (slot.Attachment?.TryCast<Il2CppSpine.RegionAttachment>() is { } region)
                { count = 8; region.ComputeWorldVertices(slot, vertices, 0, 2); }
                else if (slot.Attachment?.TryCast<Il2CppSpine.MeshAttachment>() is { } mesh)
                    mesh.ComputeWorldVertices(slot, 0, count, vertices, 0, 2);
                else throw new InvalidDataException("Pool zombie has no torso attachment.");
                var bone = slot.Bone;
                float determinant = bone.A * bone.D - bone.B * bone.C;
                if (Math.Abs(determinant) < .000001f) throw new InvalidDataException("Pool torso has no finite pose.");
                float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
                for (int i = 0; i < count; i += 2)
                {
                    float x = vertices[i] - bone.WorldX, y = vertices[i + 1] - bone.WorldY;
                    float localX = (bone.D * x - bone.B * y) / determinant;
                    float localY = (bone.A * y - bone.C * x) / determinant;
                    minX = Math.Min(minX, localX); maxX = Math.Max(maxX, localX);
                    minY = Math.Min(minY, localY); maxY = Math.Max(maxY, localY);
                }
                float width = maxX - minX, height = maxY - minY;
                // Native Zombie_body: 203.37 x 250; duck: 390 x 235.
                // Relative centers: -51.37, -67.73. Preserve that fit on each rig.
                float fit = zombie.mZombieType == ZombieType.Yeti ? .85f : 1f;
                size = new Vector2(width * (390f / 203.37f) * fit, height * (235f / 250f) * fit);
                center = new Vector2((minX + maxX) / 2 - width * (51.37f / 203.37f),
                    (minY + maxY) / 2 - height * (67.73f / 250f));
                if (!float.IsFinite(size.x) || !float.IsFinite(size.y) || size.x <= 0 || size.y <= 0)
                    throw new InvalidDataException("Pool torso has no finite dimensions.");
            }
            var spriteMaterial = Material(zombie.mBoard);
            var go = new GameObject("Fusion-NativeDuckTube") { layer = skin.gameObject.layer };
            go.transform.SetParent(skin.transform, false);
            Renderer = go.AddComponent<SpriteRenderer>();
            Renderer.sprite = Sprite();
            Renderer.sharedMaterial = spriteMaterial;
            Renderer.enabled = false;
            try { if (duck == null && !(Zombies.Find(zombie)?.PlacePoolDuck != null)) arms = new PoolArmLayers(skin); }
            catch { Dispose(); throw; }
        }

        internal void Update()
        {
            var body = skin.GetComponent<MeshRenderer>();
            Renderer.enabled = Zombie.mInPool && Zombie.mVisible && body != null && body.gameObject.activeInHierarchy
                && Zombie.mZombiePhase is not (ZombiePhase.ZombieBurned or ZombiePhase.ZombieMowered or ZombiePhase.DancerRising);
            arms?.SetEnabled(Renderer.enabled);
            if (!Renderer.enabled) return;
            if (nativeDuck != null && slot.Attachment?.Pointer == nativeDuck.Pointer && slot.A > 0
                && body!.enabled && !(Zombies.Find(Zombie)?.PlacePoolDuck != null))
            { Renderer.enabled = false; return; } // Reuse an already visible native tube.
            Renderer.sprite = Sprite(); // Restore the source before clipping this frame.
            bool eyes = (Zombies.Find(Zombie)?.PlacePoolDuck != null);
            if (eyes && !(Zombies.Find(Zombie)?.PlacePoolDuck?.Invoke(Zombie, Renderer) ?? false))
            { Renderer.enabled = false; return; }
            float height;
            if (!eyes)
            {
                if (nativeDuck != null) nativeDuck.ComputeWorldVertices(slot, vertices, 0, 2);
                else
                {
                    var bone = slot.Bone;
                    for (int i = 0; i < 4; ++i)
                    {
                        float x = center.x + ((i == 0 || i == 1) ? -size.x : size.x) / 2;
                        float y = center.y + ((i == 0 || i == 3) ? -size.y : size.y) / 2;
                        vertices[i * 2] = bone.A * x + bone.B * y + bone.WorldX;
                        vertices[i * 2 + 1] = bone.C * x + bone.D * y + bone.WorldY;
                    }
                }
                Vector3 LocalPoint(int index) => Renderer.transform.parent.InverseTransformPoint(
                    skin.transform.TransformPoint(new Vector3(vertices[index], vertices[index + 1], 0)));
                var bottomLeft = LocalPoint(0);
                var topLeft = LocalPoint(2);
                var topRight = LocalPoint(4);
                var bottomRight = LocalPoint(6);
                var xAxis = bottomRight - bottomLeft;
                var yAxis = topLeft - bottomLeft;
                // Work in the actual parent space. A quaternion cannot represent a
                // reflection; keep the determinant's sign in X, including dancer
                // moonwalk, negative parent scale and hypnosis (exactly once).
                Renderer.transform.localPosition = (bottomLeft + topRight) / 2;
                Renderer.transform.localRotation = Quaternion.Euler(0, 0,
                    Mathf.Atan2(-yAxis.x, yAxis.y) * Mathf.Rad2Deg);
                // Preserve the tube's world aspect even under nonuniform parent scale.
                float worldWidth = Renderer.transform.parent.TransformVector(xAxis).magnitude;
                float worldYScale = Renderer.transform.parent.TransformVector(yAxis.normalized).magnitude;
                if (!float.IsFinite(worldWidth) || !float.IsFinite(worldYScale)
                    || worldWidth <= .000001f || worldYScale <= .000001f)
                { Renderer.enabled = false; arms?.SetEnabled(false); return; }
                height = nativeDuck != null ? yAxis.magnitude : worldWidth * (235f / 390f) / worldYScale;
                float sign = xAxis.x * yAxis.y - xAxis.y * yAxis.x < 0 ? -1 : 1;
                Renderer.transform.localScale = new Vector3(sign * xAxis.magnitude / Renderer.sprite.bounds.size.x,
                    height / Renderer.sprite.bounds.size.y, 1);
                Renderer.flipX = false;
                Renderer.sortingLayerID = body!.sortingLayerID;
                Renderer.sortingOrder = arms != null ? skin.skeleton.DrawOrder.Count : body.sortingOrder + 1;
                arms?.Update(body);
            }
            height = Renderer.transform.TransformVector(new Vector3(0, Renderer.sprite.bounds.size.y, 0)).magnitude;
            Renderer.color = Zombie.mIceTrapCounter > 0 || Zombie.mChilledCounter > 0
                ? new Color(.65f, .8f, 1, 1) : Color.white;
            if (Waterline(Zombie, body!.transform, out var water, out var up))
            {
                // A synthetic tube floats on the same water plane as the body
                // clip. Torso origins differ between rigs and cannot own height.
                if (nativeDuck == null || eyes)
                    Renderer.transform.position += up * (height * .35f
                        - Vector3.Dot(Renderer.transform.position - water, up));
                clip.Apply(Renderer, water, up, true);
            }
        }

        public void Dispose()
        {
            if (Renderer) { Renderer.enabled = false; Object.Destroy(Renderer.gameObject); }
            arms?.Dispose(); arms = null;
            clip.Dispose();
        }
    }
    private sealed class PoolArmLayers : IDisposable
    {
        private readonly Il2CppSpine.Unity.SkeletonAnimation skin;
        private readonly Il2CppSpine.Unity.SkeletonRenderSeparator separator;
        private readonly List<Il2CppSpine.Slot> boundaries = new();
        private readonly HashSet<IntPtr> originalBoundaries = new();
        private readonly bool owned, originalEnabled, originalCopyBlock, originalCopyFlags;
        private readonly List<(Il2CppSpine.Unity.SkeletonPartsRenderer Part, int Layer, int Order)> originals = new();
        private readonly List<Il2CppSpine.Unity.SkeletonPartsRenderer> addedParts = new();
        private readonly SortingGroup group = null!;
        private readonly bool ownedGroup, originalGroupEnabled;
        private readonly int originalGroupLayer, originalGroupOrder;
        private bool active;

        private static bool Arm(Il2CppSpine.Slot slot) => LeftArm(slot.Data.Name);

        internal PoolArmLayers(Il2CppSpine.Unity.SkeletonAnimation skin)
        {
            this.skin = skin;
            var existing = skin.GetComponent<Il2CppSpine.Unity.SkeletonRenderSeparator>();
            owned = existing == null;
            foreach (var slot in skin.separatorSlots) originalBoundaries.Add(slot.Pointer);
            boundaries.AddRange(Boundaries());
            foreach (var slot in boundaries) skin.separatorSlots.Add(slot);
            try
            {
                separator = existing ?? Il2CppSpine.Unity.SkeletonRenderSeparator.AddToSkeletonRenderer(skin,
                    skin.GetComponent<MeshRenderer>().sortingLayerID, 0, 0,
                    skin.GetComponent<MeshRenderer>().sortingOrder, true);
                originalEnabled = separator.enabled;
                originalCopyBlock = separator.copyPropertyBlock;
                originalCopyFlags = separator.copyMeshRendererFlags;
                if (!owned)
                    foreach (var part in separator.partsRenderers)
                        originals.Add((part, part.MeshRenderer.sortingLayerID, part.MeshRenderer.sortingOrder));
                while (separator.partsRenderers.Count < skin.separatorSlots.Count + 1)
                    addedParts.Add(separator.AddPartsRenderer(0, "FusionPoolArmPart"));
                separator.copyPropertyBlock = true;
                separator.copyMeshRendererFlags = true;
                var existingGroup = skin.GetComponent<SortingGroup>();
                ownedGroup = existingGroup == null;
                group = existingGroup ?? skin.gameObject.AddComponent<SortingGroup>();
                if (ownedGroup) group.enabled = false;
                originalGroupEnabled = group.enabled;
                originalGroupLayer = group.sortingLayerID; originalGroupOrder = group.sortingOrder;
                armLayers.Add(skin.Pointer, this);
            }
            catch
            {
                if (ownedGroup && group) { group.enabled = false; Object.Destroy(group); }
                if (existing != null)
                {
                    existing.copyPropertyBlock = originalCopyBlock;
                    existing.copyMeshRendererFlags = originalCopyFlags;
                    existing.enabled = originalEnabled;
                }
                foreach (var slot in boundaries) skin.separatorSlots.Remove(slot);
                foreach (var part in addedParts)
                { if (existing != null) existing.partsRenderers.Remove(part); if (part) Object.Destroy(part.gameObject); }
                if (owned)
                {
                    var failed = skin.GetComponent<Il2CppSpine.Unity.SkeletonRenderSeparator>();
                    if (failed != null)
                    {
                        failed.enabled = false;
                        foreach (var part in failed.partsRenderers) if (part) Object.Destroy(part.gameObject);
                        Object.Destroy(failed);
                    }
                }
                throw;
            }
        }

        private List<Il2CppSpine.Slot> Boundaries()
        {
            var result = new List<Il2CppSpine.Slot>();
            var order = skin.skeleton.DrawOrder;
            Il2CppSpine.Slot? previous = null;
            for (int i = 0; i < order.Count; ++i)
            {
                var slot = order.Items[i];
                // Spine emits geometry boundaries at attached slots. An empty
                // arm marker cannot split the mesh before the next real arm.
                if (slot.Attachment == null) continue;
                if (previous != null && Arm(previous) != Arm(slot)
                    && !originalBoundaries.Contains(slot.Pointer)) result.Add(slot);
                previous = slot;
            }
            return result;
        }

        internal void Prepare()
        {
            if (!active) return;
            var next = Boundaries();
            if (boundaries.Select(s => s.Pointer).SequenceEqual(next.Select(s => s.Pointer))) return;
            foreach (var slot in boundaries) skin.separatorSlots.Remove(slot);
            boundaries.Clear(); boundaries.AddRange(next);
            foreach (var slot in boundaries) skin.separatorSlots.Add(slot);
            while (separator.partsRenderers.Count < skin.separatorSlots.Count + 1)
                addedParts.Add(separator.AddPartsRenderer(0, "FusionPoolArmPart"));
        }

        internal void SetEnabled(bool enabled)
        {
            if (active && !enabled && !owned)
                foreach (var (part, layer, order) in originals)
                    if (part) { part.MeshRenderer.sortingLayerID = layer; part.MeshRenderer.sortingOrder = order; }
            active = enabled;
            if (!originalGroupEnabled)
            {
                group.enabled = enabled;
                if (!enabled && !ownedGroup)
                { group.sortingLayerID = originalGroupLayer; group.sortingOrder = originalGroupOrder; }
            }
            bool wanted = enabled || (!owned && originalEnabled);
            if (separator.enabled != wanted) separator.enabled = wanted;
        }

        internal void Update(MeshRenderer body)
        {
            if (!active) return;
            // Keep each zombie atomic against other actors. Inside that group,
            // preserve native draw order on both sides of the tube. Equal orders
            // let Unity sort separate meshes by bounds, scrambling helmets and Gargs.
            if (!originalGroupEnabled)
            { group.sortingLayerID = body.sortingLayerID; group.sortingOrder = body.sortingOrder; }
            int count = skin.skeleton.DrawOrder.Count;
            foreach (var part in separator.partsRenderers)
            {
                var instructions = part.currentInstructions?.submeshInstructions;
                bool arm = false;
                int first = count;
                if (instructions != null)
                    for (int i = 0; i < instructions.Count; ++i)
                    {
                        var instruction = instructions.Items[i];
                        first = Math.Min(first, instruction.startSlot);
                        for (int j = instruction.startSlot; j < instruction.endSlot; ++j)
                            arm |= skin.skeleton.DrawOrder.Items[j].Attachment != null && Arm(skin.skeleton.DrawOrder.Items[j]);
                    }
                part.MeshRenderer.sortingLayerID = body.sortingLayerID;
                part.MeshRenderer.sortingOrder = first + (arm ? count + 1 : 0);
            }
        }

        internal void AfterRender() => Update(skin.GetComponent<MeshRenderer>());

        public void Dispose()
        {
            if (!skin || !separator) return;
            armLayers.Remove(skin.Pointer);
            if (ownedGroup && group) { group.enabled = false; Object.Destroy(group); }
            else if (group && !originalGroupEnabled)
            {
                group.enabled = false;
                group.sortingLayerID = originalGroupLayer; group.sortingOrder = originalGroupOrder;
            }
            foreach (var slot in boundaries) skin.separatorSlots.Remove(slot);
            if (owned)
            {
                separator.enabled = false;
                foreach (var part in separator.partsRenderers) if (part) Object.Destroy(part.gameObject);
                Object.Destroy(separator);
            }
            else
            {
                foreach (var part in addedParts)
                { separator.partsRenderers.Remove(part); if (part) Object.Destroy(part.gameObject); }
                foreach (var (part, layer, order) in originals)
                    if (part) { part.MeshRenderer.sortingLayerID = layer; part.MeshRenderer.sortingOrder = order; }
                separator.copyPropertyBlock = originalCopyBlock;
                separator.copyMeshRendererFlags = originalCopyFlags;
                separator.enabled = originalEnabled;
            }
        }
    }

    public static bool LeftArm(string name) => name.Contains("outerarm", StringComparison.OrdinalIgnoreCase)
        || name.Contains("outerhand", StringComparison.OrdinalIgnoreCase)
        || name.Contains("leftarm", StringComparison.OrdinalIgnoreCase);

    private static readonly Dictionary<IntPtr, PoolArmLayers> armLayers = new();
    internal static void BeforeArmRender(Il2CppSpine.Unity.SkeletonRenderer skin)
    { if (armLayers.TryGetValue(skin.Pointer, out var arms)) arms.Prepare(); }
    internal static void AfterArmRender(Il2CppSpine.Unity.SkeletonRenderSeparator separator)
    { if (separator.skeletonRenderer != null && armLayers.TryGetValue(separator.skeletonRenderer.Pointer, out var arms)) arms.AfterRender(); }

    private static readonly Dictionary<IntPtr, View> views = new();
    private static readonly HashSet<string> reported = new();
    private static Sprite? sprite;
    private static Texture2D? texture;
    private static Material? material;

    private static Material Material(Board board)
    {
        if (material != null && material) return material;
        var preview = board.mApp.CreatePreviewController(SeedType.Peashooter, board.mCursorObject);
        try
        {
            preview.HidePreview();
            var source = preview.m_previewDrawer?.m_previewSprite?.sharedMaterial
                ?? throw new InvalidOperationException("Native pool sprite material is unavailable.");
            material = new Material(source) { name = "Fusion-PoolDuck", hideFlags = HideFlags.HideAndDontSave };
            material.SetColor("_Color", Color.white);
            return material;
        }
        finally { if (preview != null && preview) preview.Die(); }
    }

    private static Sprite Sprite()
    {
        if (sprite != null) return sprite;
        using var stream = typeof(PoolDuckVisuals).Assembly.GetManifestResourceStream("ZombieLibrary.Assets.duck.png")
            ?? throw new InvalidDataException("Missing native duck tube.");
        using var bytes = new MemoryStream(); stream.CopyTo(bytes);
        texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.hideFlags = HideFlags.HideAndDontSave;
        if (!ImageConversion.LoadImage(texture, bytes.ToArray(), true))
            throw new InvalidDataException("Invalid native duck tube.");
        texture.wrapMode = TextureWrapMode.Clamp;
        sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100);
        sprite.hideFlags = HideFlags.HideAndDontSave;
        return sprite;
    }

    internal static void Register(Board board)
    {
        for (int i = 0; i < board.m_zombies.Count; ++i)
        {
            var zombie = board.m_zombies[i];
            if (zombie == null || zombie.mDead || !zombie.mInPool || !ZombiePoolRules.Duck(zombie)
                || zombie.mController == null) continue;
            var skin = zombie.mController.AnimationController?.SkeletonAnimation;
            var skeleton = skin?.skeleton;
            if (skin == null || skeleton == null) continue;
            var duckSlot = skeleton.FindSlot("Zombie_duckytube");
            bool customPlacement = Zombies.Find(zombie)?.PlacePoolDuck != null;
            if (duckSlot?.Attachment != null && duckSlot.A > 0
                && duckSlot.Attachment.Name.Contains("duckytube", StringComparison.OrdinalIgnoreCase)
                && Zombies.Find(zombie)?.PlacePoolDuck == null)
            {
                if (views.Remove(zombie.Pointer, out var redundant)) redundant.Dispose();
                continue;
            }
            if (views.TryGetValue(zombie.Pointer, out var old))
            {
                if (old.Id == zombie.DataID && old.Controller == zombie.mController.Pointer && old.Renderer) continue;
                old.Dispose(); views.Remove(zombie.Pointer);
            }
            var duck = duckSlot == null ? null : skeleton.Data.FindSkin("duckytube")?
                .GetAttachment(duckSlot.Data.Index, "Zombie_duckytube")?.TryCast<Il2CppSpine.RegionAttachment>();
            Il2CppSpine.Slot? torso = null;
            float largestTorso = 0;
            for (int index = 0; index < skeleton.Slots.Count; ++index)
            {
                var candidate = skeleton.Slots.Items[index];
                string name = candidate.Data.Name.ToLowerInvariant();
                if (candidate.Attachment != null && candidate.Bone.Active
                    && (name.Contains("body") || name.Contains("torso") || name.Contains("pelvis")))
                {
                    // Lower-body and pelvis slots occur before the torso. Use
                    // the largest actual attachment, not draw order or its name.
                    float area = 0;
                    if (candidate.Attachment.TryCast<Il2CppSpine.RegionAttachment>() is { } region)
                        area = Math.Abs(region.Width * region.Height);
                    else if (candidate.Attachment.TryCast<Il2CppSpine.MeshAttachment>() is { } mesh)
                    {
                        // Mesh Width/Height can be zero in shipped rigs (Pogo).
                        // Select using actual posed geometry, as View does.
                        var points = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<float>(mesh.WorldVerticesLength);
                        mesh.ComputeWorldVertices(candidate, 0, points.Length, points, 0, 2);
                        float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
                        for (int v = 0; v + 1 < points.Length; v += 2)
                        {
                            minX = Math.Min(minX, points[v]); maxX = Math.Max(maxX, points[v]);
                            minY = Math.Min(minY, points[v + 1]); maxY = Math.Max(maxY, points[v + 1]);
                        }
                        area = (maxX - minX) * (maxY - minY);
                    }
                    if (area > largestTorso) { torso = candidate; largestTorso = area; }
                }
            }
            if (duck == null && torso == null && !customPlacement)
            {
                if (reported.Add($"{zombie.mZombieType}:no-torso"))
                    LibraryMod.Instance.LoggerInstance.Warning($"Pool duck {zombie.mZombieType}: no active torso attachment; retrying.");
                continue;
            }
            try { views[zombie.Pointer] = new(zombie, skin, duck != null ? duckSlot! : torso ?? skeleton.Slots.Items[0], duck); }
            catch (Exception error)
            {
                if (reported.Add($"{zombie.mZombieType}:{error.Message}"))
                    LibraryMod.Instance.LoggerInstance.Warning($"Pool duck {zombie.mZombieType}: {error.Message}");
            }
        }
    }

    public static bool Waterline(Zombie zombie, Transform body, out Vector3 water, out Vector3 up)
    {
        water = Vector3.zero; up = Vector3.up;
        if (!zombie.mInPool && zombie.mZombieHeight != ZombieHeight.OutOfPool) return false;
        var effects = zombie.mController?.m_materialEffectController;
        var camera = Camera.main;
        if (effects == null || !effects.m_enableClipRect || camera == null) return false;
        // Use the native clip rectangle and its actual screen offset. Shadow
        // bounds and GetDrawPos omit controller offsets and render-only depth.
        // MaterialEffectController.SetClipRect scales board pixels by 2.82
        // against the native 3840 x 2160 frame and stores top-origin screen Y.
        float screenY = Screen.height - (effects.m_clipRect.yMax * (Screen.height / 2160f) * 2.82f
            + effects.m_clipRectOffset.y);
        var screen = camera.WorldToScreenPoint(body.position);
        water = camera.ScreenToWorldPoint(new Vector3(screen.x, screenY, screen.z));
        up = camera.transform.up;
        return true;
    }

    internal static void Update()
    {
        foreach (var (key, view) in views.ToArray())
        {
            var zombie = view.Zombie;
            if (zombie.DataID != view.Id || zombie.mDead || zombie.mController == null
                || zombie.mController.Pointer != view.Controller || !view.Renderer || !ZombiePoolRules.Duck(zombie))
            { view.Dispose(); views.Remove(key); continue; }
            try { view.Update(); }
            catch (Exception error)
            {
                view.Dispose(); views.Remove(key);
                if (reported.Add($"{zombie.mZombieType}:{error.Message}"))
                    LibraryMod.Instance.LoggerInstance.Warning($"Pool duck {zombie.mZombieType}: {error.Message}");
            }
        }
    }

    internal static void Forget(Board board)
    {
        foreach (var (key, view) in views.Where(pair => pair.Value.Zombie.mBoard?.Pointer == board.Pointer).ToArray())
        { view.Dispose(); views.Remove(key); }
    }

    internal static void Clear()
    {
        foreach (var view in views.Values) view.Dispose();
        views.Clear(); reported.Clear();
        if (sprite != null) Object.Destroy(sprite);
        if (texture != null) Object.Destroy(texture);
        if (material != null) Object.Destroy(material);
        sprite = null; texture = null; material = null;
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.UpdateGame))]
internal static class PoolDuckBoardPatch
{ private static void Postfix(Board __instance) => PoolDuckVisuals.Register(__instance); }

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class PoolDuckBoardDisposePatch
{ private static void Prefix(Board __instance) => PoolDuckVisuals.Forget(__instance); }

[HarmonyPatch(typeof(Il2CppSpine.Unity.SkeletonRenderSeparator), nameof(Il2CppSpine.Unity.SkeletonRenderSeparator.HandleRender))]
internal static class PoolArmRenderPatch
{
    private static void Postfix(Il2CppSpine.Unity.SkeletonRenderSeparator __instance) => PoolDuckVisuals.AfterArmRender(__instance);
}

[HarmonyPatch(typeof(Il2CppSpine.Unity.SkeletonRenderer), nameof(Il2CppSpine.Unity.SkeletonRenderer.LateUpdateMesh))]
internal static class PoolArmPreparePatch
{
    private static void Prefix(Il2CppSpine.Unity.SkeletonRenderer __instance) => PoolDuckVisuals.BeforeArmRender(__instance);
}
