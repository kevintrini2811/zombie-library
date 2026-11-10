using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace ZombieLibrary;

// Draw offsets and clipping share the native board-pixel coordinate system.
// Do not change altitude, collision geometry, saved fields or animation clocks.
[HarmonyPatch(typeof(Zombie), nameof(Zombie.DrawReanim))]
internal static class PoolZombieDepthPatch
{
    private static void Prefix(Zombie __instance, ref ZombieDrawPosition theDrawPos) => Apply(__instance, ref theDrawPos);

    internal static void Apply(Zombie __instance, ref ZombieDrawPosition __result)
    {
        if (!__instance.mInPool || __instance.mZombieHeight == ZombieHeight.DraggedUnder
            || __instance.mZombiePhase == ZombiePhase.DancerRising) return;
        float depth = __instance.mZombieType switch
        {
            ZombieType.Imp => -22,
            ZombieType.Gargantuar or ZombieType.RedeyeGargantuar => 35,
            ZombieType.Dancer or ZombieType.BackupDancer or ZombieType.JackInTheBox or ZombieType.Football => 20,
            _ => 0
        };
        float scale = __instance.mScaleZombie;
        if (depth == 0 || !float.IsFinite(scale) || scale <= 0) return;
        // Fade the additional depth with native entry/exit; keep the waterline
        // fixed by moving ImageOffsetY and ClipHeight by precisely the same amount.
        // BodyY positions the top of the clip, not the controller body.
        float progress = Math.Clamp(-__instance.mAltitude / (40 * scale), 0, 1);
        float offset = depth * scale * progress;
        __result.ImageOffsetY += offset;
        __result.ClipHeight += offset;
    }
}

// Native swimming assumes a short zombie. Its finite upper clip edge can cut
// the carried Imp's head after a Garg is lowered into the water.
[HarmonyPatch(typeof(Il2CppSource.Controllers.ZombieController), nameof(Il2CppSource.Controllers.ZombieController.ClipRect))]
internal static class PoolGargantuarClipPatch
{
    private static void Prefix(Il2CppSource.Controllers.ZombieController __instance, ref UnityEngine.Rect clipRect)
    {
        var zombie = __instance.m_zombie;
        if (zombie == null || !zombie.mInPool
            || zombie.mZombieType is not (ZombieType.Gargantuar or ZombieType.RedeyeGargantuar)) return;
        // SetClipRect maps board pixels to the 2160-high reference frame at
        // 2.82 pixels per unit. Cover a full viewport above the waterline.
        // Rect.yMin preserves yMax: the lower water cut stays exactly native.
        clipRect.yMin = Math.Min(clipRect.yMin, clipRect.yMax - 2160f / 2.82f);
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.SummonBackupDancer))]
internal static class PoolSummonedDancerPatch
{
    internal static void Postfix(Zombie __instance, ZombieID __result)
    {
        var dancer = __instance.mBoard.ZombieTryToGet(__result);
        // The factory chooses an initial row before SummonBackupDancer moves
        // the actor. A borrowed initial pool state cannot describe that row.
        if (dancer != null && dancer.mZombiePhase == ZombiePhase.DancerRising)
            dancer.mInPool = false;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateZombieBackupDancer))]
internal static class PoolDancerRisePatch
{
    internal static void Prefix(Zombie __instance, out (uint Id, bool Rising) __state) =>
        __state = (__instance.DataID, __instance.mZombiePhase == ZombiePhase.DancerRising);

    internal static void Postfix(Zombie __instance, (uint Id, bool Rising) __state)
    {
        if (!__state.Rising || __instance.DataID != __state.Id || __instance.mDead
            || __instance.mZombiePhase == ZombiePhase.DancerRising) return;
        // Native rising ends at altitude zero. Start the ordinary water entry
        // afterwards, including ice checks and splash, in the final row.
        __instance.mAltitude = 0;
        __instance.mInPool = false;
        __instance.mZombieHeight = ZombieHeight.ZombieNormal;
        __instance.CheckForPool();
    }
}
