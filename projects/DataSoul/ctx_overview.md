# DataSoul — Project Overview (Subagent Context)
**Stand:** 2026-03-31 | **Engine:** Godot (C#) | **Genre:** 3D Action-JRPG / Isekai

## Elevator Pitch
Ein Isekai-JRPG in einer vollständig digitalen Welt (DataWorld). Der Protagonist wird aus der realen Welt in eine Simulation transportiert. Durch das Besiegen von Gegnern absorbiert er deren Datensignaturen → neue Skills. Begleitet von NPC-Companions kämpft er durch eine Welt voller Geheimnisse.

## Kernkonzept
- **Welt:** DataWorld = verteiltes Netzwerk aus 7 Königreichen (Subnetze), Magie = System Calls, Korruption = Malware
- **Protagonist:** Fragmentierte Datensignatur → DRM (Beschwörungsfluch/Root Lock) funktioniert nicht → ermöglicht Data-Absorption
- **Antagonist:** Kael — früherer Beschworener als Ghost Process, will Certificate Authority zerstören
- **Kernfrage:** Darf man eine Welt "löschen", wenn ihre Bewohner echte Gefühle haben?
- **Multiple Endings** (Liberation / Reform / Exile / Deadlock / Ghost)

## Kernmechaniken
1. **Echtzeit-Kampf** — freie 3D-Bewegung, Combo-System, Dodge mit I-Frames
2. **Data-Absorption** — jeder Kill gibt Skill oder upgraded bestehenden Skill (kein RNG)
3. **Skill-System** — Common/Rare/Legendary/Corrupted, Lv1-10, Fusion-Skills bei Lv10-Kombis
4. **Companion System** — autonome KI-Companions (start: Lyra), Synergy-Attacks
5. **Data-Gauge** — Ressource für Spezialangriffe, aufgeladen durch Kampf

## Projektstruktur (alle relevanten Context-Dateien)
| Datei | Inhalt |
|---|---|
| `ctx_overview.md` | **Diese Datei** — Übersicht, Kernkonzept, Navigation |
| `ctx_combat.md` | Kampfsystem, Data-Absorption, Skills, Gegner-Specs |
| `ctx_progression.md` | Leveling, Stat-System, Datensignatur-Rang, Equipment |
| `ctx_story.md` | Story-Akte, Antagonist Kael, Endings, Weltstruktur |
| `ctx_world.md` | Die 7 Königreiche, Fraktionen, Dungeons |
| `ctx_companions.md` | Companion-System, Lyra (Detail), Story-Arcs |
| `ctx_tech.md` | Godot C# Projektstruktur, Kamera, Input-Map, HUD, Asset-Prioritäten |
| `GDD.md` | Vollständiges GDD (für vollständigen Überblick) |

## Status
- **Phase:** Konzept abgeschlossen, Prototyping steht an
- **Nächste Schritte:** Kernmodule implementieren (Spielerbewegung, Kampf, Data-Absorption, Gegner-KI)
