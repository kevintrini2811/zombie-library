# Hypnosis live review

Status: DONE. Verified on 2026-11-06 with Zombie Library 1.3.2 and Fusion Mod
22.2.4, including the consumed-target return contract.

Both Release builds pass without errors or warnings. Registry regression covers
Normal, Conehead, Buckethead and PeaHead registrations, marker retention,
idempotence, target/eligibility guards, and consumption when another mod resists
conversion. Fusion's ScaredContent regression and static recipe checks pass.

The final live test uses a temporary Night debug level. It sets only that
board's effective difficulty to Normal and places awake native Hypno-shrooms.
Native Normal, Eyes, Conehead Eyes and Buckethead Eyes approach and bite their
actual targets. The API never writes `mMindControlled` or invokes a direct
hypnosis shortcut.

Verified results:

- All four actual Hypno-shrooms are consumed through the native callback.
- All four zombies acquire native `mMindControlled:true`.
- Zombie IDs and registered GraveX identity markers remain intact.
- Subsequent simulation moves each actor rightward by 5.22–27.88 native pixels.
- Enabled imported head renderers lie to the right of their torso renderers
  after conversion: X offsets are +24.38 to +44.01 world units. The unchanged
  hostile artwork baseline has approximately -44 world units, confirming
  actual imported graphics reverse with allegiance.
- The converted actors damage separate hostile zombies in their respective
  rows. The test observes 36, 2100, 2600 and 2300 damage with no attacking
  plants present.

The first control run used the existing Extreme difficulty. Its intentional
50% hypnosis resistance can consume a Hypno-shroom without conversion,
including for an unmodified native Normal zombie. The final implementation
preserves that native/mod callback result and ends ordinary bite processing
when the plant has already been consumed. It does not force conversion.

The test restores the previous speed and board difficulty, requests no profile
mutations, and shuts down regularly. Installed hashes match the tested build.
Only the separately documented shutdown errors appear; no new runtime errors
are introduced by the hypnosis adapter.

[Selected native states, movement, combat and renderer evidence](runtime.json).
Full API records and screenshots remain locally under
`/home/kevin/modding/pvzreplanted/.test-runs/zombie-hypnosis-final-20261106/`.
