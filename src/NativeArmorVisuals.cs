using System.Text.Json;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppReloaded.Gameplay;
using Il2CppSpine;
using Il2CppSpine.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZombieLibrary;

// A single native head-socket overlay supports every registered AI rig. The
// body and its animation, attachments and equipment remain entirely native.
internal static class NativeArmorVisuals
{
    private static readonly Dictionary<string, Sprite> sprites = new();
    private static readonly Dictionary<IntPtr, View> views = new();
    private static readonly HashSet<string> warnings = new();
    private static Dictionary<string, float[]>? fits;
    private static Material? material;

    private static Stream Resource(string name) => typeof(NativeArmorVisuals).Assembly
        .GetManifestResourceStream("ZombieLibrary.Armored." + name)
        ?? throw new InvalidDataException("Missing native armor asset: " + name);

    private static Sprite Sprite(string name)
    {
        if (sprites.TryGetValue(name, out var sprite)) return sprite;
        using var source = Resource(name + ".png");
        using var bytes = new MemoryStream(); source.CopyTo(bytes);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        if (!ImageConversion.LoadImage(texture, bytes.ToArray(), true))
        { Object.Destroy(texture); throw new InvalidDataException("Invalid armor image: " + name); }
        texture.wrapMode = TextureWrapMode.Clamp;
        sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, 0), 100);
        sprite.name = "ZombieLibrary.Armored." + name;
        sprite.hideFlags = HideFlags.HideAndDontSave;
        sprites.Add(name, sprite);
        return sprite;
    }

    internal static Sprite Portrait(ZombieType type, bool bucket) => Sprite(type + (bucket ? "-bucket" : "-cone"));
    internal static string HeadSlot(ZombieType type) => type == ZombieType.Catapult ? "Zombie_catapult_driver_head" : "anim_head1";

    private static float[] Fit(string name)
    {
        if (fits == null)
        {
            using var source = Resource("placement.json");
            fits = JsonSerializer.Deserialize<Dictionary<string, float[]>>(source)
                ?? throw new InvalidDataException("Invalid native armor placement.");
        }
        var fit = fits[name];
        if (fit.Length != 3 || fit.Any(value => !float.IsFinite(value)) || fit[0] <= 0)
            throw new InvalidDataException("Invalid native armor fit: " + name);
        return fit;
    }

    internal static void Observe(Zombie zombie)
    {
        if (zombie.mBoard == null) return;
        if (views.TryGetValue(zombie.Pointer, out var previous))
        {
            if (previous.Valid) return;
            previous.Dispose(); views.Remove(zombie.Pointer);
        }
        if (!zombie.mDead) views.Add(zombie.Pointer, new(zombie));
    }

    internal static void Update()
    {
        foreach (var (pointer, view) in views.ToArray())
        {
            if (!view.Valid) { view.Dispose(); views.Remove(pointer); continue; }
            try { view.Update(); }
            catch (Exception error)
            {
                if (warnings.Add(view.Zombie.mZombieType + ":" + error.Message))
                    LibraryMod.Instance.LoggerInstance.Error("Native armor: " + error);
            }
        }
    }

    internal static bool TrackPosition(Zombie zombie, string track, out Unity.Mathematics.float2 result)
    {
        result = default;
        if (track is not ("anim_cone" or "anim_bucket") || !NativeArmoredZombies.TryKind(zombie, out _)
            || zombie.mController == null) return false;
        var head = zombie.GetTrackPosition(HeadSlot(zombie.mZombieType));
        result = head;
        if (views.TryGetValue(zombie.Pointer, out var view) && view.Helmet != null && view.Skin != null && view.Slot != null)
        {
            var bone = view.Slot.Bone;
            var origin = view.Skin.transform.TransformPoint(new Vector3(bone.WorldX, bone.WorldY, 0));
            var delta = view.Helmet.transform.position - origin;
            // Native GetTrackPosition uses board pixels; the same 2.82 world
            // conversion is shared by native controller placement and clipping.
            result = new(head.x + delta.x / 2.82f, head.y - delta.y / 2.82f);
        }
        return true;
    }

    internal static void Dispose()
    {
        foreach (var view in views.Values) view.Dispose();
        views.Clear(); warnings.Clear();
        foreach (var sprite in sprites.Values) { Object.Destroy(sprite.texture); Object.Destroy(sprite); }
        sprites.Clear(); fits = null;
        if (material != null) Object.Destroy(material);
        material = null;
    }

    internal static void Forget(Board board)
    {
        foreach (var (pointer, view) in views.Where(pair => pair.Value.Board == board.Pointer).ToArray())
        { view.Dispose(); views.Remove(pointer); }
    }

    private sealed class View : IDisposable
    {
        internal readonly Zombie Zombie;
        private readonly uint id;
        private readonly IntPtr board;
        internal IntPtr Board => board;
        internal SpriteRenderer? Helmet;
        internal SkeletonAnimation? Skin;
        internal Slot? Slot;
        private IntPtr controller;
        private readonly MaterialPropertyBlock colors = new();
        private readonly PoolSpriteClip clip = new();
        private Il2CppStructArray<float> vertices = new(8);

        internal View(Zombie zombie) { Zombie = zombie; id = zombie.DataID; board = zombie.mBoard.Pointer; }
        internal bool Valid => Zombie.DataID == id && !Zombie.mDead && Zombie.mBoard?.Pointer == board
            && NativeArmoredZombies.TryKind(Zombie, out _);

        internal void Update()
        {
            var native = Zombie.mController;
            int health = NativeArmoredZombies.Remaining(Zombie);
            if (native == null || !native || !Zombie.mHasHead || health <= 0 || !Zombie.mVisible
                || Zombie.mZombiePhase is ZombiePhase.ZombieBurned or ZombiePhase.ZombieMowered)
            { if (Helmet != null) Helmet.enabled = false; return; }
            var skin = native.AnimationController?.SkeletonAnimation;
            if (skin == null || skin.skeleton == null)
            { if (Helmet != null) Helmet.enabled = false; return; }
            var slot = skin.skeleton.FindSlot(HeadSlot(Zombie.mZombieType));
            if (slot == null || !slot.Bone.Active || slot.Attachment == null || slot.A <= 0)
            { if (Helmet != null) Helmet.enabled = false; return; }
            var body = NativeArmorRenderOwner.Head(skin, slot);
            if (body == null)
            { if (Helmet != null) Helmet.enabled = false; return; }
            if (controller != native.Pointer || Skin?.Pointer != skin.Pointer || Helmet == null || !Helmet)
            {
                if (Helmet != null) Object.Destroy(Helmet.gameObject);
                var go = new GameObject("ZombieLibrary-NativeHelmet") { layer = body.gameObject.layer };
                go.transform.SetParent(skin.transform, false);
                Helmet = go.AddComponent<SpriteRenderer>();
                material ??= ZombieSpriteColor.CreateMaterial(Zombie);
                Helmet.sharedMaterial = material;
                // Native damage swaps can activate a cone/bucket skin even on
                // a base rig that originally has no helmet. Keep those native
                // slots hidden; the shared socket overlay is their sole owner.
                native.AssignRenderGroupToPrefix("anim_cone", -1);
                native.AssignRenderGroupToPrefix("anim_bucket", -1);
                controller = native.Pointer;
            }
            Skin = skin; Slot = slot;
            NativeArmoredZombies.TryKind(Zombie, out bool bucket);
            int maximum = Zombie.mZombieType == ZombieType.Bobsled && Zombie.mGraveY > 0
                ? NativeArmorTypes.Health(bucket) : Zombie.mHelmMaxHealth;
            int stage = health * 3L > maximum * 2L ? 1 : health * 3L > maximum ? 2 : 3;
            string name = (bucket ? "bucket" : "cone") + (stage == 1 ? "" : stage.ToString());
            var sprite = Sprite(name); Helmet.sprite = sprite;
            var fit = Fit(name);
            if (!HeadBounds(slot, out var min, out var max)) { Helmet.enabled = false; return; }
            var bone = slot.Bone;
            float headWidth = max.x - min.x, headHeight = max.y - min.y;
            float width = headWidth * fit[0];
            float height = width * sprite.bounds.size.y / sprite.bounds.size.x;
            var local = new Vector2((min.x + max.x) / 2 + headWidth * fit[1], min.y + headHeight * fit[2]);
            var xAxis = new Vector2(bone.A * width, bone.C * width);
            var yAxis = new Vector2(bone.B * height, bone.D * height);
            float determinant = xAxis.x * yAxis.y - xAxis.y * yAxis.x;
            if (!float.IsFinite(determinant) || Math.Abs(determinant) < .000001f)
            { Helmet.enabled = false; return; }
            Helmet.transform.localPosition = new Vector3(bone.A * local.x + bone.B * local.y + bone.WorldX,
                bone.C * local.x + bone.D * local.y + bone.WorldY, 0);
            Helmet.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(-yAxis.x, yAxis.y) * Mathf.Rad2Deg);
            Helmet.transform.localScale = new Vector3((determinant < 0 ? -1 : 1) * xAxis.magnitude / sprite.bounds.size.x,
                yAxis.magnitude / sprite.bounds.size.y, 1);
            Helmet.gameObject.layer = body.gameObject.layer;
            Helmet.sortingLayerID = body.sortingLayerID;
            // Stay immediately above the mesh that actually emits this head,
            // preserving the order of later native separator parts/equipment.
            Helmet.sortingOrder = body.sortingOrder + 1;
            Helmet.enabled = true;
            float attachmentAlpha = slot.Attachment.TryCast<RegionAttachment>()?.A
                ?? slot.Attachment.TryCast<MeshAttachment>()?.A ?? 1;
            float alpha = slot.A * skin.skeleton.A * attachmentAlpha;
            ZombieSpriteColor.From(Zombie).Apply(Helmet, new Color(1, 1, 1, alpha), colors);
            bool clipping = PoolDuckVisuals.NativeClipPlane(Zombie, skin.transform, out var plane, out var up);
            clip.Apply(Helmet, plane, up, clipping);
        }

        private bool HeadBounds(Slot slot, out Vector2 min, out Vector2 max)
        {
            min = new(float.MaxValue, float.MaxValue); max = new(float.MinValue, float.MinValue);
            int count;
            if (slot.Attachment?.TryCast<RegionAttachment>() is { } region)
            { count = 8; region.ComputeWorldVertices(slot, vertices, 0, 2); }
            else if (slot.Attachment?.TryCast<MeshAttachment>() is { } mesh)
            {
                count = mesh.WorldVerticesLength;
                if (vertices.Length < count) vertices = new(count);
                mesh.ComputeWorldVertices(slot, 0, count, vertices, 0, 2);
            }
            else return false;
            var bone = slot.Bone;
            float determinant = bone.A * bone.D - bone.B * bone.C;
            if (!float.IsFinite(determinant) || Math.Abs(determinant) < .000001f) return false;
            for (int i = 0; i < count; i += 2)
            {
                float x = vertices[i] - bone.WorldX, y = vertices[i + 1] - bone.WorldY;
                float localX = (bone.D * x - bone.B * y) / determinant;
                float localY = (-bone.C * x + bone.A * y) / determinant;
                min.x = Math.Min(min.x, localX); min.y = Math.Min(min.y, localY);
                max.x = Math.Max(max.x, localX); max.y = Math.Max(max.y, localY);
            }
            return float.IsFinite(min.x + min.y + max.x + max.y) && max.x > min.x && max.y > min.y;
        }

        public void Dispose()
        { clip.Dispose(); if (Helmet != null) Object.Destroy(Helmet.gameObject); Helmet = null; Skin = null; Slot = null; }
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.DisposeBoard))]
internal static class NativeArmorBoardDisposePatch
{
    private static void Prefix(Board __instance) => NativeArmorVisuals.Forget(__instance);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.GetTrackPosition))]
internal static class NativeArmorTrackPatch
{
    private static bool Prefix(Zombie __instance, string __0, ref Unity.Mathematics.float2 __result)
    {
        if (!NativeArmorVisuals.TrackPosition(__instance, __0, out var position)) return true;
        __result = position;
        return false;
    }
}
