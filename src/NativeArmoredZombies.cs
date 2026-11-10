using HarmonyLib;
using Il2CppReloaded.Gameplay;

namespace ZombieLibrary;

// Registration and native gameplay integration share one owner. The native
// actor retains its AI, summon counters, equipment, movement and serialization.
internal static class NativeArmoredZombies
{
    private static readonly ModCatalog catalog = new("native-armored", "Armored Native Zombies");
    private static readonly Dictionary<(ZombieType Type, bool Bucket), ZombieDefinition> definitions = new();
    private static readonly Dictionary<ZombieDefinition, bool> kinds = new();

    internal static void Register()
    {
        Zombies.RegisterCatalog(catalog);
        foreach (var type in NativeArmorTypes.All)
        foreach (bool bucket in new[] { false, true })
        {
            string armor = bucket ? "Buckethead" : "Conehead";
            string germanArmor = bucket ? "Eimer" : "Pylonen";
            int health = NativeArmorTypes.Health(bucket);
            var definition = Zombies.RegisterZombie(catalog, NativeArmorTypes.Id(type.Type, bucket),
                new(type.Type, MarkerField.GraveX, NativeArmorTypes.Marker(type.Type, bucket)),
                new($"{armor} {type.English}", $"{health} additional helmet health. Retains the native zombie's abilities and equipment."),
                new($"{germanArmor}-{type.German}", $"{health} zusätzliche Helm-Lebenspunkte. Behält Fähigkeiten und Ausrüstung des nativen Zombies."),
                rubberDuck: true);
            bool armoredBucket = bucket;
            definition.Initialize = zombie => Initialize(zombie, armoredBucket);
            definition.Tick = Tick;
            definition.Portrait = _ => NativeArmorVisuals.Portrait(type.Type, armoredBucket);
            definitions.Add((type.Type, bucket), definition);
            kinds.Add(definition, bucket);
        }
    }

    internal static bool TryKind(Zombie zombie, out bool bucket)
    {
        bucket = false;
        var definition = Zombies.Find(zombie);
        return definition != null && kinds.TryGetValue(definition, out bucket);
    }

    internal static void Initialize(Zombie zombie, bool bucket)
    {
        int health = NativeArmorTypes.Health(bucket);
        if (zombie.mZombieType == ZombieType.Bobsled && zombie.mHelmType == HelmType.Bobsled)
        {
            // Bobsled's native helmet is the sled itself. Its health, thirds
            // and DropHelm crash transition must never become the added hat.
            zombie.mGraveY = health;
        }
        else
        {
            zombie.mHelmType = bucket ? HelmType.Pail : HelmType.TrafficCone;
            zombie.mHelmHealth = zombie.mHelmMaxHealth = health;
        }
        NativeArmorVisuals.Observe(zombie);
        BindBobsledGroup(zombie, bucket);
    }

    internal static void Tick(Zombie zombie)
    {
        if (TryKind(zombie, out bool bucket)) BindBobsledGroup(zombie, bucket);
        NativeArmorVisuals.Observe(zombie);
    }

    private static void BindBobsledGroup(Zombie zombie, bool bucket)
    {
        // Native follower IDs establish ownership before the cached leader
        // flag becomes true. Retry at Tick for links installed after Apply.
        if (zombie.mZombieType == ZombieType.Bobsled && zombie.mBoard != null && zombie.mFollowerZombieID != null)
            foreach (var id in zombie.mFollowerZombieID)
            {
                var follower = zombie.mBoard.ZombieTryToGet(id);
                if (follower != null && !follower.mDead && !follower.IsDeadOrDying()
                    && follower.mBoard?.Pointer == zombie.mBoard.Pointer && follower.mZombieType == ZombieType.Bobsled
                    && follower.mGraveX == 0)
                    Zombies.Apply(definitions[(ZombieType.Bobsled, bucket)], follower);
            }
    }

    internal static int Remaining(Zombie zombie) => zombie.mZombieType == ZombieType.Bobsled && zombie.mGraveY > 0
        ? zombie.mGraveY
        : zombie.mHelmType is HelmType.TrafficCone or HelmType.Pail ? Math.Max(0, zombie.mHelmHealth) : 0;

