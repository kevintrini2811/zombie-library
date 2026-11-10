using Il2CppReloaded.Gameplay;
using ZombieLibrary;

int checks = 0;
void Check(bool value, string reason) { ++checks; if (!value) throw new Exception(reason); }
Zombie Actor(ZombieType type, Board? board = null) => new() { mZombieType=type, mBoard=board??new() };

// Presentation-only native duck entries must not claim ownership of a freshly
// created actor when a real, serialized variant is applied afterwards.
var native = new ModCatalog("native-test", "Native ducks"); Zombies.RegisterCatalog(native);
Zombies.RegisterDuckVariant(native, "newspaper", ZombieType.Newspaper, new("Native", ""), new("Native", ""));
NativeArmoredZombies.Register();
var entries = Zombies.All.Where(entry => entry.Mod.Id == "native-armored").ToArray();
Check(entries.Length == 34, "Every requested native actor has both helmet registrations.");
Check(entries.Select(entry=>entry.Identity).Distinct().Count()==34, "Serialized identities are unique.");
Check(NativeArmorTypes.All.Select(entry=>entry.Type).Distinct().Count()==17, "Exactly seventeen native AI types.");

foreach (var entry in entries)
{
    bool bucket = entry.Id.StartsWith("bucket-", StringComparison.Ordinal);
    var actor = Actor(entry.Identity.NativeType);
    actor.mShieldHealth=actor.mShieldMaxHealth=500;
    Zombies.Apply(entry, actor);
    Check(Zombies.Find(actor)==entry && actor.mZombieType==entry.Identity.NativeType, "Native AI and identity retained: "+entry.Id);
    Check(NativeArmoredZombies.Remaining(actor)==NativeArmorTypes.Health(bucket), "Native helmet health: "+entry.Id);
    Check(actor.mShieldHealth==500 && actor.mShieldMaxHealth==500 && actor.mBodyHealth==270 && actor.mBodyMaxHealth==270,
        "Body and held equipment are unchanged: "+entry.Id);
    Check(entry.RubberDuck && actor.PoolChecks==1, "Each variant uses native pool entry: "+entry.Id);
    var loaded=Actor(actor.mZombieType); loaded.mGraveX=actor.mGraveX; loaded.mGraveY=actor.mGraveY;
    Check(Zombies.Find(loaded)==entry, "Serialized identity restores without initializer: "+entry.Id);
    Zombies.Reset(loaded); Check(loaded.mGraveX==0 && loaded.mGraveY==0 && Zombies.Find(loaded)?.DuckOnly!=false, "Pool reuse clears variant: "+entry.Id);
}

foreach (var spec in NativeArmorTypes.All)
foreach (bool coneAllowed in new[]{false,true})
foreach (bool bucketAllowed in new[]{false,true})
{
    var board=new Board(); board.mZombieAllowed[(int)ZombieType.TrafficCone]=coneAllowed; board.mZombieAllowed[(int)ZombieType.Pail]=bucketAllowed;
    // Deliberately leave the base allowed bit false: the actual actor can be
    // an implicit flag, dancer, imp or bobsled follower created by native AI.
    var actor=Actor(spec.Type,board); NativeArmoredZombies.ReplaceNatural(actor,null);
    bool replaced=NativeArmoredZombies.TryKind(actor,out bool bucket);
    Check(replaced==(coneAllowed||bucketAllowed),"Actual native base occurrence plus armor availability controls selection.");
    if(replaced)Check(bucket?bucketAllowed:coneAllowed,"Selected helmet must occur in the level.");
}
foreach(string guard in new[]{"no-wave","menu","paused","dead","controlled","marked","debug"})
{
    var board=new Board();board.mZombieAllowed[(int)ZombieType.TrafficCone]=true;
    var actor=Actor(ZombieType.Flag,board);
    switch(guard){case "no-wave":actor.mFromWave=-1;break;case "menu":board.mApp.GameScene=GameScenes.Menu;break;
        case "paused":board.mPaused=true;break;case "dead":actor.mDead=true;break;case "controlled":actor.mMindControlled=true;break;
        case "marked":actor.mGraveX=-12;break;case "debug":Zombies.SuppressSightings=_=>true;break;}
    NativeArmoredZombies.ReplaceNatural(actor,null);
    Check(!NativeArmoredZombies.TryKind(actor,out _),"Natural replacement guard: "+guard); Zombies.SuppressSightings=null;
}

