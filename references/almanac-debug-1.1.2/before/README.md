# Zombie Library 1.1.1

MelonLoader-Bibliothek für PvZ Replanted. Die Bibliothek registriert eigene Zombies, Sounds und Gummientenvarianten. Jede registrierte Mod erhält im Zombie-Almanach einen eigenen Tab. BloomEngine stellt den Menüeintrag bereit: nativer Zombie als Icon, abgedunkelter nativer Rasen im Icon als Hintergrund.

## Almanachkorrektur (1.1.1)

Die vorherige Filterung entfernte Einträge aus `almanac.zombies.all`. Die nativen Detailbinder erwarten dort weiterhin gültige, feste Indizes, insbesondere `all.0`. `Player-prev.log` meldet entsprechend `Failed to find model [name almanac.zombies.all.0.name]` sowie fehlende Beschreibungs-, Zombie- und Hintergrundmodelle. Bei einer leeren Sichtungsliste existierte das benötigte Modell überhaupt nicht.

Version 1.1.1 behält alle nativen Eintragsmodelle und deren Indizes bei. Die Sichtungsregel steuert stattdessen den nativen Lock-Status: Das Spiel erstellt daraus seine Kartenliste `visible`. Ungesehene Zombies und die sechs separat dargestellten Zombotany-Typen bleiben dort ausgeschlossen. Pflanzen werden nicht verändert. Neue Sichtungen aktualisieren die Kartenliste in `LateUpdate`; ein Levelabschluss ist dafür nicht erforderlich. Profilwechsel und erneutes Binden des Datenproviders lösen ebenfalls eine Aktualisierung aus. Bestehende Sichtungen bleiben erhalten; native Spielstände werden nicht geändert.

Statische Belege: `references/almanac-1.1.1.md`. Kompilierung gegen die installierten Spielreferenzen erfolgreich. Spiel und Testprogramme wurden nicht gestartet. Manuell zu prüfen: leeres Profil ohne rote Fehlertexte, vorhandene Sichtungen nach Neustart, erste Sichtung ohne Levelabschluss, Levelabschluss, Profilwechsel sowie Zombotany- und Fusion-Tabs.

## Zombotany und Sichtungen (1.1.0)

Zusätzlich zu den Mod-Tabs gibt es den Tab **Zombotany** mit Erbsenkanonen-, Wallnuss-, Jalapeno-, Gatling-Erbsen-, Kürbis- und Riesenwallnuss-Zombie. Diese sechs nativen Typen stehen ausschließlich in diesem Tab. Ihre native Erzeugung, KI und gespeicherte Identität bleiben unverändert. Die Porträts stammen aus den bereits vorhandenen nativen Pose-Exporten; `assets/zombotany-sources.json` dokumentiert Herkunft und Hashes.

Alle Zombie-Einträge erscheinen erst nach der ersten Sichtung: native Zombies, Zombotany und Mod-Zombies. Eine Sichtung erfordert einen lebenden, sichtbaren Zombie auf dem aktiven Spielfeld während des Spiels. Levelvorschau, Almanachvorschau, pausierte Boards und vollständig außerhalb des Spielfelds stehende Zombies zählen nicht. Land- und zusätzliche Entenvarianten werden getrennt erfasst. Ein Mod-Zombie schaltet den zugrunde liegenden nativen Zombie nicht frei.

Die Bibliothek speichert Sichtungen pro Profil-GUID unter `UserData/ZombieLibrary/sightings.json`. Native Spielstände, Erfolge und Freischaltungen werden nicht verändert. Frühere Sichtungen können nicht zuverlässig rekonstruiert werden: Auch vorhandene Profile beginnen mit einer leeren Sichtungsliste. Bereits besiegte Zombies werden nicht pauschal freigeschaltet. Bei Lesefehlern bleibt die vorhandene Datei erhalten; bei Schreibfehlern bleibt neuer Fortschritt für die Sitzung verfügbar und der Fehler wird protokolliert.

