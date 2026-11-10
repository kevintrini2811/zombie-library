"""Export native armored zombie portraits from the same head geometry as runtime.

Run with ../.tools/fusion-workflow/bin/python tools/export_armored_assets.py.
The native assets and decoded poses are retained as reproducibility evidence.
No Unity process, render camera, or guessed portrait-pixel offset is used.
"""
from __future__ import annotations

import copy
import hashlib
import json
import math
from pathlib import Path
import shutil
import subprocess
import sys

from PIL import Image, ImageDraw
import UnityPy

ROOT = Path(__file__).resolve().parents[1]
FUSION = ROOT.parent / "fusion-mod"
sys.path.insert(0, str(FUSION / "tools"))
from render_zombie_pose import render

GAME = Path("/mnt/FastGaming1/SteamLibrary/steamapps/common/PVZ Replanted")
BUNDLE = GAME / "Replanted_Data/StreamingAssets/aa/StandaloneWindows64/spineassets_assets_assets/art/characters/spine.bundle"
REFERENCE = ROOT / "references/armored-assets"
SOURCE = REFERENCE / "spine"
OUTPUT = ROOT / "assets/armored"

# These are native base-family skins, including their held items. Portraits use
# the same complete head-bearing pose that the added runtime helmet overlays.
RECIPES = {
    "Flag": ("Zombie", "default,FrontArm,Head,flag_default", "walk"),
    "Polevaulter": ("Zombie_polevaulter", "default,FrontArm,Head,InnerArm", "run"),
    "Newspaper": ("Zombie_paper", "default,FrontArm,Head_Calm,Paper1", "walk"),
    "Door": ("Zombie", "default,BackArm_Hidden,FrontArm_Hidden,Head,screenDoor", "walk"),
    "BackupDancer": ("Zombie_backup", "default,FrontArm,Head", "walk"),
    "Bobsled": ("Zombie_bobsled", "default,Zombie_Bobsled_Head,Zombie_Bobsled_Uninjured", "walk"),
    "DolphinRider": ("Zombie_dolphinrider", "default,FrontArm,Head", "walkdolphin"),
    "JackInTheBox": ("Zombie_jackbox", "default,FrontArm,Head,Jackbox", "walk"),
    "Pogo": ("Zombie_pogo", "default,Zombie_Pogo_Head,Zombie_Pogo_Uninjured,Zombie_Pogo_Uninjured_PogoHand,Zombie_Pogo_Undamaged", "pogo"),
    "Yeti": ("Zombie_yeti", "default,Yeti_Head,Yeti_Uninjured", "walk"),
    "Bungee": ("Zombie_bungi", "default", "idle"),
    "Ladder": ("Zombie_ladder", "default,FrontArm,Head,Ladder_Default", "ladderwalk"),
    "Catapult": ("Zombie_catapult", "default,Zombie_Catapult_Pole_Undamaged,Zombie_Siding_Undamaged", "walk"),
    "Gargantuar": ("Zombie_gargantuar", "default,Zombie_Gargantuar_Body1,Zombie_Gargantuar_Head,Zombie_Gargantuar_Imp,Zombie_Gargantuar_Outerarm_Uninjured,Zombie_Gargantuar_Outerfoot_Uninjured,Zombie_Gargantuar_Pole", "walk"),
    "Imp": ("Zombie_imp", "default,Zombie_Imp_Head,Zombie_Imp_Uninjured", "walk"),
    "TrashCan": ("Zombie", "default,BackArm_Hidden,FrontArm_Hidden,Head,trashcan1", "walk"),
    "RedeyeGargantuar": ("Zombie_gargantuar", "default,Zombie_Gargantuar_Body1,Zombie_Gargantuar_Head_Redeye,Zombie_Gargantuar_Imp,Zombie_Gargantuar_Outerarm_Uninjured,Zombie_Gargantuar_Outerfoot_Uninjured,Zombie_Gargantuar_Pole", "walk"),
}


