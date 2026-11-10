using HarmonyLib;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using MelonLoader;
using UnityEngine;
using ZombieLibrary;

[assembly: MelonInfo(typeof(ZombieLibraryExample.ExampleMod), "Zombie Library Example", "1.0.0", "Kevin / Codex")]
[assembly: MelonGame("PopCap Games", "PvZ Replanted")]
[assembly: MelonAdditionalDependencies("ZombieLibrary")]

namespace ZombieLibraryExample;

/// <summary>An optional demonstration. Installing this DLL enables random replacements.</summary>
public sealed class ExampleMod : MelonMod
{
    internal static ZombieDefinition Tough = null!;
    private static Sprite? portrait;
    public override void OnInitializeMelon()
    {
        var mod = new ModCatalog("zombie-library-example", "Zombie Example");
        Zombies.RegisterCatalog(mod);
        Zombies.RegisterSound("example:spawn", FoleyType.NewspaperRarrgh);
        Tough = Zombies.RegisterZombie(mod, "tough", new(ZombieType.Normal, MarkerField.GraveX, -763001),
            new("Tough Zombie", "A larger normal zombie with twice the health. One in twenty eligible normal spawns is replaced.", "Health: 540"),
            new("Zäher Zombie", "Ein größerer normaler Zombie mit doppelten Lebenspunkten. Er ersetzt jeden zwanzigsten geeigneten Spawn im Durchschnitt.", "Lebenspunkte: 540"),
            rubberDuck: true);
        Tough.Initialize = zombie => { zombie.mGraveY = 0; zombie.mBodyHealth = zombie.mBodyMaxHealth = 540; zombie.mScaleZombie = 1.15f; };
        Tough.SpawnSound = "example:spawn";
        Tough.Portrait = _ => Portrait();
    }
    private static Sprite Portrait()
    {
        if (portrait != null) return portrait;
        using var stream = typeof(ExampleMod).Assembly.GetManifestResourceStream("ZombieLibraryExample.Assets.zombie.png")!;
        using var bytes = new MemoryStream(); stream.CopyTo(bytes);
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!ImageConversion.LoadImage(texture, bytes.ToArray(), true))
        { UnityEngine.Object.Destroy(texture); throw new InvalidDataException("Invalid example portrait."); }
        texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
        portrait = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100);
        portrait.hideFlags = HideFlags.DontUnloadUnusedAsset;
        return portrait;
    }
    public override void OnDeinitializeMelon()
    {
        if (portrait != null) { UnityEngine.Object.Destroy(portrait.texture); UnityEngine.Object.Destroy(portrait); }
        portrait = null;
    }
}

[HarmonyPatch(typeof(Board), nameof(Board.AddZombieInRow))]
internal static class ExampleSpawnPatch
{
    [HarmonyPriority(Priority.Last)]
    private static void Postfix(Zombie? __result)
    {
        var zombie = __result; var board = zombie?.mBoard;
        if (zombie == null || board?.mApp == null || board.mApp.GameScene != GameScenes.Playing
            || board.mApp.GameMode == GameMode.ChallengeZenGarden || Zombies.IsExplicitSpawn
            || zombie.mDead || zombie.mMindControlled || zombie.mZombieType != ZombieType.Normal
            || zombie.mZombiePhase != ZombiePhase.ZombieNormal || zombie.mHelmType != HelmType.None
            || zombie.mShieldHealth > 0 || zombie.mGraveX != 0 || Zombies.Find(zombie) != null || board.Rand(20) != 0) return;
        Zombies.Apply(ExampleMod.Tough, zombie);
    }
}
