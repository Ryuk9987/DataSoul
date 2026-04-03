# DataSoul — Intro-Sequenz: Regie-Anweisungen
**Datei:** `intro_sequence.json`  
**Zuständig:** csharp-godot  
**Szene:** `Beschwoerungsraum.tscn`

---

## Übersicht: Ablauf

```
[Fade In] → ritual_cutscene → [Spieler-Erwachen] → lyra_first_contact
→ [Background-Check: lyra_first_contact_{background}]
→ first_quest_briefing → leaving_akademie → [Gameplay beginnt]
```

---

## `ritual_cutscene` — Beschwörungsraum

**Vor [0]: Szene startet**
- Kamera: statisch, Vogelperspektive auf den Runenkreis (von oben, ~45°-Winkel)
- Licht: Dunkel. Nur Fackeln. Blau-goldene Ambient-Beleuchtung.
- Audio: `ambient_aldenmere.wav` läuft, leiser Fade-In
- GpuParticles3D an allen 5 Emittern: inaktiv am Start

**[0] Zeremonienmeister spricht ersten Satz**
- Kamera: langsamer Schwenk zu Magister Aldric (Halbtotale, Profil)
- Audio: Stimme hallt im Gewölbe (leichter Hall-Effekt)

**[1] Zeremonienmeister spricht Beschwörungsspruch**
- GpuParticles3D Emitter 1+2: aktivieren (goldene Partikel, Runenkreis)
- Runenkreis-Material: beginnt zu leuchten (Shader-Parameter `glow_intensity` von 0.0 → 0.8, Dauer: 3s)
- Kamera: langsam heranzoomen auf den Runenkreis (Boden)

**[2] NARRATOR: Runenkreis erwacht**
- **Glitch-Event #1:** Kurzes Bildschirmflackern (1–2 Frames), Shader-Effekt auf der gesamten Szene
- Zwischen den Runen: kurz sichtbare Zahlen/Code-Fragmente (Partikel-System mit monospace-Text-Sprites, Alpha sehr kurz, ~0.3s)
- GpuParticles3D alle 5 Emitter: voll aktiv, Intensität hoch
- Kamera: dreht langsam um den Runenkreis (Orbit, 20% Kreis)

**[3] Zeremonienmeister: "Das Portal öffnet sich!"**
- Portal-Mesh aktivieren (SummoningFocus Node3D): Skalierung von 0 → 1, Dauer 1.5s, Easing: Elastic
- Audio: Portal-Öffnungs-SFX (falls vorhanden: `portal_open.wav`)
- Kamera: Zoom zurück — zeigt den gesamten Raum + das Portal

**[4] Akademiewächter spricht**
- **Glitch-Event #2:** Wächter-Textur flackert kurz (GlitchController.cs, Intensität 0.3, Dauer 0.2s)
- Kamera hält Position

**[5] Zeremonienmeister: "Still."**
- Alle NPCs im Raum: kurze Starre-Animation (idle mit leichter Anspannung)
- Audio: Ambient kurz absenken (ducking), dann wieder hoch

**[6] NARRATOR: Portal reißt auf**
- Blendeffekt: weißes Licht aus dem Portal, 1s Fade
- Protagonist-Mesh erscheint mit Physics-Drop aus Portal-Position (~2m Höhe)
- Audio: Aufprall-SFX (`player_hurt.wav` oder dediziertes `landing.wav`)
- Kamera: schneller Cut auf Bodenperspektive (Frog-Eye, zeigt Protagonisten fallen)

**[7] NARRATOR: Flimmern, Rauschen**
- **Glitch-Event #3:** Gesamter Bildschirm — Glitch-Shader, Intensität 0.6, 0.8s, dann schnell abklingen
- Fackeln: kurzes Flackern (Light-Intensität rapid oscillation, 0.5s)
- GpuParticles3D: alle Emitter kurz überladen, dann ausschalten

