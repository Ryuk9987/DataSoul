# DataSoul — Companion System (Subagent Context)
→ Für Übersicht: `ctx_overview.md` | Für Kampf: `ctx_combat.md`

---

## Companion System — Übersicht

- **2–3 aktive Companions** gleichzeitig
- Kämpfen **autonom** mit eigener KI
- Spieler kann via **Befehlsmenü** taktische Anweisungen geben: Angreifen / Heilen / Rückzug / Fokus-Target
- Jeder Companion hat:
  - Eigenen Datentyp und Skillbaum
  - **Story-Arc** mit Loyalitätssystem
  - **Synergy-Attack:** Kombination Spieler + Companion → mächtiger Fusionsangriff (aufladbar)

---

## Companion #1 — Lyra
*Heiladeptin der Königlichen Akademie von Valdris, 19 Jahre alt*

> *"Laut Protokoll sollte ich zuerst fragen ob du verletzt bist. Also... bist du verletzt? Ich meine — du bist gerade durch einen Datenstrom aus einer anderen Realität gefallen, also ich nehme an... ja?"*

### Aussehen
- Kurzes silbergraues Haar (akademische Färbung — zeigt ihren Rang)
- Blaue Augen
- Akademie-Robe mit goldenen Stickereien
- Trägt immer ihr Lehrbuch dabei — auch im Kampf

### Persönlichkeit
- Überpräzise, leicht sozial unbeholfen
- Kompensiert Unsicherheit mit Fachwissen
- Zitiert im Stress aus ihrem Lehrbuch
- Unter der Regelgläubigkeit steckt echter Mut — weiß es nur noch nicht

### Background
- Kommt aus einfachen Verhältnissen, hat sich durch Fleiß einen Akademieplatz erarbeitet
- Wurde dem Protagonisten zugeteilt weil sie "noch nicht bereit für wichtigere Missionen" ist — kränkt sie insgeheim
- Hat noch nie wirklich gekämpft, nur simulierte Übungen
- Weiß nichts über die wahre Natur von DataWorld

### Kampf-Rolle
- **Healer / Support**
- **Datentyp:** Licht-Data / Restaurations-Data

### Kamera-Verhalten
- Hält 4–6m Distanz zum Spieler
- Weicht zurück wenn Gegner näher als 2m
- Bei eigenem HP < 40%: priorisiert Rückzug über Heilung
- **AI-Prüf-Intervall:** alle 0.5s

### Prioritäts-Reihenfolge (AI)
1. Revive (Spieler HP = 0)
2. Notfall-Heilung (Spieler HP < 20%)
3. Reinigung (Spieler hat Debuff)
4. Heilung (Spieler HP < 50%)
5. Schutzschild
6. Gruppen-Heilung
7. Barriere

### Skill-Bibliothek (wächst mit Progression)
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

### Synergy-Attack
**"Data Blessing"** — kurzzeitige Unverwundbarkeit + Schadens-Boost für den Protagonisten

### Story-Arc
| Phase | Zustand |
|---|---|
| **Früh** | Nervös, regelgläubig, versucht alles richtig zu machen |
| **Mitte** | Beginnt die Widersprüche zu sehen, hinterfragt das Königreich |
| **Spät** | Muss wählen: Loyalität zum Königreich oder zum Protagonisten |

**Loyalty-Pfade:**
- Lyra loyal → Ending "Reform" möglich
- Lyra desillusioniert → Ermöglicht Insiderwissen über Akademie und The Root

---

## Weitere Companions (geplant, noch nicht ausgearbeitet)
- **Companion #2:** Wird in späteren Dungeons eingeführt (Story-Hinweis: möglicher Firewall Brotherhood-Sentinel)
- **Companion #3:** Noch offen

---

## Loyalitätssystem (allgemein)
- Jeder Companion hat Loyalitätspunkte (intern trackt)
- Beeinflusst durch Entscheidungen des Spielers und Dialogoptionen
- Hohe Loyalität = stärkere Synergy-Attacks, mehr Story-Tiefe, spezifische Endings
- Niedrige Loyalität = Companion kann in kritischen Momenten zögern oder weglaufen
