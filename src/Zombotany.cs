using Il2CppReloaded.Gameplay;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZombieLibrary;

internal static class Zombotany
{
    internal static readonly ModCatalog Catalog = new("zombie-library:zombotany", "Zombotany");
    private static readonly List<ZombieDefinition> entries = new();
    private static readonly Dictionary<ZombieType, Sprite> portraits = new();
    internal static IReadOnlyList<ZombieDefinition> Entries => entries;
    internal static bool IsNative(ZombieType type) => type is ZombieType.PeaHead or ZombieType.WallnutHead
        or ZombieType.JalapenoHead or ZombieType.GatlingHead or ZombieType.SquashHead or ZombieType.TallnutHead;

    internal static void Register()
    {
        Zombies.RegisterCatalog(Catalog);
        Add(ZombieType.PeaHead, "Peashooter Zombie", "Erbsenkanonen-Zombie", "Shoots peas at your plants.", "Schießt Erbsen auf deine Pflanzen.");
        Add(ZombieType.WallnutHead, "Wall-nut Zombie", "Wallnuss-Zombie", "Its Wall-nut head absorbs heavy damage.", "Sein Wallnusskopf hält viel Schaden aus.");
        Add(ZombieType.JalapenoHead, "Jalapeno Zombie", "Jalapeno-Zombie", "Explodes and burns the plants in its lane.", "Explodiert und verbrennt die Pflanzen seiner Reihe.");
        Add(ZombieType.GatlingHead, "Gatling Pea Zombie", "Gatling-Erbsen-Zombie", "Fires bursts of peas at your plants.", "Feuert Erbsensalven auf deine Pflanzen.");
        Add(ZombieType.SquashHead, "Squash Zombie", "Kürbis-Zombie", "Crushes plants with its Squash head.", "Zerquetscht Pflanzen mit seinem Kürbiskopf.");
        Add(ZombieType.TallnutHead, "Tall-nut Zombie", "Riesenwallnuss-Zombie", "Its Tall-nut head provides exceptional protection.", "Sein Riesenwallnusskopf bietet besonders starken Schutz.");
    }

    private static void Add(ZombieType type, string english, string german, string info, string germanInfo)
    {
        // Presentation only. Native actors retain their factory, AI and save identity.
        var entry = new ZombieDefinition(Catalog, type.ToString(), new(type, MarkerField.GraveX, 0),
            new(english, info), new(german, germanInfo), false);
        entry.Portrait = _ => Portrait(type);
        entries.Add(entry);
    }

    private static Sprite Portrait(ZombieType type)
    {
        if (portraits.TryGetValue(type, out var sprite)) return sprite;
        using var stream = typeof(Zombotany).Assembly.GetManifestResourceStream($"ZombieLibrary.Assets.zombotany-{(int)type}.png")
            ?? throw new InvalidDataException($"Missing Zombotany portrait: {type}");
        using var bytes = new MemoryStream(); stream.CopyTo(bytes);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false) { hideFlags = HideFlags.HideAndDontSave };
        if (!ImageConversion.LoadImage(texture, bytes.ToArray(), true))
        { Object.Destroy(texture); throw new InvalidDataException($"Invalid Zombotany portrait: {type}"); }
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100);
        sprite.hideFlags = HideFlags.HideAndDontSave;
        portraits.Add(type, sprite); return sprite;
    }

    internal static void Dispose()
    {
        foreach (var sprite in portraits.Values) { Object.Destroy(sprite.texture); Object.Destroy(sprite); }
        portraits.Clear();
    }
}
