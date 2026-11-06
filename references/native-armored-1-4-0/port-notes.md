# Native armored zombies: 1.4.5 port notes

The feature adds two registered variants of seventeen real native zombie types. The archive directory retains the initial feature version in its name; its final candidate currently has version 1.4.5. The final review passed within the explicitly recorded verification limits. This report separates static/offline evidence from completed live review; [validation.json](validation.json) lists the exact local evidence and the 17-family ability results.

## Ownership and invariants

Zombie Library owns registration, the additional helmet, portraits, pool support and natural substitution. Armor registration does not replace the original native factory type or actor. Native AI transitions remain authoritative, including Trash Can's transition to `Normal` after shield removal. No invented enum, replacement AI, replacement projectile, substituted group member or alternate serialization format is introduced.

The initial native helmet fields are empty for the sixteen requested types other than the bobsled sled owner. Their Conehead/Buckethead variant uses native `HelmType.TrafficCone`/`Pail` with 370/1100 health. Native `TakeDamage`, shield handling, helmet damage, dropped equipment, magnet attraction and body damage remain the actual gameplay path. Body and shield fields are not assigned by the new initializer.

| Native type | Preserved native equipment or behavior | Original helmet slot |
| --- | --- | --- |
| Flag | Flag and native flag-wave role | None |
| Polevaulter | Pole, run and vault | None |
| Newspaper | Newspaper shield and rage transition | None |
| Door | Screen-door shield | None |
| BackupDancer | Summoned group links, rise and dance | None |
| Bobsled | Sled, leader/follower identities and crash | Leader: Bobsled, 300 health in the supported build; followers: None |
| DolphinRider | Dolphin, pool entry and jump | None |
| JackInTheBox | Box, timer and native explosion | None |
| Pogo | Pogo and native break transition | None |
| Yeti | Native walking/fleeing state | None |
| Bungee | Rope, descent, grab and ascent | None |
| Ladder | Held ladder shield and placement | None |
| Catapult | Vehicle and native ammunition/firing | None |
| Gargantuar | Club and carried/thrown Imp | None |
| Imp | Native Imp actor and movement | None |
| TrashCan | Trash-can shield | None |
| RedeyeGargantuar | Native Giga health/behavior, club and Imp | None |

This table records native slot ownership, not a second set of body or shield tuning constants. Runtime comparison against native control actors remains necessary for their actual computed values.

## Native shield and added helmet composition

Version 1.4.5 preserves exact damage boundaries when a registered added helmet coexists with a native Door, Newspaper, Ladder or Trash Can shield. Native `TakeShieldDamage` already returns the unabsorbed damage, but the original `TakeDamage` implementation can then pass its full original argument to `TakeHelmDamage`. These native base families normally do not combine both layers. Adding a helmet exposed that previously absent composition: a shield-breaking hit could be charged again in full against the helmet.

The generic `NativeArmorShieldDamageScope` records the actual native shield result during that registered actor's current hit and forwards that remainder once to its added helmet. The original native shield, helmet and body methods still calculate their own damage and effects. The scope matches pointer, DataID, board and exact registered marker; a nested or ineligible hit cannot inherit another hit's pending remainder. Finalizers restore the previous scope. `BypassesShield` and `HitsShieldAndBody` retain their native semantics and do not replace their helmet argument with ordinary shield overflow.

Trash Can's native shield detachment changes its type to `Normal`; the library does not reset or override that transition. Only the two exact registered Trash Can armor identities accept that alias, and only while the actual type is `Normal` and shield health is zero. The saved marker and remaining helmet therefore continue to identify the same actor after its native equipment transition. Unrelated markers, native types and live shields do not gain an alias. The same hit can still forward its shield remainder after the transition because its exact owned marker and life are unchanged.

## Persistent identity

The catalog is `native-armored`; entry IDs are `cone-{ZombieType}` and `bucket-{ZombieType}`. Native `mGraveX` holds a stable negative marker:

```text
cone:   -741000 - 2 * (int)ZombieType
bucket: -741001 - 2 * (int)ZombieType
```

