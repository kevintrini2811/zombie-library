# Static analysis of almanac correction 1.1.1

## observation

In the existing `Player-prev.log`, missing models are `almanac.zombies.all.0.name`, `zombieType` and `thumbnailBackground`. The existing triage file already contains five native types and a Fusion entry. The recording of sightings basically works; resetting this file is neither necessary nor intended.

## Native data binding

The installed bundle `uipanels_assets_all_68c57e595d634e479472142eadcc5c3a.bundle` contains two card lists with `m_key.Path = almanac.zombies.visible`. The detailed views use `almanac.zombies.selected` and the format `almanac.zombies.all.{0}`. The full list and the visible list have different tasks.

`GameAssembly.dll`, RVA `0x70ca20` (`AlmanacEntriesModel.UpdateEntries`): builds the full entry models with stable source indices. Only unlocked entries will be included in `m_visibleEntriesModel`. RVA `0x70c470` (`UpdateEntriesLockedState`): empties and refreshes the visible list based on the lock status; If the numbers differ between the source and existing entry models, an error is logged. Previous source filtering violated these binding and indexing requirements and left `all.0` completely missing from zero sightings.

## Fixed and invariants

- Source lists are no longer filtered or renumbered. Native entry models are retained.
- `UserService.IsLocked(AlmanacEntryData)` only decides on zombie entries based on profile-related sightings. Zombotany is reserved for your own tab.
- The native lock refresh handles building and updating `visible`, including counters and native selection actions.
- New sightings increment a session revision even if saving fails. The bound AppDataProvider updates its zombie list when the revision or profile GUID changes. When unbinding, the reference is released.
- Mod and duck releases, viewing criteria, native savegames and existing viewing files are retained.

## Test and limits

Release build with IL2CPP references installed: successful, no warnings or errors. No test programs and no game start. The UI display, controller navigation, profile changes and first encounters remain to be checked manually. The separate messages `Releasing render texture that is set as Camera.targetTexture!` upon exit are not evidence of the missing almanac models; their fix is ​​not confirmed with this change.
