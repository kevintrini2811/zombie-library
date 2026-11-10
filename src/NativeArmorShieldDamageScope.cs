using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace ZombieLibrary;

// Native TakeDamage forwards its original argument to TakeHelmDamage after a
// shield, although TakeShieldDamage returned the remaining damage. These native
// families normally have no helmet; compose only our added helmet with that
// actual native remainder. All damage/effect methods still execute natively.
internal static class NativeArmorShieldDamageScope
{
    internal sealed class Hit
    {
        internal readonly IntPtr Pointer, Board;
        internal readonly uint Id;
        internal readonly int Marker;
        internal int? ShieldOverflow;
        internal Hit(Zombie zombie)
        { Pointer = zombie.Pointer; Board = zombie.mBoard?.Pointer ?? IntPtr.Zero; Id = zombie.DataID; Marker = zombie.mGraveX; }
    }

    [ThreadStatic] private static Hit? current;
    private static bool AddedHelmet(Zombie zombie) =>
        zombie.mHelmType is HelmType.TrafficCone or HelmType.Pail
        && NativeArmoredZombies.TryKind(zombie, out _);
    private static bool Matches(Zombie zombie) => current is { } value
        && value.Pointer == zombie.Pointer && value.Id == zombie.DataID && value.Board == zombie.mBoard?.Pointer
        && value.Marker == zombie.mGraveX;

    internal static Hit? Begin(Zombie zombie)
    {
        var previous = current;
        // Even an ineligible nested hit must hide its caller's pending result.
        current = zombie.mShieldHealth > 0 && AddedHelmet(zombie) ? new Hit(zombie) : null;
        return previous;
    }
    internal static void End(Hit? previous) => current = previous;
    internal static void Record(Zombie zombie, int overflow, DamageFlags flags)
    {
        if (Matches(zombie) && (flags & (DamageFlags.BypassesShield | DamageFlags.HitsShieldAndBody)) == 0)
            current!.ShieldOverflow = overflow;
    }
    internal static void Forward(Zombie zombie, ref int damage)
    {
        if (!Matches(zombie) || !AddedHelmet(zombie) || current!.ShieldOverflow is not { } overflow) return;
        current.ShieldOverflow = null;
        damage = overflow;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeDamage))]
internal static class NativeArmorShieldHitPatch
{
    private static void Prefix(Zombie __instance, out NativeArmorShieldDamageScope.Hit? __state) =>
        __state = NativeArmorShieldDamageScope.Begin(__instance);
    private static void Finalizer(NativeArmorShieldDamageScope.Hit? __state) => NativeArmorShieldDamageScope.End(__state);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeShieldDamage))]
internal static class NativeArmorShieldResultPatch
{
    private static void Postfix(Zombie __instance, int __result, DamageFlags __1) =>
        NativeArmorShieldDamageScope.Record(__instance, __result, __1);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeHelmDamage))]
internal static class NativeArmorShieldForwardPatch
{
    private static void Prefix(Zombie __instance, ref int __0) => NativeArmorShieldDamageScope.Forward(__instance, ref __0);
}