Those markers must not be changed after release. The registration allowlist extends the existing `ZombieIdentity` mechanism to exactly these supported native types. Native duck-only entries are presentation metadata: applying a real variant can supersede that fallback without stealing another registered actor's identity.

The supported `GameAssembly.dll` SHA-256 is `e7333eaf87996fa44b4a3526986272319b7d01f7acdbcff52451652f61ceb696`. Native field evidence places `mGraveX` at `0x80` and `mGraveY` at `0x84`. Both are serialized by native `Zombie.Serialize` and restored by `Deserialize`. Loading identifies the variant without repeating its initializer.

Version 1.4.3 handles actor reuse at both the pooled `Reset` hook and the fresh `ZombieInitialize` prefix. Native initialization can retain these legacy coordinates while the actor's native type has already changed. Cleanup therefore matches exact **registered field/marker pairs** without relying on the current `ZombieType` or DataID. A matched GraveX identity also clears its paired owned `mGraveY` state, including the bobsled's outer hat health. Duck-only entries own no serialized field and are skipped. Unknown marker values, native fields and unrelated counters are left untouched. The cleanup is not attached to deserialization, so restored save identities remain valid.

The native `RiseFromGrave` and `UpdateZombieRiseFromGrave` methods in this build do not use these legacy grave-coordinate fields. Field-offset candidates in `UpdateReanim` belong to a controller/renderer object, not to the zombie instance; similarly, the candidate in `ZombiquariumFindClosestBrain` refers to another object. Raw offset hits alone were not accepted as type evidence. The native input checksums, candidate lists, lifecycle methods and full disassembly are in [native/armored-zombies-native](native/armored-zombies-native/) and [native/armored-bobsled-native](native/armored-bobsled-native/).

## Bobsled: separate outer hat

The bobsled leader's native `HelmType.Bobsled` and 300 health are the sled, not an ordinary hat. Its native `DropHelm` branch changes the leader and followers into their crash/disembark phases. `DrawBobsledReanim` also directly reads the native helmet thirds to choose damaged sled art. Merely replacing the helmet type, or adding the new hat's health to that native slot, would change those rules.

The leader therefore stores only its additional hat health in its existing unused, serialized `mGraveY`. For actual hits, `TakeHelmDamage` first consumes that outer layer and forwards any remaining damage to the unchanged native sled method. Flash and chill still occur when the outer layer absorbs the whole hit; partial overflow lets the native method apply those effects once. Once the outer layer reaches zero it no longer intercepts damage. Headless actors do not receive protection from the added hat.

Version 1.4.4 distinguishes native sled wear from those actual hits. `UpdateZombieBobsled` synchronously calls `TakeDamage(6, DoesntCauseFlash)` for sled wear; an actor-, generation- and board-checked call scope lets that exact native wear path reach the original sled health without consuming the added hat. Damage of the same size/flags outside that native update remains a real hit. The scopes restore their previous state in finalizers and introduce no new saved state.

After the sled crashes, its native helmet can be `None` while the added hat still has health. The normal `TakeDamage` scope then lets the outer layer absorb at its body-damage stage. Direct `TakeBodyDamage` calls, which intentionally bypass helmets, are not intercepted as ordinary helmet hits. When the added hat breaks, its real native Cone/Pail drop effect runs in a narrow temporary helmet scope and finally restores the original sled fields; the effect does not invoke the Bobsled crash branch.

Native followers whose helmet slot is empty use the ordinary native helmet fields. Their existing follower DataIDs establish ownership even before the native cached `mIsBobsledLeader` flag becomes true. Binding therefore uses those real group links and retries during the variant's tick when links become available later. During natural construction, a follower can inherit its already armored native parent's variant. No group member, DataID, controller or actor is replaced, and already registered actors are not overwritten.

Evidence includes `BobsledCrash`, `IsBobsledTeamWithSled`, `DrawBobsledReanim`, `DropHelm`, `TakeHelmDamage` and the native initialization branches under [native/armored-bobsled-health-native](native/armored-bobsled-health-native/). An offset candidate in the Bungee routines points to a carried **plant**; it was not treated as a zombie-helmet writer.

