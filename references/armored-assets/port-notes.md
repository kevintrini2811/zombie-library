# Native armored zombie artwork

The product assets in `assets/armored/` are exported by `tools/export_armored_assets.py`. They contain 34 transparent, complete native zombie poses, six native cone/bucket damage-stage images, the unchanged native-derived `placement.json`, and explicit native head-slot metadata.

## Source and reproduction

Run the exporter with `../.tools/fusion-workflow/bin/python tools/export_armored_assets.py` from the Zombie Library project. It reads the installed game Spine bundle, retains unchanged skeleton/atlas/texture inputs, and reuses the existing offline Spine decoder and triangle rasterizer from Fusion Mod. It does not run Unity or create rendering cameras. `export-proof.json` records source checksums, Unity path IDs, each output checksum, bone-local head bounds, the posed bone affine, and the resulting helmet quad.

The native zombie prefab bundle supplies `m_skinSets`, including its named replacement and hidden skins. Full hierarchy paths and original component path IDs are retained in `native-skinsets.json`. The Flag set omits the ordinary back arm because its flag skin supplies the holding arm. The screen-door and trash-can skins supply both holding arms; their ordinary front and back arms use the native hidden variants. This prevents duplicate hands. Held flags, poles, newspapers, doors, ladders, jack-in-the-boxes, dolphins, pogo sticks, catapults and the Gargantuar's carried Imp remain part of their complete native poses.

## Shared head fit

Every family uses the active `anim_head1` slot except Catapult, whose real driver head is `Zombie_catapult_driver_head`. The Gargantuar's carried Imp head is deliberately not used as the Gargantuar helmet socket.

The posed region or weighted-mesh vertices are transformed through the inverse head-bone affine into bone-local coordinates. Their local bounds define a conceptual head quad. The normalized native fit controls helmet width, horizontal bottom-center offset and vertical offset relative to this quad. Both portrait export and runtime use the same six helmet images and placement values.

SpriteRenderer cannot represent arbitrary shear. The export therefore matches runtime's transform approximation: rotation follows the posed bone Y axis, X reflection follows the affine determinant, and the two scale lengths follow the bone axes. The helmet bottom-center position still uses the full bone affine. No portrait-pixel offsets or world-axis-aligned head boxes are used.

Product portrait images are uniformly reduced to at most 512 pixels in either dimension after fitting the full pose. Their transparent bounds are cropped; the consumer creates the requested bottom-center foot pivot. The separate runtime helmet images remain unchanged and retain their native-derived padding and aspect ratio.

## Limits and verification

The portraits represent one intact native pose for each family; they are not a runtime animation capture. Runtime must continue to follow the animated head slot and native armor stages. Portraits do not add a separate duck pose: existing terrain-aware pool presentation remains the runtime owner's responsibility. The three committed contact sheets [1](portrait-review-1.png), [2](portrait-review-2.png) and [3](portrait-review-3.png), together with exported recipes and source metadata, permit visual and numeric review without changing the game installation.
