using System.Text.Json;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppReloaded.Data;
using Il2CppReloaded.DataModels;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using MelonLoader.Utils;

namespace ZombieLibrary;

// Profile-scoped sidecar. Never change native unlocks, achievements or save enums.
internal static class Sightings
{
    private static readonly string FilePath = Path.Combine(MelonEnvironment.UserDataDirectory, "ZombieLibrary", "sightings.json");
    private static HashSet<string>? seen;
    private static bool storageFailed;
    internal static IUserService? User;
    internal static int Revision { get; private set; }
    internal static string? Profile => User?.ActiveUserProfile?.mGuid;
    private static string NativeKey(ZombieType type) => "native/" + (int)type;
    private static string Key(ZombieDefinition definition, bool duck) => definition.Mod == Zombotany.Catalog
        ? NativeKey(definition.Identity.NativeType)
        : "mod/" + definition.Mod.Id + "/" + definition.Id + (duck ? "/duck" : "/land");

    private static void Load()
    {
        if (seen != null) return;
        try
        {
            seen = File.Exists(FilePath) ? JsonSerializer.Deserialize<HashSet<string>>(File.ReadAllText(FilePath))
                ?? throw new InvalidDataException("Invalid ZombieLibrary sightings file.") : new();
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or JsonException)
        {
            seen = new(); storageFailed = true;
            LibraryMod.Instance.LoggerInstance.Error($"Cannot read sightings; existing file retained: {error.Message}");
        }
    }

    private static bool Contains(string key)
    { Load(); return Profile is { Length: > 0 } profile && seen!.Contains(profile + "/" + key); }
    internal static bool Contains(ZombieDefinition definition, bool duck) => Contains(Key(definition, duck));
    internal static bool Contains(ZombieType type) => Contains(NativeKey(type));

    private static void Add(string key)
    {
        Load();
        if (Profile is not { Length: > 0 } profile || !seen!.Add(profile + "/" + key)) return;
        Revision++;
        if (storageFailed) return;
        string temporary = FilePath + ".tmp";
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(temporary, JsonSerializer.Serialize(seen));
            File.Move(temporary, FilePath, true);
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException)
        {
            storageFailed = true;
            LibraryMod.Instance.LoggerInstance.Error($"Cannot save sightings; session progress remains available: {error.Message}");
        }
        finally
        {
            try { if (File.Exists(temporary)) File.Delete(temporary); }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }

    internal static void Observe(Zombie zombie)
    {
        var board = zombie.mBoard;
        // Exclude level-intro previews, almanac actors and offscreen spawns.
        if (board?.mApp == null || Zombies.SuppressSightings?.Invoke(board) == true || board.mApp.GameScene != GameScenes.Playing || board.mPaused
            || zombie.mDead || !zombie.IsOnBoard() || !zombie.mVisible
            || zombie.mX >= Il2CppReloaded.Constants.Gameplay.BOARD_WIDTH || zombie.mX + zombie.mWidth <= 0
            || zombie.mY >= Il2CppReloaded.Constants.Gameplay.BOARD_HEIGHT || zombie.mY + zombie.mHeight <= 0) return;
        User = board.mApp.UserService;
        var definition = Zombies.Find(zombie);
        if (definition == null || definition.DuckOnly) Add(NativeKey(zombie.mZombieType));
        if (definition != null && (!definition.DuckOnly || zombie.mInPool)) Add(Key(definition, zombie.mInPool));
    }
}

[HarmonyPatch(typeof(Zombie), nameof(Zombie.Update))]
internal static class ZombieSightingsPatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(Zombie __instance) => Sightings.Observe(__instance);
}

// Native detail binders require stable paths such as almanac.zombies.all.0.
// Keep the complete native list. The native lock refresh builds `visible`
// from unlocked entries; sightings therefore belong in IsLocked below.
internal static class NativeZombieAlmanac
{
    private static AppDataProvider? provider;
    private static int revision = -1;
    private static string? profile;
    private static bool revealAll;

    internal static void Bind(AppDataProvider value)
    {
        provider = value;
        revision = -1;
        Update();
    }

    internal static void Unbind(Il2CppTekly.Extensions.DataProviders.RootModelDataProvider value)
    {
        if (provider?.Pointer == value.Pointer) Clear();
    }

    internal static void Clear() { provider = null; revision = -1; profile = null; }

    internal static void Update()
    {
        var almanac = provider?.m_almanacModel;
        if (almanac?.m_zombiesModel == null || almanac.m_dataService == null) return;
        Sightings.User = almanac.m_userService;
        string? currentProfile = Sightings.Profile;
        bool currentReveal = Zombies.RevealAlmanac;
        if (revision == Sightings.Revision && profile == currentProfile && revealAll == currentReveal) return;
        // Refresh in LateUpdate, outside Zombie.Update and native profile callbacks.
        // Preserve the model objects and their original selection/button indices.
        almanac.m_zombiesModel.UpdateEntriesLockedState(almanac.m_dataService.ZombieAlmanacData);
        revision = Sightings.Revision;
        profile = currentProfile;
        revealAll = currentReveal;
    }
}

[HarmonyPatch(typeof(AppDataProvider), "OnBind")]
internal static class NativeZombieAlmanacBindPatch
{
    private static void Postfix(AppDataProvider __instance) => NativeZombieAlmanac.Bind(__instance);
}

[HarmonyPatch(typeof(Il2CppTekly.Extensions.DataProviders.RootModelDataProvider), "Unbind")]
internal static class NativeZombieAlmanacUnbindPatch
{
    private static void Prefix(Il2CppTekly.Extensions.DataProviders.RootModelDataProvider __instance) =>
        NativeZombieAlmanac.Unbind(__instance);
}

[HarmonyPatch(typeof(UserService), nameof(UserService.IsLocked), new[] { typeof(AlmanacEntryData) })]
internal static class SeenZombieAvailabilityPatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(UserService __instance, AlmanacEntryData __0, ref bool __result)
    {
        Sightings.User = __instance.Cast<IUserService>();
        if (__0.EntryType == AlmanacEntryType.Zombie)
            // Keep Zombotany in its own tab even during a temporary reveal.
            __result = Zombotany.IsNative(__0.ZombieType)
                || !Zombies.RevealAlmanac && !Sightings.Contains(__0.ZombieType);
    }
}

[HarmonyPatch(typeof(AlmanacEntryModel), "EntryIsHidden")]
internal static class SeenZombieHiddenPatch
{
    private static void Postfix(AlmanacEntryModel __instance, ref bool __result)
    {
        var entry = __instance.Value;
        if (entry != null && entry.EntryType == AlmanacEntryType.Zombie && (Zombies.RevealAlmanac || Sightings.Contains(entry.ZombieType))) __result = false;
    }
}
