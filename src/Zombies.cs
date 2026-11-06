using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using UnityEngine;

namespace ZombieLibrary;

public enum MarkerField { GraveX, SummonCounter }

/// <summary>Native, serialized identity. Only fields unused by the selected native AI are supported.</summary>
public readonly record struct ZombieIdentity(ZombieType NativeType, MarkerField Field, int Marker)
{
    internal int Read(Zombie zombie) => Field == MarkerField.GraveX ? zombie.mGraveX : zombie.mSummonCounter;
    internal void Write(Zombie zombie, int value)
    {
        if (Field == MarkerField.GraveX) zombie.mGraveX = value;
        else zombie.mSummonCounter = value;
    }
}

public sealed record EntryText(string Name, string Info, string Flavor = "");

public sealed class ModCatalog
{
    public string Id { get; }
    public string Name { get; }
    public ModCatalog(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("A mod needs an ID and name.");
        Id = id; Name = name;
    }
}

public sealed class ZombieDefinition
{
    public ModCatalog Mod { get; }
    public string Id { get; }
    public ZombieIdentity Identity { get; }
    public EntryText English { get; }
    public EntryText German { get; }
    public bool RubberDuck { get; }
    public bool DuckOnly { get; internal init; }
    public Action<Zombie>? Initialize { get; set; }
    public Action<Zombie>? Tick { get; set; }
    /// <summary>Called once per frame before duck rendering, including when a board is paused.</summary>
    public Action? RefreshVisuals { get; set; }
    /// <summary>Borrowed sprite. The registering mod owns its lifetime.</summary>
    public Func<bool, Sprite?>? Portrait { get; set; }
    /// <summary>Optional complete native-style card thumbnail; arguments are German language and duck variant.</summary>
    public Func<bool, bool, Sprite?>? Thumbnail { get; set; }
    /// <summary>Optional pool portrait; otherwise the library adds its native duck artwork.</summary>
    public Func<bool, Sprite?>? DuckPortrait { get; set; }
    /// <summary>Optional custom rig placement. Return false until the rig is ready.</summary>
    public Func<Zombie, SpriteRenderer, bool>? PlacePoolDuck { get; set; }
    public string? SpawnSound { get; set; }
    internal ZombieDefinition(ModCatalog mod, string id, ZombieIdentity identity, EntryText english, EntryText german, bool duck)
    { Mod = mod; Id = id; Identity = identity; English = english; German = german; RubberDuck = duck; }
}

public static class Zombies
{
    /// <summary>Optional session-only almanac reveal, for example while a debug level is active.
    /// Does not record sightings or change profiles. Clear the callback when its owner unloads.</summary>
    public static Func<bool>? RevealAllAlmanacEntries { get; set; }
    /// <summary>Exclude temporary boards from persistent sightings. Clear when the owner unloads.</summary>
    public static Func<Board, bool>? SuppressSightings { get; set; }
    internal static bool RevealAlmanac => RevealAllAlmanacEntries?.Invoke() == true;

    private static readonly List<ModCatalog> catalogs = new();
    private static readonly List<ZombieDefinition> definitions = new();
    private static readonly Dictionary<string, Action<Zombie>> sounds = new(StringComparer.Ordinal);
    private static readonly HashSet<ZombieType> nativeDucks = new();
    public static IReadOnlyList<ModCatalog> Catalogs => catalogs.AsReadOnly();
    public static IReadOnlyList<ZombieDefinition> All => definitions.AsReadOnly();

    public static void RegisterCatalog(ModCatalog mod)
    {
        ArgumentNullException.ThrowIfNull(mod);
        if (catalogs.Any(item => item.Id == mod.Id)) throw new InvalidOperationException($"Duplicate mod ID: {mod.Id}");
        catalogs.Add(mod);
    }

    public static ZombieDefinition RegisterZombie(ModCatalog mod, string id, ZombieIdentity identity,
        EntryText english, EntryText german, bool rubberDuck = false)
    {
        if (!catalogs.Contains(mod)) throw new ArgumentException("Register the catalog first.");
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Zombie ID cannot be empty.");
        ArgumentNullException.ThrowIfNull(english); ArgumentNullException.ThrowIfNull(german);
        // Normal does not consume grave coordinates while walking. PeaHead
        // does not consume the dancer/imp summon counter. Never invent enum IDs.
        if (identity.Marker >= 0 || !((identity.NativeType is ZombieType.Normal or ZombieType.TrafficCone or ZombieType.Pail
                || NativeArmorTypes.Supports(identity.NativeType)) && identity.Field == MarkerField.GraveX
            || identity.NativeType == ZombieType.PeaHead && identity.Field == MarkerField.SummonCounter))
            throw new ArgumentException("Use a negative marker in a supported native GraveX field or PeaHead.SummonCounter.");
        if (definitions.Any(item => item.Identity == identity || item.Mod == mod && item.Id == id))
            throw new InvalidOperationException($"Duplicate zombie identity or ID: {mod.Id}:{id}");
        var definition = new ZombieDefinition(mod, id, identity, english, german, rubberDuck);
        definitions.Add(definition);
        return definition;
    }

