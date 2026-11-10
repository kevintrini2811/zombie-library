// Offline data doubles. No game assemblies, player, profiles or saves are loaded.
namespace HarmonyLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class HarmonyPatch : Attribute { public HarmonyPatch(Type type, string method) { } }
    public sealed class HarmonyPriority : Attribute { public HarmonyPriority(int value) { } }
    public static class Priority { public const int First = 800, Last = 0; }
}
namespace UnityEngine {
 public sealed class Sprite { } public sealed class SpriteRenderer { }
 public struct Vector3 {
  public float x,y,z; public Vector3(float x,float y,float z){this.x=x;this.y=y;this.z=z;}
  public static Vector3 up=>new(0,1,0); public Vector3 normalized=>this;
  public static Vector3 operator -(Vector3 a,Vector3 b)=>new(a.x-b.x,a.y-b.y,a.z-b.z);
  public static Vector3 operator *(Vector3 a,float n)=>new(a.x*n,a.y*n,a.z*n);
 }
 public sealed class Transform { public Vector3 position; public Vector3 TransformVector(Vector3 value)=>value; }
}
namespace ZombieLibrary {
 public static class PoolDuckVisuals { public static bool Waterline(Il2CppReloaded.Gameplay.Zombie zombie,UnityEngine.Transform artwork,out UnityEngine.Vector3 plane,out UnityEngine.Vector3 up){plane=new();up=UnityEngine.Vector3.up;return false;} }
}
namespace Il2CppReloaded.Services { public enum FoleyType { NewspaperRarrgh } }
namespace Il2CppReloaded.Gameplay
{
    public enum ZombiePhase { ZombieNormal, RisingFromGrave }
    public enum ZombieHeight { ZombieNormal, OutOfPool }
    public enum SeedType { Peashooter, Hypnoshroom }
    public enum ZombieAttackType { Chew }
    public sealed class Plant
    {
        public uint DataID = 1;
        public IntPtr Pointer = (IntPtr)1;
        public Board mBoard = new();
        public bool mDead, mIsAsleep, mIsOnBoard = true;
        public int mPlantHealth = 300;
        public SeedType mSeedType = SeedType.Hypnoshroom;
        public void UpdateMagnetShroom() { }
    }
    public sealed class ZombieDrawPosition { public float BodyY; }
    public enum ZombieType { Normal, Flag, TrafficCone, Polevaulter, Pail, Newspaper, Door, Football, Dancer, BackupDancer, DuckyTube, Snorkel, Zamboni, Bobsled, DolphinRider, JackInTheBox, Balloon, Digger, Pogo, Yeti, Bungee, Ladder, Catapult, Gargantuar, Imp, Boss, PeaHead, WallnutHead, JalapenoHead, GatlingHead, SquashHead, TallnutHead, RedeyeGargantuar, Zombatar, Target, TrashCan, Gravestone, NumZombieTypes }
    public enum HelmType { None, TrafficCone, Pail, Football, Digger, Redeyes, Headband, Bobsled }
    [Flags] public enum DamageFlags { BypassesShield=1, HitsShieldAndBody=2, Freeze=4, DoesntCauseFlash=8, DoesntLeaveBody=16, Spike=32 }
    public enum GameScenes { Playing, Menu }
    public sealed class SoundSystem
    { public int Calls; public void PlayFoley(Il2CppReloaded.Services.FoleyType sound) => Calls++; }
    public sealed class App { public SoundSystem SoundSystem = new(); public GameScenes GameScene = GameScenes.Playing; }
    public sealed class Board
    {
        public IntPtr Pointer = (IntPtr)1;
        public App mApp = new(); public bool mPaused; public bool Fail; public Zombie? Last;
        public readonly List<Zombie> m_zombies = new();
        public bool[] mZombieAllowed = new bool[(int)ZombieType.NumZombieTypes];
        public readonly Queue<int> Rolls = new();
        public int Rand(int maximum) => Rolls.Count == 0 ? 0 : Rolls.Dequeue() % maximum;
        public Zombie? ZombieTryToGet(uint id) => m_zombies.FirstOrDefault(zombie=>zombie.DataID==id);
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
        public uint DataID = 1; public IntPtr Pointer = (IntPtr)1;
        public HelmType mHelmType; public int mHelmHealth, mHelmMaxHealth;
        public int mBodyHealth=270,mBodyMaxHealth=270,mShieldHealth,mShieldMaxHealth;
        public int mFromWave=0,mJustGotShotCounter,ChillCalls;
        public bool mIsBobsledLeader;
        public uint[]? mFollowerZombieID;
        public void ApplyChill(bool trap) { ChillCalls++; }
        public bool mHasHead = true, mMindControlled, mIsEating = true, Dying;
        public int mIceTrapCounter, mButteredCounter, ChewCalls;
        public Plant? Target;
        public bool ResistsHypnosis;
        public bool IsDeadOrDying() => mDead || Dying;
        public Plant? FindPlantTarget(ZombieAttackType attack) => Target;
        public void AnimateChewSound() { ChewCalls++; mMindControlled = !ResistsHypnosis; Target!.mDead = true; }
        public ZombieType mZombieType; public Board mBoard = new();
        public int mGraveX, mGraveY, mSummonCounter, PoolChecks; public bool mDead,mInPool;
        public ZombiePhase mZombiePhase; public ZombieHeight mZombieHeight;
        public ZombieDrawPosition DrawPosition=new(); public ZombieDrawPosition GetDrawPos()=>DrawPosition;
        public void CheckForPool() => PoolChecks++;
        public void DieNoLoot() => mDead = true;
        public void Update() { } public void Reset() { } public void Deserialize() { }
        public int DropCalls; public Action<Zombie, DamageFlags>? Dropping;
        public void ZombieInitialize() { } public void TakeHelmDamage() { }
        public Action<int, DamageFlags>? TakingDamage;
        public void TakeDamage(int damage, DamageFlags flags) => TakingDamage?.Invoke(damage, flags);
        public void UpdateZombieBobsled() { } public void TakeDamage() { } public void TakeBodyDamage() { } public void TakeShieldDamage() { }
        public void DropHelm(DamageFlags flags) { ++DropCalls; Dropping?.Invoke(this, flags); }
    }
}