## Bobsled bucket and native magnets

Only the sled owner's separately stored bucket needs an adapter. The synchronous native `Plant.UpdateMagnetShroom` call temporarily sees that bucket as `HelmType.Pail` with its remaining 1100-based health. The original native range, allegiance, target ranking, equipment, item and cooldown rules remain active.

`UpdateMagnetShroom` calls `MagnetShroomAttactItem` directly at native RVA `0x6ba3a8`. In the bucket branch starting at `0x6b307e`, attraction determines the damage index and directly clears helmet health/type. It does **not** call `DropHelm`. The scope's finalizer recognizes that native clear, commits removal to the added bucket's `mGraveY`, and restores the exact original sled type/health/maximum. A DataID guard prevents restoring old fields into a reused actor; nested scopes do not borrow the same actor twice.

No game-wide magnet behavior or native sled field is permanently changed. The source tests simulate the observed native field clear, not an invented DropHelm callback. The actual native call sequence and complete branches are archived in `native/armored-bobsled-health-native/replanted-0x6001a43.asm` and `replanted-0x6001a52.asm`.

## Natural level selection

The new initialization postfix considers only a real native actor with a nonnegative `mFromWave` during active gameplay. That actual spawn establishes that the base type occurs in this level. It then requires `mZombieAllowed[TrafficCone]` and/or `mZombieAllowed[Pail]` for the corresponding new armor. This avoids excluding legitimate implicit flag, backup-dancer, thrown-Imp or bobsled-follower actors solely because their own base allowed bit is absent.

Eligible ordinary native spawns have a one-in-three substitution chance. If both armor types are allowed, either is chosen with equal probability. An armored bobsled parent's existing followers inherit its allowed kind. The explicit `Zombies.Spawn` scope, temporary debug/sighting-suppressed boards, menu/paused state, dead or mind-controlled actors, and occupied markers are excluded.

The native Backup Dancer summon reads the parent's `mFromWave` when calling the ordinary factory. Parent and wave binding are supported by source tests and native call evidence. Live explicit Backup Dancers exhibit the same native dance phases as controls, but those API actors have no parent. Neither a complete live summoned-group binding nor automatic wave substitution is claimed from debug placement.

## Head sockets, rendering and assets

The shared overlay reads the actual animated head attachment. All requested rigs use `anim_head1`, except Catapult's `Zombie_catapult_driver_head`. Gargantuar's carried Imp head is not used as the main head. Region and mesh vertices are transformed back into bone-local coordinates to form the common head bounds. Native helmet fits then determine width and bottom-center position; the bone pose supplies orientation and reflection. As with the existing duck renderer, a SpriteRenderer uses the posed Y-axis rotation and signed axis magnitudes rather than reproducing arbitrary shear in a custom shader or renderer.

Version 1.4.2 selects the mesh that actually emits the head through native Spine submesh ranges. Separator rigs may legitimately have a disabled root MeshRenderer and enabled part renderers; the root's disabled state is not interpreted as an invisible actor. The helmet uses the head owner's layer/order and is removed when its armor, head, actor, controller or board is gone. Native `anim_cone`/`anim_bucket` slots are hidden so native damage-image swaps cannot create duplicate visible helmets.

`CharacterSkinController.Reset` clears its image-swap and hidden-track collections before controller reuse. The archived implementation is [native/armored-zombies-render-native/replanted-0x6002083.asm](native/armored-zombies-render-native/replanted-0x6002083.asm), including collection accesses at `0x75372c` through `0x753751`. Live native Cone/Pail controls remain correctly rendered with zero own GraveX/Y and no custom overlay after armored actor disposal. API snapshots do not expose controller addresses, so direct same-pointer controller reuse is not claimed; the native reset trace is the corresponding lifecycle contract.

The shared `ZombieSpriteColor` uses the existing native color shader and actual cold/additive/overlay inputs. Source alpha includes the native skeleton, slot and attachment alpha. The existing clipping-plane extraction is reused for both water and grave clipping. All six helmet stages and 34 complete portraits are exported from native assets; recipes use actual prefab skin sets and held-item arm rules. See [armored asset proof](../armored-assets/port-notes.md).

