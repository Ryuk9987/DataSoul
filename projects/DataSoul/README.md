# DataSoul

**Anime-Dungeon-RPG** (Godot 4 + C#) — Art Style: düster, Anime, Mittelalter-Setting, Referenz: .hack

Ein Spieler landet in einer digitalen Welt (DataWorld) und kämpft sich durch den Firewall-Dungeon, während er Data-Fragmente von besiegten Gegnern absorbiert und so neue Skills erlernt.

---

## Vision & Design-Pfeiler

1. **Dark Anime Aesthetic**: Mittelalter-Setting, neon-glitch Effekte, .hack-Referenz
2. **Data-Absorption**: Gegner töten → Skills absorbieren → Charakter wächst
3. **Combo-Kampfsystem**: Leicht/Schwer-Ketten, Dodge mit I-Frames, Dodge-Counter
4. **Phasen-Bosse**: BreachInstanceAlpha mit 3 Phasen und Klon-Mechanic
5. **Companion-System**: Lyra als KI-Begleiterin mit Loyalitätssystem

---

## Quickstart (Dev)

1. Projekt in **Godot 4.6 (mit .NET/C# Support)** öffnen → `game/` Ordner
2. Startszene: `res://scenes/ui/CharacterCreation.tscn`
3. Direkt ins Gameplay: `res://scenes/world/FirewallRuins/FirewallRuins.tscn`

---

## Controls

- **WASD**: Bewegung
- **Linksklick**: Leichter Angriff
- **Rechtsklick**: Schwerer Angriff
- **Space**: Dodge (mit I-Frames)
- **1/2/3/4**: Skill-Slots
- **Q**: Synergy-Attack (mit Lyra)
- **E**: Companion-Befehlsmenü
- **Tab**: Lock-On Toggle
- **I**: Fragment-Menü
- **F**: Interact

---

## Projektstruktur

```
game/
  assets/
    audio/
      sfx/          ← Kampf-SFX (hit, dodge, skill, etc.)
      music/        ← Ambient (ambient_ruins.wav)
    models/
      enemies/      ← GLB-Modelle: Sentinel, FirewallNode, Overseer, Boss
      player/       ← player_capsule.glb, kaykit_knight.glb
      companions/   ← lyra.glb
      vfx/          ← absorption_orb.glb
      world/
        aldenmere/  ← aldenmere_main_square, akademie_exterior/interior, beschwoerungsraum, props
    shaders/
      GlitchShader.gdshader   ← Glitch-Effekt auf Gegnern
      toon_character.gdshader
      toon_outline.gdshader
    textures/ui/hud/ ← HUD-Elemente (17 PNGs)
  scenes/
    enemies/        ← FragmentedSentinel, FirewallNode, OverseerUnit7, BreachInstanceAlpha
    player/         ← Player.tscn
    companions/     ← Lyra.tscn
    vfx/            ← DataFragmentParticles.tscn
    world/FirewallRuins/ ← Zone1–4, SecretRoom, FirewallRuins.tscn
    world/aldenmere/ ← Aldenmere.tscn, AkademieInnen.tscn, Beschwoerungsraum.tscn
    ui/             ← CharacterCreation, HUD, PauseMenu, FragmentMenu
  scripts/
    player/         ← PlayerController, PlayerCombat, PlayerStats, DataAbsorption
    enemies/        ← EnemyBase, EnemyAI, BossBase, OverseerUnit7, BreachInstanceAlpha, GlitchController
    systems/        ← AudioManager, GameManager, FragmentSystem, SkillSystem, DataGauge
    companions/     ← CompanionBase, CompanionAI, LoyaltySystem, LyraDialogue
    ui/             ← HUDController, DialogueSystem, etc.
    world/          ← DungeonZone, FirewallTerminal, DataCrystal, etc.
    world/aldenmere/ ← AldenmereZone, AkademieInnenZone, BeschwoerungsraumZone
```

---

## Audio

- **AudioManager** (Node in FirewallRuins.tscn): Lädt alle SFX beim Start, spielt Ambient automatisch
- SFX via `AudioManager.Instance?.PlaySfx("name", position)` (3D) oder `PlaySfx2D("name")` (UI/global)
- Ambient `ambient_ruins.wav` startet automatisch als Loop (Firewall Ruins)
- Ambient `ambient_aldenmere.wav` läuft als Loop in allen Aldenmere-Szenen (AudioStreamPlayer, loop_mode=1)

## Glitch-Shader

- **GlitchController.cs** als Node auf jedem Gegner
- `NormalIntensity`: 0.15–0.25 je nach Gegnertyp
- Unter 25% HP → `overload_mode = true` (rotes Flackern)
- Boss (BreachInstanceAlpha): Phase 1 = 0.2, Phase 2 = 0.5, Phase 3 = 0.8

## Data-Absorption & Partikel

- Nach Kill → `DataAbsorption.StartAbsorption(enemy)`
- Spawnt `DataFragmentParticles.tscn` an Gegner-Position (auto-destroy nach ~1.1s)
- Absorption-Sound + HUD-Popup

---

## GitHub

https://github.com/Ryuk9987/DataSoul

**Workflow:** Direkt auf master — kein PR nötig