    public static ZombieDefinition? Find(Zombie zombie) => definitions.FirstOrDefault(item => !item.DuckOnly
        && (item.Identity.NativeType == zombie.mZombieType || NativeArmorTypes.IsDetachedTrashCan(item.Identity, zombie))
        && item.Identity.Read(zombie) == item.Identity.Marker)
        ?? definitions.FirstOrDefault(item => item.DuckOnly && item.Identity.NativeType == zombie.mZombieType);

    /// <summary>At a custom AI's committed plant-bite event, preserve native
    /// Hypno-shroom consumption and hypnosis. Native AI already invokes this
    /// through its chew animation; custom bite clocks must forward the event.
    /// Returns true when the target was consumed or hypnosis committed, so
    /// ordinary bite damage must stop even if another mod resists hypnosis.</summary>
    public static bool TryHypnotizeBite(Zombie zombie, Plant plant)
    {
        if (Find(zombie) == null || zombie.mDead || zombie.IsDeadOrDying() || !zombie.mHasHead
            || zombie.mMindControlled || !zombie.mIsEating || zombie.mBoard == null || zombie.mBoard.mPaused
            || zombie.mIceTrapCounter > 0 || zombie.mButteredCounter > 0
            || plant == null || plant.mDead || !plant.mIsOnBoard || plant.mPlantHealth <= 0 || plant.mIsAsleep
            || plant.mSeedType != SeedType.Hypnoshroom || plant.mBoard?.Pointer != zombie.mBoard.Pointer)
            return false;
        var target = zombie.FindPlantTarget(ZombieAttackType.Chew);
        if (target == null || target.Pointer != plant.Pointer || target.DataID != plant.DataID) return false;
        // Reuse the complete native path: target rules, consumption, control,
        // sound, particles and callbacks from other mods. Never write the flag.
        var plantId = plant.DataID;
        zombie.AnimateChewSound();
        return plant.DataID != plantId || plant.mDead || zombie.mMindControlled;
    }

    /// <summary>Convert a fresh native actor. Loading uses Find, never Initialize or SpawnSound.</summary>
    public static void Apply(ZombieDefinition definition, Zombie zombie)
    {
        if (!definitions.Contains(definition)) throw new ArgumentException("Unregistered zombie definition.");
        if (definition.DuckOnly) throw new ArgumentException("Native duck entries use the native zombie factory.");
        if (zombie.mDead || zombie.mZombieType != definition.Identity.NativeType)
            throw new ArgumentException("Zombie must be alive and have the definition's native type.");
        if (Find(zombie) == definition) return;
        if (Find(zombie) is { DuckOnly: false }) throw new InvalidOperationException("Zombie already belongs to another definition.");
        if (definition.Identity.Read(zombie) != 0)
            throw new InvalidOperationException("The serialized marker field is already in use.");
        if (definition.SpawnSound != null && !sounds.ContainsKey(definition.SpawnSound))
            throw new KeyNotFoundException($"Unknown zombie sound: {definition.SpawnSound}");
        definition.Identity.Write(zombie, definition.Identity.Marker);
        definition.Initialize?.Invoke(zombie);
        if (definition.SpawnSound != null) PlaySound(definition.SpawnSound, zombie);
        if (definition.RubberDuck) zombie.CheckForPool();
    }

    public static Zombie? Spawn(ZombieDefinition definition, Board board, int row, int wave = -1)
    {
        if (!definitions.Contains(definition)) throw new ArgumentException("Unregistered zombie definition.");
        if (definition.DuckOnly) throw new ArgumentException("Native duck entries use the native zombie factory.");
        if (row < 0 || row >= board.GetNumRows())
            throw new ArgumentOutOfRangeException(nameof(row));
        // A custom spawn cannot accidentally win another mod's random replacement.
        explicitSpawn++;
        Zombie? zombie = null;
        try
        {
            zombie = board.AddZombieInRow(definition.Identity.NativeType, row, wave, false);
            if (zombie != null && !zombie.mDead) Apply(definition, zombie);
            return zombie;
        }
        catch { if (zombie != null && !zombie.mDead) zombie.DieNoLoot(); throw; }
        finally { explicitSpawn--; }
    }

    [ThreadStatic] private static int explicitSpawn;
    public static bool IsExplicitSpawn => explicitSpawn > 0;

