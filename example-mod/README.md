# Zombie Library Example

Optional MelonLoader mod with dependency on Zombie Library. It registers a “Tough Zombie” with 540 life points, slightly enlarged display, native sound and automatic rubber duck variant. A separate “Zombie Example” tab appears in the Zombie Almanac.

The mod replaces on average one out of twenty suitable natural normal spawns. Explicit library summons, already marked zombies, armored zombies and Zen Garden are excluded. The saved marker is `Normal.mGraveX = -763001`. The additional state is recognized from native saves without reinitialization.

`ExampleMod.cs` shows registration, initialization, portrait ownership, sound mapping and safe random replacement. Change IDs and markers for your own mods; they must remain stable after publication.

Build:

```sh
rtk proxy /home/kevin/modding/pvzreplanted/.tools/dotnet/dotnet build /home/kevin/modding/pvzreplanted/zombie-library/example-mod/ZombieLibraryExample.csproj -c Release
```

Optionally install:

```sh
rtk proxy python3 /home/kevin/modding/pvzreplanted/zombie-library/tools/install.py --example
```

Standard installation does not include this mod. Game and tests have not been run.