    internal static void ReplaceNatural(Zombie zombie, Zombie? parent)
    {
        var board = zombie.mBoard;
        if (Zombies.IsExplicitSpawn || board == null || board.mApp == null || board.mPaused
            || board.mApp.GameScene != GameScenes.Playing || zombie.mFromWave < 0
            || zombie.mDead || zombie.mMindControlled || zombie.mGraveX != 0
            || !NativeArmorTypes.Supports(zombie.mZombieType)
            || Zombies.SuppressSightings?.Invoke(board) == true) return;
        var allowed = board.mZombieAllowed;
        bool Allowed(ZombieType type) => allowed != null && (int)type < allowed.Length && allowed[(int)type];
        bool cone = Allowed(ZombieType.TrafficCone), bucket = Allowed(ZombieType.Pail);
        if (!cone && !bucket) return;
        if (parent != null && zombie.mZombieType == ZombieType.Bobsled && parent.mZombieType == ZombieType.Bobsled
            && TryKind(parent, out bool parentBucket) && (parentBucket ? bucket : cone))
        { Zombies.Apply(definitions[(ZombieType.Bobsled, parentBucket)], zombie); return; }
        if (board.Rand(3) != 0) return;
        // The actual native spawn proves the base type occurs here, including
        // flag waves, summoned dancers/imps and bobsled followers. Requiring its
        // own allowed bit would wrongly exclude those implicit native spawns.
        bool useBucket = bucket && (!cone || board.Rand(2) == 0);
        Zombies.Apply(definitions[(zombie.mZombieType, useBucket)], zombie);
    }

    internal static bool AbsorbBobsled(Zombie zombie, ref int damage, DamageFlags flags)
    {
        if (damage <= 0 || !zombie.mHasHead || zombie.mZombieType != ZombieType.Bobsled || zombie.mGraveY <= 0
            || !TryKind(zombie, out bool bucket) || NativeBobsledDamageScope.IsWear(zombie)) return false;
        int absorbed = Math.Min(damage, zombie.mGraveY);
        zombie.mGraveY -= absorbed;
        damage -= absorbed;
        if ((flags & DamageFlags.DoesntCauseFlash) == 0) zombie.mJustGotShotCounter = 25;
        if (damage == 0 && (flags & DamageFlags.Freeze) != 0) zombie.ApplyChill(false);
        if (zombie.mGraveY == 0) DropAddedBobsledHelmet(zombie, bucket, flags);
        return true;
    }
    internal static bool AbsorbBobsledEntry(Zombie zombie, ref int damage, DamageFlags flags)
    {
        if (!NativeBobsledDamageScope.IndependentEntry(zombie, out bool valid))
            return AbsorbBobsled(zombie, ref damage, flags);
        if (!valid) { damage = 0; return true; }
        if (!NativeBobsledDamageScope.ClaimIndependentArmor()) return false;
        // A simultaneous source attack hits the added hat with the amount
        // that reached this boundary after ordinary outer damage modifiers.
        // Native sled/body handling receives its own complete argument.
        int addedArmorDamage = damage;
        AbsorbBobsled(zombie, ref addedArmorDamage, flags);
        if (!NativeBobsledDamageScope.IndependentEntry(zombie, out valid) || !valid)
        { damage = 0; return true; }
        return false;
    }
    private static void DropAddedBobsledHelmet(Zombie zombie, bool bucket, DamageFlags flags)
    {
        // Emit the ordinary native hat-drop effect without replacing/crashing
        // the sled. Its native HP and fade/damage image remain authoritative.
        uint id = zombie.DataID;
        var board = zombie.mBoard?.Pointer;
        var helmet = zombie.mHelmType; int health = zombie.mHelmHealth, maximum = zombie.mHelmMaxHealth;
        try
        {
            zombie.mHelmType = bucket ? HelmType.Pail : HelmType.TrafficCone;
            zombie.mHelmHealth = 0; zombie.mHelmMaxHealth = NativeArmorTypes.Health(bucket);
            zombie.DropHelm(flags);
        }
        finally
        {
            if (zombie.DataID == id && zombie.mBoard?.Pointer == board)
            { zombie.mHelmType = helmet; zombie.mHelmHealth = health; zombie.mHelmMaxHealth = maximum; }
        }
    }

}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.ZombieInitialize))]
internal static class NativeArmoredSpawnPatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(Zombie __instance, Zombie theParentZombie) => NativeArmoredZombies.ReplaceNatural(__instance, theParentZombie);
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.Deserialize))]
internal static class NativeArmoredLoadPatch
{
    private static void Postfix(Zombie __instance)
    { if (NativeArmoredZombies.TryKind(__instance, out _)) NativeArmorVisuals.Observe(__instance); }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.TakeHelmDamage))]
internal static class NativeArmoredBobsledDamagePatch
{
    private static bool Prefix(Zombie __instance, ref int theDamage, DamageFlags theDamageFlags, ref int __result)
    {
        if (!NativeArmoredZombies.AbsorbBobsledEntry(__instance, ref theDamage, theDamageFlags) || theDamage > 0) return true;
        __result = 0;
        return false;
    }
}