Die vollständige native Eintragsliste bleibt erhalten, damit Detailbinder und Auswahlindizes gültig bleiben. Die native Liste `visible` enthält ausschließlich gesichtete Zombies außerhalb von Zombotany. Neue Sichtungen und Profilwechsel aktualisieren die Sichtbarkeit über den nativen Lock-Refresh, ohne die Eintragsmodelle zu ersetzen. Gesehene native Zombies erhalten für die Almanachanzeige Zugriff unabhängig vom bisherigen Level-Lock. Leere Mod-Tabs zeigen „Noch keine Zombies gesichtet.“; Controller-Ziele bleiben innerhalb der sichtbaren Seite.

Kompilierung und Installation erfolgen ohne Spielstart und ohne Testprogramme. Manuell zu prüfen: Erstbegegnung aller sechs Typen, unsichtbare Vorschauen, native Einträge vor/nach Sichtung, getrennte Entenfreigabe, Profilwechsel, Neustart, Sprache und Controller.

## Menü-Icon und Startfehler (1.0.4)

Die Implementierung folgt jetzt der aktuellen Plant Library: Zombie und dunkler nativer Rasen sind in einem einzigen deckenden, quadratischen `menu-icon.png` zusammengefügt. BloomEngine übernimmt ausschließlich Platzierung und Rahmen. Es gibt keine zusätzliche Hintergrundfläche hinter dem Beschreibungstext und keinen periodischen Menü-Layout-Code.

Version 1.0.3 rief `RectTransformUtility.CalculateRelativeRectTransformBounds` auf. Dieser Helfer ist im installierten IL2CPP-Spiel nicht nutzbar; `Latest.log` meldet `System.NotSupportedException: Method unstripping failed`. Der Aufruf und der gesamte Menü-Updatepfad wurden entfernt. Die wiederholten Fehler werden nicht abgefangen oder verdeckt, sondern ihre aufrufende Implementierung entfällt. Ein erfolgreicher Build allein prüft die Verfügbarkeit solcher entstrippten Methoden nicht.

`tools/export_assets.py` erstellt das Icon reproduzierbar aus den vorhandenen nativen Zombie- und Rasenexporten. Die Helligkeit des Rasens beträgt 35 %, die Icongröße 512 × 512 Pixel. `assets/sources.json` dokumentiert Quellen, Komposition und Hashes. Spiel und Tests wurden nicht gestartet; die Darstellung beim nächsten Spielstart bleibt manuell zu prüfen.

## Frühere Menü-Implementierung (1.0.3, ersetzt durch 1.0.4)

Im Screenshot `Bildschirmfoto_20261103_074616.png` endet der Rasen mitten im transparenten Zombie-Icon. BloomEngine positioniert den Icon-Container links außerhalb der eigentlichen Eintragsfläche. Der Hintergrund schließt jetzt die tatsächlichen Icon-Grenzen einschließlich Rahmen ein. Die Grenzen werden nach Layout- und Auflösungsänderungen erneut berechnet. Der dekorative Hintergrund nimmt nicht am nativen Layout teil. Das native Zombie-Sprite und der native Menürahmen bleiben erhalten. Spiel und Tests wurden nicht gestartet.

## Auswahlhelligkeit (1.0.2)

Die Screenshots `Bildschirmfoto_20261103_071611.png` und `Bildschirmfoto_20261103_071627.png` zeigen die native Aufhellung und die zuvor weiße Mod-Karte. Die Auswahl verwendet jetzt den unveränderten nativen `ColorBlock` des Zombie-Buttons direkt auf dem Kartenbild. Die deckende weiße `OverState`-Grafik bleibt deaktiviert. Getrennte Mod-Porträts und Entenbilder folgen derselben Farbübergangsanimation. Spiel und Tests wurden nicht gestartet.

## Native Kartenansicht (1.0.1)

