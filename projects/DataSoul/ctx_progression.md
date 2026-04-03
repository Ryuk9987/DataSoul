# DataSoul — Progression & Systeme (Subagent Context)
→ Für Übersicht: `ctx_overview.md` | Für Kampf: `ctx_combat.md`

---

## Protagonist-Basiswerte (Level 1)
| Stat | Wert | Beschreibung |
|---|---|---|
| HP | 100 | Lebenspunkte |
| ATK | 10 | Basisschaden |
| DEF | 5 | Schadensreduktion |
| SPD | 8 | Bewegungs-/Angriffsgeschwindigkeit |
| DATA-CAP | 10 | Max. aktive Fragments gleichzeitig |
| DATA-GAUGE | 0/100 | Spezialressource |

### Background-Boni
| Background | Bonus | Start-Skill |
|---|---|---|
| Programmierer | +3 ATK, +2 SPD | Code-Injection |
| Gamer | +5 ATK | Combo-Rush |
| Hacker | +4 SPD | System-Exploit |
| Creator | +10 HP | Illusion-Field |
| Analyst | +3 DEF | Weakness-Scan |

Background beeinflusst auch Dialog-Optionen (z.B. Hacker/Programmierer können Puzzles durch Exploit lösen).

---

## Leveling

**Max Level:** 100

**Level-Up:** 5 Punkte frei verteilen:
| Stat | Kosten | Effekt pro Punkt |
|---|---|---|
| HP | 1 Punkt | +10 HP |
| ATK | 1 Punkt | +2 ATK |
| DEF | 1 Punkt | +1 DEF |
| SPD | 2 Punkte | +1 SPD |
| DATA-GAUGE | 2 Punkte | +5 Max. Gauge |

### XP-Kurve
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

### XP-Quellen
| Quelle | XP |
|---|---|
| Normal-Gegner | Level × 10 |
| Elite-Gegner | Level × 25 |
| Mini-Boss | Level × 100 |
| Boss | Level × 500 |
| Quest abgeschlossen | 50–2.000 XP (variabel) |
| Geheimraum entdeckt | 200 XP flat |

---

## Datensignatur-Rang

Steigt **unabhängig von XP** durch Skill-Absorption:

| Rang | Voraussetzung | Bonus |
|---|---|---|
| 1 | Start | 4 Aktiv + 4 Passiv Slots |
| 2 | 20 verschiedene Skills | +1 Aktiv Slot |
| 3 | 50 verschiedene Skills | +1 Passiv Slot |
| 4 | 100 verschiedene Skills | +1 Aktiv + 1 Passiv Slot |
| 5 | 5 Skill-Kombinationen | +2 Aktiv Slots |
| 6 | 200 verschiedene Skills | +2 Passiv Slots |
| Max | 20 Skill-Kombinationen | 8 Aktiv + 8 Passiv Slots |

### Aktiv-Slot System
- **Skill-Datenbank:** Unbegrenzt viele Skills sammelbar
- **Aktiv laden:** Begrenzte Slots (Start: 4 Aktiv + 4 Passiv, Max: 8+8)
- **Slot-Wechsel:** Jederzeit an Data-Nodes

---

## Equipment
- Waffen + Rüstung lootbar oder kaufbar
- Optionale **Data-Mod-Slots** an Equipment: eingesetzte Skills geben Ausrüstungs-Boni

---

## Speicherpunkte / Fast Travel
- **Data-Nodes** in der Welt:
  - Speicherpunkt
  - Fast-Travel-Netz
  - Skill-Wechsel-Punkt
  - Respawn-Punkt nach Game-Over

---

## Charaktererstellung — Background-System
Der Spieler wählt einen Background der Start-Datensignatur und Starter-Skills definiert:

| Background | Playstyle |
|---|---|
| 🧑‍💻 Programmierer | Trickster — Code-Injection, Debuff-Stack |
| ⚔️ Gamer | Brawler — Combo-Attacks, Aggro-Boost |
| 🧠 Hacker | Infiltrator — System-Exploit, Stealth |
| 🎨 Creator | Support/Mage — Illusions, Party-Buffs |
| 📚 Analyst | Tactician — Schwächen-Scan, Heilung |

- Name, Aussehen und Geschlecht frei wählbar
- Background beeinflusst Dialog-Optionen

---

## Prototype vs. Spätere Module

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
