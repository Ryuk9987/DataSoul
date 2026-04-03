# PROJECT_CONTEXT — DataSoul

**Single Source of Truth** für Architektur, Systeme und Konventionen.
Vor jeder Änderung zuerst hier lesen, dann README.md.

---

## Always-read order

1. PROJECT_CONTEXT.md
2. README.md
3. ROADMAP.md
4. Relevante ctx_*.md Dateien

---

## Tech-Stack

- **Godot 4.6** mit **.NET/C#**
- **Forward Plus** Renderer
- Startszene: `res://scenes/ui/MainMenu.tscn` ← **Hauptmenü (neu)**
- Game Loop Entry: `res://scenes/world/FirewallRuins/FirewallRuins.tscn`

---

## Kern-Systeme

### AudioManager
- **Node** in `FirewallRuins.tscn` (kein Autoload, aber `ProcessMode = Always`)
- Singleton: `AudioManager.Instance`
- Lädt beim Start alle SFX aus `assets/audio/sfx/` vor
- API: `PlaySfx(name, position)` für 3D, `PlaySfx2D(name)` für UI/global
- Ambient (`ambient_ruins.wav`) startet automatisch mit 0.5s Delay als Loop

### PlayerCombat
- Kombo-System: Light1 → Light2 → Light3, Heavy, LightLightHeavy, HeavyHeavy
- Dodge-Cancel in frühen Kombo-Stufen erlaubt
- SFX: `hit_light`/`hit_heavy` bei Treffer, `skill_activate` bei Skill 1

### PlayerController
- SFX: `dodge` oder `dodge_counter` (wenn HitIncoming-Window aktiv) beim Dodge

### PlayerStats
- SFX: `player_hurt` bei Treffer, `player_death` bei Tod

### DataAbsorption
- Startet nach jedem Kill via `PlayerCombat.OnEnemyKilled(enemy)`
- Spawnt `DataFragmentParticles.tscn` an Gegner-Position → auto-destroy
- SFX: `absorption`

### GlitchController
- Node auf jedem Gegner-Root
- Lädt `GlitchShader.gdshader` und setzt `MaterialOverride` auf erstem MeshInstance3D
- `NormalIntensity` per Export konfigurierbar (0.15–0.3 je Gegner)
- Automatisch `overload_mode = true` unter 25% HP
- Bei `IsBoss = true`: Phase-basierte Intensität (1: 0.2, 2: 0.5, 3: 0.8)

---

## Gegner-Übersicht

| Szene | Typ | GlitchIntensity | Besonderheit |
|-------|-----|-----------------|--------------|
| FragmentedSentinel.tscn | Melee Patrol | 0.20 | Zone 1 |
| FirewallNode.tscn | Stationär Ranged | 0.15 | Zone 2, Projektile |
| OverseerUnit7.tscn | Mini-Boss | 0.25 (IsBoss=true) | Schild-Mechanic |
| BreachInstanceAlpha.tscn | Boss | 0.20→0.50→0.80 (IsBoss=true) | 3 Phasen, Klone |

---

## Scene-Hierarchie (FirewallRuins)

```
FirewallRuins (Node3D)
  ├── WorldEnvironment
  ├── NavigationRegion3D
  ├── [Walls & Floors]
  ├── Zone1–4 (instanced)
  ├── SecretRoom (instanced)
  ├── Player (instanced)
  ├── Lyra (instanced)
  ├── HUD (instanced)
  ├── GameManager
  ├── ZoneManager
  ├── PauseMenu
  ├── DialogueSystem
  ├── LoyaltySystem
  ├── SynergySystem
  ├── FragmentMenu
  ├── CorruptionSystem
  └── AudioManager   ← SFX + Ambient
```

---

## Coding-Regeln

