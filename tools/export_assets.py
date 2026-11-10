"""Reuse native exports for Zombie Library assets without loading game code."""
import hashlib
import json
from pathlib import Path
import shutil
from PIL import Image, ImageEnhance, ImageOps

ROOT = Path(__file__).resolve().parents[1]
FUSION = ROOT.parent / "fusion-mod"

if __name__ == "__main__":
    source = FUSION / "assets/debug-cards"
    entry = next(item for item in json.loads((source / "zombie-packets.json").read_text())
                 if item["Type"] == 0 and item["Role"] == "Portrait")
    output = ROOT / "assets"
    with Image.open(source / "zombie-packets.png") as atlas:
        x, y, width, height = (entry[key] for key in ("X", "Y", "Width", "Height"))
        atlas.crop((x, atlas.height - y - height, x + width, atlas.height - y)).save(output / "zombie.png")
    shutil.copyfile(FUSION / "assets/pool-duck/duck.png", output / "duck.png")
    shutil.copyfile(ROOT.parent / "plant-library/assets/lawn.png", output / "lawn.png")
    # Match Plant Library: BloomEngine receives one opaque square sprite and
    # owns placement and border. No runtime background or bounds API is needed.
    with Image.open(output / "lawn.png") as lawn:
        icon = ImageOps.fit(lawn.convert("RGBA"), (512, 512), method=Image.Resampling.LANCZOS)
    icon = ImageEnhance.Brightness(icon).enhance(.35)
    icon.putalpha(255)
    with Image.open(output / "zombie.png") as zombie:
        foreground = zombie.convert("RGBA")
        bounds = foreground.getbbox()
        if bounds is None:
            raise ValueError("Native zombie portrait is empty")
        foreground = ImageOps.contain(foreground.crop(bounds), (440, 440), method=Image.Resampling.LANCZOS)
    icon.alpha_composite(foreground, ((512 - foreground.width) // 2, 512 - foreground.height - 28))
    icon.save(output / "menu-icon.png")
    proof = {"icon_source": str(source / "zombie-packets.png"), "icon_rect": entry,
             "duck_source": "fusion-mod/assets/pool-duck/duck.png", "lawn_source": "plant-library/assets/lawn.png",
             "icon_size": [512, 512], "lawn_brightness": .35, "foreground_bounds": [440, 440], "foreground_bottom": 28,
             "game_started": False, "assets": {path.name: hashlib.sha256(path.read_bytes()).hexdigest()
                                                for path in output.glob("*.png")}}
    (output / "sources.json").write_text(json.dumps(proof, indent=2) + "\n")