foreach(bool bucket in new[]{false,true})
{
    var board=new Board();var leader=Actor(ZombieType.Bobsled,board);
    leader.mHelmType=HelmType.Bobsled;leader.mHelmHealth=leader.mHelmMaxHealth=300;leader.mIsBobsledLeader=false;
    var followers=Enumerable.Range(2,3).Select(i=>new Zombie{DataID=(uint)i,Pointer=(IntPtr)i,mZombieType=ZombieType.Bobsled,mBoard=board}).ToArray();
    board.m_zombies.AddRange(followers);leader.mFollowerZombieID=followers.Select(f=>f.DataID).ToArray();
    var definition=entries.Single(entry=>entry.Id==NativeArmorTypes.Id(ZombieType.Bobsled,bucket));Zombies.Apply(definition,leader);
    Check(leader.mHelmType==HelmType.Bobsled&&leader.mHelmHealth==300&&leader.mHelmMaxHealth==300,"Sled identity, health and visual thresholds preserved.");
    Check(followers.All(f=>Zombies.Find(f)==definition&&f.mHelmType==(bucket?HelmType.Pail:HelmType.TrafficCone)),"Explicit native group retains members and adds matching hats.");
    int damage=100;Check(NativeArmoredZombies.AbsorbBobsled(leader,ref damage,DamageFlags.Freeze)&&damage==0&&leader.ChillCalls==1,"Outer layer absorbs before native sled, preserving chill.");
    Check(leader.mHelmHealth==300&&leader.mHelmMaxHealth==300,"Outer hits never consume or distort native sled.");
    damage=NativeArmoredZombies.Remaining(leader)+80;
    Check(NativeArmoredZombies.AbsorbBobsled(leader,ref damage,DamageFlags.Freeze)&&damage==80&&leader.ChillCalls==1,"Overflow passes once to native sled with native freeze flag.");
    Check(NativeArmoredZombies.Remaining(leader)==0,"Broken outer hat stays absent above intact sled.");
    damage=20;Check(!NativeArmoredZombies.AbsorbBobsled(leader,ref damage,0)&&damage==20,"No outer layer means completely native damage.");

    // The real native magnet selector sees a temporary bucket, while its
    // field clear removes only the added hat and finally restores the sled.
    if(bucket)
    {
        leader.mGraveY=900;board.m_zombies.Add(leader);var plant=new Plant{mBoard=board};
        var scope=NativeArmorMagnet.Begin(plant);
        Check(leader.mHelmType==HelmType.Pail&&leader.mHelmHealth==900,"Native magnet sees remaining bucket HP.");
        // MagnetShroomAttactItem clears these fields directly in the game;
        // it does not call DropHelm. The scope observes that native outcome.
        leader.mHelmType=HelmType.None;leader.mHelmHealth=0;
        NativeArmorMagnet.Restore(scope);
        Check(leader.mGraveY==0&&leader.mHelmType==HelmType.Bobsled&&leader.mHelmHealth==300&&leader.mHelmMaxHealth==300,"Native removal commits added bucket and finally restores sled exactly.");
        leader.mGraveY=1100;scope=NativeArmorMagnet.Begin(plant);leader.DataID++;NativeArmorMagnet.Restore(scope);
        Check(leader.mHelmType==HelmType.Pail,"Pool generation change must not receive stale native fields.");
    }
}
// Native group links may appear after the registered initializer. Rebinding
// follows IDs rather than a stale leader flag and never refills existing hats.
foreach (bool bucket in new[] { false, true })
{
    var board = new Board();
    var leader = Actor(ZombieType.Bobsled, board);
    leader.mHelmType = HelmType.Bobsled; leader.mHelmHealth = leader.mHelmMaxHealth = 300;
    var definition = entries.Single(e => e.Id == NativeArmorTypes.Id(ZombieType.Bobsled, bucket));
    Zombies.Apply(definition, leader);
    var followers = Enumerable.Range(2, 7).Select(i => new Zombie
        { DataID = (uint)i, Pointer = (IntPtr)i, mZombieType = ZombieType.Bobsled, mBoard = board }).ToArray();
    followers[3].mDead = true; followers[4].Dying = true;
    followers[5].mBoard = new Board { Pointer = (IntPtr)99 };
    followers[6].mZombieType = ZombieType.Normal;
    board.m_zombies.AddRange(followers);
    leader.mFollowerZombieID = followers.Select(f => f.DataID).Append(999u).ToArray();
    NativeArmoredZombies.Tick(leader);
    Check(followers.Take(3).All(f => Zombies.Find(f) == definition), "Late native links bind all three live followers with leader flag false.");
    Check(followers.Skip(3).All(f => f.mGraveX == 0), "Dead, dying, wrong-board, wrong-type and stale links do not gain armor.");
    followers[0].mHelmHealth -= 17;
    followers[1].mHelmType = HelmType.None; followers[1].mHelmHealth = 0;
    NativeArmoredZombies.Tick(leader);
    Check(followers[0].mHelmHealth == NativeArmorTypes.Health(bucket) - 17 && followers[1].mHelmType == HelmType.None,
        "Repeated group ticks preserve damaged and broken follower hats.");

    int health = leader.mGraveY, damage = 6;
    var update = NativeBobsledDamageScope.BeginUpdate(leader);
    var hit = NativeBobsledDamageScope.BeginHit(leader, damage, DamageFlags.DoesntCauseFlash);
    Check(NativeBobsledDamageScope.IsWear(leader) && !NativeArmoredZombies.AbsorbBobsled(leader, ref damage, DamageFlags.DoesntCauseFlash)
        && leader.mGraveY == health && damage == 6, "Only native scoped six-point wear bypasses the outer hat.");
    var nested = NativeBobsledDamageScope.BeginHit(leader, 7, DamageFlags.DoesntCauseFlash);
    damage = 7;
    Check(!NativeBobsledDamageScope.IsWear(leader) && NativeArmoredZombies.AbsorbBobsled(leader, ref damage, DamageFlags.DoesntCauseFlash)
        && damage == 0, "Nested real hit does not inherit wear classification.");
    NativeBobsledDamageScope.EndHit(nested);
    Check(NativeBobsledDamageScope.IsWear(leader), "Nested hit restores the caller's wear classification.");
    var nestedUpdate = NativeBobsledDamageScope.BeginUpdate(followers[0]);
    nested = NativeBobsledDamageScope.BeginHit(leader, 6, DamageFlags.DoesntCauseFlash);
    Check(!NativeBobsledDamageScope.IsWear(leader), "Another actor's update cannot classify this actor's hit as wear.");
    NativeBobsledDamageScope.EndHit(nested); NativeBobsledDamageScope.EndUpdate(nestedUpdate);
    leader.DataID++;
    Check(!NativeBobsledDamageScope.IsWear(leader) && !NativeBobsledDamageScope.IsHit(leader), "Pooled generation cannot inherit old hit scope.");
    leader.DataID--;
    var originalBoard = leader.mBoard; leader.mBoard = followers[5].mBoard;
    Check(!NativeBobsledDamageScope.IsWear(leader), "Different board cannot inherit old hit scope.");
    leader.mBoard = originalBoard;
    NativeBobsledDamageScope.EndHit(hit); NativeBobsledDamageScope.EndUpdate(update);
    hit = NativeBobsledDamageScope.BeginHit(leader, 6, DamageFlags.DoesntCauseFlash);
    damage = 6;
    Check(!NativeBobsledDamageScope.IsWear(leader) && NativeArmoredZombies.AbsorbBobsled(leader, ref damage, DamageFlags.DoesntCauseFlash)
        && damage == 0, "Identical damage and flags outside native sled update still consume armor.");
    NativeBobsledDamageScope.EndHit(hit);
    Check(!NativeBobsledDamageScope.IsHit(leader), "Hit scope ends after native call.");

    // Call the product prefix itself; no fake native damage implementation.
    var bodyPrefix = typeof(NativeArmoredZombies).Assembly.GetType("ZombieLibrary.NativeBobsledCrashedHatPatch")!
        .GetMethod("Prefix", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
    bool Body(ref int value, DamageFlags flags)
    {
        object[] args = { leader, value, flags };
        bool run = (bool)bodyPrefix.Invoke(null, args)!; value = (int)args[1]; return run;
    }
    leader.mHelmType = HelmType.None; leader.mHelmHealth = 0; leader.mGraveY = 25;
    damage = 10;
    Check(Body(ref damage, 0) && damage == 10 && leader.mGraveY == 25, "Direct native body damage keeps intentional helmet bypass.");
    hit = NativeBobsledDamageScope.BeginHit(leader, 10, 0);
    Check(!Body(ref damage, 0) && damage == 0 && leader.mGraveY == 15 && leader.mHelmType == HelmType.None,
        "Normal hit after native sled crash still consumes surviving outer hat.");
    NativeBobsledDamageScope.EndHit(hit);
    int drops = leader.DropCalls;
    leader.Dropping = (actor, flags) =>
    {
        Check(actor.mHelmType == (bucket ? HelmType.Pail : HelmType.TrafficCone) && actor.mHelmHealth == 0
            && actor.mHelmMaxHealth == NativeArmorTypes.Health(bucket), "Native drop receives added hat identity and thresholds.");
        actor.mHelmType = HelmType.None; actor.mHelmHealth = 0; actor.mHelmMaxHealth = 0;
    };
    hit = NativeBobsledDamageScope.BeginHit(leader, 23, 0); damage = 23;
    Check(Body(ref damage, 0) && damage == 8 && leader.mGraveY == 0 && leader.DropCalls == drops + 1,
        "Crashed outer hat breaks once and sends only overflow to native body.");
    NativeBobsledDamageScope.EndHit(hit);
    Check(leader.mHelmType == HelmType.None && leader.mHelmHealth == 0 && leader.mHelmMaxHealth == 300,
        "Native post-crash sled fields survive added-hat drop.");
    leader.mHelmType = HelmType.Bobsled; leader.mHelmHealth = 123; leader.mHelmMaxHealth = 300; leader.mGraveY = 1;
    damage = 1; NativeArmoredZombies.AbsorbBobsled(leader, ref damage, 0);
    Check(leader.mHelmType == HelmType.Bobsled && leader.mHelmHealth == 123 && leader.mHelmMaxHealth == 300,
        "Intact native sled HP/type/max survive native added-hat drop.");
    leader.mGraveY = 1; leader.Dropping = (_, _) => throw new InvalidOperationException("drop boundary");
    damage = 1; bool threw = false;
    try { NativeArmoredZombies.AbsorbBobsled(leader, ref damage, 0); } catch (InvalidOperationException) { threw = true; }
    Check(threw && leader.mHelmType == HelmType.Bobsled && leader.mHelmHealth == 123,
        "Native drop exception still restores original sled fields.");
    leader.mGraveY = 1; leader.Dropping = (actor, _) => { actor.DataID++; actor.mHelmType = HelmType.None; actor.mHelmHealth = 42; };
    damage = 1; NativeArmoredZombies.AbsorbBobsled(leader, ref damage, 0);
    Check(leader.mHelmType == HelmType.None && leader.mHelmHealth == 42, "Drop callback cannot restore stale fields into a reused actor.");
}
// Verify the compiled handoff hooks, using native method return values as
// inputs. Native shield/helm/body arithmetic and effect code are not replaced
// by offline doubles; live tests verify those gameplay boundaries.
var shieldResult = typeof(NativeArmorShieldDamageScope).Assembly.GetType("ZombieLibrary.NativeArmorShieldResultPatch")!
    .GetMethod("Postfix", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
var shieldForward = typeof(NativeArmorShieldDamageScope).Assembly.GetType("ZombieLibrary.NativeArmorShieldForwardPatch")!
    .GetMethod("Prefix", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
void RecordShield(Zombie zombie, int result, DamageFlags flags) => shieldResult.Invoke(null, new object[] { zombie, result, flags });
int ForwardShield(Zombie zombie, int original)
{
    object[] args = { zombie, original }; shieldForward.Invoke(null, args); return (int)args[1];
}
foreach (var type in new[] { ZombieType.Door, ZombieType.Newspaper, ZombieType.TrashCan, ZombieType.Ladder })
foreach (bool bucket in new[] { false, true })
{
    var actor = Actor(type);
    var definition = entries.Single(e => e.Id == NativeArmorTypes.Id(type, bucket));
    Zombies.Apply(definition, actor);
    actor.mShieldHealth = 1080; actor.mShieldMaxHealth = 1100;
    int helmet = actor.mHelmHealth;
    var scope = NativeArmorShieldDamageScope.Begin(actor);
    Check(ForwardShield(actor, 20) == 20, "Before a native shield result, helmet entry remains unchanged.");
    RecordShield(actor, 1, 0);
    Check(ForwardShield(actor, 1081) == 1, "Compiled hooks forward shield overflow1 instead of original1081: " + definition.Id);
    Check(ForwardShield(actor, 1081) == 1081, "A native shield return is consumed only once.");
    Check(actor.mShieldHealth == 1080 && actor.mHelmHealth == helmet && actor.mBodyHealth == 270,
        "Scope handoff does not manually alter armor/body fields or effects.");
    RecordShield(actor, 1, 0); actor.mShieldHealth = 0;
    Check(ForwardShield(actor, 1081) == 1, "Native shield destruction does not discard its captured overflow.");
    actor.mShieldHealth = 1080;
    foreach (int overflow in new[] { 0, 1, helmet - 1, helmet, helmet + 1, helmet + 269, helmet + 270 })
    {
        RecordShield(actor, overflow, DamageFlags.Freeze | DamageFlags.DoesntCauseFlash);
        Check(ForwardShield(actor, 1080 + overflow) == overflow,
            "Exact native remainder reaches helmet boundary, including full helmet/body limits.");
    }
    foreach (var flags in new[] { DamageFlags.BypassesShield, DamageFlags.HitsShieldAndBody,
        DamageFlags.BypassesShield | DamageFlags.HitsShieldAndBody,
        DamageFlags.HitsShieldAndBody | DamageFlags.Freeze })
    {
        RecordShield(actor, 1, flags);
        Check(ForwardShield(actor, 1081) == 1081, "Native shield bypass/full-body flags retain original helmet argument.");
    }
    RecordShield(actor, 1, 0);
    var nested = NativeArmorShieldDamageScope.Begin(actor);
    Check(ForwardShield(actor, 31) == 31, "Nested same-actor hit cannot consume caller's pending remainder.");
    RecordShield(actor, 7, 0);
    Check(ForwardShield(actor, 31) == 7, "Nested same-actor hit has its own native remainder.");
    NativeArmorShieldDamageScope.End(nested);
    Check(ForwardShield(actor, 1081) == 1, "Nested hit restores the caller's native remainder.");
    RecordShield(actor, 1, 0);
    var nativeActor = Actor(type);
    nested = NativeArmorShieldDamageScope.Begin(nativeActor);
    RecordShield(nativeActor, 3, 0);
    Check(ForwardShield(nativeActor, 51) == 51 && ForwardShield(actor, 1081) == 1081,
        "Ineligible nested native hit hides parent scope without changing native arguments.");
    NativeArmorShieldDamageScope.End(nested);
    actor.DataID++;
    Check(ForwardShield(actor, 1081) == 1081, "Pooled generation cannot inherit pending shield overflow.");
    actor.DataID--;
    var board = actor.mBoard; actor.mBoard = new Board { Pointer = (IntPtr)99 };
    Check(ForwardShield(actor, 1081) == 1081, "Another board cannot inherit pending shield overflow.");
    actor.mBoard = board;
    actor.mHelmType = HelmType.None;
    Check(ForwardShield(actor, 1081) == 1081, "Removed added helmet cannot claim pending overflow.");
    actor.mHelmType = bucket ? HelmType.Pail : HelmType.TrafficCone;
    NativeArmorShieldDamageScope.End(scope);
    RecordShield(actor, 1, 0);
    Check(ForwardShield(actor, 1081) == 1081, "Direct shield/helm calls outside TakeDamage remain unchanged.");
}
foreach (var type in new[] { ZombieType.Door, ZombieType.Newspaper, ZombieType.TrashCan, ZombieType.Flag })
{
    var actor = Actor(type);
    if (type == ZombieType.Flag) Zombies.Apply(entries.Single(e => e.Id == NativeArmorTypes.Id(type, false)), actor);
    else { actor.mHelmType = HelmType.TrafficCone; actor.mHelmHealth = 370; }
    var scope = NativeArmorShieldDamageScope.Begin(actor);
    RecordShield(actor, 1, 0);
    Check(ForwardShield(actor, 1081) == 1081, "Unregistered actors and shieldless registered actors retain native damage.");
    NativeArmorShieldDamageScope.End(scope);
}
// TrashCan's native shield detach changes its AI type to Normal inside the
// same hit. Only the exact two built-in serialized markers retain ownership.
foreach (bool bucket in new[] { false, true })
{
    var definition = entries.Single(e => e.Id == NativeArmorTypes.Id(ZombieType.TrashCan, bucket));
    var actor = Actor(ZombieType.TrashCan); Zombies.Apply(definition, actor);
    actor.mShieldHealth = 780; actor.mShieldMaxHealth = 800;
    var scope = NativeArmorShieldDamageScope.Begin(actor);
    // Actual native DetachShield writes Normal + shieldHP0 before return.
    actor.mZombieType = ZombieType.Normal; actor.mShieldHealth = 0;
    RecordShield(actor, 1, 0);
    Check(Zombies.Find(actor) == definition && NativeArmoredZombies.TryKind(actor, out bool kind) && kind == bucket,
        "Native TrashCan detach retains exact variant ownership for gameplay, ticking and visuals.");
    Check(ForwardShield(actor, 781) == 1 && actor.mZombieType == ZombieType.Normal,
        "Same-hit TrashCan detach forwards actual remainder without restoring its original native type.");
    NativeArmorShieldDamageScope.End(scope);
    int health = actor.mHelmHealth - 1; actor.mHelmHealth = health;
    var loaded = Actor(ZombieType.Normal); loaded.mGraveX = actor.mGraveX;
    loaded.mHelmType = actor.mHelmType; loaded.mHelmHealth = health; loaded.mHelmMaxHealth = actor.mHelmMaxHealth;
    Check(Zombies.Find(loaded) == definition && NativeArmoredZombies.Remaining(loaded) == health,
        "Deserialized detached TrashCan restores variant and remaining hat without initializer or transient scope.");
    NativeArmoredZombies.Tick(loaded);
    Check(loaded.mHelmHealth == health && loaded.mZombieType == ZombieType.Normal,
        "Detached variant tick preserves native transition and damaged hat.");
    loaded.mHelmType = HelmType.None; loaded.mHelmHealth = 0;
    Check(Zombies.Find(loaded) == definition, "Detached TrashCan retains save identity after hat break or magnet removal.");
    loaded.mShieldHealth = 1;
    Check(Zombies.Find(loaded) != definition, "Normal actor with an intact shield is not a native detached TrashCan state.");
    loaded.mShieldHealth = 0; loaded.mZombieType = ZombieType.Zamboni;
    Check(Zombies.Find(loaded) != definition, "TrashCan alias never accepts arbitrary native-type changes.");
    actor.mZombieType = ZombieType.TrashCan; actor.mShieldHealth = 20;
    scope = NativeArmorShieldDamageScope.Begin(actor); RecordShield(actor, 1, 0);
    actor.mGraveX = NativeArmorTypes.Marker(ZombieType.TrashCan, !bucket);
    Check(ForwardShield(actor, 21) == 21, "Changed variant marker cannot claim a pending hit remainder.");
    NativeArmorShieldDamageScope.End(scope);
    Zombies.Reset(actor);
    Check(actor.mGraveX == 0 && Zombies.Find(actor) != definition, "Pool reset ends detached TrashCan ownership.");
}
var externalTrashCatalog = new ModCatalog("external-trash-alias-test", "External trash");
Zombies.RegisterCatalog(externalTrashCatalog);
var externalTrash = Zombies.RegisterZombie(externalTrashCatalog, "custom", new(ZombieType.TrashCan, MarkerField.GraveX, -876543), new("Custom", ""), new("Custom", ""));
var externalTrashActor = Actor(ZombieType.TrashCan); Zombies.Apply(externalTrash, externalTrashActor);
externalTrashActor.mZombieType = ZombieType.Normal;
Check(Zombies.Find(externalTrashActor) != externalTrash, "Native alias does not broaden arbitrary third-party marker contracts.");
// Reproduce native pool reuse on the same actor after its type and generation
// changed. Invoke the actual factory prefix, not merely a test-only reset path.
var factoryReset = typeof(Zombies).Assembly.GetType("ZombieLibrary.ZombieInitializeMarkerResetPatch")!
    .GetMethod("Prefix", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
var flag = entries.Single(entry => entry.Id == NativeArmorTypes.Id(ZombieType.Flag, false));
foreach (var entry in entries)
foreach (var nextType in new[] { ZombieType.Normal, ZombieType.Zamboni, ZombieType.Flag })
{
    var actor = Actor(entry.Identity.NativeType);
    if (entry.Identity.NativeType == ZombieType.Bobsled)
    { actor.mHelmType = HelmType.Bobsled; actor.mHelmHealth = actor.mHelmMaxHealth = 300; }
    Zombies.Apply(entry, actor);
    actor.mDead = true; actor.DataID++; actor.mDead = false; actor.mZombieType = nextType;
    actor.mSummonCounter = 17;
    factoryReset.Invoke(null, new object[] { actor });
    Check(actor.mGraveX == 0 && actor.mGraveY == 0, "Factory resets old exact marker/auxiliary across type and generation: " + entry.Id);
    Check(actor.mSummonCounter == 17 && actor.mHelmHealth > 0, "Marker cleanup leaves native summon/helmet fields to native initialization.");
    actor.mZombieType = ZombieType.Flag;
    Zombies.Apply(flag, actor);
    Check(Zombies.Find(actor) == flag, "Registered Cone Flag can reuse cleaned native actor after " + entry.Id);
    actor.DataID++; actor.mZombieType = ZombieType.Normal;
    Zombies.Reset(actor);
    Check(actor.mGraveX == 0 && actor.mGraveY == 0, "Reset also handles modded-to-native type change.");
    Zombies.Reset(actor);
    Check(actor.mGraveX == 0, "Repeated reset is idempotent.");
}
foreach (int marker in new[] { 0, 3, -999999 })
{
    var actor = Actor(ZombieType.Zamboni); actor.mGraveX = marker; actor.mGraveY = 27; actor.mSummonCounter = 19;
    factoryReset.Invoke(null, new object[] { actor });
    Check(actor.mGraveX == marker && actor.mGraveY == 27 && actor.mSummonCounter == 19,
        "Native/foreign grave and counter state is never cleared merely by sign or current type.");
}
var counterMod = new ModCatalog("counter-reset-test", "Counter reset"); Zombies.RegisterCatalog(counterMod);
var counter = Zombies.RegisterZombie(counterMod, "pea", new(ZombieType.PeaHead, MarkerField.SummonCounter, -54321), new("Pea", ""), new("Pea", ""));
var reusedPea = Actor(ZombieType.PeaHead); Zombies.Apply(counter, reusedPea);
reusedPea.mZombieType = ZombieType.Zamboni; reusedPea.DataID++; reusedPea.mGraveX = 4; reusedPea.mGraveY = 7;
factoryReset.Invoke(null, new object[] { reusedPea });
Check(reusedPea.mSummonCounter == 0 && reusedPea.mGraveX == 4 && reusedPea.mGraveY == 7,
    "Owned counter marker resets across type change without touching native grave fields.");
var crossedFields = Actor(ZombieType.Zamboni);
crossedFields.mGraveX = counter.Identity.Marker;
crossedFields.mSummonCounter = flag.Identity.Marker;
crossedFields.mGraveY = 33;
factoryReset.Invoke(null, new object[] { crossedFields });
Check(crossedFields.mGraveX == counter.Identity.Marker && crossedFields.mSummonCounter == flag.Identity.Marker && crossedFields.mGraveY == 33,
    "Known marker numbers in the wrong field do not establish ownership.");
var saved = Actor(ZombieType.RedeyeGargantuar);
factoryReset.Invoke(null, new object[] { saved });
// Native Deserialize runs after factory allocation: loading must retain the
// serialized marker and outer-state values without initialization callbacks.
saved.mGraveX = NativeArmorTypes.Marker(ZombieType.RedeyeGargantuar, true); saved.mGraveY = 42;
Check(Zombies.Find(saved)?.Id == NativeArmorTypes.Id(ZombieType.RedeyeGargantuar, true) && saved.mGraveY == 42,
    "Save identity and state restore unchanged after the fresh-life boundary.");
Console.WriteLine($"PASS {checks} native armor registration, level, serialization, group, damage and magnet checks.");

namespace ZombieLibrary
{
    internal static class NativeArmorVisuals
    {
        internal static void Observe(Zombie zombie) { }
        internal static UnityEngine.Sprite Portrait(ZombieType type,bool bucket)=>new();
    }
}