    public static void RegisterSound(string id, Action<Zombie> play)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Sound ID cannot be empty.");
        ArgumentNullException.ThrowIfNull(play);
        if (!sounds.TryAdd(id, play)) throw new InvalidOperationException($"Duplicate sound ID: {id}");
    }
    public static void RegisterSound(string id, FoleyType sound) =>
        RegisterSound(id, zombie => zombie.mBoard.mApp.SoundSystem.PlayFoley(sound));
    public static void PlaySound(string id, Zombie zombie)
    {
        if (!sounds.TryGetValue(id, out var play)) throw new KeyNotFoundException($"Unknown zombie sound: {id}");
        if (zombie.mBoard?.mApp != null) play(zombie);
    }

    /// <summary>Opt an additional, supported native rig into swimming and native duck rendering.</summary>
    public static void RegisterNativeDuck(ZombieType type)
    {
        if (type is not (ZombieType.Football or ZombieType.JackInTheBox or ZombieType.Dancer
            or ZombieType.BackupDancer or ZombieType.Yeti or ZombieType.Gargantuar
            or ZombieType.RedeyeGargantuar or ZombieType.SquashHead or ZombieType.PeaHead
            or ZombieType.Newspaper or ZombieType.Door or ZombieType.Bobsled or ZombieType.Balloon
            or ZombieType.Pogo or ZombieType.Ladder or ZombieType.Imp or ZombieType.Target or ZombieType.TrashCan))
            throw new ArgumentException("This native rig has no supported duck placement.");
        nativeDucks.Add(type);
    }
    /// <summary>Add a catalog entry for a native zombie's additional pool variant.</summary>
    public static ZombieDefinition RegisterDuckVariant(ModCatalog mod, string id, ZombieType type, EntryText english, EntryText german)
    {
        if (!catalogs.Contains(mod)) throw new ArgumentException("Register the catalog first.");
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Zombie ID cannot be empty.");
        ArgumentNullException.ThrowIfNull(english); ArgumentNullException.ThrowIfNull(german);
        if (definitions.Any(item => item.Mod == mod && item.Id == id || item.DuckOnly && item.Identity.NativeType == type))
            throw new InvalidOperationException($"Duplicate duck identity or ID: {mod.Id}:{id}");
        RegisterNativeDuck(type);
        var definition = new ZombieDefinition(mod, id, new(type, MarkerField.GraveX, 0), english, german, true) { DuckOnly = true };
        definitions.Add(definition);
        return definition;
    }
    internal static bool HasNativeDuck(ZombieType type) => nativeDucks.Contains(type);
    internal static bool HasPoolType(ZombieType type) => HasNativeDuck(type)
        || definitions.Any(item => item.RubberDuck && item.Identity.NativeType == type);
    internal static void Tick(Zombie zombie)
    {
        if (zombie.mBoard == null || zombie.mBoard.mPaused) return;
        Find(zombie)?.Tick?.Invoke(zombie);
    }
    /// <summary>Transfer the native body displacement to imported artwork after
    /// anchoring its ground node. Includes grave rises and pool entry/exit.
    /// Call once after resetting the ground anchor each frame; never on native rigs.</summary>
    public static void ApplyBodyDisplacement(Zombie zombie, Transform artwork)
    {
        if (zombie.mZombiePhase == ZombiePhase.RisingFromGrave || zombie.mInPool
            || zombie.mZombieHeight == ZombieHeight.OutOfPool)
            artwork.position -= artwork.TransformVector(Vector3.up).normalized
                * (zombie.GetDrawPos().BodyY * 2.82f);
    }

    /// <summary>Shared clipping plane for imported bodies. Grave rises use the
    /// native ground anchor, swimming uses the library's native waterline.</summary>
    public static bool BodyClipPlane(Zombie zombie, Transform artwork, Vector3 ground, out Vector3 plane, out Vector3 up)
    {
        if (PoolDuckVisuals.Waterline(zombie, artwork, out plane, out up)) return true;
        plane = ground;
        up = artwork.TransformVector(Vector3.up).normalized;
        return zombie.mZombiePhase == ZombiePhase.RisingFromGrave;
    }

    internal static void RefreshVisuals()
    { foreach (var definition in definitions) definition.RefreshVisuals?.Invoke(); }
    internal static void Reset(Zombie zombie)
    {
        // A pooled actor's native type may already describe its next life.
        // Match only exact registered field/marker pairs, never a sign/range or
        // the current type. Duck-only entries own no serialized field at all.
        foreach (var definition in definitions)
        {
            if (definition.DuckOnly || definition.Identity.Read(zombie) != definition.Identity.Marker) continue;
            definition.Identity.Write(zombie, 0);
            // GraveX variants own the paired auxiliary state (Eyes state or
            // Bobsled's outer helmet HP). Other fields/counters stay native.
            if (definition.Identity.Field == MarkerField.GraveX) zombie.mGraveY = 0;
        }
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.Update))]
internal static class ZombieTickPatch
{ private static void Postfix(Zombie __instance) => Zombies.Tick(__instance); }

[HarmonyPatch(typeof(Zombie), nameof(Zombie.Reset))]
internal static class ZombieResetPatch
{ private static void Prefix(Zombie __instance) => Zombies.Reset(__instance); }

// Native ZombieInitialize leaves legacy grave coordinates intact. This is the
// fresh-life boundary for every allocator, independently of pool Reset timing.
// Deserialization restores saved identity afterwards and never invokes Apply.
[HarmonyPatch(typeof(Zombie), nameof(Zombie.ZombieInitialize))]
internal static class ZombieInitializeMarkerResetPatch
{
    [HarmonyPriority(Priority.First)]
    private static void Prefix(Zombie __instance) => Zombies.Reset(__instance);
}
