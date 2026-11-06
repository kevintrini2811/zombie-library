# Zombie Library 1.4.5

Version 1.4.5 forwards the actual native shield overflow into added helmets on registered zombies with both a native shield and an added helmet, including Door, Newspaper, Trash Can and Ladder zombies. Native shield-break abilities and damage flags remain responsible for gameplay. Trash Can variants retain their serialized identity and remaining helmet through the native transition to Normal after shield removal.

Version 1.4.4 binds native Bobsled followers when their group links become available and keeps internal sled wear separate from added helmet damage. Surviving added hats still absorb normal hits after the native sled crashes; their native drop effects preserve the original sled fields.

Version 1.4.3 clears exact registered marker fields at native actor initialization, including pooled native-type and generation changes. Unregistered/native fields and deserialized save identities remain unchanged.

Version 1.4 adds 34 native armored zombies: Conehead and Buckethead variants of Flag, Polevaulter, Newspaper, Screen Door, Backup Dancer, Bobsled, Dolphin Rider, Jack-in-the-Box, Pogo, Yeti, Bungee, Ladder, Catapult, Gargantuar, Imp, Trash Can and Giga-gargantuar. Their original native AI types, abilities, bodies and held equipment remain responsible for gameplay. Each registration also supports the library's native pool and duck presentation.

Cones have 370 helmet health; buckets have 1100. Existing Newspaper, Door, Ladder and Trash Can shields remain separate. The bobsled leader retains its original sled helmet and health: its additional hat uses a separate saved outer layer, so damaging or magnetizing the hat does not replace the sled or its native crash rules. Native bobsled followers receive matching hats without replacing group members.

Natural variants are selected from actors the game actually spawns. A suitable native spawn has a one-in-three chance of receiving armor, and only armor whose normal Conehead/Buckethead type is allowed by that level can be chosen. When both are allowed, the selected armor is equally likely to be either kind. Actual native summons establish the base type's presence; an unrelated allowed-list bit is not required for implicit flag, dancer, imp or follower spawns. Explicit library spawns and temporary debug sessions do not receive random substitutions.

The catalog owner is `native-armored`. IDs are `cone-{ZombieType}` and `bucket-{ZombieType}`, for example `cone-Flag` and `bucket-Gargantuar`. Cards and previews use complete native Spine portraits with matching head-socket armor. Runtime armor uses the real animated head attachment, native color effects and native clipping; separator rigs use the actual head mesh rather than their disabled root renderer. Version 1.4.1 fixes Harmony argument binding, 1.4.2 fixes separator-renderer ownership, 1.4.3 clears owned markers at native actor reuse, and 1.4.4 preserves bobsled group binding and the distinction between native sled wear and actual helmet hits.

See [native gameplay and saved-state evidence](references/native-armored-1-4-0/port-notes.md) and [portrait export evidence](references/armored-assets/port-notes.md). The full 1.4.5 runtime matrix remains under review; a successful build or offline suite alone does not complete that review.

Version 1.3.2 forwards custom zombie bite events through native Hypno-shroom consumption and hypnosis. Zombie Library validates registered actors and the actual native bite target; Eyes calls the shared helper at its own bite event. Native AI and saved identities remain unchanged.

Version 1.3.1 uses native character colour state for imported zombie sprites, including cold blue, armour and pool ducks. Zombie Library owns the shared colour adapter; source alpha and pool clipping remain intact.


Version 1.3.0 owns registration and artwork for the additional native duck variants formerly registered by Plant Fusion. Native pool behavior works without Fusion. Existing sightings under the old Fusion catalog remain readable. `DebugCatalog.SelectionEntries` excludes duck-only cards while preserving the existing full catalog export. Register walking Normal, TrafficCone or Pail variants through their unused negative GraveX marker; native armor remains intact.

Imported zombie artwork calls `Zombies.ApplyBodyDisplacement` after anchoring its ground node, then `Zombies.BodyClipPlane` with that native ground position. These library helpers transfer native grave-rise and pool displacement and provide a shared clipping plane. Native zombie phases, altitude, duration and animation clocks remain unchanged.

