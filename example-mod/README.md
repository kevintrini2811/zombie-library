# Zombie Library Example

Optionale MelonLoader-Mod mit Abhängigkeit auf Zombie Library. Sie registriert einen „Zähen Zombie“ mit 540 Lebenspunkten, leicht vergrößerter Darstellung, nativem Sound und automatischer Gummientenvariante. Im Zombie-Almanach erscheint ein eigener Tab „Zombie Example“.

Die Mod ersetzt durchschnittlich einen von zwanzig geeigneten natürlichen normalen Spawns. Explizite Library-Beschwörungen, bereits markierte Zombies, gepanzerte Zombies und Zen-Garten werden ausgeschlossen. Der gespeicherte Marker ist `Normal.mGraveX = -763001`. Der zusätzliche Zustand wird ohne erneute Initialisierung aus nativen Spielständen erkannt.

`ExampleMod.cs` zeigt Registrierung, Initialisierung, Porträtbesitz, Soundzuordnung und sichere Zufallsersetzung. Für eigene Mods IDs und Marker ändern; nach Veröffentlichung müssen sie stabil bleiben.

Bauen:

```sh
rtk proxy /home/kevin/modding/pvzreplanted/.tools/dotnet/dotnet build /home/kevin/modding/pvzreplanted/zombie-library/example-mod/ZombieLibraryExample.csproj -c Release
```

Optional installieren:

```sh
rtk proxy python3 /home/kevin/modding/pvzreplanted/zombie-library/tools/install.py --example
```

Standardinstallation enthält diese Mod nicht. Spiel und Tests wurden nicht ausgeführt.
