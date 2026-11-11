using MelonLoader;

[assembly: MelonInfo(typeof(ZombieLibrary.LibraryMod), "Zombie Library", "1.4.7", "Kevin / Codex")]
[assembly: MelonGame("PopCap Games", "PvZ Replanted")]
[assembly: MelonOptionalDependencies("DebugMod")]
[assembly: MelonAdditionalDependencies("BloomEngine")]
[assembly: MelonPriority(-100)]

namespace ZombieLibrary;

public sealed class LibraryMod : MelonMod
{
    internal static LibraryMod Instance = null!;
    public override void OnInitializeMelon() { Instance = this; Zombotany.Register(); NativeDuckVariants.Register(); NativeArmoredZombies.Register(); MenuPresentation.Register(this); }
    public override void OnLateUpdate() { NativeZombieAlmanac.Update(); LibraryAlmanac.Update(); Zombies.RefreshVisuals(); NativeArmorVisuals.Update(); PoolDuckVisuals.Update(); }
    public override void OnDeinitializeMelon() { NativeZombieAlmanac.Clear(); LibraryAlmanac.Dispose(); Zombotany.Dispose(); NativeDuckVariants.Dispose(); NativeArmorVisuals.Dispose(); PoolDuckVisuals.Clear(); MenuPresentation.Dispose(); }
}
