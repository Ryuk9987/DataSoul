# DataSoul — Roadmap & Subagent-Übersicht
**Stand:** 2026-04-03 | **Phase:** Prototyping aktiv

---

## Legende
- [ ] Offen
- [~] In Arbeit
- [x] Erledigt
- ⛔ Blockiert (Abhängigkeit)

**Zuständige Agenten:**
- `csharp-godot` — Godot C# Implementierung
- `nano-banana-artist` — 3D Assets, UI, VFX, Shader
- `clickadventure-writer` — Story, Dialoge, Lore

---

## Phase 1 — Projekt-Setup ✅
- [x] GDD v1.1 abgeschlossen
- [x] Kontext-Dateien erstellt (ctx_*.md)
- [x] Roadmap erstellt
- [x] Godot-Projekt anlegen → `csharp-godot`
- [x] Ordnerstruktur anlegen → `csharp-godot`

---

## Phase 2 — Prototype Kernmodule ✅

### 2.1 Spieler & Bewegung
- [x] `PlayerController.cs` — WASD, Sprung, Dodge mit I-Frames
- [x] `PlayerCamera.cs` — Third-Person, Lock-On, Zoom, Kollision
- [x] `PlayerStats.cs` — HP, ATK, DEF, SPD, DATA-CAP, DATA-GAUGE
- [x] Background-Auswahl (Prototyp: hardcoded)

### 2.2 Kampfsystem
- [x] `PlayerCombat.cs` — Leicht/Schwer-Kette, Dodge-Cancel
- [x] `DataGauge.cs` — Aufladung durch Treffer/Kill/Dodge-Counter
- [x] Start-Skill per Taste 1

### 2.3 Gegner-KI
- [x] `EnemyBase.cs` + `EnemyAI.cs` — Patrol, Aggro, Chase, Angriff
- [x] `FragmentedSentinel.tscn` — Melee Patrol (Zone 1)
- [x] `FirewallNode.tscn` — Stationär Ranged (Zone 2)

### 2.4 Data-Absorption & Fragment-System
- [x] `DataAbsorption.cs` — nach Kill, kein RNG
- [x] `FragmentSystem.cs` — Skill-Datenbank, Lv1–10
- [x] `SkillSystem.cs` — Aktiv/Passiv Slots (4+4)

### 2.5 Companion-KI (Lyra)
- [x] `CompanionBase.cs` + `CompanionAI.cs` — Folgen, Abstand, Rückzug
- [x] Lyra Start-Skills: Heilung + Schutzschild + Revive
- [x] Befehlsmenü (Angreifen / Heilen / Rückzug / Fokus-Target)

### 2.6 Boss-System
- [x] `BossBase.cs` — Phasen-System (HP-Schwellen)
- [x] `BreachInstanceAlpha.tscn` — 3 Phasen, Klon-Mechanic

### 2.7 HUD & UI
- [x] `HUDController.cs` — HP-Bar, Data-Gauge, Skill-Slots 1–4
- [x] Absorption-Popup
- [x] Overload Warning (Flackern bei HP<20%)

---

## Phase 3 — Prototype Assets ✅

### 3.1 Kritische Assets ✅
- [x] Spieler-Kapsel mit Waffe → `assets/models/player/player_capsule.glb`
- [x] Fragmented Sentinel → `assets/models/enemies/fragmented_sentinel.glb`
- [x] Corrupted Firewall Node → `assets/models/enemies/corrupted_firewall_node.glb`
- [x] Firewall Ruins Zone 1–2 Level-Blockout → `zone1_outer_ruins.glb`, `zone2_corridor_boss.glb`
- [x] HUD-Elemente → `assets/textures/ui/hud/` (17 PNGs)

### 3.2 Hohe Priorität ✅
- [x] Lyra (3D Mesh) → `assets/models/companions/lyra.glb`
- [x] Overseer Unit 7 (3D Mesh) → `assets/models/enemies/overseer_unit7.glb`
- [x] BREACH-INSTANCE ALPHA (3D Mesh) → `assets/models/enemies/breach_instance_alpha.glb`
- [x] Absorption VFX Orb → `assets/models/vfx/absorption_orb.glb`
- [x] Kampf-SFX (Treffer, Dodge, Skill-Sounds) → AudioManager.cs integriert