def digest(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def head_slot(kind: str) -> str:
    return "Zombie_catapult_driver_head" if kind == "Catapult" else "anim_head1"


def extract_native() -> dict:
    """Read the original skeletons, atlas texts and texture pages unchanged."""
    SOURCE.mkdir(parents=True, exist_ok=True)
    names = {recipe[0] for recipe in RECIPES.values()}
    environment = UnityPy.load(str(BUNDLE))
    sources = {}
    for obj in environment.objects:
        if obj.type.name == "TextAsset":
            data = obj.read()
            if data.m_Name.rsplit(".", 1)[0] not in names:
                continue
            content = data.m_Script
            if isinstance(content, str):
                content = content.encode("utf-8", errors="surrogateescape")
            (SOURCE / data.m_Name).write_bytes(content)
            sources[data.m_Name] = obj.path_id
        elif obj.type.name == "Texture2D":
            data = obj.read()
            if data.m_Name not in names:
                continue
            filename = data.m_Name + ".png"
            data.image.save(SOURCE / filename)
            sources[filename] = obj.path_id
    expected = {name + extension for name in names for extension in (".skel", ".atlas", ".png")}
    if set(sources) != expected:
        raise ValueError(f"Incomplete native Spine assets: {expected - set(sources)}")
    return sources


def extract_skin_sets() -> dict:
    """Retain the actual native prefab skin-set and hidden-skin definitions."""
    bundle = next(BUNDLE.parents[3].glob("defaultlocalgroup_assets_zombies_*.bundle"))
    environment = UnityPy.load(str(bundle))
    names, transforms, components = {}, {}, []
    for obj in environment.objects:
        if obj.type.name == "GameObject":
            names[obj.path_id] = obj.read().m_Name
        elif obj.type.name == "Transform":
            data = obj.read_typetree()
            transforms[obj.path_id] = (data["m_GameObject"]["m_PathID"], data["m_Father"]["m_PathID"])
        elif obj.type.name == "MonoBehaviour":
            data = obj.read_typetree()
            if "m_skinSets" in data:
                components.append((obj.path_id, data))
    by_object = {game_object: identifier for identifier, (game_object, _) in transforms.items()}

    def hierarchy(game_object):
        parts = [names.get(game_object, str(game_object))]
        transform = by_object.get(game_object)
        while transform in transforms:
            _, parent = transforms[transform]
            if parent not in transforms:
                break
            parent_object, _ = transforms[parent]
            parts.append(names.get(parent_object, str(parent_object)))
            transform = parent
        return "/".join(reversed(parts))

    result = {hierarchy(data["m_GameObject"]["m_PathID"]): dict(path_id=identifier, sets=data["m_skinSets"])
              for identifier, data in components}
    (REFERENCE / "native-skinsets.json").write_text(json.dumps(result, indent=2) + "\n")
    return dict(bundle=str(bundle), sha256=digest(bundle), components=len(result))


def decode(kind: str) -> dict:
    skeleton, skins, animation = RECIPES[kind]
    path = REFERENCE / (kind + "-pose.json")
    subprocess.run([
        "node", str(FUSION / "tools/decode_zombie_pose.mjs"), str(SOURCE),
        skeleton, str(path), skins, animation,
    ], check=True, capture_output=True, text=True)
    return json.loads(path.read_text())


def helmet_geometry(pose: dict, slot_name: str, fit: list[float], image: Image.Image) -> tuple[list[float], dict]:
    """Match the runtime bone-local bounds and its SpriteRenderer TRS.

    Both region and weighted-mesh heads are first expressed in the posed head
    bone's local coordinates. The sprite uses the bone's Y-axis rotation, axis
    lengths, and determinant sign; shear is approximated exactly as runtime.
    """
    head = next(part for part in pose["attachments"] if part["slot"] == slot_name)
    bone = next(bone for bone in pose["bones"] if bone["name"] == head["bone"])
    a, b, c, d = (bone[key] for key in ("a", "b", "c", "d"))
    determinant = a * d - b * c
    if abs(determinant) < 1e-8:
        raise ValueError("Degenerate native head bone")
    local = []
    for x, y in zip(head["vertices"][::2], head["vertices"][1::2]):
        x, y = x - bone["x"], y - bone["y"]
        local.append(((d * x - b * y) / determinant, (-c * x + a * y) / determinant))
    left, right = min(x for x, _ in local), max(x for x, _ in local)
    bottom, top = min(y for _, y in local), max(y for _, y in local)
    width, height = right - left, top - bottom
    x_length, y_length = math.hypot(a, c), math.hypot(b, d)
    if min(width, height, x_length, y_length) <= 0:
        raise ValueError("Empty native head geometry")
    x = (left + right) / 2 + width * fit[1]
    y = bottom + height * fit[2]
    center = (bone["x"] + a * x + b * y, bone["y"] + c * x + d * y)
    # SpriteRenderer cannot represent shear. Match its Y-axis-driven rotation
    # and preserve reflection separately in the X scale.
    sign = 1 if determinant > 0 else -1
    right_axis = (d / y_length * sign, -b / y_length * sign)
    up_axis = (b / y_length, d / y_length)
    local_width = width * fit[0]
    world_width = local_width * x_length
    world_height = local_width * image.height / image.width * y_length
    vertices = []
    for u, v in ((-.5, 0), (-.5, 1), (.5, 1), (.5, 0)):
        vertices.extend((center[0] + right_axis[0] * world_width * u + up_axis[0] * world_height * v,
                         center[1] + right_axis[1] * world_width * u + up_axis[1] * world_height * v))
    return vertices, dict(slot=slot_name, attachment=head["name"], bone=head["bone"],
                          local_bounds=[left, bottom, right, top], fit=fit,
                          bottom_center=list(center), vertices=vertices,
                          determinant=determinant, bone_axes=[a, b, c, d])


def build() -> None:
    OUTPUT.mkdir(parents=True, exist_ok=True)
    REFERENCE.mkdir(parents=True, exist_ok=True)
    sources = extract_native()
    skin_sources = extract_skin_sets()
    armor_source = FUSION / "assets/eyes-armor"
    fits = json.loads((armor_source / "placement.json").read_text())
    for kind in ("cone", "bucket"):
        for stage in ("", "2", "3"):
            filename = kind + stage + ".png"
            shutil.copy2(armor_source / filename, OUTPUT / filename)
    shutil.copy2(armor_source / "placement.json", OUTPUT / "placement.json")
    (OUTPUT / "head-slots.json").write_text(json.dumps({kind: head_slot(kind) for kind in RECIPES}, indent=2) + "\n")
    (REFERENCE / "pose-recipes.json").write_text(json.dumps({
        kind: dict(skeleton=recipe[0], skins=recipe[1], animation=recipe[2], head=head_slot(kind))
        for kind, recipe in RECIPES.items()
    }, indent=2) + "\n")
    native_portraits = REFERENCE / "portraits"
    native_portraits.mkdir(exist_ok=True)
    proof = {}
    contact = Image.new("RGBA", (600, len(RECIPES) * 200), (225, 229, 219, 255))
    draw = ImageDraw.Draw(contact)
    for row, kind in enumerate(RECIPES):
        pose = decode(kind)
        render(pose["attachments"], SOURCE, 2).save(native_portraits / (kind + ".png"))
        proof[kind] = {}
        for column, armor in enumerate(("cone", "bucket")):
            filename = kind + "-" + armor + ".png"
            with Image.open(OUTPUT / (armor + ".png")) as helmet:
                vertices, geometry = helmet_geometry(pose, head_slot(kind), fits[armor], helmet)
            parts = copy.deepcopy(pose["attachments"])
            parts.append(dict(slot="ArmoredHelmet", bone=geometry["bone"], name=armor,
                              page=str(OUTPUT / (armor + ".png")), vertices=vertices,
                              triangles=[0, 1, 2, 2, 3, 0],
                              uvs=[0, 1, 0, 0, 1, 0, 1, 1], color=[1, 1, 1, 1]))
            portrait = render(parts, SOURCE, 2)
            portrait = portrait.crop(portrait.getbbox())
            portrait.thumbnail((512, 512), Image.Resampling.LANCZOS)
            portrait.save(OUTPUT / filename)
            if not portrait.getbbox() or portrait.getextrema()[3] != (0, 255):
                raise ValueError("Missing opaque/transparent portrait pixels: " + filename)
            geometry["sha256"] = digest(OUTPUT / filename)
            geometry["size"] = list(portrait.size)
            proof[kind][armor] = geometry
            preview = portrait.copy()
            preview.thumbnail((190, 172), Image.Resampling.LANCZOS)
            x, y = 190 + column * 205 + (190 - preview.width) // 2, row * 200 + 22
            contact.alpha_composite(preview, (x, y))
        draw.text((6, row * 200 + 80), kind, fill=(0, 0, 0, 255))
    contact.save(REFERENCE / "portrait-review.png")
    for page, top in enumerate(range(0, contact.height, 1200), 1):
        contact.crop((0, top, contact.width, min(top + 1200, contact.height))).save(
            REFERENCE / f"portrait-review-{page}.png")
    manifest = dict(game_started=False, source_bundle=str(BUNDLE), source_sha256=digest(BUNDLE),
                    native_skin_sources=skin_sources,
                    path_ids=sources,
                    native_inputs={name: digest(SOURCE / name) for name in sources},
                    shared_armor_inputs={path.name: digest(path) for path in OUTPUT.iterdir()
                                         if path.name.startswith(("cone", "bucket")) or path.name == "placement.json"},
                    portraits=proof)
    (REFERENCE / "export-proof.json").write_text(json.dumps(manifest, indent=2) + "\n")
    print(f"Exported {len(proof) * 2} armored native portraits, six helmet stages and matching head metadata.")


if __name__ == "__main__":
    build()
