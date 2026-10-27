# RimWorld 1.6 – Archotech Race + Dubs Lite kompatibilitaetsfixes

Stand: 27.10.2026. Grundlage sind die vom Nutzer hochgeladenen Originaldateien sowie der Crash-Stacktrace.

## Was ist enthalten?

1. `ArchotechRace/Patches/*.xml`: genau vier XML-Dateien mit **einem** ersetzten Klassennamen je Datei:
   `VFECore.PatchOperationToggableSequence` -> `VEF.PatchOperationToggableSequence`.
   Keine weiteren Tags veraendert. **Kein Compiler erforderlich.**
2. `install_archotech_macos.command`: wendet die vier korrigierten Dateien auf dem Mac an;
   erstellt Backups in `~/Documents/RimWorld_Archotech_Backups/` und ueberschreibt
   nur Dateien mit dem veralteten Klassennamen.
3. `DubsLiteAsyncSewageFix`: experimentelle eigenstaendige Harmony-Mod.
   Patchziel per Reflection: `DubsBadHygiene.XSectionLayer_Sewage.Regenerate()`.
   Ein Prefix verhindert nur die Ausfuehrung auf Threads ausser dem beim Modstart
   erfassten Hauptthread. **Dies verschiebt die Arbeit NICHT automatisch spaeter
   auf den Hauptthread.** Dadurch kann die Abwasser-Grafik nach einem solchen
   Import fehlen. Die eigentliche Ursache liegt moeglicherweise in der asynchronen
   Map-Generierung von Tunnel and Underground bzw. dem Aufrufpfad.
4. `.github/workflows/build.yml`: GitHub Actions kompilieren den Dubs-Fix und
   stellen die installierbare Mod unter **Actions -> Build -> Artifacts** bereit.

## Archotech Race unter macOS

Entpacken und im Terminal im entpackten Projektordner:

```bash
bash ./install_archotech_macos.command
```

Das Script bearbeitet Dateien unter:
`~/Library/Application Support/Steam/steamapps/workshop/content/294100/2500371387/Mods/VE_Framework/Patches/`

Die vier Dateien werden nicht als eigenstaendige Mod geladen, sondern muessen
in den originalen Workshop-Pfad kopiert werden. Workshop-Updates koennen das
ueberschreiben.

Dieser Fix betrifft ausschliesslich `PatchOperationToggableSequence`;
`VFECore.Shields.HediffCompProperties_Shield` ist ein **anderer** Fehler.

## Dubs Lite – GitHub Actions

- Neues GitHub-Repository anlegen und **den Inhalt** dieses Ordners in die Repo-Wurzel pushen.
- In GitHub `Actions` den Workflow `Build RimWorld 1.6 Dubs Lite workaround` auswaehlen.
- Nach einem erfolgreichen Workflow-Lauf das Artifact `DubsLiteAsyncSewageFix-RW1.6` herunterladen.
- Das Artifact entpacken, so dass die Modstruktur `About/About.xml` und
  `1.6/Assemblies/DubsLiteAsyncSewageFix.dll` erhalten bleibt.
- Den kompletten Modordner unter
  `~/Library/Application Support/Steam/steamapps/common/RimWorld/RimWorldMac.app/Mods/DubsLiteAsyncSewageFix/`
  ablegen. Mod nach **Dubs Bad Hygiene Lite** aktivieren.

Fuer die GitHub-Kompilierung werden NuGet-Pakete `Krafs.Rimworld.Ref 1.6.4871`,
`Lib.Harmony 2.4.1`, und die .NET-Framework-Referenzassemblies geladen.
Die originalen Spiel- und Mod-DLLs werden **nicht** ins GitHub-Repo kopiert.
Diese Buildumgebung wurde hier nicht ausgefuehrt; ein erfolgreicher Cloud-Build
und ein funktionierender Spielstart muessen noch geprueft werden.

## Sicherheit / Test

- Vorab **Backups** der Mods und Spielstaende erstellen.
- Testen zuerst in einer **kopierten** Kolonie bzw. frischem Testsave.
- RimWorld komplett beenden und neu starten.
- Im `Player.log` pruefen, ob der Dubs-Patch `Active: sewage layer mesh regeneration ...` meldet.
- Dann dieselbe Untergrund-Kartengenerierung wie vor dem Crash ausloesen.
- Wenn sie weiter abstuerzt, das **neue Player.log** auf `Graphics device is null`
  und den vollstaendigen Stacktrace untersuchen.
- Falls die Grafikebene nach Import unvollstaendig ist, Patch deaktivieren.

## Projektstruktur

```
.github/workflows/build.yml
ArchotechRace/Patches/{Archotech_PowerArmor,Beard,Hair,Tattoo}.xml
DubsLiteAsyncSewageFix/About/About.xml
DubsLiteAsyncSewageFix/Source/DubsLiteAsyncSewageFixMod.cs
DubsLiteAsyncSewageFix/DubsLiteAsyncSewageFix.csproj
install_archotech_macos.command
```

*Weder ein vollstaendig reparierter Untergrundgenerator noch die Loesung aller
anderen Modfehler werden durch diesen Workaround garantiert.*