### 3.3 Mittlere Priorität ✅
- [x] Glitch-Shader → GlitchController.cs auf allen 4 Gegnern (Sentinel 0.20, FirewallNode 0.15, Overseer 0.25, Boss Phase 0.2/0.5/0.8)
- [x] Datenfragment-Partikel → DataAbsorption.cs: spawnt DataFragmentParticles.tscn bei Kill
- [x] Ambient-Sound → AudioManager.cs: ambient_ruins.wav als Loop in FirewallRuins

---

## Phase 4 — Prototype-Dungeon (Firewall Ruins) ✅
- [x] Zone 1 — Äußere Mauern (Sentinels, Tutorial-Flow)
- [x] Zone 2 — Toranlage + Overseer Unit 7 (Mini-Boss)
- [x] Zone 3 — Inneres Netz (Data Wraiths, Terminal-Puzzle)
- [x] Zone 4 — Boss-Raum BREACH-INSTANCE ALPHA
- [x] Geheimraum + Datenkristall (Story-Item)
- [x] Hacker/Programmierer-Dialog-Option für Terminal-Puzzle

---

## Phase 5 — Aldenmere (Startstadt) + Intro-Sequenz

> **Hinweis:** Firewall Ruins = Prototyp-Dungeon (Mechanik-Test). Echter Spielstart ist Aldenmere.
> Reihenfolge: Assets → Szene → Intro-Dialoge/Cutscene

### 5.1 Aldenmere Assets → `nano-banana-artist` ✅
- [x] Stadt-Blockout / Hauptplatz GLB → `aldenmere_main_square.glb`
- [x] Akademie-Gebäude GLB (Außen + Innen) → `akademie_exterior.glb`, `akademie_interior_hall.glb`
- [x] Beschwörungsraum GLB (im Akademie-Keller) → `beschwoerungsraum.glb`
- [x] Atmosphärische Props (Brunnen, Marktstände, Laternen, Fahnen) → `aldenmere_props.glb`
- [x] Ambient-Sound Aldenmere (ruhige Stadtgeräusche) → `ambient_aldenmere.wav`

### 5.2 Aldenmere Szene → `csharp-godot` ✅
- [x] `Aldenmere.tscn` aufbauen (Hauptplatz, Akademie-Eingang, Props, 4 NPCs)
- [x] `AkademieInnen.tscn` + `Beschwoerungsraum.tscn`
- [x] NPCs platzieren (Wachen, Bürger, Händler — 4× CharacterBody3D)
- [x] Trigger-Zonen: Akademie-Eingang, Südausgang → FirewallRuins, Keller, SummoningFocus
- [x] Übergang: Beschwörungsraum → Signal `SummoningTriggered` (Cutscene-Hook)
- [x] Übergang: Aldenmere → Firewall Ruins (Südausgang → Zone1.tscn)
- [x] Ambient-Musik (loopend) in allen 3 Szenen
- [x] GpuParticles3D an 5 Emitter-Stellen (blau/golden)
- [x] `SummoningFocus` Node3D als Cutscene-Hook bereit

### 5.3 Intro-Sequenz → `clickadventure-writer` + `csharp-godot` ✅
- [x] Beschwörungsritual Cutscene-Text + Regie-Anweisungen
- [x] Ankunft in DataWorld — erster Dialog (NARRATOR-Sequenz)
- [x] Lyra Erstkontakt-Dialog (inkl. 5 Background-Varianten)
- [x] Erster Auftrag — Briefing mit Magister Aldric (inkl. Suspense-Subtext)
- [x] Lyra-Kommentar beim Verlassen der Akademie
- [x] Regie-Anweisungen: Kamera, Glitch-Events, Szenen-Übergänge
- [ ] Cutscene in Godot implementieren (csharp-godot)

### Story & Dialoge (bereits erledigt)
- [x] Lyra Intro-Dialog
- [x] Zone 1–4 Environmental Storytelling
- [x] Datenkristall-Aufzeichnung (Geheimraum)
- [x] Post-Boss-Szene (Monolith + Karte)

---

## Phase 6 — Erweiterte Systeme
- [x] Charaktererstellung UI (Background, Name) → `CharacterCreation.tscn`
- [ ] Companion Story-System + Loyalitätspunkte → `csharp-godot`
- [ ] Synergy-Attack System → `csharp-godot`
- [ ] Data-Node Speicher/FastTravel → `csharp-godot`
- [ ] Fragment-Upgrade System (Fusion-Skills) → `csharp-godot`
- [ ] Corruption Overload-Mechanik → `csharp-godot`
- [ ] Korrupte Zonen (visuell + gameplay) → `csharp-godot` + `nano-banana-artist`

