using MelonLoader;

[assembly: MelonInfo(typeof(ZombieLibrary.LibraryMod), "Zombie Library", "1.1.1", "Kevin / Codex")]
[assembly: MelonGame("PopCap Games", "PvZ Replanted")]
[assembly: MelonAdditionalDependencies("BloomEngine")]
[assembly: MelonPriority(-100)]

namespace ZombieLibrary;

public sealed class LibraryMod : MelonMod
{
    internal static LibraryMod Instance = null!;
    public override void OnInitializeMelon() { Instance = this; Zombotany.Register(); MenuPresentation.Register(this); }
    public override void OnLateUpdate() { NativeZombieAlmanac.Update(); LibraryAlmanac.Update(); Zombies.RefreshVisuals(); PoolDuckVisuals.Update(); }
    public override void OnDeinitializeMelon() { NativeZombieAlmanac.Clear(); LibraryAlmanac.Dispose(); Zombotany.Dispose(); PoolDuckVisuals.Clear(); MenuPresentation.Dispose(); }
}
