"""Export native zombie UI artwork and Fusion thumbnails without starting Unity."""
from pathlib import Path
import hashlib
import json
import UnityPy
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
FUSION = ROOT.parent / "fusion-mod"
GAME = Path("/mnt/FastGaming1/SteamLibrary/steamapps/common/PVZ Replanted")
BUNDLES = GAME / "Replanted_Data/StreamingAssets/aa/StandaloneWindows64"


def export():
    paths = sorted(set(BUNDLES.glob("defaultlocalgroup_assets_ui/sprites*.bundle"))
                   | set(BUNDLES.glob("uiatlas*.bundle")))
    environment = UnityPy.load(*(str(path) for path in paths))
    sprites = {obj.read().m_Name: obj for obj in environment.objects if obj.type.name == "Sprite"}
    sources = {}

    def native(name):
        obj = sprites[name]
        sources[name] = obj.path_id
        return obj.read().image.convert("RGBA")

    for file, name in {
        "zombie-card-background.png": "SPR_Almanac_zombieEntries_background",
        "zombie-card-border.png": "SPR_Almanac_zombieEntries_border",
        "zombie-card-over.png": "SPR_Almanac_zombieEntries_Over",
    }.items():
        native(name).save(ROOT / "assets" / file)

    output = FUSION / "assets/zombie-almanac"
    output.mkdir(exist_ok=True)
    background = native("SPR_Almanac_zombieEntries_background").resize((262, 262), Image.Resampling.LANCZOS)
    border = native("SPR_Almanac_zombieEntries_border").resize((262, 262), Image.Resampling.LANCZOS)
    portraits = FUSION / "references/zombie-accuracy-13-0-9/portraits"
    # Vanilla busts already contain the original broken stone frame.
    cards = {str(kind): native("SPR_Almanac_Zombies_" + name) for kind, name in {
        5: "NewspaperZombie", 6: "ScreenDoorZombie", 13: "ZombieBobsledTeam",
        16: "BalloonZombie", 18: "PogoZombie", 21: "LadderZombie",
        24: "Imp",
        7: "FootballZombie", 15: "JackInTheBoxZombie", 8: "DancingZombie",
        9: "BackupDancerZombie", 19: "YetiZombie", 23: "Gargantuar",
    }.items()}
    # The imported Eyes artwork and native PeaHead/SquashHead pose exports
    # have no native almanac thumbnail. Frame their upper bodies consistently.
    for key, source in {
        "eyes": FUSION / "assets/debug-cards/eyes-zombie.png",
        "34": portraits / "34.png",
        "hypno": FUSION / "assets/hypno-zombie.png", "30": portraits / "30.png", "32": portraits / "32.png",
    }.items():
        art = Image.open(source).convert("RGBA")
        art = art.crop(art.getbbox())
        # A bust fills the frame; feet stay outside the inner portrait window.
        art = art.crop((0, 0, art.width, round(art.height * .62)))
        art.thumbnail((222, 222), Image.Resampling.LANCZOS)
        card = background.copy()
        card.alpha_composite(art, ((262 - art.width) // 2, 24))
        card.alpha_composite(border)
        cards[key] = card
    # Trash Can uses the native preview attachment, absent from the bust atlas.
    preview = FUSION / "assets/debug-cards/zombie-packets.png"
    entries = json.loads((FUSION / "assets/debug-cards/zombie-packets.json").read_text())
    entry = next(e for e in entries if e["Type"] == 35 and e["Role"] == "Portrait")
    atlas = Image.open(preview).convert("RGBA")
    y = atlas.height - entry["Y"] - entry["Height"]
    art = atlas.crop((entry["X"], y, entry["X"] + entry["Width"], y + entry["Height"]))
    art = art.crop(art.getbbox()); art = art.crop((0, 0, art.width, round(art.height * .62)))
    art.thumbnail((222, 222), Image.Resampling.LANCZOS)
    card = background.copy(); card.alpha_composite(art, ((262-art.width)//2, 24)); card.alpha_composite(border)
    cards["35"] = card
    duck = Image.open(ROOT / "assets/duck.png").convert("RGBA")
    duck.thumbnail((156, 94), Image.Resampling.LANCZOS)
    for key, card in cards.items():
        card.save(output / (key + ".png"))
        swimming = card.copy()
        swimming.alpha_composite(duck, ((262 - duck.width) // 2, 242 - duck.height))
        swimming.save(output / (key + "-duck.png"))
    proof = {"sprites": sources, "game_started": False,
             "bundles": {str(path): hashlib.sha256(path.read_bytes()).hexdigest() for path in paths},
             "outputs": {str(path): hashlib.sha256(path.read_bytes()).hexdigest()
                         for path in list(output.glob("*.png")) + list((ROOT / "assets").glob("zombie-card-*.png"))}}
    (ROOT / "assets/almanac-sources.json").write_text(json.dumps(proof, indent=2) + "\n")
    print(f"Exported 3 native card layers and {len(cards) * 2} Fusion thumbnails.")


if __name__ == "__main__":
    export()