**[8] NARRATOR: Du öffnest die Augen**
- Kamera: wechselt zu First-Person (Augen des Protagonisten öffnen sich → Iris-Wipe von schwarz)
- Glitch-Overlay: subtil, dauerhaft (niedriger alpha, Shader `glitch_idle_intensity = 0.05`)
- Hint für Spieler: Welt sieht nicht ganz richtig aus (leichte Farbverschiebung, Aberration-Effekt)

**[9] Zeremonienmeister: "Es hat funktioniert."**
- Kamera: Third-Person etablieren (sanfter Übergang von First zu Third)
- Kamera zeigt Protagonisten auf dem Boden, Zeremonienmeister tritt einen Schritt zurück

**[10] NARRATOR: letzter Satz (Zahlen zwischen Runen)**
- **Glitch-Event #4:** Letzte kurze Code-Fragment-Partikel im Runenkreis — nur für Spieler sichtbar (NPCs reagieren nicht)
- Runenkreis: Leuchten verblasst langsam (glow_intensity → 0.2, Dauer 4s)
- Übergang zu: `lyra_first_contact` (kein Fade — direkter Schnitt)

---

## `lyra_first_contact` — Erster Kontakt

**Vor [0]: Setup**
- Kamera: Halbtotale, Third-Person-Standard
- Protagonist steht (aufgestanden während letzter NARRATOR-Text)

**[0] Lyra: erste Zeile**
- Lyra betritt von links (Tür-Eingang)
- Animation: leicht gesenkter Kopf, Lehrbuch vor der Brust gehalten
- Kamera: schwenkt zu Lyra (bleibt auf ihr, ~60% Bildschirm)
- Lyras Stimme: leicht erhöhtes Tempo (Nervosität)

**[1] Lyra: "Laut Protokoll..."**
- Kamera: wechselt zu Über-die-Schulter (hinter Lyra, Protagonist im Bild)
- Lyra: schaut kurz auf ihr Lehrbuch, dann wieder hoch

**[2] Lyra: "Ich bin ausgebildete Heiladeptin."**
- Kamera hält Über-die-Schulter
- Lyra: kleines Auf-und-Ab-Gewippt, unbewusstes Nervöses-Verhalten

**[3] Lyra: "Okay. Lass mich von vorne anfangen."**
- Lyra schließt kurz die Augen, atmet durch
- Kamera: wechselt zur Frontansicht (Lyras Gesicht sichtbar, ¾-Profil)
- Glitch-Overlay: subtil im Hintergrund immer noch sichtbar (0.05 Intensität)

**[4] Lyra: "Das bist jetzt du."**
- Kamera: kurze Reverse-Einstellung (zeigt Protagonisten aus Lyras Perspektive — was sie sieht)
- Protagonist steht in Portal-Licht-Nachleuchtung

**[5] Lyra: "'noch nicht bereit...'"**
- Lyra: Mikro-Expression — kurzes Zusammenkneifen der Augen
- Kamera: Close-Up auf Lyras Gesicht (dieser Moment soll lesbar sein)
- Audio: kurze Stille zwischen Sätzen (0.5s Pause)

**[6] Lyra: "Was eine Rolle spielt..."**
- Lyra richtet sich auf (subtile Haltungsänderung — bewusste Entscheidung)
- Kamera: zurück zur Halbtotale (beide Charaktere im Bild)

**NACH [6]: Background-Dialog einfügen**
- System: `PLAYER_BACKGROUND` auslesen
- Entsprechende Sequenz laden: `lyra_first_contact_{background}`
- Kamera: bleibt auf Halbtotale
- Nach Background-Zeile: kurze Pause (1s), dann weiter zu `first_quest_briefing`

---

## `lyra_first_contact_{background}` — Background-Reaktionen

**Alle Background-Varianten:**
- Kamera: Über-die-Schulter (Lyras Perspektive, leicht zu ihr gedreht)
- Lyra: schaut auf ihr Buch, dann auf Protagonisten
- Emotion-Animation entsprechend dem `emotion`-Tag aus JSON
- Danach: kurze Pause (0.8s), Kamera schwenkt zu Aldric für `first_quest_briefing`

