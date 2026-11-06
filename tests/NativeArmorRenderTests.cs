using Il2CppSpine;
using Il2CppSpine.Unity;
using UnityEngine;
using ZombieLibrary;

static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
var slots = Enumerable.Range(0, 4).Select(i => new Slot { Pointer = (IntPtr)(i + 1) }).ToArray();
var skin = new SkeletonAnimation { skeleton = new Skeleton { DrawOrder = new(slots) } };
var head = slots[2];
Check(ReferenceEquals(NativeArmorRenderOwner.Head(skin, head), skin.Body), "unsplit head belongs to base mesh");
skin.Body.enabled = false;
Check(NativeArmorRenderOwner.Head(skin, head) == null, "disabled unsplit body must not show a helmet");
var before = new Parts { currentInstructions = new(new Il2CppSpine.Unity.Range(0, 2)) };
var owner = new Parts { currentInstructions = new(new Il2CppSpine.Unity.Range(2, 3)) };
var after = new Parts { currentInstructions = new(new Il2CppSpine.Unity.Range(3, 4)) };
owner.MeshRenderer.sortingOrder = 100; owner.MeshRenderer.sortingLayerID = 9;
owner.MeshRenderer.gameObject.layer = 7;
skin.Separator = new() { partsRenderers = new() { before, owner, after } };
Check(ReferenceEquals(NativeArmorRenderOwner.Head(skin, head), owner.MeshRenderer), "split head must resolve its emitted range despite disabled source mesh");
owner.MeshRenderer.enabled = false;
Check(NativeArmorRenderOwner.Head(skin, head) == null, "visible arms/flag parts cannot substitute for hidden head part");
owner.MeshRenderer.enabled = true; owner.MeshRenderer.gameObject.activeInHierarchy = false;
Check(NativeArmorRenderOwner.Head(skin, head) == null, "inactive head part is hidden");
owner.MeshRenderer.gameObject.activeInHierarchy = true; skin.gameObject.activeInHierarchy = false;
Check(NativeArmorRenderOwner.Head(skin, head) == null, "inactive rig cannot leave a floating helmet");
skin.gameObject.activeInHierarchy = true;
owner.currentInstructions = new(new Il2CppSpine.Unity.Range(0, 2));
Check(NativeArmorRenderOwner.Head(skin, head) == null, "endSlot is exclusive; neighboring geometry cannot claim head");
owner.currentInstructions = null;
Check(NativeArmorRenderOwner.Head(skin, head) == null, "pending separator geometry must retry without orphan helmet");
skin.Body.enabled = true;
Check(ReferenceEquals(NativeArmorRenderOwner.Head(skin, head), skin.Body), "live base owns head while separation is pending");
Check(NativeArmorRenderOwner.Head(skin, new Slot { Pointer = (IntPtr)99 }) == null, "missing draw-order head has no owner");
Console.WriteLine("PASS actual head-range ownership, disabled source mesh, hidden head/rig, exclusive boundaries and pending-geometry transitions.");

namespace UnityEngine {
 public class Object { public static implicit operator bool(Object? value) => value != null; }
 public sealed class GameObject { public bool activeInHierarchy = true; public int layer; }
 public sealed class MeshRenderer : Object { public bool enabled = true; public GameObject gameObject = new(); public int sortingLayerID, sortingOrder; }
}
namespace Il2CppSpine {
 public sealed class Slot { public IntPtr Pointer; }
 public sealed class NativeList<T>(params T[] items) { public T[] Items = items; public int Count => Items.Length; }
 public sealed class Skeleton { public NativeList<Slot> DrawOrder = new(); }
}
namespace Il2CppSpine.Unity {
 public readonly record struct Range(int startSlot, int endSlot);
 public sealed class Instructions(params Range[] ranges) { public Il2CppSpine.NativeList<Range> submeshInstructions = new(ranges); }
 public sealed class Parts : UnityEngine.Object { public UnityEngine.MeshRenderer MeshRenderer = new(); public Instructions? currentInstructions; }
 public sealed class SkeletonRenderSeparator { public List<Parts> partsRenderers = new(); }
 public sealed class SkeletonAnimation {
    public UnityEngine.GameObject gameObject = new(); public Il2CppSpine.Skeleton? skeleton;
    public UnityEngine.MeshRenderer Body = new(); public SkeletonRenderSeparator? Separator;
    public T? GetComponent<T>() where T : class => typeof(T) == typeof(UnityEngine.MeshRenderer) ? Body as T : Separator as T;
 }
}
