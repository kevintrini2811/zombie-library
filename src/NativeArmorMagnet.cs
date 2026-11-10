using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace ZombieLibrary;

// The sled leader's native helmet slot belongs to the sled. Expose its added
// bucket only for the synchronous native magnet selection/attraction call;
// every native distance, allegiance, equipment and cooldown rule still runs.
// Attraction synchronously clears the borrowed native fields; finally commits
// that removal to the added hat and restores the untouched native sled fields.
internal static class NativeArmorMagnet
{
    internal readonly record struct Borrowed(Zombie Zombie, uint Id, HelmType Helm, int Health, int Maximum);
    [ThreadStatic] private static HashSet<IntPtr>? exposed;

    internal static List<Borrowed>? Begin(Plant plant)
    {
        if (plant.mBoard == null) return null;
        List<Borrowed>? borrowed = null;
        for (int i = 0; i < plant.mBoard.m_zombies.Count; ++i)
        {
            var zombie = plant.mBoard.m_zombies[i];
            if (zombie == null || zombie.mDead || zombie.mZombieType != ZombieType.Bobsled || zombie.mGraveY <= 0
                || !NativeArmoredZombies.TryKind(zombie, out bool bucket) || !bucket
                || exposed?.Contains(zombie.Pointer) == true) continue;
            borrowed ??= new();
            borrowed.Add(new(zombie, zombie.DataID, zombie.mHelmType, zombie.mHelmHealth, zombie.mHelmMaxHealth));
            (exposed ??= new()).Add(zombie.Pointer);
            zombie.mHelmType = HelmType.Pail;
            zombie.mHelmHealth = zombie.mGraveY;
            zombie.mHelmMaxHealth = NativeArmorTypes.BucketHealth;
        }
        return borrowed;
    }

    internal static void Restore(List<Borrowed>? borrowed)
    {
        if (borrowed == null) return;
        foreach (var item in borrowed)
        {
            exposed?.Remove(item.Zombie.Pointer);
            if (item.Zombie.DataID != item.Id) continue;
            if (item.Zombie.mHelmType == HelmType.None && item.Zombie.mHelmHealth == 0)
                item.Zombie.mGraveY = 0;
            item.Zombie.mHelmType = item.Helm;
            item.Zombie.mHelmHealth = item.Health;
            item.Zombie.mHelmMaxHealth = item.Maximum;
        }
    }
}

[HarmonyPatch(typeof(Plant), nameof(Plant.UpdateMagnetShroom))]
internal static class NativeArmorMagnetSelectionPatch
{
    private static void Prefix(Plant __instance, out List<NativeArmorMagnet.Borrowed>? __state) =>
        __state = NativeArmorMagnet.Begin(__instance);
    private static void Finalizer(List<NativeArmorMagnet.Borrowed>? __state) => NativeArmorMagnet.Restore(__state);
}