---

## `first_quest_briefing` — Briefing mit Magister Aldric

**Vor [0]: Setup**
- Szene: Kamera bewegt sich zum Podest/Pult des Zeremonienmeistmers
- Magister Aldric: steht hinter Pult, Hände verschränkt

**[0] Aldric: "Willkommen in Valdris"**
- Kamera: formelle Etablierungseinstellung (weiter Schuss, zeigt Protagonisten + Lyra vor Aldric)
- Lighting: leicht anders — wärmer, formeller, aber mit kleinen Schatten

**[1] Aldric: "Ich werde nicht lügen..."**
- **Glitch-Event #5 (sehr subtil):** Aldrics Robe flackert für 1 Frame (GlitchController, Intensität 0.1)
- Dieser Glitch ist BEWUSST: Hinweis dass etwas mit ihm nicht stimmt
- Kamera: Bleibt weit, zeigt Raumarchitektur

**[2] Aldric: "Valdris steht vor einer Bedrohung."**
- Kamera: langsam heranfahren auf Aldric (schleichend, kaum merklich)
- Audio: Ambient kurz absenken (Spannung)

**[3] Aldric: "Unsere Wachen konnten nicht vordringen."**
- Kamera: Close-Up auf Aldrics Hände (verschränkt — zeigt Kontrolliertheit)
- Schnitt zurück auf Halbtotale

**[4] Lyra: "Magister — wenn ich fragen darf..."**
- Kamera: schwenkt zu Lyra (sie tritt einen halben Schritt vor)
- Lyra: Lehrbuch leicht angehoben (recherchiert-Modus)

**[5] Aldric: "Die Ursache ist... noch Gegenstand der Untersuchung."**
- **Pause:** 0.7s nach "ist..." (Ellipsen-Effekt — er überlegt zu lange)
- Kamera: Über-die-Schulter hinter Lyra — Aldric füllt das Bild
- Aldric: Augenkontakt mit Lyra bricht kurz ab (Blick zur Seite), dann zurück

**[6] Lyra: "... Verstanden, Magister."**
- Kamera: Close-Up Lyra (unsichere Mimik — sie glaubt ihm nicht ganz)

**[7] Aldric: "Ihr werdet die Ruins erkunden..."**
- Kamera: zurück zur weiten Einstellung
- Aldric: dreht sich leicht ab (halb-abgewandt — schaut auf Karte/Dokument)

**[8] Aldric: "Noch eine Sache. Es mag sein dass ihr auf Anomalien stoßt."**
- Kamera: langsam Close-Up auf Aldric — sein Gesicht wird größer im Bild
- Audio: Ambient Volume nochmal minimal absenken
- **Glitch-Event #6:** Kurze Zahlen-Überlagerung auf dem Hintergrund-Gemälde hinter Aldric (0.2s, fast subliminal)

**[9] Lyra: "Keine... Fragen an Objekte?"**
- Kamera: Cut zu Lyra, dann Protagonisten, dann zurück zu Lyra
- Lyra: runzelt die Stirn (Confusion-Animation)

**[10] Aldric: "Viel Erfolg, Beschworener."**
- Aldric: dreht sich vollständig ab (gibt ihnen den Rücken — Szene beenden)
- Kamera: zeigt Protagonisten + Lyra von vorne (Aldric im Hintergrund, abgewandt)
- Fade zu: `leaving_akademie` (kurzer schwarzer Schnitt, 0.3s)

---

## `leaving_akademie` — Verlassen der Akademie

**Setup:**
- Szene: Außenportal der Akademie (Trigger-Zone aus `AkademieInnen.tscn`)
- Zeit: früher Morgen, Nebel (Ambient-Licht kühl-blau)
- Audio: `ambient_aldenmere.wav` crossfade von innen nach außen

