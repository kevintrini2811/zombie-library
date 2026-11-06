using Il2CppSpine;
using Il2CppSpine.Unity;
using UnityEngine;

namespace ZombieLibrary;

// A separator disables the source MeshRenderer. Visibility and sorting belong
// to the part whose emitted draw-order range contains the actual head slot.
internal static class NativeArmorRenderOwner
{
    private static bool Visible(MeshRenderer? renderer) => renderer != null && renderer
        && renderer.enabled && renderer.gameObject.activeInHierarchy;

    internal static MeshRenderer? Head(SkeletonAnimation skin, Slot head)
    {
        if (!skin.gameObject.activeInHierarchy || skin.skeleton == null) return null;
        int index = -1;
        var order = skin.skeleton.DrawOrder;
        for (int i = 0; i < order.Count; ++i)
            if (order.Items[i].Pointer == head.Pointer) { index = i; break; }
        if (index < 0) return null;
        var separator = skin.GetComponent<SkeletonRenderSeparator>();
        if (separator != null)
            foreach (var part in separator.partsRenderers)
            {
                if (part == null || !part) continue;
                var instructions = part.currentInstructions?.submeshInstructions;
                if (instructions == null || !Visible(part.MeshRenderer)) continue;
                for (int i = 0; i < instructions.Count; ++i)
                {
                    var range = instructions.Items[i];
                    if (range.startSlot <= index && index < range.endSlot) return part.MeshRenderer;
                }
            }
        // Before separation (or while returning to one mesh), the live base
        // mesh can still own the whole head. Never borrow an unrelated part.
        var body = skin.GetComponent<MeshRenderer>();
        return Visible(body) ? body : null;
    }
}
