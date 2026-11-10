// The registry suite links the real public damage API and its owner. Rendering
// remains an external boundary and does not load Unity assets in these tests.
namespace ZombieLibrary;
internal static class NativeArmorVisuals
{
    internal static void Observe(Il2CppReloaded.Gameplay.Zombie zombie) {}
    internal static UnityEngine.Sprite Portrait(Il2CppReloaded.Gameplay.ZombieType type, bool bucket) => new();
}
