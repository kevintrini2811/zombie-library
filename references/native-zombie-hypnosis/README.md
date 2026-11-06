# Native Hypno-shroom bite forwarding

The game fingerprints are in `native-inputs.json`. The existing `research_native.py` tool decoded these native paths.

- `Zombie.EatPlant`, token `0x6001cc7`, RVA `0x6f0eb0`: the awake Hypno-shroom branch at `0x6f0fa1` / `0x6f0fcb` returns before ordinary bite damage.
- `Zombie.Animate`, token `0x6001cc4`, RVA `0x6e51d0`: native animation timing dispatches `AnimateChewSound`.
- `Zombie.AnimateChewSound`, token `0x6001d7a`, RVA `0x6e4f30`: `FindPlantTarget(Chew)` selects the actual target; type 12 and awake state enter native consumption. The sequence calls Plant.Die (`0x6e4ff5`), StartMindControlled (`0x6e4fff`), particles and animation-speed updates.
- `Zombie.StartMindControlled`, token `0x6001d29`, RVA `0x6f7760`: native control updates include the mind-control state, target and group relationships. The registered GraveX/SummonCounter identity markers are not replaced.

A custom bite clock must forward its committed event to this animation-owned gameplay path. `Zombies.TryHypnotizeBite` checks the registered actor, life/head/eating state, pause/stuns, plant life/awake state and exact native target, then invokes `AnimateChewSound`. It never writes the mind-control flag or consumes plants itself. The native method preserves sound, particles and other mods' chew callbacks, including Silver Hypno-shroom rewards.

Eyes invokes the helper only after its source-animation BiteReady reservation. A consumed target or committed hypnosis skips ordinary bite processing. If another mod resists conversion (for example Difficulty Mod Extreme), the helper preserves that rejection while treating the consumed target as handled. Sleeping Hypno-shrooms, other targets and unregistered native actors keep their existing behavior. Native AI does not use this helper and retains its own animation timing.

Regression: `dotnet run --project tests/RegistryTests.csproj -c Release` covers registered normal/cone/bucket/pea identities, identity retention, repeated invocation and rejected eligibility/target cases. Fusion's existing ScaredContent suite covers its bite reservation and combat regressions. Actual Hypno-shroom consumption and post-hypnosis movement/combat are verified separately in live review.