Die Screenshots vom 3. November 2026 zeigen den Unterschied zwischen nativen Zombie-Karten und den bisherigen grauen Mod-Karten. Die Bibliothek verwendet exportierte native Kartenhintergründe und Rahmen. Native Button-Farben markieren Hover und Auswahl. Zusätzliche grüne Kartennamen entfallen; Namen stehen wie im Original in der Detailansicht. Die native Rasterausrichtung bleibt erhalten. Die Vorlagen benötigen ihre entfernten Bildbinder nicht mehr, um gültige Kartengrafiken zu erhalten.

`ZombieDefinition.Thumbnail(german, duck)` kann ein vollständiges Kartenbild einschließlich Rahmen liefern. Ohne Callback ergänzt die Bibliothek den nativen Rahmen um `Portrait`. Fusion liefert eigene Nahansichten für beide Spezialzombies und originale Almanachbilder für native Zombies. Entenvarianten enthalten die native Ente im Vorschaubild. Alle Sprites bleiben im Besitz der jeweiligen Mod.

`tools/export_almanac_assets.py` exportiert die nativen Kartenebenen und baut die Fusion-Vorschaubilder aus vorhandenen Originalgrafiken. Herkunft und Hashes stehen in `assets/almanac-sources.json`. Der Export startet weder Spiel noch Tests.

## Registrierung

```csharp
using ZombieLibrary;
using Il2CppReloaded.Gameplay;
using Il2CppReloaded.Services;

var mod = new ModCatalog("my-mod", "Meine Zombies");
Zombies.RegisterCatalog(mod);
Zombies.RegisterSound("my-mod:spawn", FoleyType.NewspaperRarrgh);
var zombie = Zombies.RegisterZombie(mod, "tough",
    new ZombieIdentity(ZombieType.Normal, MarkerField.GraveX, -763100),
    new EntryText("Tough Zombie", "Twice the normal health."),
    new EntryText("Zäher Zombie", "Doppelte normale Lebenspunkte."),
    rubberDuck: true);
zombie.Initialize = actor => actor.mBodyHealth = actor.mBodyMaxHealth = 540;
zombie.SpawnSound = "my-mod:spawn";
zombie.Portrait = german => myPortraitSprite;
// In einem vorhandenen Board:
var actor = Zombies.Spawn(zombie, board, row);
```

Abhängigkeit in der eigenen Mod angeben:

```csharp
[assembly: MelonAdditionalDependencies("ZombieLibrary")]
```

`RegisterCatalog` muss vor `RegisterZombie` aufgerufen werden. IDs und Identitäten müssen eindeutig sein. Nach Veröffentlichung dürfen gespeicherte Marker nicht geändert werden. Die Bibliothek lehnt synthetische Zombie-Enums ab. Unterstützt sind negative Marker in `Normal.mGraveX` und `PeaHead.mSummonCounter`; diese Felder sind bei der jeweiligen nativen Lauf-KI unbenutzt. Weitere native KI-Typen benötigen zuerst einen Nachweis geeigneter gespeicherter Felder.

`Apply(definition, actor)` wandelt einen frisch erzeugten, lebenden nativen Zombie passenden Typs um. Das Markerfeld muss frei sein. Andere registrierte Varianten werden nicht überschrieben. Wiederholtes Anwenden derselben Definition initialisiert und vertont den Zombie nicht erneut. `Spawn` kapselt die native Erzeugung, prüft die Zeilengrenzen und entfernt den neuen Zombie, wenn seine nachgelagerte Initialisierung fehlschlägt. Bewusst erzwungene Beschwörungen behalten die native Factory-Semantik; natürliche Spawn-Auswahl und Levelregeln bleiben Aufgabe der erzeugenden Mod.

