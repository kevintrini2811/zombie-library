using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;
using ZombieLibrary;

static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
static void Reject<T>(Action action) where T : Exception
{ try { action(); } catch (T) { return; } throw new Exception("Expected " + typeof(T).Name); }

var mod = new ModCatalog("suite", "Registry suite");
var text = new EntryText("Zombie", "Description");
Reject<ArgumentException>(() => Zombies.RegisterZombie(mod, "unowned", new(ZombieType.Normal, MarkerField.GraveX, -1), text, text));
Zombies.RegisterCatalog(mod);
Reject<InvalidOperationException>(() => Zombies.RegisterCatalog(new("suite", "Duplicate")));
Reject<ArgumentException>(() => Zombies.RegisterZombie(mod, "bad-field", new(ZombieType.PeaHead, MarkerField.GraveX, -1), text, text));
Reject<ArgumentException>(() => Zombies.RegisterZombie(mod, "bad-marker", new(ZombieType.Normal, MarkerField.GraveX, 0), text, text));
var definition = Zombies.RegisterZombie(mod, "normal", new(ZombieType.Normal, MarkerField.GraveX, -100), text, text, rubberDuck: true);
Reject<InvalidOperationException>(() => Zombies.RegisterZombie(mod, "other-id", definition.Identity, text, text));
Reject<InvalidOperationException>(() => Zombies.RegisterZombie(mod, "normal", new(ZombieType.Normal, MarkerField.GraveX, -101), text, text));
int initialized = 0, ticks = 0;
definition.Initialize = zombie => initialized++;
definition.Tick = zombie => ticks++;
Zombies.RegisterSound("suite:spawn", FoleyType.NewspaperRarrgh); definition.SpawnSound = "suite:spawn";
Reject<InvalidOperationException>(() => Zombies.RegisterSound("suite:spawn", FoleyType.NewspaperRarrgh));
var board = new Board();
var zombie = Zombies.Spawn(definition, board, 2)!;
Check(initialized == 1 && zombie.PoolChecks == 1 && board.mApp.SoundSystem.Calls == 1, "Initialize, pool check and spawn sound run once.");
Zombies.Apply(definition, zombie);
Check(initialized == 1 && board.mApp.SoundSystem.Calls == 1, "Repeated Apply is idempotent.");
var loaded = new Zombie { mGraveX = -100, mZombieType = ZombieType.Normal, mBoard = board };
Check(Zombies.Find(loaded) == definition && initialized == 1, "Serialized identity restores without Initialize or sound.");
board.mPaused = true; Zombies.Tick(loaded); Check(ticks == 0, "Pause blocks ticks.");
board.mPaused = false; Zombies.Tick(loaded); Check(ticks == 1, "Live ticks reach the registered definition.");
Zombies.Reset(loaded); Check(Zombies.Find(loaded) == null && loaded.mGraveX == 0, "Reset clears custom identity.");
Reject<ArgumentOutOfRangeException>(() => Zombies.Spawn(definition, board, 6));
Check(!Zombies.IsExplicitSpawn, "Invalid row never leaks spawn scope.");
definition.Initialize = _ => throw new InvalidOperationException("Initialization failed");
Reject<InvalidOperationException>(() => Zombies.Spawn(definition, board, 0));
Check(board.Last!.mDead && !Zombies.IsExplicitSpawn, "Failed initialization removes the new actor and restores scope.");
var duck = Zombies.RegisterDuckVariant(mod, "football-duck", ZombieType.Football, text, text);
Check(duck.DuckOnly && duck.RubberDuck && Zombies.HasPoolType(ZombieType.Football), "Native duck registers its catalog entry and pool type.");
Reject<ArgumentException>(() => Zombies.Apply(duck, new Zombie { mZombieType = ZombieType.Football }));
Reject<ArgumentException>(() => Zombies.RegisterNativeDuck(ZombieType.Boss));
Console.WriteLine("Registry contracts passed.");

