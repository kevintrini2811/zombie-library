using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace ZombieLibrary;

// Both native pool allowlists are embedded at their call sites. Extend the
// actual row query and entry path, keeping every actor's identity and AI.
internal static class ZombiePoolRules
{
    internal static bool Extra(ZombieType type) => Zombies.HasPoolType(type);
    internal static bool Duck(Zombie zombie) => Zombies.HasNativeDuck(zombie.mZombieType) || Zombies.Find(zombie)?.RubberDuck == true;
}

[HarmonyPatch(typeof(Board), nameof(Board.RowCanHaveZombieType))]
internal static class ExtraZombiePoolRowPatch
{
    internal static bool Prefix(Board __instance, int theRow, ZombieType theZombieType, ref bool __result)
    {
        if (!ZombiePoolRules.Extra(theZombieType) || theRow < 0 || theRow >= __instance.GetNumRows()
            || __instance.mPlantRow[theRow] != PlantRowType.Pool) return true;
        // The level still selects its permitted types. This only adds water
        // rows; unavailable rows and a dancer's required neighbors stay blocked.
        __result = __instance.RowCanHaveZombies(theRow)
            && (theZombieType != ZombieType.Dancer
                || __instance.RowCanHaveZombies(theRow - 1) && __instance.RowCanHaveZombies(theRow + 1));
        return false;
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.CheckForPool))]
internal static class SquashZombiePoolPatch
{
    internal static bool Prefix(Zombie __instance, out (uint Id, ZombieType Type, bool Changed, bool InPool) __state)
    {
        // Rising dancers own their altitude until the native rise finishes.
        bool rising = __instance.mZombieType == ZombieType.BackupDancer
            && __instance.mZombiePhase == ZombiePhase.DancerRising;
        bool flying = __instance.mZombieType == ZombieType.Balloon
            && (__instance.mZombiePhase is ZombiePhase.BalloonFlying or ZombiePhase.BalloonPopping
                || __instance.mZombieHeight == ZombieHeight.Falling);
        if (!rising && !flying && ZombiePoolRules.Extra(__instance.mZombieType)
            && __instance.mBoard != null && !__instance.mDead
            && __instance.mRow >= 0 && __instance.mRow < __instance.mBoard.GetNumRows()
            && __instance.mPosX >= 0 && __instance.mPosX < Il2CppReloaded.Constants.Gameplay.BOARD_WIDTH
            && __instance.mBoard.IsPoolSquare(__instance.mBoard.PixelToGridX((int)__instance.mPosX + 60, (int)__instance.mPosY), __instance.mRow))
        {
            if (__instance.mZombieType == ZombieType.Pogo && __instance.mHasObject)
            { __instance.PogoBreak(0); __instance.mAltitude = 0; }
            if (__instance.mZombieType == ZombieType.Bobsled && __instance.IsBobsledTeamWithSled()
                && __instance.mIsBobsledLeader && __instance.mController != null)
                PoolBobsled.ReadyCrash(__instance);
        }
        bool change = !rising && !flying && ZombiePoolRules.Extra(__instance.mZombieType);
        __state = (__instance.DataID, __instance.mZombieType, change, __instance.mInPool);
        if (change) __instance.mZombieType = ZombieType.Normal;
        return !rising && !flying;
    }

    internal static void Finalizer(Zombie __instance, (uint Id, ZombieType Type, bool Changed, bool InPool) __state)
    {
        if (!__state.Changed || __instance.DataID != __state.Id) return;
        __instance.mZombieType = __state.Type;
        // Water entry borrows Normal only for the native allowlist. Its skin
        // transition removes held-shield attachments; restore the real owner's
        // shield and arm layers after restoring the actual zombie type.
        if (__instance.mInPool != __state.InPool && !__instance.mDead && __instance.mController != null
            && __instance.mShieldHealth > 0 && __state.Type is ZombieType.Door or ZombieType.TrashCan)
            __instance.AttachShield();
    }
}


// LandFlyer has an embedded water death branch, independent of CheckForPool.
// Borrow only its terrain read so native popping and falling remain intact.
[HarmonyPatch(typeof(Zombie), nameof(Zombie.LandFlyer))]
internal static class PoolBalloonLandingPatch
{
    private static void Prefix(Zombie __instance, out (Board? Board, int Row, PlantRowType Terrain) __state)
    {
        __state = default;
        var board = __instance.mBoard;
        int row = __instance.mRow;
        if (__instance.mZombieType != ZombieType.Balloon || !Zombies.HasNativeDuck(ZombieType.Balloon)
            || board == null || row < 0 || row >= board.GetNumRows() || board.mPlantRow[row] != PlantRowType.Pool) return;
        __state = (board, row, board.mPlantRow[row]);
        board.mPlantRow[row] = PlantRowType.Normal;
    }
    private static void Finalizer((Board? Board, int Row, PlantRowType Terrain) __state)
    { if (__state.Board != null) __state.Board.mPlantRow[__state.Row] = __state.Terrain; }
}
[HarmonyPatch(typeof(Zombie), nameof(Zombie.UpdateZombieFalling))]
internal static class PoolBalloonFallenPatch
{
    private static void Postfix(Zombie __instance)
    {
        if (!__instance.mDead && __instance.mZombieType == ZombieType.Balloon
            && Zombies.HasNativeDuck(ZombieType.Balloon) && __instance.mZombieHeight == ZombieHeight.ZombieNormal)
            __instance.CheckForPool();
    }
}

internal static class PoolBobsled
{
    internal static void ReadyCrash(Zombie leader)
    {
        var ids = leader.mFollowerZombieID;
        if (ids == null || ids.Length < 3) return;
        var followers = new List<Zombie>();
        foreach (var id in ids)
        {
            var follower = leader.mBoard.ZombieTryToGet(id);
            if (follower == null || follower.mDead || follower.mController == null) return;
            followers.Add(follower);
        }
        leader.BobsledCrash();
        foreach (var follower in followers) follower.CheckForPool();
    }
}
