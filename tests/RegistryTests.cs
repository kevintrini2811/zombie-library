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
