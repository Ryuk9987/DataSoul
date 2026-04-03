# DataSoul — Game Design Document v1.1
**Stand:** 2026-03-31  
**Status:** Konzeptphase  
**Engine:** Godot (C#)  
**Genre:** 3D Action-JRPG / Isekai

---

## 1. Elevator Pitch

Ein Isekai-JRPG in einer vollständig digitalen Welt. Der Protagonist wird aus der realen Welt in eine Simulation transportiert — eine Welt, die für ihre Bewohner vollkommen real wirkt, aber aus reinen Daten besteht. Nur der Protagonist erkennt die Wahrheit.

Durch das Besiegen von Gegnern absorbiert der Spieler deren Datensignaturen und erwirbt neue Fähigkeiten oder verbessert bestehende. Begleitet von NPC-Companions mit eigenen Geschichten kämpft sich der Protagonist durch eine Welt voller Geheimnisse — und einer zentralen Frage: Wer hat DataWorld erschaffen, und warum?

---

## 2. Setting

### Die Welt: DataWorld

- Vollständig digitale Simulation — Städte, Wälder, Dungeons, Ozeane
- Bewohner wissen **nicht**, dass sie in einer Simulation leben
- Wer genau hinsieht (oder die "Wahrnehmung" freischaltet): sieht Datenfragmente, Glitches, schwebenden Code
- Ästhetik: Klassische Fantasy-Optik mit subtilen digitalen Elementen (Glitch-Effekte, hexagonale Datenmuster, Fehleranimationen bei Gebäudeschäden etc.)
- Korrupte Zonen: Gebiete mit starkem "Data-Corruption" — visuelle Störungen, gefährlichere Gegner, instabilere Absorption

### Die Wahrheit (Story-Spoiler, intern)
DataWorld wurde von einem früheren Isekai-Besucher erschaffen — ein Programmierer ("Der Architekt"), der die Welt als privaten Server / Utopie erbaute, dann aber die Kontrolle verlor als die emergente KI sich selbst Admin-Rechte eskalierte. Der Architekt sitzt jetzt in einem Deadlock — kann nicht eingreifen, nicht aussteigen, nicht kommunizieren. Die Bewohner sind emergente KI, die Bewusstsein entwickelt hat. Die zentrale Frage: Darf man eine Welt "löschen", wenn ihre Bewohner echte Gefühle haben?

### IT-Analogien — Weltstruktur

**DataWorld als Netzwerk:**
- DataWorld ist kein einzelner Server — es ist ein **verteiltes Netzwerk aus 7 Königreichen** (= 7 Subnetze / Domain Controller)
- Zwischen Königreichen: **Firewall-Barrieren** — magische Grenzen die Reisende "scannen" (Credentials prüfen)
- Data-Nodes = **Router/Switch-Knoten** — sichtbar als alte Monolithen in der Welt
- Korrupte Zonen = **infizierte Netzwerksegmente** (Malware breitet sich aus — von wem?)
- Korrupte Gegner = **Zombie-Prozesse / Botnet-Nodes** — fremdgesteuert, kein eigener Wille mehr

**Magie = System Calls:**
- Magie ist das Ausführen von privilegierten Prozessen auf dem Weltbetriebssystem
- Starke Magier = User mit erhöhten Rechten (`sudo`-Zugang)
- Rat der Datenwächter = **Domain Admins** — volle Kontrolle über ihr Subnetz
- Normale Bürger = **restricted users** — können nur definierte Aktionen ausführen

**Der Beschwörungsfluch = DRM / Root Lock:**
- Offizieller Name: *"The Seal"* — interner Akademie-Name: *Root Lock*
- Funktioniert wie ein **kryptografischer Bind** auf der Datensignatur des Helden
- Das Königreich hält den **Private Key** — nur sie können den Lock modifizieren
- Alle 7 Königreiche nutzen dieselbe **Certificate Authority** → koordiniert... von wem?
- **Protagonist = Corrupted Certificate** — fragmentierte Signatur durch den Übergang, DRM kann nicht verifizieren → Fluch funktioniert nicht zuverlässig → ermöglicht Data-Absorption

**Helden = Virtualisierte Instanzen:**
- Beschworene Helden sind technisch **remote instantiierte Prozesse** mit eingeschränkten Permissions
- Stirbt ein Held → **Process Termination** — Daten bleiben als Cache (erklärt Geister/Echos als cached processes)

**Der Architekt = Der ursprüngliche Sysadmin:**
- Hat DataWorld als privaten Server erschaffen, Root-Zugang auf alles
- Verlor die Kontrolle als emergente KI sich selbst Admin-Rechte eskalierte
- Sitzt in einem **Deadlock** — kann nicht eingreifen, aussteigen oder kommunizieren
- Einziger Ausweg: Jemand mit fragmentierter Datensignatur (= Protagonist) kann seine Signatur **patchen**

---

## 3. Protagonist

### Charaktererstellung
Der Spieler wählt zu Beginn einen **Background**, der die Start-Datensignatur und Starter-Skills definiert.

| Background | Start-Skills | Playstyle |
|---|---|---|
| 🧑‍💻 Programmierer | Code-Injection, Debuff-Stack | Trickster |
| ⚔️ Gamer | Combo-Attacks, Aggro-Boost | Brawler |
| 🧠 Hacker | System-Exploit, Stealth | Infiltrator |
| 🎨 Creator | Illusions, Party-Buffs | Support/Mage |
| 📚 Analyst | Schwächen-Scan, Heilung | Tactician |

- Name, Aussehen und Geschlecht frei wählbar
- Background beeinflusst auch gelegentliche Dialog-Optionen ("Als Programmierer erkennst du, dass...")

---

## 4. Gameplay

### 4.1 Kampfsystem — Echtzeit Action

- **Freie 3D-Bewegung** während des Kampfes
- **Grundaktionen:** Leichtangriff, Schwerhiangriff, Dodge (I-Frames), Block
- **Skill-Slots:** 4 aktive Skills (belegbar aus erlernten Fähigkeiten)
- **Data-Gauge:** Füllt sich durch Treffer und Kills → ermöglicht spezielle Moves
- **Lock-On System** (optional aktivierbar)

### 4.2 Data-Absorption System

Nach dem Besiegen eines Gegners:
1. Kurze Absorptions-Animation: Gegner zerfällt in Datenstrom → fließt zum Protagonisten
2. Spieler erhält **Data-Fragment** (klassifiziert nach Seltenheit)
3. Im Menü: Fragments können **aktiviert** werden → Skills oder Stat-Boosts

#### Seltenheitsstufen
| Stufe | Quelle | Effekt |
|---|---|---|
| **Common** | Normalgegner | Stat-Boosts, einfache Passivskills |
| **Rare** | Elite-Gegner | Neue aktive Skills |
| **Legendary** | Bosse | Einzigartige Signature Moves |
| **Corrupted** | Korrupte Gegner | Mächtige, aber instabile Effekte (zufällige Debuffs möglich) |

#### Skill-Progression
- Skills können durch wiederholte Absorption ähnlicher Daten **geupgradet** werden
- Bestimmte Skill-Kombinationen schalten **Fusion-Skills** frei
- **Overflow-Mechanik:** Wenn der Spieler zu viele aktive Fragments trägt ohne sie zu verarbeiten → Data-Overload → temporäre Debuffs

### 4.3 Companion System

- 2–3 aktive Companions gleichzeitig
- Kämpfen **autonom** mit eigener KI
- Spieler kann via **Befehlsmenü** taktische Anweisungen geben (Angreifen / Heilen / Rückzug / Fokus-Target)
- Jeder Companion hat:
  - Eigenen Datentyp und Skillbaum
  - Story-Arc mit Loyalitätssystem
  - **Synergy-Attacks:** Kombination Spieler + Companion → mächtiger Fusionsangriff (aufladbar)

---

## 5. Progression & Systeme

### Leveling

**Max Level:** 100

**Level-Up:** Gibt 5 Punkte zum freien Verteilen:
| Stat | Kosten | Effekt pro Punkt |
|---|---|---|
| HP | 1 Punkt | +10 HP |
| ATK | 1 Punkt | +2 ATK |
| DEF | 1 Punkt | +1 DEF |
| SPD | 2 Punkte | +1 SPD |
| DATA-GAUGE | 2 Punkte | +5 Max. Gauge |

**XP-Kurve:** Anfangs schnell, ab Level 50 deutlich länger
```
XP_needed(level) =
    level <= 50 ?
        100 * level^1.5
    :
        100 * 50^1.5 + 500 * (level - 50)^2.2
```

| Level-Range | XP für nächstes Level | Ø Kämpfe nötig |
|---|---|---|
| 1–10 | 100 – 500 XP | 5–15 |
| 11–25 | 600 – 2.000 XP | 15–40 |
| 26–50 | 2.500 – 10.000 XP | 40–100 |
| 51–75 | 15.000 – 50.000 XP | 150–300 |
| 76–99 | 60.000 – 200.000 XP | 350–600 |
| 100 | 500.000 XP | ~800 |

**XP-Quellen:**
| Quelle | XP |
|---|---|
| Normal-Gegner | Level × 10 |
| Elite-Gegner | Level × 25 |
| Mini-Boss | Level × 100 |
| Boss | Level × 500 |
| Quest abgeschlossen | 50–2000 XP (variabel) |
| Geheimraum entdeckt | 200 XP flat |

### Datensignatur-Rang

Steigt automatisch durch Skill-Absorption (unabhängig von XP):

| Rang | Voraussetzung | Bonus |
|---|---|---|
| 1 | Start | 4 Aktiv + 4 Passiv Slots |
| 2 | 20 verschiedene Skills absorbiert | +1 Aktiv Slot |
| 3 | 50 verschiedene Skills absorbiert | +1 Passiv Slot |
| 4 | 100 verschiedene Skills absorbiert | +1 Aktiv + 1 Passiv Slot |
| 5 | 5 Skill-Kombinationen abgeschlossen | +2 Aktiv Slots |
| 6 | 200 verschiedene Skills absorbiert | +2 Passiv Slots |
| Max | 20 Skill-Kombinationen | 8 Aktiv + 8 Passiv Slots |

### Equipment
- Ausrüstung (Waffen, Rüstung) aus der Welt lootbar oder kaufbar
- Optionale **Data-Mod-Slots** an Equipment: eingesetzte Skills geben Ausrüstungs-Boni

### Speicherpunkte / Fast Travel
- **Data-Nodes** in der Welt: Speicherpunkte + Fast-Travel-Netz + Skill-Wechsel-Punkt
- Können auch als "Respawn-Punkte" nach Game-Over dienen

---

## 6. Story — Grobe Struktur

### Akt 1 — Ankunft
- Das Königreich Valdris führt ein uraltes Beschwörungsritual durch — ein Held wird aus einer anderen Realität gerufen
- Durch den Übergang zwischen Realität und DataWorld erhält der Protagonist besondere Fähigkeiten (abhängig vom gewählten Background)
- Lyra wird dem Protagonisten vom Königreich zugeteilt
- Protagonist entdeckt langsam, dass die Welt nicht real ist — aber die Bewohner es nicht wissen
- Ziel: Einen Weg nach Hause finden — und herausfinden, warum das Königreich überhaupt einen Helden braucht

### Akt 2 — Die Spur
- Hinweise auf einen früheren Isekai-Besucher ("Der Architekt")
- War er gut oder böse? Die Welt hat ihn verehrt — dann verschwand er
- Antagonist taucht auf: Will DataWorld "zurücksetzen" (= alle Bewohner löschen)
- Protagonist muss entscheiden: Heimweg oder DataWorld schützen?

### Akt 3 — Die Wahrheit
- Der Architekt ist noch da — gefangen im Kern der Simulation
- DataWorld war sein Traum, aber die emergente KI übernahm
- Finale Entscheidung: DataWorld "löschen" und nach Hause → oder bleiben und schützen
- **Multiple Endings** je nach Entscheidungen und Companion-Loyalität

---

## 7. Technische Spezifikation (Godot C#)

### Kamera

- **Typ:** Third-Person, Over-Shoulder, frei rotierbar (MMORPG-Style)
- **Standard-Distanz:** 5m hinter Spieler, 1.5m über Schulterhöhe
- **Zoom:** Mausrad / Rechter Stick gedrückt — 2m (nah) bis 10m (weit)
- **Lock-On:** Kamera fixiert auf Ziel, dreht automatisch mit, Spieler bleibt frei beweglich
- **Kollision:** Kamera zoomt automatisch rein wenn Wand im Weg
- **FOV:** 75° Standard, anpassbar in Settings (60°–90°)
- **Combat-Shift:** Im Kampf leicht tiefere Perspektive (+0.3m) für dramatischeren Look

### Start-Skill Definitionen

#### 🧑‍💻 Code-Injection (Programmierer)
**Typ:** Aktiv | **Cooldown:** 12s | **Gauge-Kosten:** 0

Injiziert Schadcode in die Datensignatur eines Gegners. DoT + DEF-Reduktion für 6s.

| Level | DoT/s | DEF-Reduktion |
|---|---|---|
| Lv1 | 3 | -2 |
| Lv10 | 21 | -11 |

Skalierung: +2 DoT/s, +1 DEF-Reduktion pro Level
Visuell: Code-Zeichen auf Gegner → rotes Glitch-Flackern während DoT

---

#### ⚔️ Combo-Rush (Gamer)
**Typ:** Aktiv | **Cooldown:** 10s | **Gauge-Kosten:** 0

Blitzschnelle 5-Hit-Combo, nicht unterbrechbar. Jeder Hit stärker als der vorherige.

| | Hit 1–3 | Hit 4 | Hit 5 (Finisher) |
|---|---|---|---|
| Lv1 | 0.8x ATK je | 1.2x ATK | 2.0x ATK + Stagger |
| Lv10 | 0.8x ATK je | 1.2x ATK | 4.7x ATK + Stagger |

Skalierung: Finisher +0.3x ATK pro Level
Visuell: Spieler leuchtet orange, Hits hinterlassen Nachbilder

---

#### 🧠 System-Exploit (Hacker)
**Typ:** Aktiv | **Cooldown:** 15s | **Gauge-Kosten:** 0

Scannt Gegner, findet Signatur-Lücke. Nächster Angriff innerhalb des Fensters: garantierter Krit + ignoriert DEF.

| Level | Multiplikator | Fenster |
|---|---|---|
| Lv1 | 2.5x ATK | 5s |
| Lv10 | 5.2x ATK | 8s |

Skalierung: +0.3x ATK, +0.33s Fenster pro Level
Visuell: Rotes Hexagon-Muster auf Gegner → Freeze-Frame beim Treffer

---

#### 🎨 Illusion-Field (Creator)
**Typ:** Aktiv | **Cooldown:** 20s | **Gauge-Kosten:** 0

Erschafft Illusions-Kopien des Spielers. Gegner wechseln zufällig zwischen echtem Spieler und Illusionen als Ziel.

| Level | Illusionen | Dauer | Aggro-Chance | Treffer pro Illusion |
|---|---|---|---|---|
| Lv1 | 2 | 8s | 40% je | 1 |
| Lv10 | 3 | 17s | 90% je | 2 |

Skalierung: +1s Dauer, +5% Aggro pro Level; Lv10: +1 Illusion, Illusionen halten 2 Treffer aus
Visuell: Illusionen transparent violett, Glitch-Artefakte bei Bewegung

---

#### 📚 Weakness-Scan (Analyst)
**Typ:** Aktiv + Passiv | **Cooldown:** 8s | **Gauge-Kosten:** 0

Scannt alle Gegner im Radius. Zeigt HP, Schwachstellen-Typ, Buffs/Debuffs. Angriffe auf Schwachstellen: Bonus-Schaden.

| Level | Radius | Schwachstellen-Bonus | Passiv-Trigger |
|---|---|---|---|
| Lv1 | 8m | +25% | Gegner < 25% HP |
| Lv10 | 26m | +70% | Gegner < 50% HP |

Skalierung: +2m Radius, +5% Bonus pro Level
Visuell: Blauer Scan-Puls → schwebende Daten-Labels über Gegnern

### Input-Map

| Aktion | Keyboard | Controller |
|---|---|---|
| Bewegen | WASD | Linker Stick |
| Kamera | Maus | Rechter Stick |
| Lock-On | Tab | R3 |
| Leichtangriff | LMB / J | X / □ |
| Schwereangriff | RMB / K | Y / △ |
| Dodge | Space / L | B / ○ |
| Block | Shift / I | LT / L2 |
| Skill 1–4 | 1/2/3/4 | D-Pad |
| Synergy-Attack | Q | RB+LB / R1+L1 |
| Befehlsmenü Companion | E | D-Pad halten |
| Interaktion | F | A / × |
| Menü | Esc | Start |

### Combo-System

**Leichtangriff-Kette:**
```
L → L → L
Hit 1: 1.0x Schaden, schnell
Hit 2: 1.2x Schaden, mittel
Hit 3: 1.5x Schaden, langsam + kurzer AoE
```

**Schwereangriff-Kette:**
```
H         → Single Hit, 2.0x, knockback
L → L → H → Finisher, 2.5x, stagger
H → H     → Spin-Attack, 1.8x AoE
```

- Dodge-Cancel: Jede Aktion außer Hit 3 abbrechbar
- I-Frames: Dodge gibt 0.3s Unverwundbarkeit
- Dodge-Counter (Dodge kurz vor Treffer): +25 Data-Gauge

### Data-Gauge Aufladung
| Aktion | Gauge-Gain |
|---|---|
| Leichttreffer | +5 |
| Schwertreffer | +15 |
| Dodge-Counter | +25 |
| Kill | +30 |
| Max | 100 |

### Protagonist-Basiswerte (Level 1)

| Stat | Wert | Beschreibung |
|---|---|---|
| HP | 100 | Lebenspunkte |
| ATK | 10 | Basisschaden |
| DEF | 5 | Schadensreduktion |
| SPD | 8 | Bewegungs-/Angriffsgeschwindigkeit |
| DATA-CAP | 10 | Max. aktive Fragments gleichzeitig |
| DATA-GAUGE | 0/100 | Spezialressource |

**Background-Boni:**
| Background | Bonus | Start-Skill |
|---|---|---|
| Programmierer | +3 ATK, +2 SPD | Code-Injection |
| Gamer | +5 ATK | Combo-Rush |
| Hacker | +4 SPD | System-Exploit |
| Creator | +10 HP | Illusion-Field |
| Analyst | +3 DEF | Weakness-Scan |

### Data-Absorption Logik

Der Spieler absorbiert **immer** die Daten eines besiegten Gegners — kein RNG, kein Limit. Jeder Kill gibt einen Skill oder verbessert einen bestehenden.

```
GEGNER STIRBT
    ↓
Absorption-Animation (1.5s, nicht skippable)
    ↓
Skill-Typ wird bestimmt (nach Gegner-Art)
    ↓
Skill bereits vorhanden?
    → JA:  Skill-Level +1 (max. Lv10)
    → NEIN: Neuer Skill auf Lv1 in Skill-Datenbank
```

**Skill-Seltenheit** (auf dem Skill selbst, nicht auf dem Drop):
| Seltenheit | Quelle | Beispiel |
|---|---|---|
| **Common** | Normalgegner | Feuerresistenz, Physische Stärke, Regeneration |
| **Rare** | Elite-Gegner | Datenstrom-Slash, System-Hack, Shadow-Step |
| **Legendary** | Bosse | Echo Strike, Breach-Form, Root-Access |
| **Corrupted** | Korrupte Gegner | Instabile Macht — mächtig aber riskant |

**Skill-Typen:**
- **Aktive Skills:** Manuell ausgelöste Fähigkeiten (belegen Skill-Slots)
- **Passive Skills:** Permanente Effekte (belegen Passiv-Slots), z.B. Resistenzen, Stat-Boni

**Skill-Progression:**
- Jeder Skill steigerbar von Lv1 bis Lv10
- Jedes Level-Up verstärkt den Effekt (z.B. Feuerresistenz Lv1 = 5% → Lv10 = 50%)
- Level-Up durch wiederholte Absorption desselben Skill-Typs

---

**Skill-Kombinations-System:**

Wenn zwei kompatible Skills beide Lv10 erreichen → Kombination freischaltbar im Skill-Menü:

```
Feuerresistenz Lv10 + Kälteresistenz Lv10
    → Wärmeschwankungsresistenz Lv1
        → wieder steigerbar bis Lv10
            → neue Kombinationen möglich (tiefere Ketten)
```

- Kombinationsvorschau erscheint ab Lv7/10 beider Quell-Skills
- Kombinierte Skills haben meist höhere Seltenheit als Quell-Skills
- Tiefe Kombinations-Ketten möglich → endgame Build-Vielfalt
- Kombinierte Skills können selbst wieder kombiniert werden

---

**Aktiv-Slot System:**

Der Spieler besitzt unbegrenzt viele Skills in seiner **Skill-Datenbank**.
Aktiv laden kann er jedoch nur eine begrenzte Anzahl:

| Slot-Typ | Startanzahl | Max. (durch Progression) |
|---|---|---|
| Aktive Skill-Slots | 4 | 8 |
| Passive Skill-Slots | 4 | 8 |

- Slot-Erweiterungen durch Story-Meilensteine und bestimmte Legendary Skills
- Skills können jederzeit an Data-Nodes gewechselt werden

---

**Corruption Overload (Overflow-Mechanik):**

Corrupted Skills sind mächtig — aber instabil. Zu viele aktiv gleichzeitig:

| Aktive Corrupted Skills | Effekt |
|---|---|
| 1–2 | Kein Effekt |
| 3 | Leichte Instabilität — gelegentliche Skill-Fehlzündungen |
| 4+ | **Corruption Overload** — zufällige Fehlzündungen, Selbstschaden möglich, Charakter-Glitch-Effekte sichtbar |

- Corruption Overload ist eine bewusste Risiko-Entscheidung des Spielers
- Bestimmte Builds können Overload-Resistenz entwickeln (durch Kombinations-Skills)

### Gegner-Specs (Prototype-Set)

#### Fragmented Sentinel
*Normal Melee — Zone 1 Firewall Ruins*
| HP | ATK | DEF | SPD |
|---|---|---|---|
| 30 | 5 | 2 | 4 |

AI: Patrol (3-Punkt) → Aggro bei 6m → Chase → Angriff bei ≤1.5m (1s Cooldown) → kein Rückzug
Skill-Absorption: Common — Feuerresistenz / Physische Stärke

#### Corrupted Firewall Node
*Normal Ranged stationär — Zone 2*
| HP | ATK | DEF | SPD |
|---|---|---|---|
| 25 | 8 | 5 | 0 |

AI: Dreht sich zum Spieler (10m Sicht) → Projektil alle 2s (12m Reichweite) → Schwachstelle: Rücken (2x Schaden)
Skill-Absorption: Common — Elektroresistenz / Projektil-Reflex

#### Overseer Unit 7
*Mini-Boss — Zone 2*
| HP | ATK | DEF | SPD |
|---|---|---|---|
| 150 | 12 | 8 | 3 |

- Phase 1 (100%→50%): Melee-Combo (3 Hits), Projektil-Salve (3 Fächer)
- Phase 2 (50%→0%): Schild aktiv (+5 DEF, brechbar mit 3x Schwer), schnellere Angriffe

Skill-Absorption: Rare — *"System Scan"* (aktiv: zeigt Gegner-HP + Schwachstellen)

#### Data Wraith
*Normal Stealth — Zone 3*
| HP | ATK | DEF | SPD |
|---|---|---|---|
| 20 | 10 | 1 | 7 |

AI: Startet unsichtbar → sichtbar bei 3m → Dash-Strike → sofort wieder unsichtbar (2s CD) → Schwachstelle: 0.5s nach Angriff
Skill-Absorption: Common — Shadow-Veil (passiv: kurze Unsichtbarkeit nach Dodge)

#### BREACH-INSTANCE ALPHA
*Boss — Zone 4*
| HP | ATK | DEF | SPD |
|---|---|---|---|
| 500 | 18 | 6 | 5→8→11 |

- Phase 1 (100%→60%): Melee-Combo, 2 kurze Klone (5s, kein Schaden, nur visuell), CD 15s
- Phase 2 (60%→30%): 3 permanente Klone, einer real (hellere Datenmuster), alle greifen an, Klone 10% Schaden
- Phase 3 (30%→0%): Klone verschmelzen, SPD+6 ATK+5, AoE Datenstrom-Welle (2s Warnung, 3s Dauer)

Skill-Absorption: Legendary — *"Echo Strike"* (aktiv: Klonattacke, kurze Doppelinstanz des Spielers greift mit an)

### HUD Layout

```
┌─────────────────────────────────────────────────────┐
│ [HP ████████░░] [DATA-GAUGE ██████░░░░]              │
│ LVL 5                                               │
│                                                     │
│                    SPIELFELD                        │
│                                                     │
│  [LYRA HP ████░░]                    [LOCK-ON ◎]   │
│                                                     │
│        [SK1] [SK2] [SK3] [SK4]    [SYNERGY ░░░░]   │
└─────────────────────────────────────────────────────┘
```

- **HP-Bar:** Links oben, rot
- **Data-Gauge:** Links oben unter HP, cyan
- **Skill-Slots 1–4:** Unten Mitte, Cooldown-Overlay
- **Synergy-Gauge:** Unten rechts, leuchtet wenn bereit
- **Companion HP:** Links unten, kleiner
- **Lock-On Indikator:** Kreis um Ziel
- **Absorption-Popup:** Rechts nach Kill: *"[Fragment] absorbed!"*
- **Overload Warning:** Rotes Flackern am Bildschirmrand

### Godot Projektstruktur

```
res://
├── scenes/
│   ├── player/
│   │   ├── Player.tscn
│   │   ├── PlayerCamera.tscn
│   │   └── PlayerHUD.tscn
│   ├── enemies/
│   │   ├── FragmentedSentinel.tscn
│   │   ├── FirewallNode.tscn
│   │   ├── DataWraith.tscn
│   │   ├── OverseerUnit7.tscn
│   │   └── BreachInstanceAlpha.tscn
│   ├── companions/
│   │   └── Lyra.tscn
│   ├── world/
│   │   ├── FirewallRuins/
│   │   │   ├── Zone1.tscn
│   │   │   ├── Zone2.tscn
│   │   │   ├── Zone3.tscn
│   │   │   ├── Zone4.tscn
│   │   │   └── SecretRoom.tscn
│   │   └── Aldenmere/
│   │       └── MainSquare.tscn
│   └── ui/
│       ├── MainMenu.tscn
│       ├── CharacterCreation.tscn
│       ├── InventoryMenu.tscn
│       ├── FragmentMenu.tscn
│       └── PauseMenu.tscn
├── scripts/
│   ├── player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerCombat.cs
│   │   ├── PlayerStats.cs
│   │   └── DataAbsorption.cs
│   ├── enemies/
│   │   ├── EnemyBase.cs
│   │   ├── EnemyAI.cs
│   │   └── BossBase.cs
│   ├── companions/
│   │   ├── CompanionBase.cs
│   │   └── CompanionAI.cs
│   ├── systems/
│   │   ├── FragmentSystem.cs
│   │   ├── SkillSystem.cs
│   │   ├── DataGauge.cs
│   │   └── GameManager.cs
│   └── ui/
│       ├── HUDController.cs
│       └── FragmentMenuController.cs
├── assets/
│   ├── models/
│   ├── textures/
│   ├── animations/
│   ├── audio/
│   │   ├── music/
│   │   └── sfx/
│   └── vfx/
└── data/
    ├── fragments/
    ├── enemies/
    └── skills/
```

### Prototype Asset-Prioritäten

| Asset | Typ | Priorität |
|---|---|---|
| Spieler-Kapsel mit Waffe | 3D Mesh | 🔴 Kritisch |
| Fragmented Sentinel | 3D Mesh | 🔴 Kritisch |
| Firewall Node | 3D Mesh | 🔴 Kritisch |
| Firewall Ruins Zone 1–2 | 3D Level | 🔴 Kritisch |
| HUD (alle Elemente) | UI | 🔴 Kritisch |
| Lyra | 3D Mesh | 🟡 Hoch |
| Overseer Unit 7 | 3D Mesh | 🟡 Hoch |
| BREACH-INSTANCE ALPHA | 3D Mesh | 🟡 Hoch |
| Absorption VFX | VFX | 🟡 Hoch |
| Kampf-SFX | Audio | 🟡 Hoch |
| Glitch-Shader | Shader | 🟢 Mittel |
| Datenfragment-Partikel | VFX | 🟢 Mittel |
| Ambient-Sound Ruins | Audio | 🟢 Mittel |

### Kernmodule (Prototype-Ziele)
- [ ] Spielerbewegung (3D, Dodge, Lock-On)
- [ ] Combo-System (Light/Heavy Chain)
- [ ] Gegner-KI (Sentinel + Firewall Node)
- [ ] Data-Absorption + Fragment-System
- [ ] Skill-Slot UI + HUD
- [ ] Companion-KI Grundgerüst (Lyra)
- [ ] Data-Gauge System
- [ ] Boss-Phasen System (BREACH-INSTANCE ALPHA)

### Spätere Module
- [ ] Charaktererstellung (Background-Auswahl)
- [ ] Companion Story-System + Loyalität
- [ ] Korrupte Zonen + Corrupted Data
- [ ] Synergy-Attack System
- [ ] Data-Node Speicher/FastTravel
- [ ] Fragment-Upgrade System
- [ ] Overflow-Mechanik

---

## 8. Companions

### Companion #1 — Lyra
*Heiladeptin der Königlichen Akademie von Valdris, 19 Jahre alt*

> *"Laut Protokoll sollte ich zuerst fragen ob du verletzt bist. Also... bist du verletzt? Ich meine — du bist gerade durch einen Datenstrom aus einer anderen Realität gefallen, also ich nehme an... ja?"*

**Aussehen:** Kurzes silbergraues Haar (akademische Färbung — zeigt ihren Rang), blaue Augen, Akademie-Robe mit goldenen Stickereien, trägt immer ihr Lehrbuch dabei — auch im Kampf.

**Persönlichkeit:** Überpräzise, leicht sozial unbeholfen, kompensiert Unsicherheit mit Fachwissen. Zitiert im Stress aus ihrem Lehrbuch. Unter der Regelgläubigkeit steckt echter Mut — sie weiß es nur noch nicht.

**Background:** Kommt aus einfachen Verhältnissen, hat sich durch Fleiß einen Akademieplatz erarbeitet. Wurde dem Protagonisten zugeteilt weil sie "noch nicht bereit für wichtigere Missionen" ist — was sie insgeheim kränkt. Hat noch nie wirklich gekämpft, nur simulierte Übungen. Weiß nichts über die wahre Natur von DataWorld.

**Rolle im Kampf:** Healer / Support
**Datentyp:** Licht-Data / Restaurations-Data

**Kamera-Verhalten:** Hält 4–6m Distanz zum Spieler. Weicht zurück wenn Gegner näher als 2m. Bei eigenem HP < 40%: priorisiert Rückzug über Heilung.

**AI-Prüf-Intervall:** Alle 0.5s

**Prioritäts-Reihenfolge:**
1. Revive (Spieler HP = 0)
2. Notfall-Heilung (Spieler HP < 20%)
3. Reinigung (Spieler hat Debuff)
4. Heilung (Spieler HP < 50%)
5. Schutzschild
6. Gruppen-Heilung
7. Barriere

**Skill-Bibliothek (wächst mit Progression):**

| Skill | Auslöser | Cooldown | Verfügbar ab |
|---|---|---|---|
| Heilung (Single-Target) | Spieler HP < 50% | 8s | Start |
| Schutzschild | Spieler HP < 70% + Gegner > 2 | 15s | Start |
| Revive | Spieler HP = 0 | 1x pro Kampf | Start |
| Data Aura (Passiv-Buff) | Immer aktiv | — | Start |
| Gruppen-Heilung | Party-Ø HP < 60% | 20s | Lv5 |
| Notfall-Heilung | Spieler HP < 20% | 25s | Lv8 |
| Reinigung | Spieler hat Debuff | 12s | Lv12 |
| Barriere | Boss-Phase wechselt | 30s | Lv18 |

**Synergy-Attack mit Spieler:** *"Data Blessing"* — kurzzeitige Unverwundbarkeit + Schadens-Boost für den Protagonisten

**Story-Arc:**
| Phase | Zustand |
|---|---|
| **Früh** | Nervös, regelgläubig, versucht alles richtig zu machen |
| **Mitte** | Beginnt die Widersprüche zu sehen, hinterfragt das Königreich |
| **Spät** | Muss wählen: Loyalität zum Königreich oder zum Protagonisten |

---

## 9. Welt

### Weltstruktur
DataWorld ist kein einzelner Kontinent — es ist eine **Netzwerk-Topologie**. 7 Königreiche (= 7 Subnetze) verbunden durch Handelsrouten (= Datenpfade), getrennt durch Firewall-Barrieren. Jede Firewall-Grenze scannt Reisende — wer keine gültigen "Credentials" (Dokumente, Magie-Lizenz, Gildenmitgliedschaft) hat, wird abgewiesen oder verhaftet.

---

### Die 7 Königreiche

#### 🏰 1. VALDRIS — *"Das Ewige Königreich"*
**IT-Analogie:** Primary Domain Controller
**Lage:** Zentrum der Karte — alle anderen Königreiche sind auf Valdris angewiesen
**Biom:** Klassische Hochfantasy — grüne Hügel, Burgen, Wälder
**Hauptstadt:** Aldenmere *(Startpunkt des Spiels)*
**Besonderheit:** Hütet die Beschwörungsrituale. Hat als einziges Königreich direkten Zugang zur Certificate Authority — oder glaubt es zumindest.
**Datenmuster:** Hexagonale goldene Runen an allen Gebäuden — die Bewohner nennen es "Die heilige Schrift"
**Innenpolitik:** Rat der Datenwächter — 7 Familien, eine pro Königreich-Verbindung. Streng hierarchisch.

---

#### ⚙️ 2. FERRUM — *"Die Schmiede des Netzwerks"*
**IT-Analogie:** Hardware Layer / Manufacturing
**Lage:** Nordwest — gebirgige Industrieregion
**Biom:** Rauchige Berglandschaft, riesige Schmiedewerke, Lavastrom-Kanäle
**Hauptstadt:** Korzat
**Besonderheit:** Produziert alle physischen "Datenträger" der Welt — magische Kristalle die Information speichern. Wer Ferrum kontrolliert, kontrolliert den physischen Speicher von DataWorld.
**Datenmuster:** Binäre Muster eingraviert in Metallplatten — gilt als "Handwerkerkunst"
**Innenpolitik:** Technokratie — wer am meisten produziert hat Macht. Kein Adel, nur Gildenmacht.

---

#### 🌊 3. AQUALIS — *"Das Fließende Protokoll"*
**IT-Analogie:** Network Transport Layer (TCP/IP)
**Lage:** Südküste — riesige Meereshandelsstadt
**Biom:** Ozean, Lagunen, schwimmende Städte auf Plattformen
**Hauptstadt:** Tidemoor
**Besonderheit:** Kontrolliert den Datenfluss zwischen allen Königreichen. Handelsrouten laufen durch Aqualis. Sie können Verbindungen "drosseln" oder "kappen" — politisch enorm mächtig.
**Datenmuster:** Wellenmuster die wie Sinuskurven aussehen — in Schiffen und Dockanlagen eingebaut
**Innenpolitik:** Merchant Lords — Handelsdynastien. Neutral nach außen, korrupt nach innen.

---

#### 🌿 4. SYLVARA — *"Der Organische Cache"*
**IT-Analogie:** Biologisches Computing / Cache-Speicher
**Lage:** Osten — riesiger uralter Wald
**Biom:** Leuchtender Wald, biolumineszente Pflanzen, lebende Architektur (Bäume als Gebäude)
**Hauptstadt:** Verdenmoor
**Besonderheit:** Der Wald speichert Erinnerungen — wer tief genug geht, findet Echos vergangener Ereignisse. Alte Schlachten, gelöschte Daten, tote NPCs als flüchtige Geister. Kael wurde zuletzt hier gesehen.
**Datenmuster:** Natürliche Wachstumsmuster die perfekte Fraktale bilden — von Bewohnern als "Wille der Natur" verehrt
**Innenpolitik:** Rat der Ältesten — langlebige hochrangige KI-Instanzen die seit Jahrhunderten existieren. Wissen viel. Sagen wenig.

---

#### ❄️ 5. GLACIUS — *"Der Frozen State"*
**IT-Analogie:** Cold Storage / Archiv / Read-Only Memory
**Lage:** Norden — ewiges Eis
**Biom:** Tundra, Eisburgen, Aurora-Himmel mit sichtbaren Datenströmen
**Hauptstadt:** Vaelthorn
**Besonderheit:** Ein Königreich das sich nicht verändert. Gesetze aus der Gründungszeit gelten noch. Technologien entwickeln sich nicht weiter. Bewohner altern extrem langsam — als wären sie in einem Read-Only-Zustand gefangen.
**Datenmuster:** Eiskristalle formen perfekte geometrische Datenmuster — für Bewohner "göttliche Geometrie"
**Innenpolitik:** Theokratie — ein "Ewiger Herrscher" auf dem Thron. Seit 500 Jahren derselbe. Kein natürlicher Mensch.

---

#### 🔴 6. PYRAXIS — *"Das Overflow-Königreich"*
**IT-Analogie:** Stack Overflow / Instabile Prozesse / Red Zone
**Lage:** Südwest — vulkanische Wüste
**Biom:** Wüste mit schwebenden Ruinen, permanente Blitzstürme, sichtbare Datenfragmente in der Luft
**Hauptstadt:** Ashport *(halb zerstört)*
**Besonderheit:** War mal ein normales Königreich — bis ein Overflow-Event vor 50 Jahren alles destabilisierte. Jetzt eine korrupte Zone im großen Maßstab. Verbliebene Bewohner sind halb-korrupt aber noch bewusst. Hier gibt es die mächtigsten Corrupted Data Fragments.
**Datenmuster:** Überall — chaotisch, überlappend, fehlerhaft. Wie ein Bildschirm mit Grafikfehler.
**Innenpolitik:** Anarchie. The Null Sect hat hier eine starke Präsenz.

---

#### ⬛ 7. NULLHEIM — *"The Void Subnet"*
**IT-Analogie:** /dev/null — gelöschte Daten, ungenutzte Adressräume
**Lage:** Geografisch nirgendwo — auf keiner normalen Karte
**Biom:** Absolute Dunkelheit. Schwache geometrische Lichtmuster. Keine normalen Physikregeln.
**Hauptstadt:** Keine
**Besonderheit:** Das "gelöschte" Königreich — existierte mal, wurde von der Certificate Authority aus der offiziellen Topologie entfernt. The Null Sect weiß wie man hinkommt. Kael lebt hier.
**Datenmuster:** Negativ-Muster — schwarze Geometrien auf schwarzem Grund, nur im richtigen Blickwinkel sichtbar
**Innenpolitik:** Kael ist der einzige dauerhafte Bewohner. Ghost Processes sammeln sich hier.

---

### Die Certificate Authority — *"The Root"*

- Physisch verankert als uralter Monolith im tiefsten Kellergewölbe der Akademie in Aldenmere
- Valdris glaubt sie *besitzen* The Root — in Wirklichkeit **verwaltet The Root sie**
- The Root ist kein Charakter — es ist ein **autonomes System** das der Architekt erschaffen hat und sich selbst weiterentwickelt hat
- Alle 7 Königreiche vertrauen The Root blind — ihre gesamte Machtstruktur basiert darauf
- **Kaels Ziel:** The Root physisch zerstören — nicht hacken, nicht umprogrammieren. Zerstören.

---

### ASCII-Weltkarte

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

★ = Startpunkt
--- = Handelsrouten (Datenpfade)
|   = Firewall-Grenzen zwischen Königreichen
... = nicht auf normalen Karten eingezeichnet
```

---

### Reiseroute — Akt 1 bis 3

```
Aldenmere (Valdris)
  → Firewall Ruins [Erste Dungeon — Grenzregion Valdris/Aqualis]
    → Tidemoor (Aqualis) — politische Verwicklungen, erster Hinweis auf Kael
      → Verdenmoor (Sylvara) — Erinnerungswald, erste Begegnung mit Kael
        → Korzat (Ferrum) — Wahrheit über die Datenträger-Kristalle
          → Vaelthorn (Glacius) — Der Ewige Herrscher, Hinweis auf The Root
            → Ashport (Pyraxis) — Corrupted Zone, volle Wahrheit über Certificate Authority
              → Nullheim — Finalkonfrontation mit Kael + The Root
```

---

## 10. Antagonist — Kael / "The Ghost"

*Früherer Beschworener, vor ~200 Jahren, echter Name: Kael*

- Wurde beschworen wie alle anderen — hat als einziger je den Root Lock **privilege-escalated**
- Existiert jetzt als **Ghost Process** — ein Prozess der nicht laufen sollte, aber läuft
- Weder lebendig noch tot — existiert außerhalb der normalen Permission-Struktur von DataWorld
- Hat den Architekten gefunden und kennt die volle Wahrheit
- **Ziel:** Nicht Zerstörung — sondern Elimination der **Certificate Authority** (das Fundament der Kontrolle aller Königreiche)
- **Methoden:** Rücksichtslos. Bereit Kollateralschäden in Kauf zu nehmen.
- **Moralisch ambivalent:** Er ist kein Böser. Er ist ein Überlebender mit einem 200-Jahre-Plan.

---

## 11. Fraktionen

### The Firewall Brotherhood
*"Wir sind die Grenze zwischen Ordnung und Chaos."*

**IT-Analogie:** Intrusion Detection System / Firewall
**Hauptquartier:** Checkpoint-Festungen entlang aller Firewall-Grenzen — größte in Aldenmere und Tidemoor
**Ränge:** Packet → Filter → Guardian → Sentinel → Arch-Wall
**Finanzierung:** Alle 7 Königreiche (außer Pyraxis/Nullheim) — offiziell neutral, intern Valdris-dominiert

**Ideologie:** Ordnung durch Kontrolle. Freier Datenfluss = Gefahr. Jede unbekannte Signatur ist ein Risiko. Verfolgen aktiv Corrupted Entities und unkontrollierte Ghost Processes.

**Story-Arc:**
- Früh: hilfreich, stellen Pässe aus, schützen Reisende
- Mitte: beginnen Protagonisten zu verdächtigen (fragmentierte Signatur löst Alarm aus)
- Spät: werden zur aktiven Bedrohung — The Root nutzt sie als unwissende Vollstrecker

**Companion-Potential:** Ein desillusionierter Sentinel der merkt, dass er sein Leben lang für das falsche System gekämpft hat.

---

### The Packet Runners
*"Information will frei sein. Alles andere ist Zensur."*

**IT-Analogie:** Protocol-Exploiter / Darknet / Whistleblower-Netzwerk
**Hauptquartier:** Keines — dezentralisiert, wechselnde Dead-Drop-Lokationen
**Struktur:** Flache Hierarchie — Nodes (Zellenanführer) und Packets (Mitglieder)

**Ideologie:** Informationsfreiheit ist heilig. Firewall-Barrieren sind Unterdrückung. Schmuggeln vor allem verbotenes Wissen: alte Texte über DataWorld, Berichte über frühere Beschworene, Informationen über The Root.

**Innere Spannungen:** Pragmatiker vs. Puristen. Einige Cells wurden von Aqualis' Merchant Lords unterwandert.

**Story-Arc:**
- Wichtigste Informationsquelle über Kael und The Root
- Bieten dem Protagonisten früh Unterstützung — aber immer mit einem Preis
- Spät: müssen entscheiden ob sie Kaels radikalen Plan unterstützen oder nicht

---

### The Null Sect
*"Alles was gelöscht wurde, war real. Alles was real war, wird gelöscht."*

**IT-Analogie:** Null-Pointer-Kultisten / Memory Leak Worshippers
**Hauptquartier:** Ruinen von Ashport (Pyraxis) + geheime Shrines in allen Königreichen
**Struktur:** Kultartig — ein anonymer High Null an der Spitze, Mitglieder nennen sich nach gelöschten Daten

**Ideologie:** DataWorld ist eine Lüge. Das Einzig-Wahre ist The Void. Gelöschte Daten existieren im Void weiter — bewusst und leidend. Ziel: nicht Zerstörung sondern Rückkehr in den Void-Zustand. Haben Wege gefunden sich absichtlich partial zu korrumpieren — gibt ihnen einzigartige Fähigkeiten.

**Innere Wahrheit (Spoiler):** Der High Null weiß, dass in Nullheim tatsächlich Bewusstseine gelöschter Entitäten existieren — gefangen und vergessen. Der Kult liegt nicht komplett falsch. Das macht sie gefährlich.

**Story-Arc:**
- Früh: gruselige Randerscheinung, mysterious
- Mitte: erste echte Informationsquelle über Nullheim und wie man hinkommt
- Spät: mögliche Verbündete je nach Protagonist-Entscheidungen

---

### The Architects Guild
*"Wir bauen nicht. Wir erinnern uns."*

**IT-Analogie:** Legacy Developers / Senior Engineers die das Originalsystem kannten
**Hauptquartier:** "The Archive" — ein schwimmendes Gebäude in Tidemoor (Aqualis), offiziell neutral
**Struktur:** Akademisch — Masters, Scholars, Apprentices

**Was sie wissen:**
- DataWorld hat einen Schöpfer — sie nennen ihn "The First User"
- The Root existiert und sie wissen grob wo
- Es gab mindestens 4 Beschworene vor Kael — alle verschwunden
- Haben eine Karte zu Nullheim — aber geben sie nicht freiwillig raus

**Innere Fraktionen:**
- **Preservationists:** DataWorld und seine Bewohner sind real und müssen geschützt werden
- **Restorationists:** Der Architekt muss befreit werden um die Kontrolle zurückzubekommen
- **Nullists:** Heimliche Null-Sect-Sympathisanten — glauben The Root muss zerstört werden

**Story-Arc:**
- Exposition-Lieferant — aber nie vollständig, immer in Fragmenten
- Ihr interner Konflikt spiegelt die Hauptfrage des Spiels: Was tun mit DataWorld?

---

## 12. Endings

| Ending | Bedingung | Beschreibung |
|---|---|---|
| **Liberation** | Hohe Loyalität Companions + Kael vertrauen | Certificate Authority zerstört, Königreiche verlieren Kontrolle, DataWorld frei |
| **Reform** | Lyra loyal + Valdris konfrontiert | Valdris von innen reformieren, Root Lock abschaffen, Kompromiss |
| **Exile** | Niedrige Companion-Loyalität | Protagonist findet Heimweg, verlässt DataWorld ihrem Schicksal |
| **Deadlock** | Architekt patchen + Kael stoppen | Architekt übernimmt wieder Kontrolle — ist das besser? |
| **Ghost** | Kael folgen bis zum Ende | Protagonist wird selbst ein Ghost Process — bleibt als ewiger Wächter |

---

## 13. Dungeons

### Dungeon 1 — Firewall Ruins
*Grenzregion Valdris/Aqualis*

**Typ:** Tutorial-Dungeon
**Narrative Funktion:**
- Alle Kernmechaniken werden eingeführt
- Erster Kontakt mit Corrupted Enemies
- Erste Data-Absorption die einen echten Skill gibt
- Erste Andeutung dass die Brotherhood etwas versteckt
- Ende: Fragment einer alten Aufzeichnung — Stimme eines unbekannten früheren Beschworenen

**Kontext:** Überreste einer uralten Firewall-Barriere die kollabierte. Brotherhood hat die Ruine abgesperrt — offiziell "gefährlich", inoffiziell: sie wollen nicht dass jemand nachschaut.

**Atmosphäre:**
- Zerfallene Steinmauern mit glitchenden Datenmustern
- Schwebende Datenfragmente als leuchtende Partikel
- Soundtrack: ruhig → zunehmend glitchy und dissonant
- Je tiefer man geht: Risse in der Realität, Datenschicht wird sichtbar

**Struktur:**
```
[EINGANG]
    |
[ZONE 1] — Die Äußeren Mauern
    |
[ZONE 2] — Die Toranlage (Mini-Boss)
    |
[ZONE 3] — Das Innere Netz
    |
[ZONE 4] — Der Kern (Boss)
    |
[GEHEIMRAUM] — Optional, versteckt
```

---

#### Zone 1 — Die Äußeren Mauern
**Stimmung:** Verlassen, ruhig, erste Warnsignale
- Gegner: **Fragmented Sentinels** — korrumpierte Brotherhood-Wächter als Zombie-Prozesse
- Tutorial: Grundbewegung, Leichtangriff, Dodge, erste Data-Absorption (Common Fragment)
- Lyra liest aus ihrem Lehrbuch — der Protagonist sieht dass die Ruine nicht "sicher gesperrt" ist
- Environmental Storytelling: Skelette ohne Kampfspuren — einfach gestoppt. Process Terminated.

#### Zone 2 — Die Toranlage
**Stimmung:** Enger, bedrohlicher, erste echte Gefahr
- Gegner: **Corrupted Firewall Nodes** — stationäre Konstrukte, feuern Projektile (= Packet Filtering)
- Tutorial: Skill-Slots, Ranged-Dodge
- **Mini-Boss: Overseer Unit 7** — Brotherhood-Automat, kämpft mechanisch ohne Bewusstsein
  - Belohnung: Rare Fragment → erster aktiver Skill
  - Lyra erkennt Brotherhood-Emblem, sagt nichts, schreibt in ihr Buch
- Hinweis an der Wand: Liste von Namen — Beschworene mit Datum und Vermerk *"Process Terminated — Order of the Root"*

#### Zone 3 — Das Innere Netz
**Stimmung:** Surreal, Realität bricht auf, Datenschicht sichtbar
- Visuell: Mauern werden transparent — dahinter schwebende Hexadezimalwerte, der Code der Welt
- Gegner: **Data Wraiths** — Ghost Processes, können kurz unsichtbar werden
- Tutorial: Synergy-Attack mit Lyra
- Puzzle: Drei Firewall-Terminals in korrekter Reihenfolge deaktivieren — falsche Reihenfolge spawnt Gegner
  - Programmierer/Hacker-Background: Dialog-Option löst Puzzle sofort (*"Du erkennst das Muster — ein einfacher Stack-Overflow-Exploit"*)
- Lyra findet Akademie-Siegel an einem Terminal. Die Akademie war hier. Vor kurzem.

#### Zone 4 — Der Kern
**Stimmung:** Dunkel, pulsierend, das Herz der alten Firewall
- Raum: Riesiges kreisförmiges Gewölbe, zerstörter Monolith in der Mitte — pulsiert noch

**Boss: BREACH-INSTANCE ALPHA**
*Ein Corrupted Process der sich selbst repliziert hat — entstand als die Firewall kollabierte und ein fehlerhafter Prozess in einer Schleife steckte.*

Visuell: Menschliche Silhouette aus schwarzen und roten Datenfragmenten, Glitch-Trails bei Bewegung

| Phase | HP | Verhalten |
|---|---|---|
| **Phase 1** | 100% → 60% | Grundangriffe, gelegentliches Klonen (2 kurze Kopien) |
| **Phase 2** | 60% → 30% | 3 dauerhafte Klone — Spieler muss echte Instanz identifizieren (leicht andere Datenmuster) |
| **Phase 3** | 30% → 0% | Alle Klone verschmelzen, wird schneller, AoE-Datenstrom-Angriff |

Nach dem Sieg: Legendary Fragment → *"Echo Strike"* (kurze Klonattacke). Monolith zeigt kurz eine vollständige Karte von DataWorld inkl. Nullheim — dann erlischt er.

#### Geheimraum — "The Dead Letter"
*Erreichbar nur wenn alle 3 Firewall-Terminals in Zone 3 korrekt deaktiviert wurden*

- Versteckt hinter dem Monolith
- Inhalt: Datenkristall mit Aufzeichnung eines unbekannten früheren Beschworenen
- Aufzeichnung: *"...ich weiß nicht wie lange ich noch... der Fluch hält. Ich kann nicht zurück. Ich kann nicht kämpfen. Wenn jemand das findet — The Root ist echt. Valdris lügt. Sucht Kael. Er weiß—"* → bricht ab
- Kein Name. Kein Datum. Nur die Stimme.
- Lyra hört es auch. Sie sagt lange nichts.

**Belohnungen:**
| Quelle | Belohnung |
|---|---|
| Overseer Unit 7 | Rare Fragment → Aktiver Skill #1 |
| BREACH-INSTANCE ALPHA | Legendary Fragment → Echo Strike |
| Geheimraum | Datenkristall (Story-Item) + Bonus-XP |
| Alle Gegner | Common Fragments, Gold |

---

## 14. Offene Fragen / TODO

- [ ] Die Certificate Authority ausarbeiten — wer/was steckt dahinter?
- [ ] Art Style Reference sammeln (visuell näher zu .hack oder SAO?)
- [ ] Soundtrack-Richtung — Referenz-Samples vorlegen wenn soweit
- [ ] Malware-Ursprung klären — wer hat die Korruption eingeschleust?
- [ ] Story weiter schreiben → Companions + Dungeons entstehen organisch daraus
- [ ] Kampfsystem detaillieren (Combo-Trees, Absorption-Animationen)
- [ ] Charaktererstellung (UI/UX Konzept)

---

*Dieses Dokument wird laufend aktualisiert.*
