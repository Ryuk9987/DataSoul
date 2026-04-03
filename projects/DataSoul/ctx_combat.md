# DataSoul — Combat & Skills (Subagent Context)
→ Für Übersicht: `ctx_overview.md` | Für Tech: `ctx_tech.md`

---

## Kampfsystem — Echtzeit Action
- **Freie 3D-Bewegung** während des Kampfes
- **Grundaktionen:** Leichtangriff, Schwereangriff, Dodge (I-Frames: 0.3s), Block
- **Skill-Slots:** 4 aktive Skills belegbar (erweiterbar bis 8)
- **Data-Gauge:** Ressource 0–100, füllt sich durch Kampf → ermöglicht spezielle Moves
- **Lock-On System** (optional, Tab / R3)

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

- **Dodge-Cancel:** Jede Aktion außer Hit 3 abbrechbar
- **Dodge-Counter** (Dodge kurz vor Treffer): +25 Data-Gauge

### Data-Gauge Aufladung
| Aktion | Gauge-Gain |
|---|---|
| Leichttreffer | +5 |
| Schwertreffer | +15 |
| Dodge-Counter | +25 |
| Kill | +30 |
| Max | 100 |

---

## Data-Absorption System

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

**Wichtig:** Spieler absorbiert **immer** die Daten — kein RNG, kein Drop-Limit.

### Seltenheitsstufen
| Stufe | Quelle | Effekt |
|---|---|---|
| **Common** | Normalgegner | Stat-Boosts, einfache Passivskills |
| **Rare** | Elite-Gegner | Neue aktive Skills |
| **Legendary** | Bosse | Einzigartige Signature Moves |
| **Corrupted** | Korrupte Gegner | Mächtig, aber instabil (zufällige Debuffs möglich) |

### Skill-Progression
- Lv1 bis Lv10, Level-Up durch wiederholte Absorption desselben Typs
- Kompatible Skills auf Lv10 → **Fusion-Skill** freischaltbar
- Kombinierte Skills haben höhere Seltenheit als Quell-Skills
- Tiefe Kombinations-Ketten möglich (endgame Build-Vielfalt)

**Beispiel:**
```
Feuerresistenz Lv10 + Kälteresistenz Lv10
    → Wärmeschwankungsresistenz Lv1 (wieder bis Lv10 steigerbar)
        → neue Kombinationen möglich
```

### Corruption Overload (Overflow-Mechanik)
| Aktive Corrupted Skills | Effekt |
|---|---|
| 1–2 | Kein Effekt |
| 3 | Leichte Instabilität — gelegentliche Skill-Fehlzündungen |
| 4+ | **Corruption Overload** — zufällige Fehlzündungen, Selbstschaden, Glitch-Effekte |

---

## Start-Skills (nach Background)

### 🧑‍💻 Code-Injection (Programmierer)
**Typ:** Aktiv | **Cooldown:** 12s | **Gauge:** 0
Injiziert Schadcode — DoT + DEF-Reduktion für 6s.
| Lv1 | DoT 3/s | DEF -2 |
| Lv10 | DoT 21/s | DEF -11 |
Skalierung: +2 DoT/s, +1 DEF-Red. pro Level

---

### ⚔️ Combo-Rush (Gamer)
**Typ:** Aktiv | **Cooldown:** 10s | **Gauge:** 0
5-Hit-Combo, nicht unterbrechbar. Jeder Hit stärker als der vorherige.
| | Hit 1–3 | Hit 4 | Hit 5 (Finisher) |
|---|---|---|---|
| Lv1 | 0.8x ATK je | 1.2x | 2.0x + Stagger |
| Lv10 | 0.8x ATK je | 1.2x | 4.7x + Stagger |
Skalierung: Finisher +0.3x ATK pro Level

---