Natürliche Zufallsersetzungen prüfen `Zombies.IsExplicitSpawn`, damit eine explizite Beschwörung nicht eine andere Variante erhält. Sie sollten nur geeignete, unmarkierte normale Zombies ersetzen. Die Bibliothek fügt selbst keine Zombies zu Wellen hinzu. Das vollständige Beispiel zeigt diese Prüfung.

`Find(actor)` erkennt Varianten unmittelbar anhand nativer gespeicherter Felder. Nach Laden läuft `Tick`, jedoch weder `Initialize` noch `SpawnSound`. Bei Wiederverwendung durch den nativen Objektpool entfernt die Bibliothek den bekannten Marker. `Tick` folgt `Zombie.Update` und pausiert mit dem Board; die Mod prüft darin ihre eigenen Regeln für Tod, Spielmodus und Sonderphasen. `RefreshVisuals` läuft einmal pro Bild vor der Entendarstellung und darf pausierte Animationen nicht selbst fortschreiben. Eigene laufende Zustände müssen in nachweislich geeigneten gespeicherten Feldern liegen.

## Sounds und Gummienten

`RegisterSound(id, FoleyType)` verwendet das native Soundsystem mit dessen Lautstärke- und Stummschaltung. `RegisterSound(id, Action<Zombie>)` erlaubt eigene Soundquellen. `PlaySound(id, actor)` spielt registrierte Sounds bei einem selbst gewählten Ereignis. Ressourcen und Laufzeit der eigenen Audioquellen gehören der registrierenden Mod. Unbekannte und doppelte Sound-IDs werden abgelehnt.

`rubberDuck: true` aktiviert Poolunterstützung und erzeugt automatisch einen zusätzlichen Almanacheintrag. Die Variante bleibt derselbe gespeicherte Zombie; native Poolzustände entscheiden zwischen Land und Wasser. Bewegung, KI, Kollisionen und Fähigkeiten bleiben erhalten. Die Bibliothek verwendet die originale Gummiente, native Spine-Posen, Wasserlinien-Clipping und passende Zeichenreihenfolgen. Tanzende Zombies behalten ihre Aufstehphase und Nachbarzeilenprüfung.

`RegisterDuckVariant(mod, id, nativeType, english, german)` registriert die zusätzliche Poolvariante eines vorhandenen nativen Zombies. Unterstützt sind Football, JackInTheBox, Dancer, BackupDancer, Yeti, Gargantuar, RedeyeGargantuar, SquashHead und PeaHead. Solche Einträge verwenden die native Erzeugung, keine eigenen Marker. `RegisterNativeDuck(type)` aktiviert dieselbe Unterstützung ohne zusätzlichen Almanacheintrag. Der gewählte native Typ erhält damit grundsätzlich Poolunterstützung.

Eigene Rigs können `PlacePoolDuck(actor, renderer)` setzen. Der Callback platziert die von der Bibliothek verwaltete Ente und gibt erst bei fertiger Pose `true` zurück. `PoolDuckVisuals.Waterline` und `PoolSpriteClip` erlauben denselben Wasserlinien-Schnitt für importierte Körpergrafiken. Fusion nutzt dies für den Augenzombie.

## Almanach und Menü

Die Bibliothek klont native Zombie-Karten, Zombie-Detailansicht, Zombie-Hintergründe, Buttons und Schriftarten unter einem inaktiven Elternobjekt. Native Datenbinder und Übergangsanimationen werden vor Aktivierung entfernt. Fünf Spalten, Seitennavigation, Scrollen, Controller-Navigation, Sprachwechsel und Wechsel zwischen Mod-Tabs werden unterstützt. Native Index- und Schließen-Aktionen bleiben erhalten. Profilfreischaltungen werden nicht geändert.

`Portrait` und optional `DuckPortrait` liefern geliehene Sprites; die Mod besitzt deren Lebensdauer. Ohne eigenes `DuckPortrait` ergänzt die Bibliothek die native Ente im normalen Porträt. Bei asynchron geladenen Sprites darf der Callback zunächst `null` liefern. Die Ansicht fragt das Porträt erneut ab.

