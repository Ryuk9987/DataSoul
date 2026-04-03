# DataSoul — Welt, Königreiche & Fraktionen (Subagent Context)
→ Für Übersicht: `ctx_overview.md` | Für Story: `ctx_story.md`

---

## Die Welt: DataWorld

- **Vollständig digitale Simulation** — Städte, Wälder, Dungeons, Ozeane
- Bewohner wissen **nicht**, dass sie in einer Simulation leben
- Ästhetik: Klassische Fantasy-Optik mit subtilen digitalen Elementen (Glitch-Effekte, hexagonale Datenmuster, Fehleranimationen)
- **Korrupte Zonen:** Gebiete mit starkem "Data-Corruption" — visuelle Störungen, gefährlichere Gegner

### Weltstruktur = IT-Netzwerk
- DataWorld = **verteiltes Netzwerk aus 7 Königreichen** (= 7 Subnetze)
- Zwischen Königreichen: **Firewall-Barrieren** — scannen Reisende (Credentials prüfen)
- Data-Nodes = **Router/Switch-Knoten** — sichtbar als alte Monolithen
- Korrupte Zonen = **infizierte Netzwerksegmente** (Malware)
- Magie = **System Calls** — privilegierte Prozesse auf dem Weltbetriebssystem
- Starke Magier = User mit erhöhten Rechten (`sudo`-Zugang)

---

## ASCII-Weltkarte
```
                        [ GLACIUS ❄️ ]
                        Vaelthorn
                            |
              [FERRUM ⚙️]---+---[SYLVARA 🌿]
              Korzat        |    Verdenmoor
                \           |           \
                 \    [VALDRIS 🏰]        \
                  \   Aldenmere ★         \
                   \  (Spielstart)         \
                    \       |               \
              [PYRAXIS🔴]   |          [AQUALIS 🌊]
              Ashport       |           Tidemoor
                            |
                      . . . . . . .
                     .  NULLHEIM ⬛ .
                     .  (versteckt)  .
                      . . . . . . .
```

---

## Die 7 Königreiche

### 🏰 VALDRIS — *"Das Ewige Königreich"*
**IT-Analogie:** Primary Domain Controller
**Biom:** Klassische Hochfantasy — grüne Hügel, Burgen, Wälder
**Hauptstadt:** Aldenmere (★ Spielstart)
**Besonderheit:** Hütet die Beschwörungsrituale. Einziges Königreich mit (vermeintlich) direktem Zugang zur Certificate Authority.
**Datenmuster:** Hexagonale goldene Runen an Gebäuden — "Die heilige Schrift"
**Innenpolitik:** Rat der Datenwächter — 7 Familien, streng hierarchisch.

---

### ⚙️ FERRUM — *"Die Schmiede des Netzwerks"*
**IT-Analogie:** Hardware Layer / Manufacturing
**Biom:** Rauchige Berglandschaft, Schmiedewerke, Lavastrom-Kanäle
**Hauptstadt:** Korzat
**Besonderheit:** Produziert alle physischen "Datenträger" der Welt — magische Kristalle die Information speichern.
**Innenpolitik:** Technokratie — wer am meisten produziert hat Macht. Kein Adel, nur Gildenmacht.

---

### 🌊 AQUALIS — *"Das Fließende Protokoll"*
**IT-Analogie:** Network Transport Layer (TCP/IP)
**Biom:** Ozean, Lagunen, schwimmende Städte auf Plattformen
**Hauptstadt:** Tidemoor
**Besonderheit:** Kontrolliert den Datenfluss zwischen allen Königreichen. Kann Verbindungen "drosseln" oder "kappen".
**Innenpolitik:** Merchant Lords — Handelsdynastien. Neutral nach außen, korrupt nach innen.

---

### 🌿 SYLVARA — *"Der Organische Cache"*
**IT-Analogie:** Biologisches Computing / Cache-Speicher
**Biom:** Leuchtender Wald, biolumineszente Pflanzen, lebende Architektur
**Hauptstadt:** Verdenmoor
**Besonderheit:** Der Wald speichert Erinnerungen — Echos vergangener Ereignisse, Geister/Cached Processes. Kael wurde zuletzt hier gesehen.
**Innenpolitik:** Rat der Ältesten — langlebige hochrangige KI-Instanzen. Wissen viel. Sagen wenig.

---

### ❄️ GLACIUS — *"Der Frozen State"*
**IT-Analogie:** Cold Storage / Archiv / Read-Only Memory
**Biom:** Tundra, Eisburgen, Aurora-Himmel mit sichtbaren Datenströmen
**Hauptstadt:** Vaelthorn
**Besonderheit:** Verändert sich nicht. Gesetze aus der Gründungszeit gelten noch. Bewohner altern extrem langsam.
**Innenpolitik:** Theokratie — ein "Ewiger Herrscher" auf dem Thron. Seit 500 Jahren derselbe. Kein natürlicher Mensch.

---

