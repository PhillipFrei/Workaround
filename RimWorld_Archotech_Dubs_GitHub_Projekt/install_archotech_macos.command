#!/bin/bash
# macOS: install corrected XMLs into Archotech Race workshop folder.
set -euo pipefail
HERE="$(cd "$(dirname "$0")" && pwd)"
SOURCE="$HERE/ArchotechRace/Patches"
TARGET="$HOME/Library/Application Support/Steam/steamapps/workshop/content/294100/2500371387/Mods/VE_Framework/Patches"
BACKUP="$HOME/Documents/RimWorld_Archotech_Backups/$(date +%Y%m%d-%H%M%S)"
if [[ ! -d "$TARGET" ]]; then
  echo "Fehler: Archotech-Race-Ordner nicht gefunden: $TARGET"
  echo "Falls du eine zweite Steam-Bibliothek nutzt, passe TARGET in diesem Skript an."
  exit 1
fi
mkdir -p "$BACKUP"
changed=0
for name in Archotech_PowerArmor.xml Beard.xml Hair.xml Tattoo.xml; do
  dst="$TARGET/$name"
  src="$SOURCE/$name"
  if [[ ! -f "$dst" || ! -f "$src" ]]; then
    echo "Fehlende Datei: $name"; exit 2
  fi
  if grep -q 'VFECore.PatchOperationToggableSequence' "$dst"; then
    cp -p "$dst" "$BACKUP/$name"
    cp "$src" "$dst"
    echo "Korrigiert: $name"
    changed=$((changed+1))
  elif grep -q 'VEF.PatchOperationToggableSequence' "$dst"; then
    echo "Bereits korrigiert: $name"
  else
    echo "WARNUNG: Unbekannte Dateiversion - nicht ueberschrieben: $name"
  fi
done
echo "$changed Datei(en) korrigiert. Sicherungen (soweit erforderlich): $BACKUP"
echo "Steam-Updates koennen den Fix ueberschreiben. RimWorld neu starten und Player.log pruefen."
