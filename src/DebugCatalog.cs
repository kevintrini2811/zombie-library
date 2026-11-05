using Il2CppReloaded.Gameplay;
using ReplantedModUi;
using UnityEngine;

namespace ZombieLibrary;

internal static class DebugCatalog
{
    internal static IEnumerable<(ZombieType NativeType, string Name, Func<Sprite?> Portrait,
        Func<Sprite?> Thumbnail, Func<Board, int, Zombie?> Spawn)> Entries()
    {
        bool german = NativeLocalization.German;
        return Zombies.All.Select(definition => (definition.Identity.NativeType,
            german ? definition.German.Name : definition.English.Name,
            (Func<Sprite?>)(() => definition.Portrait?.Invoke(german)),
            (Func<Sprite?>)(() => definition.Thumbnail?.Invoke(german, definition.DuckOnly)
                ?? definition.Portrait?.Invoke(german)),
            (Func<Board, int, Zombie?>)((board, row) => definition.DuckOnly
                ? board.mPlantRow[row] == PlantRowType.Pool
                    ? board.AddZombieInRow(definition.Identity.NativeType, row, -1, false) : null
                : Zombies.Spawn(definition, board, row)))).ToArray();
    }

    private static readonly Dictionary<(string Owner, string Id, bool Duck), bool> almanacUnlocks = new();
    internal static int Revision { get; private set; }
    private static IEnumerable<ZombieDefinition> All => Zombies.All.Concat(Zombotany.Entries).Distinct();
    internal static bool CanView(ZombieDefinition zombie, bool duck) => almanacUnlocks.TryGetValue((zombie.Mod.Id, zombie.Id, duck), out bool value)
        ? value : Zombies.RevealAlmanac || Sightings.Contains(zombie, duck);
    internal static void SetUnlock(string owner, string id, bool duck, bool unlocked)
    {
        var zombie = All.FirstOrDefault(z => z.Mod.Id == owner && z.Id == id)
            ?? throw new ArgumentException("Unknown registered zombie.");
        if (duck && !zombie.RubberDuck && !zombie.DuckOnly || !duck && zombie.DuckOnly)
            throw new ArgumentException("Unsupported zombie duck variant.");
        almanacUnlocks[(owner, id, duck)] = unlocked; ++Revision;
    }
    internal static void ResetUnlocks() { almanacUnlocks.Clear(); ++Revision; }
    internal static IEnumerable<(string Owner, string Id, string Name, string Info, ZombieType NativeType, bool Duck, bool Unlocked)> AlmanacEntries() =>
        All.SelectMany(zombie => (zombie.DuckOnly ? new[] { true } : zombie.RubberDuck ? new[] { false, true } : new[] { false })
            .Select(duck => (zombie.Mod.Id, zombie.Id, NativeLocalization.German ? zombie.German.Name : zombie.English.Name,
                NativeLocalization.German ? zombie.German.Info : zombie.English.Info, zombie.Identity.NativeType, duck, CanView(zombie, duck)))).ToArray();
}