Custom `PlacePoolDuck` callbacks own tube geometry; registration does not require an active native torso attachment for those imported rigs. Destroyed tube renderers are recreated after controller or imported-artwork reuse.

## Dependencies

- Game/runtime: PvZ Replanted (Windows/IL2CPP), MelonLoader 0.7.3, and .NET 6.
- Required mods: `BloomEngine.dll`.
- Optional integrations: `DebugMod`.
- Building: .NET SDK with `net6.0` support and the game/MelonLoader reference assemblies declared in `ZombieLibrary.csproj`.


## Pool graphics (1.3.0)

Synthetic ducks are based on the largest actual body attachment. Already visible native ducks are reused. Empty arm slots do not impose a limit on arm order. Door and TrashCan retain their native walking clip with the shield intact because the shared swimming clip hides the shield and arms. Native water movement and waterline remain responsible.

Supported rigs without a native `swim` animation, including Pogo, retain their native walk animation in water. The library borrows the animation selector's pool flag only during that call; swimming state, movement and water clipping remain native.

## Additional pool variants and sighting protection (1.2.0)

`RegisterNativeDuck` additionally supports Newspaper, Door, Bobsled, Balloon, Pogo, Ladder, Imp, Target and TrashCan. Pool movement and saved water states remain native. Bobsled teams leave the sled after complete initialization. Pogo loses his staff when he enters the water. Balloons fly over water and can float after bursting.

`Zombies.SuppressSightings` is an optional `Func<Board, bool>`. If the callback returns `true`, the viewing of this board is neither saved nor noted in the session cache. Debug Mod 1.3.2 registers its temporary boards via this callback. The owner chains previous callbacks and restores them upon unloading. Without a callback, the regular triage rule remains active. The display via `RevealAllAlmanacEntries` is independent of this.

## Temporary almanac release (1.1.2)

`Zombies.RevealAllAlmanacEntries` is an optional `Func<bool>` for a temporary full almanac view. Fusion registers the query of its active debug mode and removes it upon unloading. Without a callback, the viewing rule still applies. The query only influences the display; it does not create sightings and does not change native unlocks.

Native entries as well as Zombotany and Mod tabs honor the same query. Zombotany remains exclusively in its own tab. Native models and their fixed indices are retained. Release changes update the native map list; Open mod tabs update their entries when there are changes to the release, sightings or profile. When you exit debug mode, the normal viewing filter applies again.

The previous `Priority.Last` inspection overwrote Fusion's independent debug release; The mod tabs also filter directly for sightings. Both ways now use temporary release. Game and test programs did not start. Check manually: unknown native zombies, Zombotany, mod zombies and duck variants in debug mode; then the same entries outside of debug mode and after changing the profile. [Port Proof](../fusion-mod/references/feedback-20-0-4/report.md).

MelonLoader library for PvZ Replanted. The library registers its own zombies, sounds and rubber duck variants. Each registered mod gets its own tab in the Zombie Almanac. BloomEngine provides the menu entry: native zombie as icon, darkened native lawn in icon as background.

## Almanac correction (1.1.1)

The previous filtering removed entries from `almanac.zombies.all`. The native detail binders continue to expect valid, fixed indices there, especially `all.0`. `Player-prev.log` reports correspondingly `Failed to find model [name almanac.zombies.all.0.name]` as well as missing description, zombie and background models. If the viewing list was empty, the required model did not exist at all.

Version 1.1.1 retains all native entry models and their indexes. The sighting rule instead controls the native lock status: the game uses this to create its card list `visible`. Unseen zombies and the six Zombotany types shown separately are excluded there. Plants are not changed. New sightings update the map list in `LateUpdate`; A level completion is not required. Changing the profile and rebinding the data provider also triggers an update. Existing sightings remain intact; native savegames are not changed.

