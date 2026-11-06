using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace ZombieLibrary;

// Only UpdateZombieBobsled's synchronous TakeDamage(6, DoesntCauseFlash)
// represents sled wear. The same flags/damage outside this call still hit hats.
internal static class NativeBobsledDamageScope
{
    internal readonly record struct Life(IntPtr Pointer, uint Id, IntPtr Board, bool Wear);
    [ThreadStatic] private static Life? updating;
    [ThreadStatic] private static Life? hit;
    private static Life Capture(Zombie zombie, bool wear) => new(zombie.Pointer, zombie.DataID,
        zombie.mBoard?.Pointer ?? IntPtr.Zero, wear);
    private static bool Matches(Life? life, Zombie zombie) => life is { } value
        && value.Pointer == zombie.Pointer && value.Id == zombie.DataID && value.Board == zombie.mBoard?.Pointer;
    internal static Life? BeginUpdate(Zombie zombie)
    { var previous = updating; updating = Capture(zombie, false); return previous; }
    internal static void EndUpdate(Life? previous) => updating = previous;
    internal static Life? BeginHit(Zombie zombie, int damage, DamageFlags flags)
    {
        var previous = hit;
        hit = Capture(zombie, Matches(updating, zombie) && damage == 6 && flags == DamageFlags.DoesntCauseFlash);
        return previous;
    }
    internal static void EndHit(Life? previous) => hit = previous;
    internal static bool IsWear(Zombie zombie) => Matches(hit, zombie) && hit!.Value.Wear;
    internal static bool IsHit(Zombie zombie) => Matches(hit, zombie);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateZombieBobsled))]
internal static class NativeBobsledWearUpdatePatch
{
    private static void Prefix(Zombie __instance, out NativeBobsledDamageScope.Life? __state) =>
        __state = NativeBobsledDamageScope.BeginUpdate(__instance);
    private static void Finalizer(NativeBobsledDamageScope.Life? __state) => NativeBobsledDamageScope.EndUpdate(__state);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class NativeBobsledHitScopePatch
{
    private static void Prefix(Zombie __instance, int __0, DamageFlags __1, out NativeBobsledDamageScope.Life? __state) =>
        __state = NativeBobsledDamageScope.BeginHit(__instance, __0, __1);
    private static void Finalizer(NativeBobsledDamageScope.Life? __state) => NativeBobsledDamageScope.EndHit(__state);
}

// After a native sled crash, TakeDamage skips TakeHelmDamage when the native
// sled helmet is gone. Preserve its native fields and protect the surviving
// extra hat at the body stage of this same normal hit, never on direct body
// damage calls (which intentionally bypass helmets).
[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeBodyDamage))]
internal static class NativeBobsledCrashedHatPatch
{
    private static bool Prefix(Zombie __instance, ref int __0, DamageFlags __1)
    {
        if (!NativeBobsledDamageScope.IsHit(__instance) || __instance.mHelmType != HelmType.None
            || !NativeArmoredZombies.AbsorbBobsled(__instance, ref __0, __1)) return true;
        return __0 > 0;
    }
}
