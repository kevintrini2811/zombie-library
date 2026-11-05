# Statische Analyse der Almanachkorrektur 1.1.1

## Beobachtung

Im vorhandenen `Player-prev.log` stehen fehlende Modelle für `almanac.zombies.all.0.name`, `zombieType` und `thumbnailBackground`. Die vorhandene Sichtungsdatei enthält bereits fünf native Typen sowie einen Fusion-Eintrag. Die Erfassung von Sichtungen funktioniert damit grundsätzlich; ein Zurücksetzen dieser Datei ist weder erforderlich noch vorgesehen.

## Native Datenbindung

Das installierte Bundle `uipanels_assets_all_68c57e595d634e479472142eadcc5c3a.bundle` enthält zwei Kartenlisten mit `m_key.Path = almanac.zombies.visible`. Die Detailansichten verwenden `almanac.zombies.selected` und das Format `almanac.zombies.all.{0}`. Die vollständige Liste und die sichtbare Liste haben unterschiedliche Aufgaben.

`GameAssembly.dll`, RVA `0x70ca20` (`AlmanacEntriesModel.UpdateEntries`): erstellt die vollständigen Eintragsmodelle mit stabilen Quellindizes. Nur nicht gesperrte Einträge werden in `m_visibleEntriesModel` aufgenommen. RVA `0x70c470` (`UpdateEntriesLockedState`): leert und erneuert die sichtbare Liste anhand des Lock-Status; bei abweichenden Anzahlen zwischen Quelle und vorhandenen Eintragsmodellen wird ein Fehler protokolliert. Die frühere Quellfilterung verletzte diese Bindungs- und Indexvoraussetzungen und ließ bei null Sichtungen `all.0` vollständig fehlen.

## Korrektur und Invarianten

- Quelllisten werden nicht mehr gefiltert oder umnummeriert. Native Eintragsmodelle bleiben erhalten.
- `UserService.IsLocked(AlmanacEntryData)` entscheidet nur bei Zombie-Einträgen anhand profilbezogener Sichtungen. Zombotany bleibt dem eigenen Tab vorbehalten.
- Der native Lock-Refresh übernimmt Aufbau und Aktualisierung von `visible`, einschließlich Zählern und nativen Auswahlaktionen.
- Neue Sichtungen erhöhen eine Sitzungsrevision, auch wenn das Speichern ausfällt. Der gebundene AppDataProvider aktualisiert seine Zombie-Liste bei geänderter Revision oder Profil-GUID. Beim Unbind wird die Referenz freigegeben.
- Mod- und Entenfreigaben, Sichtungskriterien, native Spielstände und bestehende Sichtungsdateien bleiben erhalten.

## Prüfung und Grenzen

Release-Build mit installierten IL2CPP-Referenzen: erfolgreich, keine Warnungen oder Fehler. Keine Testprogramme und kein Spielstart. Die UI-Darstellung, Controller-Navigation, Profilwechsel und Erstbegegnungen bleiben manuell zu prüfen. Die separaten Meldungen `Releasing render texture that is set as Camera.targetTexture!` beim Beenden sind kein Nachweis für die fehlenden Almanachmodelle; ihre Behebung ist mit dieser Änderung nicht bestätigt.
