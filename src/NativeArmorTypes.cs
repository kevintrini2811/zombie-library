using Il2CppReloaded.Gameplay;

namespace ZombieLibrary;

internal readonly record struct NativeArmorType(ZombieType Type, string English, string German);

// These identities retain the actual native AI enum. Grave coordinates are
// serialized legacy fields, unused by these actors in the supported game build.
internal static class NativeArmorTypes
{
    internal static readonly NativeArmorType[] All = {
        new(ZombieType.Flag, "Flag Zombie", "Flaggenzombie"),
        new(ZombieType.Polevaulter, "Pole Vaulting Zombie", "Stabhochsprungzombie"),
        new(ZombieType.Newspaper, "Newspaper Zombie", "Zeitungszombie"),
        new(ZombieType.Door, "Screen Door Zombie", "Fliegengitterzombie"),
        new(ZombieType.BackupDancer, "Backup Dancer", "Background-Tänzer"),
        new(ZombieType.Bobsled, "Zombie Bobsled Team", "Zombie-Bobteam"),
        new(ZombieType.DolphinRider, "Dolphin Rider Zombie", "Delfinreiterzombie"),
        new(ZombieType.JackInTheBox, "Jack-in-the-Box Zombie", "Springteufelzombie"),
        new(ZombieType.Pogo, "Pogo Zombie", "Pogozombie"),
        new(ZombieType.Yeti, "Zombie Yeti", "Zombie-Yeti"),
        new(ZombieType.Bungee, "Bungee Zombie", "Bungeezombie"),
        new(ZombieType.Ladder, "Ladder Zombie", "Leiterzombie"),
        new(ZombieType.Catapult, "Catapult Zombie", "Katapultzombie"),
        new(ZombieType.Gargantuar, "Gargantuar", "Gargantua"),
        new(ZombieType.Imp, "Imp", "Wicht"),
        new(ZombieType.TrashCan, "Trash Can Zombie", "Mülleimerzombie"),
        new(ZombieType.RedeyeGargantuar, "Giga-gargantuar", "Giga-Gargantua")
    };

    internal static bool Supports(ZombieType type) => All.Any(item => item.Type == type);
    internal const int ConeHealth = 370, BucketHealth = 1100;
    internal static int Health(bool bucket) => bucket ? BucketHealth : ConeHealth;
    // A fixed family range leaves existing Fusion identities unchanged.
    internal static int Marker(ZombieType type, bool bucket) => -741000 - (int)type * 2 - (bucket ? 1 : 0);
    internal static string Id(ZombieType type, bool bucket) => (bucket ? "bucket-" : "cone-") + type;

    // Native DetachShield turns a TrashCan into Normal and clears shield HP.
    // These two serialized identities continue to own the surviving hat/save;
    // no other marker or native-type transition gains this alias.
    internal static bool IsDetachedTrashCan(ZombieIdentity identity, Zombie zombie) =>
        identity.NativeType == ZombieType.TrashCan && identity.Field == MarkerField.GraveX
        && (identity.Marker == Marker(ZombieType.TrashCan, false) || identity.Marker == Marker(ZombieType.TrashCan, true))
        && zombie.mZombieType == ZombieType.Normal && zombie.mShieldHealth == 0;
}
