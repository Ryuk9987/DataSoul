# DataSoul — Technische Spezifikation (Subagent Context)
→ Für Übersicht: `ctx_overview.md` | Engine: Godot (C#)

---

## Kamera

- **Typ:** Third-Person, Over-Shoulder, frei rotierbar (MMORPG-Style)
- **Standard-Distanz:** 5m hinter Spieler, 1.5m über Schulterhöhe
- **Zoom:** Mausrad / Rechter Stick gedrückt — 2m (nah) bis 10m (weit)
- **Lock-On:** Kamera fixiert auf Ziel, dreht automatisch mit, Spieler bleibt frei beweglich
- **Kollision:** Kamera zoomt automatisch rein wenn Wand im Weg
- **FOV:** 75° Standard, anpassbar in Settings (60°–90°)
- **Combat-Shift:** Im Kampf leicht tiefere Perspektive (+0.3m) für dramatischeren Look

---

## Input-Map

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

---

## HUD Layout

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

---

## Godot Projektstruktur (C#)

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

---

## Prototype Asset-Prioritäten

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

---

## Visuelle Effekte pro Skill
- **Code-Injection:** Code-Zeichen auf Gegner → rotes Glitch-Flackern während DoT
- **Combo-Rush:** Spieler leuchtet orange, Hits hinterlassen Nachbilder
- **System-Exploit:** Rotes Hexagon-Muster auf Gegner → Freeze-Frame beim Treffer
- **Illusion-Field:** Illusionen transparent violett, Glitch-Artefakte bei Bewegung
- **Weakness-Scan:** Blauer Scan-Puls → schwebende Daten-Labels über Gegnern
- **Absorption:** Gegner zerfällt in Datenstrom → fließt zum Protagonisten (1.5s, nicht skippable)
- **Corruption Overload:** Rotes Flackern am Bildschirmrand, Charakter-Glitch-Effekte

---

## Prototype-Ziele (Kernmodule)
- [ ] Spielerbewegung (3D, Dodge, Lock-On)
- [ ] Combo-System (Light/Heavy Chain)
- [ ] Gegner-KI (Sentinel + Firewall Node)
- [ ] Data-Absorption + Fragment-System
- [ ] Skill-Slot UI + HUD
- [ ] Companion-KI Grundgerüst (Lyra)
- [ ] Data-Gauge System
- [ ] Boss-Phasen System (BREACH-INSTANCE ALPHA)