Static documents: `references/almanac-1.1.1.md`. Compilation against the installed game references successful. Game and test programs did not start. To be checked manually: empty profile without red error texts, existing sightings after restart, first sighting without level completion, level completion, profile change as well as Zombotany and Fusion tabs.

## Zombotany and Sightings (1.1.0)

In addition to the mod tabs, there is the **Zombotany** tab with Peashooter, Walnut, Jalapeno, Gatling Pea, Pumpkin, and Giant Walnut Zombie. These six native types are exclusive to this tab. Their native generation, AI, and stored identities remain unchanged. The portraits come from the existing native Pose exports; `assets/zombotany-sources.json` documents origin and hashes.

All zombie entries only appear after the first sighting: native zombies, zombotany and mod zombies. A sighting requires a live, visible zombie on the active board during play. Level previews, almanac previews, paused boards, and completely off-field zombies do not count. Land and additional duck variants are recorded separately. A mod zombie does not unlock the underlying native zombie.

The library stores sightings per profile GUID under `UserData/ZombieLibrary/sightings.json`. Native savegames, achievements and activations will not be changed. Previous sightings cannot be reliably reconstructed: existing profiles also begin with an empty sighting list. Zombies that have already been defeated are not unlocked across the board. In the event of read errors, the existing file is retained; If there are typos, new progress remains available for the session and the error is logged.

The full native entry list is preserved so that detail binders and selection indexes remain valid. The native list `visible` only contains zombies sighted outside of Zombotany. New sightings and profile changes update visibility via native lock refresh without replacing entry models. Seen native zombies receive access to the almanac display regardless of their previous level lock. Empty mod tabs show “No zombies spotted yet.”; Controller targets remain within the visible page.

Compilation and installation take place without starting the game and without test programs. To be checked manually: first encounter of all six types, invisible previews, native entries before/after sighting, separate duck release, profile change, restart, language and controller.

## Menu icon and startup error (1.0.4)

The implementation now follows the current Plant Library: Zombie and dark native lawn are merged into a single opaque square `menu-icon.png`. BloomEngine only handles placement and framing. There is no additional background space behind the description text and no periodic menu layout code.

Version 1.0.3 called `RectTransformUtility.CalculateRelativeRectTransformBounds`. This helper cannot be used in the installed IL2CPP game; `Latest.log` reports `System.NotSupportedException: Method unstripping failed`. The call and the entire menu update path have been removed. The repeated errors are not caught or hidden, but their calling implementation is eliminated. A successful build alone does not check the availability of such stripped methods.

`tools/export_assets.py` creates the icon reproducibly from the existing native zombie and lawn exports. The brightness of the lawn is 35%, the icon size is 512 × 512 pixels. `assets/sources.json` documents sources, composition and hashes. Game and tests did not start; The display the next time you start the game remains to be checked manually.

## Previous menu implementation (1.0.3, replaced by 1.0.4)

In the screenshot `Bildschirmfoto_20261103_074616.png`, the lawn ends in the middle of the transparent zombie icon. BloomEngine positions the icon container to the left outside of the actual entry area. The background now includes the actual icon boundaries including frames. Boundaries are recalculated after layout and resolution changes. The decorative background does not participate in the native layout. The native zombie sprite and native menu frame are retained. Game and tests did not start.

## Selection brightness (1.0.2)

Screenshots `Bildschirmfoto_20261103_071611.png` and `Bildschirmfoto_20261103_071627.png` show the native brightening and the previously white mod map. The selection now uses the unchanged native `ColorBlock` of the zombie button directly on the map image. The solid white `OverState` graphic remains disabled. Separate mod portraits and duck images follow the same color transition animation. Game and tests did not start.

## Native map view (1.0.1)

The screenshots from November 3, 2026 show the difference between native zombie maps and the previous gray mod maps. The library uses exported native card backgrounds and frames. Native button colors highlight hover and selection. Additional green card names are omitted; Names are in the detailed view as in the original. The native grid alignment is preserved. The templates no longer require their removed image binders to obtain valid card graphics.