**[0] Lyra: "Hmm. 'Keine Fragen an Objekte stellen.'"**
- Kamera: Begleitung von hinten (Protagonist und Lyra gehen Seite an Seite)
- Lyra: leicht zu sich selbst sprechend, Blick auf den Boden
- Audio: Schritte auf Kopfsteinpflaster (SFX)

**[1] Lyra: "Das ist in Ordnung..."**
- Kamera: wechselt zur Seite (Profilaufnahme beide Charaktere)
- Lyra: schaut auf ihr Lehrbuch das sie aufgeklappt hält
- Kleines Glitch-Flimmern am Rand des Bildschirms (sehr subtil, `glitch_idle_intensity = 0.03`)

**[2] Lyra: "... Kapitel elf, Seite zweihundertdreiundzwanzig..."**
- Kamera: langsamer Zoom-Out (zeigt Aldenmere-Stadtkulisse im Hintergrund)
- Lyra: klappt Lehrbuch zu, atmet aus
- Audio: Übergang zu `ambient_aldenmere.wav` volle Lautstärke
- **→ GAMEPLAY BEGINNT**
- UI-Fade-In: HUD erscheint (HP-Bar, Data-Gauge, Skill-Slots)
- Tutorial-Prompt optional: Bewegungssteuerung einblenden

---

## Technische Anmerkungen für csharp-godot

### Glitch-Event Zusammenfassung
| Event # | Trigger | Intensität | Dauer | Sichtbar für NPCs? |
|---|---|---|---|---|
| #1 | ritual_cutscene[2] | 0.4 | 0.3s | Nein |
| #2 | ritual_cutscene[4] | 0.3 | 0.2s | Nein (nur Wächter-Tex) |
| #3 | ritual_cutscene[7] | 0.6 | 0.8s | Nein |
| #4 | ritual_cutscene[10] | 0.2 | 0.3s | Nein |
| #5 | first_quest_briefing[1] | 0.1 | 1 Frame | Nein (Spieler-exklusiv) |
| #6 | first_quest_briefing[8] | 0.15 | 0.2s | Nein |
| Idle | Gameplay-Loop | 0.03-0.05 | Permanent | Nein |

### Background-Variable
- `PLAYER_BACKGROUND` auslesen aus `CharacterCreation.tscn` / GameManager
- Mapping: `"programmer"` → `lyra_first_contact_programmer`, etc.
- Fallback bei unbekanntem Background: `lyra_first_contact_programmer`

### Dialogue-Keys Mapping
```
ritual_cutscene          → Beschwoerungsraum.tscn, beim SummoningTriggered-Signal
lyra_first_contact       → direkt nach ritual_cutscene Ende
lyra_first_contact_{bg}  → direkt nach lyra_first_contact Ende
first_quest_briefing     → nach Background-Dialog, Kamera zu Aldric
leaving_akademie         → AkademieInnen.tscn Ausgang-Trigger
```

### Kamera-System-Hinweis
- Für Cutscene-Kamerabewegungen: `CutsceneCamera.cs` oder AnimationPlayer mit Kamera-Pfaden
- Third-Person-Kamera (`PlayerCamera.cs`) während Cutscene deaktivieren
- Nach `leaving_akademie[2]`: Kontrolle an `PlayerCamera.cs` zurückgeben

### Emotion-Tags → Animation-Mapping (Vorschlag)
| Emotion-Tag | Lyra-Animation | Aldric-Animation |
|---|---|---|
| `nervous` | Lehrbuch vor Brust, leichtes Schwanken | — |
| `determined` | Aufrechte Haltung, Blickkontakt | — |
| `hurt` | Kurzes Blinzeln, Blick kurz weg | — |
| `solemn` | — | Hände falten, Blick gesenkt |
| `formal` | — | Aufrechte Haltung, Distanz |
| `dismissive` | — | Kurzes Abwinken, Blick zur Seite |
| `concerned` | Lyra tritt einen Schritt näher | — |
| `explaining` | Zeigefinger, Lehrbuch aufschlagen | — |
