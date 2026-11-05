// Offline data doubles. No game assemblies, player, profiles or saves are loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HarmonyPatch : Attribute { public HarmonyPatch(Type type, string method) { } }
}
namespace UnityEngine { public sealed class Sprite { } public sealed class SpriteRenderer { } }
namespace Il2CppReloaded.Services { public enum FoleyType { NewspaperRarrgh } }
namespace Il2CppReloaded.Gameplay
{
    public enum ZombieType { Newspaper, Door, Bobsled, Balloon, Pogo, Ladder, Imp, Target, TrashCan, Normal, PeaHead, Football, JackInTheBox, Dancer, BackupDancer, Yeti, Gargantuar, RedeyeGargantuar, SquashHead, Boss }
    public sealed class SoundSystem
    { public int Calls; public void PlayFoley(Il2CppReloaded.Services.FoleyType sound) => Calls++; }
    public sealed class App { public SoundSystem SoundSystem = new(); }
    public sealed class Board
    {
        public App mApp = new(); public bool mPaused; public bool Fail; public Zombie? Last;
        public int GetNumRows() => 6;
        public bool RowCanHaveZombies(int row) => row >= 0 && row < 6;
        public Zombie? AddZombieInRow(ZombieType type, int row, int wave, bool shake)
        {
            if (!ZombieLibrary.Zombies.IsExplicitSpawn) throw new Exception("Missing spawn scope.");
            return Fail ? null : Last = new Zombie { mZombieType = type, mBoard = this };
        }
    }
    public sealed class Zombie
    {
        public ZombieType mZombieType; public Board mBoard = new();
        public int mGraveX, mGraveY, mSummonCounter, PoolChecks; public bool mDead;
        public void CheckForPool() => PoolChecks++;
        public void DieNoLoot() => mDead = true;
        public void Update() { } public void Reset() { }
    }
}