`ZombieDefinition.Thumbnail(german, duck)` can provide a full card image including frame. Without a callback, the library adds `Portrait` to the native framework. Fusion provides custom close-ups for both specialty zombies and original almanac images for native zombies. Duck variants include the native duck in the preview image. All sprites remain the property of the respective mod.

`tools/export_almanac_assets.py` exports the native map layers and builds the Fusion preview images from existing original graphics. Origin and hashes are in `assets/almanac-sources.json`. The export does not start the game or tests.

## registration

```csharp
using ZombieLibrary;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

var mod = new ModCatalog("my-mod", "Meine Zombies");
Zombies.RegisterCatalog(mod);
Zombies.RegisterSound("my-mod:spawn", FoleyType.NewspaperRarrgh);
var zombie = Zombies.RegisterZombie(mod, "tough",
    new ZombieIdentity(ZombieType.Normal, MarkerField.GraveX, -763100),
    new EntryText("Tough Zombie", "Twice the normal health."),
    new EntryText("Zäher Zombie", "Doppelte normale Lebenspunkte."),
    rubberDuck: true);
zombie.Initialize = actor => actor.mBodyHealth = actor.mBodyMaxHealth = 540;
zombie.SpawnSound = "my-mod:spawn";
zombie.Portrait = german => myPortraitSprite;
// In einem vorhandenen Board:
var actor = Zombies.Spawn(zombie, board, row);
```

Specify dependency in your own mod:

```csharp
[assembly: MelonAdditionalDependencies("ZombieLibrary")]
```

`RegisterCatalog` must be called before `RegisterZombie`. IDs and identities must be unique. Once published, saved markers may not be changed. The library rejects synthetic zombie enums. Negative markers are supported in `Normal`/`TrafficCone`/`Pail.mGraveX`, the seventeen armored native AI types listed above using `mGraveX`, and `PeaHead.mSummonCounter`. These fields are unused by the respective native AI in the supported game build; their native serialization and reset paths are retained. Other native AI types still require field-lifetime evidence before registration is allowed.

`Apply(definition, actor)` converts a freshly created, living native zombie of the appropriate type. The marker field must be free. Other registered variants are not overwritten. Repeatedly applying the same definition will not initialize and resound the zombie. `Spawn` encapsulates native generation, checks row boundaries, and removes the new zombie if its downstream initialization fails. Deliberately forced summons retain native factory semantics; Natural spawn selection and level rules remain the responsibility of the creating mod.

Natural random substitutions check `Zombies.IsExplicitSpawn` so that an explicit summon doesn't get a different variant. You should only replace suitable, unmarked normal zombies. The library itself does not add zombies to waves. The complete example shows this test.

`Find(actor)` instantly detects variants based on native stored fields. After loading, `Tick` runs, but neither `Initialize` nor `SpawnSound`. When reused by the native object pool, the library removes the known marker. `Tick` follows `Zombie.Update` and pauses the board; The mod checks its own rules for death, game mode and special phases. `RefreshVisuals` runs once per image before the duck display and is not allowed to continue paused animations itself. Own running states must be in demonstrably suitable stored fields.

## Sounds and rubber ducks

`RegisterSound(id, FoleyType)` uses the native sound system with its volume and mute controls. `RegisterSound(id, Action<Zombie>)` allows your own sound sources. `PlaySound(id, actor)` plays registered sounds at a self-selected event. Resources and runtime of your own audio sources belong to the registering mod. Unknown and duplicate sound IDs are rejected.

`rubberDuck: true` enables pool support and automatically creates an additional almanac entry. The variant remains the same saved zombie; native pool states decide between land and water. Movement, AI, collisions and abilities are retained. The library uses the original rubber duck, native spine poses, waterline clipping and matching draw orders. Dancing zombies retain their stand-up phase and adjacent line check.

`RegisterDuckVariant(mod, id, nativeType, english, german)` registers the additional pool variant of an existing native zombie. Supported are Football, JackInTheBox, Dancer, BackupDancer, Yeti, Gargantuar, RedeyeGargantuar, SquashHead and PeaHead. Such entries use native generation, not their own markers. `RegisterNativeDuck(type)` enables the same support without additional almanac entry. The selected native type basically receives pool support.