### 🧠 System-Exploit (Hacker)
**Typ:** Aktiv | **Cooldown:** 15s | **Gauge:** 0
Scannt Gegner → nächster Angriff im Zeitfenster: garantierter Krit + ignoriert DEF.
| Lv1 | 2.5x ATK | Fenster 5s |
| Lv10 | 5.2x ATK | Fenster 8s |
Skalierung: +0.3x ATK, +0.33s Fenster pro Level

---

### 🎨 Illusion-Field (Creator)
**Typ:** Aktiv | **Cooldown:** 20s | **Gauge:** 0
Erstellt Illusionskopien — Gegner wechseln zufällig Ziel.
| Lv1 | 2 Illusionen | 8s | 40% Aggro je | 1 Treffer/Illusion |
| Lv10 | 3 Illusionen | 17s | 90% Aggro je | 2 Treffer/Illusion |
Skalierung: +1s Dauer, +5% Aggro pro Level; Lv10 +1 Illusion

---

### 📚 Weakness-Scan (Analyst)
**Typ:** Aktiv + Passiv | **Cooldown:** 8s | **Gauge:** 0
Scannt alle Gegner im Radius — HP, Schwachstellen, Buffs/Debuffs. Bonus-Schaden auf Schwachstellen.
| Lv1 | 8m Radius | +25% Bonus | Passiv: Gegner < 25% HP |
| Lv10 | 26m Radius | +70% Bonus | Passiv: Gegner < 50% HP |
Skalierung: +2m Radius, +5% Bonus pro Level

---

## Gegner-Specs (Prototype-Set)

### Fragmented Sentinel
*Normal Melee — Zone 1 Firewall Ruins*
| HP | ATK | DEF | SPD |
|---|---|---|---|
| 30 | 5 | 2 | 4 |
AI: Patrol (3-Punkt) → Aggro bei 6m → Chase → Angriff bei ≤1.5m (1s CD) → kein Rückzug
Absorption: Common — Feuerresistenz / Physische Stärke

### Corrupted Firewall Node
*Normal Ranged stationär — Zone 2*
| HP | ATK | DEF | SPD |
|---|---|---|---|
| 25 | 8 | 5 | 0 |
AI: Dreht sich zum Spieler (10m) → Projektil alle 2s (12m Reichweite) → Schwachstelle: Rücken (2x Schaden)
Absorption: Common — Elektroresistenz / Projektil-Reflex

### Overseer Unit 7
*Mini-Boss — Zone 2*
| HP | ATK | DEF | SPD |
|---|---|---|---|
| 150 | 12 | 8 | 3 |
- Phase 1 (100%→50%): Melee-Combo (3 Hits), Projektil-Salve (3 Fächer)
- Phase 2 (50%→0%): Schild aktiv (+5 DEF, brechbar mit 3x Schwer), schnellere Angriffe
Absorption: Rare — *"System Scan"* (aktiv: zeigt Gegner-HP + Schwachstellen)

### Data Wraith
*Normal Stealth — Zone 3*
| HP | ATK | DEF | SPD |
|---|---|---|---|
| 20 | 10 | 1 | 7 |
AI: Startet unsichtbar → sichtbar bei 3m → Dash-Strike → sofort unsichtbar (2s CD) → Schwachstelle: 0.5s nach Angriff
Absorption: Common — Shadow-Veil (passiv: kurze Unsichtbarkeit nach Dodge)

### BREACH-INSTANCE ALPHA
*Boss — Zone 4*
| HP | ATK | DEF | SPD |
|---|---|---|---|
| 500 | 18 | 6 | 5→8→11 |
- Phase 1 (100%→60%): Melee-Combo, 2 kurze Klone (5s, kein Schaden, visuell), CD 15s
- Phase 2 (60%→30%): 3 permanente Klone, einer real (hellere Datenmuster), alle greifen an (Klone 10% Schaden)
- Phase 3 (30%→0%): Klone verschmelzen, SPD+6 ATK+5, AoE Datenstrom-Welle (2s Warnung, 3s Dauer)
Absorption: Legendary — *"Echo Strike"* (aktiv: Klonattacke, kurze Doppelinstanz greift mit an)
