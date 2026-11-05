using UnityEngine;
using Object = UnityEngine.Object;

namespace ZombieLibrary;

// Imported sprites do not use Spine's clip shader. Clip their instance-owned
// geometry instead. Textures and shared source sprites remain untouched.
public sealed class PoolSpriteClip : IDisposable
{
    private readonly Dictionary<IntPtr, Sprite> copies = new();
    private Sprite? source, copy;

    public void Apply(SpriteRenderer renderer, Vector3 water, Vector3 up, bool enabled)
    {
        if (renderer.sprite == null) return;
        if (copy == null || renderer.sprite.Pointer != copy.Pointer) source = renderer.sprite;
        if (source == null) return;
        if (!enabled) { renderer.sprite = source; return; }
        if (!copies.TryGetValue(source.Pointer, out copy))
        {
            var rect = source.rect;
            var pivot = source.pivot;
            copy = Sprite.Create(source.texture, rect, new Vector2(pivot.x / rect.width, pivot.y / rect.height),
                source.pixelsPerUnit, 0, SpriteMeshType.FullRect);
            copies[source.Pointer] = copy;
        }
        var bounds = source.bounds;
        var min = bounds.min; var max = bounds.max;
        var corners = new[] { new Vector2(min.x, min.y), new Vector2(max.x, min.y),
            new Vector2(max.x, max.y), new Vector2(min.x, max.y) };
        float Distance(Vector2 point) => Vector3.Dot(renderer.transform.TransformPoint(new Vector3(
            renderer.flipX ? -point.x : point.x, renderer.flipY ? -point.y : point.y, 0)) - water, up);
        var polygon = new List<Vector2>(5);
        for (int i = 0; i < corners.Length; ++i)
        {
            var a = corners[i]; var b = corners[(i + 1) % corners.Length];
            float da = Distance(a), db = Distance(b);
            if (da >= 0) polygon.Add(a);
            if ((da >= 0) != (db >= 0)) polygon.Add(a + (b - a) * (da / (da - db)));
        }
        if (polygon.Count < 3) { renderer.enabled = false; return; }
        var triangles = new ushort[(polygon.Count - 2) * 3];
        for (int i = 0; i < polygon.Count - 2; ++i)
        { triangles[i * 3] = 0; triangles[i * 3 + 1] = (ushort)(i + 1); triangles[i * 3 + 2] = (ushort)(i + 2); }
        // OverrideGeometry consumes rectangle pixels, not the local units used
        // for the world-space water test. Clamp floating-point edge roundoff.
        var rectPixels = source.rect;
        var pivotPixels = source.pivot;
        var pixels = polygon.Select(p => new Vector2(
            Math.Clamp(p.x * source.pixelsPerUnit + pivotPixels.x, 0, rectPixels.width),
            Math.Clamp(p.y * source.pixelsPerUnit + pivotPixels.y, 0, rectPixels.height))).ToArray();
        copy.OverrideGeometry(pixels, triangles);
        renderer.sprite = copy;
    }

    public void Dispose()
    {
        foreach (var sprite in copies.Values) if (sprite) Object.Destroy(sprite);
        copies.Clear(); source = copy = null;
    }
}