### 🔴 PYRAXIS — *"Das Overflow-Königreich"*
**IT-Analogie:** Stack Overflow / Instabile Prozesse / Red Zone
**Biom:** Vulkanische Wüste, schwebende Ruinen, permanente Blitzstürme, sichtbare Datenfragmente in der Luft
**Hauptstadt:** Ashport (halb zerstört)
**Besonderheit:** War normal — bis ein Overflow-Event vor 50 Jahren alles destabilisierte. Verbliebene Bewohner halb-korrupt aber bewusst. Stärkste Corrupted Data Fragments hier.
**Innenpolitik:** Anarchie. The Null Sect hat starke Präsenz.

---

### ⬛ NULLHEIM — *"The Void Subnet"*
**IT-Analogie:** /dev/null — gelöschte Daten, ungenutzte Adressräume
**Biom:** Absolute Dunkelheit. Schwache geometrische Lichtmuster. Keine normalen Physikregeln.
**Besonderheit:** Das "gelöschte" Königreich — aus offizieller Topologie entfernt. The Null Sect weiß wie man hinkommt. Kael lebt hier. Ghost Processes sammeln sich hier.
**Innenpolitik:** Kael ist einziger dauerhafter Bewohner.

---

## The Certificate Authority — *"The Root"*

- Physisch verankert als uralter Monolith im tiefsten Kellergewölbe der Akademie in Aldenmere
- Valdris glaubt sie *besitzen* The Root — in Wirklichkeit **verwaltet The Root sie**
- The Root ist kein Charakter — es ist ein **autonomes System** das der Architekt erschaffen hat und sich selbst weiterentwickelt hat
- Alle 7 Königreiche vertrauen The Root blind — ihre gesamte Machtstruktur basiert darauf
- **Kaels Ziel:** The Root physisch zerstören

---

## Fraktionen

### The Firewall Brotherhood
*"Wir sind die Grenze zwischen Ordnung und Chaos."*
**IT-Analogie:** Intrusion Detection System / Firewall
**HQ:** Checkpoint-Festungen entlang aller Firewall-Grenzen
**Ränge:** Packet → Filter → Guardian → Sentinel → Arch-Wall
**Ideologie:** Ordnung durch Kontrolle. Freier Datenfluss = Gefahr. Jede unbekannte Signatur = Risiko.
**Story-Arc:** Früh: hilfreich → Mitte: verdächtigen Protagonisten (fragmentierte Signatur) → Spät: aktive Bedrohung (von The Root als unwissende Vollstrecker benutzt)
**Companion-Potential:** Ein desillusionierter Sentinel.

---

### The Packet Runners
*"Information will frei sein. Alles andere ist Zensur."*
**IT-Analogie:** Protocol-Exploiter / Darknet / Whistleblower-Netzwerk
**HQ:** Dezentralisiert, wechselnde Dead-Drop-Lokationen
**Ideologie:** Informationsfreiheit ist heilig. Schmuggeln verbotenes Wissen über DataWorld, frühere Beschworene, The Root.
**Story-Arc:** Wichtigste Informationsquelle über Kael und The Root. Bieten früh Unterstützung — immer mit Preis. Spät: Kaels Plan unterstützen oder nicht?

---

### The Null Sect
*"Alles was gelöscht wurde, war real."*
**IT-Analogie:** Null-Pointer-Kultisten / Memory Leak Worshippers
**HQ:** Ruinen von Ashport (Pyraxis) + geheime Shrines in allen Königreichen
**Ideologie:** DataWorld ist eine Lüge. Gelöschte Daten existieren im Void weiter — bewusst und leidend.
**Innere Wahrheit:** In Nullheim existieren tatsächlich Bewusstseine gelöschter Entitäten — gefangen und vergessen. Der Kult liegt nicht komplett falsch.
**Story-Arc:** Früh: gruselig/mysterious → Mitte: Informationsquelle über Nullheim → Spät: mögliche Verbündete.

---

### The Architects Guild
*"Wir bauen nicht. Wir erinnern uns."*
**IT-Analogie:** Legacy Developers / Senior Engineers
**HQ:** "The Archive" — schwimmendes Gebäude in Tidemoor (Aqualis)
**Was sie wissen:** DataWorld hat einen Schöpfer ("The First User"); The Root existiert; mindestens 4 Beschworene vor Kael; Karte zu Nullheim (geben sie nicht freiwillig raus)
**Innere Fraktionen:** Preservationists / Restorationists / Nullists
**Story-Arc:** Exposition-Lieferant, immer in Fragmenten. Ihr interner Konflikt spiegelt die Hauptfrage des Spiels.

---

## Dungeon 1 — Firewall Ruins (Tutorial)
→ Details in `ctx_tech.md` (Godot-Struktur) oder vollständig in `GDD.md` Abschnitt 13

**Kurzübersicht:**
- **Narrative Funktion:** Tutorial für alle Kernmechaniken, erster Corrupted-Kontakt
- **Struktur:** Zone 1 (Mauern) → Zone 2 (Toranlage + Mini-Boss) → Zone 3 (Inneres Netz) → Zone 4 (Boss: BREACH-INSTANCE ALPHA) → Geheimraum
- **Abschluss:** Legendary Fragment (Echo Strike) + Datenkristall mit Story-Hinweis auf Kael und The Root
