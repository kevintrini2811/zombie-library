"""Install the matching library/Fusion bundle; never start the game or tests."""
import argparse
import fcntl
import importlib.util
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
spec = importlib.util.spec_from_file_location("library_installer", ROOT.parent / "plant-library/tools/install.py")
installer = importlib.util.module_from_spec(spec)
spec.loader.exec_module(installer)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--game-dir", type=Path, default=installer.GAME)
    parser.add_argument("--example", action="store_true", help="Also install the optional zombie example")
    parser.add_argument("--no-build", action="store_true", help="Install existing Release artifacts")
    args = parser.parse_args()
    with (args.game_dir / "Mods/.FusionMod.release.lock").open("a") as lock:
        fcntl.flock(lock, fcntl.LOCK_EX | fcntl.LOCK_NB)
        installer.install(args.game_dir, False, not args.no_build, zombie_example=args.example, backup_root=ROOT)
