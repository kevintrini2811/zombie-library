# Native zombie sprite colours

Game binary and metadata fingerprints are in `native-inputs.json`. `research_native.py` decoded the three `replanted-*.asm` files. Shader bundle identity and parameter-record offsets are in `shader-evidence.json`. The shader DXBC was extracted read-only with UnityPy/LZ4 and disassembled with Wine D3DDisassemble.

## Native CPU state

`Reloaded.Gameplay.Zombie.DrawReanim`, token `0x6001d0f`, RVA `0x6ece50`: cold counters at offsets `0x104` and `0x10c` select branch `0x6ed403`, writing RGB bytes 75, 75, 255. Calls `0x6ed5c8`, `0x6ed60f`, `0x6ed62e` set colour override, extra additive colour and its enable flag. Native material-controller state stores the enable flag at `0x61` and colour at `0x64`.

## Native pixel operation

`native-skeleton-straight-fragment.asm` uses native colour material shader `Universal Render Pipeline/Spine/SkeletonAdditive`, straight-alpha variant. Relevant constants are `cb0[129].x` (ignore override), `.y` (enable extra additive), `.z` (enable extra overlay), `cb0[131]` (extra additive), `[132]` (extra overlay), `[133]` (override).

Let `T` be sampled RGB, `O` the native override and `A` the extra additive input, all in linear colour space. With normal override and additive enabled, and no grayscale/extra overlay/high contrast:

```text
B = T * O
overlay(B, A) = B <= 0.5 ? 2 * B * A : 1 - 2 * (1 - B) * (1 - A)
RGB = B + overlay(B, A)
```

The fragment program then applies texture alpha (straight-alpha variant) and premultiplied vertex colour. This operation preserves black outlines and texture detail while boosting blue. For the review's source pixel sRGB `(128,142,116)`, cold native inputs `(75,75,255)` predict approximately `(36,41,191)`, within two RGB units of the observed `(35,39,192)` screenshot sample (filtering/coordinate sampling can differ).

The preview sprite shader's `_BlendStrength=1` instead discards texture RGB through a lerp, producing flat silhouettes. It cannot implement the native operation by forwarding additive colour. The rejected first implementation is not retained.

## Shader reuse and ownership

`ZombieSpriteColor.CreateMaterial` clones the actor's existing native colour material. It reuses the original shader and compiled pixel operation; no custom shader or per-pixel C# approximation is introduced. Exported PNGs select `_STRAIGHT_ALPHA_INPUT`. `native-skeleton-vertex.asm` accepts POSITION/COLOR/UV and expects premultiplied vertex colour; `Apply` supplies it while forwarding native override/additive/overlay inputs through a property block.

Consumers cache one shared material per artwork family, destroy it on unload, and supply this frame's unmodified source colour. Native material instances are not modified. Source alpha reaches the native shader exactly once, including armour. Shadows bypass character colour effects. Existing pool clipping properties remain intact.

Offline regression: `dotnet run --project tests/ColorTests.csproj -c Release`. Covers native material reuse, PMA vertex encoding, straight-alpha texture variant, exact shader inputs, pool properties, shadows, thaw, current armour alpha, and overlays. Runtime screenshots are recorded separately by review.