---

## Offene Design-Fragen
- [ ] Certificate Authority weiter ausarbeiten
- [ ] Malware-Ursprung klären
- [x] Art Style Reference festlegen → **Anime, düster, Mittelalter-Setting, Referenz: .hack**
- [ ] Soundtrack-Richtung definieren
- [ ] Weitere Companions planen (Companion #2, #3)
- [ ] Dungeon 2+ grob skizzieren

---

## Apollo-Notizen

- **GitHub:** https://github.com/Ryuk9987/DataSoul
- **Workflow:** Direkt auf master — kein PR nötig (Erickos greift direkt auf Server zu)
- **nano-banana-artist:** Nur Anthropic (Claude) nutzen — ChatGPT OAuth Token expired
- **GLB Kollision:** Mesh-Name muss `-col` Suffix haben damit Godot Kollision generiert
- **StaticBody ohne MeshInstance:** Kollidiert nicht zuverlässig → immer MeshInstance3D dazu

### Zuletzt erledigt (2026-04-03)
- **Phase 5.2 Aldenmere Szenen ✅**
  - `Aldenmere.tscn`: Hauptplatz, Props, 4 NPCs, Ambient-Musik, Trigger Akademie + Südausgang
  - `AkademieInnen.tscn`: Innenhalle, Kollisionen, Trigger Keller + Ausgang
  - `Beschwoerungsraum.tscn`: 5× GpuParticles3D (blau/golden), SummoningFocus Node3D, Signal `SummoningTriggered`, Trigger Ausgang
  - Szenen-Wechsel via `GetTree().ChangeSceneToFile()` für alle Übergänge
  - C#-Scripts: `AldenmereZone.cs`, `AkademieInnenZone.cs`, `BeschwoerungsraumZone.cs`
- **Phase 3 vollständig integriert ✅**
  - Kampf-SFX: alle 8 Dateien (hit_light/heavy, dodge, dodge_counter, skill_activate, player_hurt, player_death, absorption) via AudioManager in C#-Code eingebunden
  - GlitchShader: neuer `GlitchController.cs` als Node auf allen 4 Gegner-Szenen — dynamische Intensität, overload_mode bei <25% HP, Boss-Phasen
  - DataFragmentParticles: spawnt bei jedem Kill/Absorption-Event via DataAbsorption.cs
  - Ambient: `ambient_ruins.wav` als automatischer Loop im AudioManager
- README.md und PROJECT_CONTEXT.md für DataSoul erstellt

### Zuletzt erledigt (2026-04-03 — Phase 5.3)
- **Intro-Sequenz Dialoge ✅**
  - `intro_sequence.json`: 8 Dialogue-Keys, 40+ Einträge, vollständig Godot-kompatibel
    - `ritual_cutscene`: 11 Einträge (Beschwörung, Portal-Öffnung, Ankunft, Glitch-Momente)
    - `lyra_first_contact`: 7 Einträge (Erstkontakt, Persönlichkeit, Zuteilung)
    - 5× Background-Varianten: programmer / gamer / hacker / creator / analyst
    - `first_quest_briefing`: 11 Einträge (Aldric, Bedrohung, suspense-Subtext, ausweichende Antworten)
    - `leaving_akademie`: 3 Einträge (Lyra murmelt Lehrbuch, Nervosität)
  - `intro_sequence_regie.md`: vollständige Regie-Anweisungen
    - Kamera-Bewegungen pro Dialog-Eintrag
    - 6 Glitch-Events + Idle-Glitch (GlitchController-Parameter)
    - Emotion-Tag → Animation-Mapping
    - Technische Hinweise für csharp-godot (Background-Mapping, Dialogue-Keys, Kamera-System)

### Nächste offene Punkte (Priorität)
1. **Phase 5.3 Teil 2:** Cutscene in Godot implementieren → `csharp-godot` (CutsceneCamera.cs, AnimationPlayer, Signal-Hooks)
2. **Phase 6:** Companion Story, Synergy-Attack, Data-Node FastTravel → `csharp-godot`