Fusion 19.1.0 verwendet die Library für Hypnoerbsen-Zombie und Augenzombie sowie deren Gummientenvarianten. Acht zusätzliche native Entenvarianten stehen ebenfalls in Fusions Zombie-Tab. Die bisherigen Marker `-430153` und `-521012` bleiben erhalten. Spezialfähigkeiten, Zufallsersetzung, Treffer und Beute bleiben in Fusion; Registrierung, Sounds, Enten und Almanach gehören der Library.

## Bauen und installieren

```sh
rtk proxy /home/kevin/modding/pvzreplanted/.tools/dotnet/dotnet build /home/kevin/modding/pvzreplanted/fusion-mod/FusionMod.csproj -c Release
rtk proxy /home/kevin/modding/pvzreplanted/.tools/dotnet/dotnet build /home/kevin/modding/pvzreplanted/zombie-library/example-mod/ZombieLibraryExample.csproj -c Release
rtk proxy python3 /home/kevin/modding/pvzreplanted/zombie-library/tools/install.py
```

Der Installer baut und installiert `ZombieLibrary.dll`, `PlantLibrary.dll` und `FusionMod.dll` gemeinsam. Er verwendet dieselbe Releasesperre wie Fusion, sichert alte DLLs, erkennt zwischenzeitliche Änderungen und ersetzt Dateien atomar. Bei einem Fehler während der DLL-Installation stellt er bereits ersetzte DLLs wieder her. `backups/<Zeitstempel>/installation.json` dokumentiert alte und neue SHA-256-Hashes. Spiel und Testprogramme werden nicht gestartet.

Das [Beispiel](example-mod/README.md) bleibt standardmäßig außerhalb des Spielordners. `--example` installiert es zusätzlich. `--no-build` installiert bereits gebaute Release-Dateien.

Zum Rückbau das Spiel schließen und alle drei DLLs gemeinsam aus dem dokumentierten Backup wiederherstellen. Eine Datei mit vorherigem Hash `null` war zuvor nicht installiert. Diese Datei beim Rückbau entfernen. Die Bibliothek darf nicht entfernt werden, solange eine abhängige Mod installiert ist.

## Offene manuelle Prüfung

Builds prüfen Kompilierung gegen installierte IL2CPP-Referenzen. Spiel und Tests wurden auf Benutzerwunsch nicht ausgeführt. Manuell zu prüfen: Mod-Menü, Mod-Tabs, native Zombie-Assets, alle Enten auf Land und im Pool, Controller-Navigation, Sprachwechsel, Pause, Hypno-Beschwörung, Augenzombie-Fähigkeiten, Soundhäufigkeit sowie Speichern und Fortsetzen bestehender Varianten.

Vorbereitete Offline-Suite, ausschließlich manuell ausführen:

```sh
rtk proxy /home/kevin/modding/pvzreplanted/.tools/dotnet/dotnet run --project /home/kevin/modding/pvzreplanted/zombie-library/tests/RegistryTests.csproj -c Release
```

Die Suite prüft echte Registrierungslogik mit Daten-Doubles ohne Spielassemblies. Sie deckt Identitätskonflikte, Initialisierung, Sounds, Speichermarker, Pause, Reset, fehlerhafte Spawns und native Entenregistrierung ab. Fusions vorhandene Hypno-, Augenzombie- und Pool-Suites sind an die neue Abhängigkeitsgrenze angepasst.

Grafikherkunft: `assets/sources.json`; native Zombie-Layoutdaten: `assets/native-almanac-layout.json`. `tools/export_assets.py` übernimmt bereits vorhandene native Exporte ohne Spielstart. Gemeinsame native UI-Sanitisierung und Sprachabfrage liegen als eingebundene Quellen unter `../shared/`; zwischen Zombie Library und Plant Library besteht keine Laufzeitabhängigkeit.
