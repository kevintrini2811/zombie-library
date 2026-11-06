using Il2CppReloaded.Gameplay;
using ReplantedModUi;
using UnityEngine;

namespace ZombieLibrary;

// Native pool support belongs to the library, independently of Plant Fusion.
internal static class NativeDuckVariants
{
    private static readonly Dictionary<string, Sprite> thumbnails = new();
    private static Sprite Thumbnail(string key, bool duck)
    {
        string file = key + (duck ? "-duck" : "") + ".png";
        if (thumbnails.TryGetValue(file, out var cached)) return cached;
        using var stream = typeof(NativeDuckVariants).Assembly.GetManifestResourceStream("ZombieLibrary.DuckCards." + file)
            ?? throw new InvalidDataException(file);
        using var bytes = new MemoryStream(); stream.CopyTo(bytes);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        ImageConversion.LoadImage(texture, bytes.ToArray(), false);
        texture.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100);
        sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        thumbnails.Add(file, sprite); return sprite;
    }
    internal static void Register()
    {
        var mod = new ModCatalog("native-ducks", "Native Pool Zombies");
        Zombies.RegisterCatalog(mod);
        foreach (var (type, english, german) in new[] {
            (ZombieType.Newspaper, "Newspaper Zombie", "Zeitungszombie"),
            (ZombieType.Door, "Screen Door Zombie", "Fliegengitterzombie"),
            (ZombieType.Bobsled, "Zombie Bobsled Team", "Zombie-Bobteam"),
            (ZombieType.Balloon, "Balloon Zombie", "Ballonzombie"),
            (ZombieType.Pogo, "Pogo Zombie", "Pogozombie"),
            (ZombieType.Ladder, "Ladder Zombie", "Leiterzombie"),
            (ZombieType.Imp, "Imp", "Wicht"),
            (ZombieType.Target, "Target Zombie", "Zombie-Ziel"),
            (ZombieType.TrashCan, "Trash Can Zombie", "Mülleimerzombie"),
            (ZombieType.Football, "Football Zombie", "Football-Zombie"),
            (ZombieType.JackInTheBox, "Jack-in-the-Box Zombie", "Springteufel-Zombie"),
            (ZombieType.Dancer, "Dancing Zombie", "Tänzer-Zombie"),
            (ZombieType.BackupDancer, "Backup Dancer", "Background-Tänzer"),
            (ZombieType.Yeti, "Zombie Yeti", "Zombie-Yeti"),
            (ZombieType.Gargantuar, "Gargantuar", "Gargantuar"),
            (ZombieType.RedeyeGargantuar, "Giga-gargantuar", "Giga-Gargantuar"),
            (ZombieType.SquashHead, "Squash Zombie", "Kürbis-Zombie") })
        {
            var duck = Zombies.RegisterDuckVariant(mod, "duck-" + type, type,
                new(english, "Native zombie with a ducky tube. Retains native health, attacks and behavior."),
                new(german, "Nativer Zombie mit Gummiente. Lebenspunkte, Angriffe und Verhalten bleiben erhalten."));
            duck.Portrait = _ => Thumbnail(((int)type).ToString()+"-portrait", false);
            duck.Thumbnail = (_, swimming) => Thumbnail(((int)type).ToString(), swimming);
        }
    }
    internal static void Dispose()
    {
        foreach (var sprite in thumbnails.Values) { UnityEngine.Object.Destroy(sprite.texture); UnityEngine.Object.Destroy(sprite); }
        thumbnails.Clear();
    }
}