## Pool behavior

All 34 registrations opt into the existing native-type pool support. As in the existing library contract, pool row support operates on the native type, not an invented per-variant enum. The same type's native actor can therefore use the shared tube presentation as well. Pogo's ordinary pool break and the complete native bobsled disembark path remain in place.

Airborne Bungee phases do not borrow the normal zombie's pool entry behavior. Its tube is shown only during the bottom/grab phases. Dolphin entry/riding/jumping keeps the original native water path and dolphin; the additional tube is shown after that mount phase rather than covering the native dolphin or replacing its jump.

## Validation status

Final review: **PASS for 1.4.5**, product SHA-256 `e5e86c7ae7d1ddd3f016a2333e32fede8cc75228ca4e098332ad2ffce0db2425`. The [consolidated validation record](validation.json) maps all 17 families to local runtime evidence and records the limits below.

The Release build has zero warnings/errors. The production registration/gameplay/lifecycle/shield suite passes 1121 assertions. Registry/Hypnosis, native Color, separated-head ownership and SDK/Harmony checks pass. The latter checks 13 compiled patch classes and 34 hook arguments against the installed SDK, including a real-Harmony negative control for the old track-argument mismatch.

Live registration and terrain coverage comprises all 34 variants: the 32 ordinary cases plus both complete Bobsled teams. Native bodies, held equipment and original AI types are compared with native controls; the legitimate TrashCan-to-Normal transition is preserved. The 22 unshielded ordinary break results were re-evaluated from their actual body health and each leaves initial body health minus one, still positive. All eight shield/helmet combinations now have explicit positive-body checks at partial hits, one-point shield overflow, exact helmet depletion, subsequent body damage and large combined-layer overflow. Earlier shield checks using only `mDead == false` are superseded because a dying actor can already have zero body health.

All 17 family behaviors have targeted evidence: native flag/Imp walking, pole vaulting, newspaper rage, shield break, dancer phases, Bobsled group/wear/crash/magnet, Dolphin entry/ride/jump/water walking, Jack explosion, Pogo jumping, Yeti retreat, Bungee plant theft, ladder placement, catapult basketballs, and both Gargantuar smash/Imp throws. These comparisons retain native controls and equipment. Bungee damage/water eligibility remains phase-specific; ordinary ground-hit assertions are not generalized to every airborne phase.

Bobsled followers all receive matching hats. Internal six-point sled wear decreases the native sled exactly like the native control without consuming outer health. Actual hits and overflow consume the correct layers, the outer native drop effect does not crash the sled early, native crashes preserve all four actors and their remaining hats, and surviving hats still protect after a crash. A native Magnet-shroom removes the added bucket while keeping the sled and body intact. Both pool teams finish the native crash animation as eight living armored ducks; direct API placement can briefly slide before native wear causes that crash.

Marker reuse from TrashCan/Redeye into native Zamboni/Normal and then registered Cone-Flag passes, as does Bobsled auxiliary-state cleanup. TrashCan's native transition retains both remaining hat visuals and identity; later exact armor/body boundaries remain correct. Native Cone/Pail are visibly correct after variant disposal. The API does not expose controller addresses, so that observation is combined with inspected native reset behavior rather than described as direct same-pointer proof.

The 34 native portrait assets and six armor stages were independently exported/reviewed. Runtime Flag separators, held flags, head animation, cold tint, pool ducks/clipping, native armor breaks and ordinary Catapult rendering were observed. Full local native research archives and logs are intentionally retained outside Git; the committed source/asset notes and concise validation record identify them.

Limits: protected debug boards suppress random substitutions and do not write player profiles. Production selection/eligibility, native parent/FromWave propagation, serialized fields and reconstructed saved identities are source/native-contract and offline-test verified. No normal-level random-wave distribution, complete live parent-bound dancer group, live player-save roundtrip or direct same-controller-pointer reuse is claimed. No profile workaround or persistent setting change was used.