foreach (var type in new[] { ZombieType.TrafficCone, ZombieType.Pail }) {
 var armored = Zombies.RegisterZombie(mod, "armored-"+type, new(type, MarkerField.GraveX, -200), text, text, rubberDuck: true);
 var actor = new Zombie { mZombieType=type };
 Zombies.Apply(armored,actor);
 Check(Zombies.Find(actor)==armored && actor.PoolChecks==1,"Armored identity survives native type and enables pool entry.");
}
var emerging = new Zombie { mZombiePhase=ZombiePhase.RisingFromGrave };
emerging.DrawPosition.BodyY=100;
var artwork = new UnityEngine.Transform();
Zombies.ApplyBodyDisplacement(emerging,artwork);
Check(Math.Abs(artwork.position.y+282)<.001f,"Grave rise transfers native body displacement to imported artwork.");
Check(Zombies.BodyClipPlane(emerging,artwork,new(),out _,out _),"Grave rise enables ground clipping.");

var pea = Zombies.RegisterZombie(mod, "pea", new(ZombieType.PeaHead, MarkerField.SummonCounter, -400), text, text);
foreach (var registered in new[] { definition, Zombies.All.First(x => x.Identity.NativeType == ZombieType.TrafficCone),
    Zombies.All.First(x => x.Identity.NativeType == ZombieType.Pail), pea })
{
    var actor = new Zombie { mZombieType = registered.Identity.NativeType, mBoard = board };
    registered.Identity.Write(actor, registered.Identity.Marker);
    var mushroom = new Plant { mBoard = board }; actor.Target = mushroom;
    Check(Zombies.TryHypnotizeBite(actor, mushroom), "Every registered identity can forward committed native hypnosis.");
    Check(actor.ChewCalls == 1 && actor.mMindControlled && mushroom.mDead, "Full native chew callback consumes and controls.");
    Check(Zombies.Find(actor) == registered, "Hypnosis preserves serialized identity.");
    Check(!Zombies.TryHypnotizeBite(actor, mushroom) && actor.ChewCalls == 1, "Committed hypnosis cannot run twice.");
}
foreach (string guard in new[] { "unregistered", "asleep", "ordinary", "dead-plant", "detached", "zero-health", "other-board",
    "other-target", "paused", "dead-zombie", "dying", "headless", "not-eating", "ice", "butter" })
{
    var actor = new Zombie { mZombieType = ZombieType.Normal, mGraveX = -100, mBoard = board };
    var mushroom = new Plant { mBoard = board }; actor.Target = mushroom;
    switch (guard)
    {
        case "unregistered": actor.mGraveX = 0; break;
        case "asleep": mushroom.mIsAsleep = true; break;
        case "ordinary": mushroom.mSeedType = SeedType.Peashooter; break;
        case "dead-plant": mushroom.mDead = true; break;
        case "detached": mushroom.mIsOnBoard = false; break;
        case "zero-health": mushroom.mPlantHealth = 0; break;
        case "other-board": mushroom.mBoard = new Board { Pointer = (IntPtr)2 }; break;
        case "other-target": actor.Target = new Plant { Pointer = (IntPtr)2 }; break;
        case "paused": board.mPaused = true; break;
        case "dead-zombie": actor.mDead = true; break;
        case "dying": actor.Dying = true; break;
        case "headless": actor.mHasHead = false; break;
        case "not-eating": actor.mIsEating = false; break;
        case "ice": actor.mIceTrapCounter = 1; break;
        case "butter": actor.mButteredCounter = 1; break;
    }
    Check(!Zombies.TryHypnotizeBite(actor, mushroom) && actor.ChewCalls == 0, "Native hypnosis eligibility preserved: " + guard);
    board.mPaused = false;
}
Console.WriteLine("Native hypnosis forwarding: registered normal/cone/bucket/pea, identity retention, idempotence and eligibility guards passed.");
var resistant = new Zombie { mZombieType = ZombieType.Normal, mGraveX = -100, mBoard = board, ResistsHypnosis = true };
var resistedMushroom = new Plant { mBoard = board }; resistant.Target = resistedMushroom;
Check(Zombies.TryHypnotizeBite(resistant, resistedMushroom) && resistedMushroom.mDead,
    "A consumed Hypno-shroom finishes the bite even when another mod resists conversion.");
Check(!resistant.mMindControlled && resistant.ChewCalls == 1 && Zombies.Find(resistant) == definition,
    "Resisted hypnosis preserves the native rejection and registered identity.");
Console.WriteLine("Hypnosis resistance: consumed-target handling preserves rejected native conversion.");