Your own rigs can set `PlacePoolDuck(actor, renderer)`. The callback places the duck managed by the library and only returns `true` when the pose is finished. `PoolDuckVisuals.Waterline` and `PoolSpriteClip` allow the same waterline cut for imported body graphics. Fusion uses this for the eye zombie.

## Almanac and menu

The library clones native zombie maps, zombie detail view, zombie backgrounds, buttons and fonts under an inactive parent object. Native data binders and transition animations are removed before activation. Five columns, page navigation, scrolling, controller navigation, language switching and mod tab switching are supported. Native index and close actions are preserved. Profile activations will not be changed.

`Portrait` and optionally `DuckPortrait` provide borrowed sprites; the mod has its lifespan. Without its own `DuckPortrait`, the library complements the native duck in the normal portrait. For sprites loaded asynchronously, the callback may initially deliver `null`. The view queries the portrait again.

Fusion 19.1.0 uses the library for Hypnopea Zombie and Eye Zombie and their rubber duck variants. Eight additional native duck variants are also in Fusion's Zombie tab. The previous markers `-430153` and `-521012` will be retained. Special abilities, random substitutions, hits and loot remain in Fusion; Registration, sounds, ducks and almanac belong to the library.

## Building and installing

```sh
rtk proxy /home/kevin/modding/pvzreplanted/.tools/dotnet/dotnet build /home/kevin/modding/pvzreplanted/fusion-mod/FusionMod.csproj -c Release
rtk proxy /home/kevin/modding/pvzreplanted/.tools/dotnet/dotnet build /home/kevin/modding/pvzreplanted/zombie-library/example-mod/ZombieLibraryExample.csproj -c Release
rtk proxy python3 /home/kevin/modding/pvzreplanted/zombie-library/tools/install.py
```

The installer builds and installs `ZombieLibrary.dll`, `PlantLibrary.dll` and `FusionMod.dll` together. It uses the same release lock as Fusion, backs up old DLLs, detects interim changes and replaces files atomically. In case of an error during DLL installation, it restores DLLs that have already been replaced. `backups/<Zeitstempel>/installation.json` documents old and new SHA-256 hashes. Game and test programs will not start.

The [Example](example-mod/README.md) stays outside the game folder by default. `--example` installs it additionally. `--no-build` installs already built release files.

To restore, close the game and restore all three DLLs together from the documented backup. A file with previous hash `null` was not previously installed. Remove this file when dismantling. The library may not be removed while a dependent mod is installed.

## Open manual review

Builds check compilation against installed IL2CPP references. Game and tests were not running at user request. To check manually: mod menu, mod tabs, native zombie assets, all ducks on land and in the pool, controller navigation, voice switching, pause, hypno-summon, eye zombie abilities, sound frequency, and saving and resuming existing variants.

Prepared offline suite, run manually only:

```sh
rtk proxy /home/kevin/modding/pvzreplanted/.tools/dotnet/dotnet run --project /home/kevin/modding/pvzreplanted/zombie-library/tests/RegistryTests.csproj -c Release
```

The suite tests real registration logic with data doubles without game assemblies. It covers identity conflicts, initialization, sounds, memory markers, pause, reset, bad spawns, and native duck registration. Fusion's existing Hypno, Eye Zombie and Pool suites are adapted to the new dependency limit.

Graphics source: `assets/sources.json`; Native Zombie layout data: `assets/native-almanac-layout.json`. `tools/export_assets.py` takes over existing native exports without starting the game. Common native UI sanitization and voice query are included sources under `../shared/`; There is no runtime dependency between Zombie Library and Plant Library.

## Debug-API

The optional Debug Mod 1.1.0 can query registered almanac entries and override their visibility for the current session. Plant Library additionally supports session-related plant selection overrides. Without a debug mod, the normal library rules still apply. Instructions: `../debug-mod/src/documentation.md`.