- `EnemyBase.IsDead` immer vor Aktionen prüfen
- `IsInstanceValid(node)` bei verzögerten Timer-Callbacks
- GlitchController per `FindMesh()` rekursiv → funktioniert auch bei tief geschachtelten Meshes
- `DataFragmentParticles.tscn` ist `one_shot = true` → `Emitting = true` genügt
- Ambient loop via `AudioStreamWav.LoopMode = Forward`
- Kein PR nötig: direkt auf master pushen

---

## Aldenmere Szenen-Hierarchie (Phase 5.2)

```
Aldenmere.tscn  (AldenmereZone.cs)
  ├── LevelGeometry (aldenmere_main_square.glb)
  ├── Props (aldenmere_props.glb)
  ├── Collision/ (StaticBody3D Boden + 4 Wände)
  ├── Triggers/
  │   ├── AkademieTrigger (Area3D → AkademieInnen.tscn)
  │   └── SuedausgangTrigger (Area3D → FirewallRuins/Zone1.tscn)
  ├── SpawnPoint (Node3D, Zentrum)
  ├── NPCs/ (4× CharacterBody3D — Bürger, Händler, Wache)
  ├── Lighting/ (DirectionalLight3D + OmniLight3D ambient)
  └── AmbientMusic (AudioStreamPlayer, loop)

AkademieInnen.tscn  (AkademieInnenZone.cs)
  ├── LevelGeometry (akademie_interior_hall.glb)
  ├── Collision/ (StaticBody3D Boden + 4 Wände)
  ├── Triggers/
  │   ├── KellerTrigger (Area3D → Beschwoerungsraum.tscn)
  │   └── AusgangTrigger (Area3D → Aldenmere.tscn)
  ├── SpawnPoint (Node3D, Eingang)
  ├── Lighting/ (blaue Deckenleuchten + Wandfackeln)
  └── AmbientMusic (AudioStreamPlayer, loop)

Beschwoerungsraum.tscn  (BeschwoerungsraumZone.cs)
  ├── LevelGeometry (beschwoerungsraum.glb)
  ├── Collision/ (StaticBody3D Boden + 4 Wände)
  ├── Particles/ (5× GpuParticles3D, blau/golden, loopend)
  ├── SummoningFocus (Node3D + OmniLight3D) ← Cutscene-Hook
  ├── Triggers/
  │   ├── FocusTrigger (Area3D → Signal SummoningTriggered)
  │   └── AusgangTrigger (Area3D → AkademieInnen.tscn)
  ├── SpawnPoint (Node3D, Eingang)
  ├── Lighting/ (RuneGlow blau/lila)
  └── AmbientMusic (AudioStreamPlayer, loop)
```

### Szenen-Übergänge (alle via GetTree().ChangeSceneToFile())
- Aldenmere → AkademieInnen: `F` in AkademieTrigger-Zone
- Aldenmere → Zone1 (FirewallRuins): `F` in SuedausgangTrigger-Zone
- AkademieInnen → Beschwoerungsraum: `F` in KellerTrigger-Zone
- AkademieInnen ← Beschwoerungsraum: `F` in AusgangTrigger-Zone
- Beschwoerungsraum: `F` bei SummoningFocus → Signal `SummoningTriggered` (Phase-5.3-Hook)

---

## Stand (2026-04-03)

- Phase 1–5.2 vollständig ✅
- Phase 5.2 Aldenmere Szenen ✅
  - `Aldenmere.tscn`, `AkademieInnen.tscn`, `Beschwoerungsraum.tscn` angelegt
  - 3 C#-Scripts: AldenmereZone, AkademieInnenZone, BeschwoerungsraumZone
  - 5× GpuParticles3D im Beschwörungsraum (blau + golden)
  - SummoningFocus mit Signal `SummoningTriggered` → bereit für Intro-Cutscene (5.3)
- Phase 3 vollständig integriert ✅
  - SFX: alle 8 Dateien über AudioManager eingebunden
  - GlitchShader: auf allen 4 Gegnern via GlitchController.cs
  - DataFragmentParticles: spawnen bei Absorption-Event
  - Ambient: ambient_ruins.wav als Loop in AudioManager
