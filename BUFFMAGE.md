# Buffmage implementation

Buffmage uses `Companion`, `CharacterData`, the two existing party slots, `Skill.Execute`, the existing battle planning/resolution flow, normal equipment, and normal victory XP. Knight's skill implementations and saved numeric skill IDs are unchanged.

## Register and select

The GameManager discovers `PlayableHeroDefinition` assets under `Resources/Heroes`. Buffmage's definition, default data, prefab, settings, concept-derived transparent sprite, and fallback Animator controller are in `Assets/Resources/Heroes/Buffmage/`.

Open **Guild Hall → Buffmage → Add to party**. To test him alone, add him first, then remove Knight. The current maximum remains two heroes. All three Buffmage skills are available and equipped at level 1. The legacy home-screen class selector also discovers his form and observes the same availability policy; active party assignment continues through the normal party controls.

No scene wiring or inspector reference assignment is required. Registration also discovers him when loading an older save.

## Temporary unlock versus quest unlock

Set `developmentUnlock` to false in `Assets/Resources/Heroes/Buffmage/Definition.asset` to disable immediate testing availability. The override applies only in the Editor and Development Builds; it is ignored in release builds. His authored `CharacterData.isUnlocked` stays false. Normal progress saves do not turn the testing override into a permanent unlock.

Future quest rewards can call `GameManager.Instance.UnlockHero(Buffmage.HeroID)`. The stable identifier is **`Buffmage`**, also used by the existing `CharacterData_Buffmage` save key. An explicit quest unlock persists through the existing `isUnlocked` flag. A locked registered hero is removed from the active runtime party when party IDs load; Knight remains the normal fallback.

## Stats and equipment

ATK represents physical strength; HP represents constitution. No intelligence or other new attribute was added. Defaults: **28 HP, 6 ATK, 1 DEF, 3 SPD, 3 MP**. He is slower and has less defense than Knight, with higher physical burst and HP. Each level grants 6 HP and 1 ATK; defense grows by 0.4 per level, rounded down cumulatively. Maximum MP increases every fifth level. Existing stat/skill points are awarded.

Growth is incremental, retaining rolled equipment bonuses and purchased stats. Existing equipment slots and shop gear work without class-specific restrictions. CharacterData retains its existing equipment instance and progression save format.

## Combat interaction

Select a skill, then an enemy, using the existing planning controls. When Buffmage's planned turn resolves, a compact framed control appears near the bottom of the battle screen. This is the active hold/tap control; input while planning does not charge a future action.

| Skill | Input | Defaults | Outcome |
| --- | --- | --- | --- |
| Fireball? | Hold, then release in the gold zone | Perfect 0.9–1.4 s; maximum hold 2.2 s; initial-input timeout 6 s | Early power rises from 0.55× to 0.95×; perfect 1.55×; overcharged 0.7×. Base physical damage 1.45× ATK. Costs 0 MP, gains 1 MP, no cooldown. |
| Magic Missile | Repeated distinct presses | 2.2 s, at most 6 hits, minimum 0.12 s between accepted presses | Each press deals 0.43× ATK, rounded per hit, through normal defense. Holding does not repeat punches. No presses means no damage. Costs 2 MP; one-round cooldown. |
| Meteor | Tap as the moving marker enters gold | Sweep 1.6 s; gold zone 58–82% | Physical damage 1.65× ATK, multiplied by 1.5× for perfect or 0.7× otherwise. Costs 3 MP; two-round cooldown. |

Cooldowns block the specified number of subsequent planning rounds. They are battle-local and clear on stage reset. Skill buttons display MP or remaining cooldown, with dimmed art when unavailable.

All values above, starting stats, level growth, energy costs/gain, cooldowns, props, icons, audio, and motion durations are configurable in **`Settings.asset`**. Charge and sweep settings have bounded runtime values; early/overcharged damage remains below perfect damage. Rapid input has a hard safety cap of 30 even if authored tuning is excessive.

`BattleManager.TimedActions.cs` adds overloads to the existing hold/release and active-time events, using the same `TimingEventResult` contract. Knight's existing signatures remain intact. Shared pointer control supports mouse/touch, one-pointer ownership, releases outside the button, lost-release polling, focus/pause cancellation, input timeouts, and maximum hold resolution. Battle completion, disabling, and reset close the control; bounded skill coroutines and presentation cleanup restore actor state.

## Art and animation

The original concept-derived transparent cutout remains the portrait. Battle presentation now uses a **48-frame, 8�6 sprite sheet**, 14 Unity animation clips, and an assigned Animator controller covering idle, book windup/hold/throw, alternating punches, Meteor flight/slam/recovery, hurt/block, and death.

Grounded frames use aligned foot pivots. The sheet is point-filtered and uncompressed, and a dedicated material rejects low-alpha matte while preserving the palette. Skills own damage, synchronized to clip contact/release poses without animation events. Death plays fully before the scene transition, then uses existing fade/UI cleanup.

See **BUFFMAGE_ANIMATIONS.md** for clip details, timing configuration, the offline animated pose preview, validation status, and the exact sprite-sheet generation prompts. Dedicated sprite art replaces the old transform-only pose fallback; world-space travel, the book projectile, and existing pixel VFX remain separate.

Reused project assets: Ninja Adventure book, BookFire/Punch/RockSpike icons and gold Spark effect; existing campfire frames for the burning book; existing hit/critical sound clips. No battle environment, enemy, or Knight art was replaced.

Final built-in imagegen prompt:

> Use case: background-extraction. Edit target: the attached Buffmage pixel-art concept. Create a clean game-ready transparent cutout of this exact character. Remove only the dark background. Preserve the original muscular male would-be wizard, oversized bent orange/brown hat with purple band, shadowed face, bare chest, huge arms, dark bracers, orange waist cloth, dark pants and boots. Preserve pose, silhouette, proportions, colors, and chunky SNES pixel art. Center one full-body idle character, whole hat and both feet visible, modest transparent margins. No redesign, no additional objects, no text, no ground/shadow, no smoothing or painterly detail. Actual alpha transparency.

## Validation

- `Tools/Test-Buffmage.ps1`: executes the production timing rules, rapid-input session, cooldown ledger, and hero availability policy in an independent C# harness. **40 checks passed.**
- `python Tools/Validate-BuffmageAssets.py`: verifies GUIDs, sliced sprite IDs, prefab references, default loadout, old saved enum IDs, transparency format, and point-filter imports. **188 checks passed.**
- Full runtime and editor source assemblies compiled successfully with the installed Unity 6000.2.8f1 Roslyn compiler and the project's actual references. Existing unrelated gameplay warnings remain.
- **Not executed:** Unity import/render, live combat, mouse/touch event delivery, progression save/load, and equipment round-trip checks. The Unity executable exits before opening the project; previous launch logs show licensing failures. Compilation and offline checks do not establish Play-mode success.

Available once Unity launches:

1. **StepQuest → Heroes → Validate Buffmage assets** checks loaded Unity objects and references.
2. **StepQuest → Heroes → Validate Buffmage save and progression (Play mode)** checks actual XP growth, equipment, normal save/load, repeated loading, and the nonpersistent testing unlock using an isolated temporary save key. Enter the home scene first.
3. Add Buffmage to the party; exercise short/perfect/long holds, release outside the control, never press, focus loss/pause, rapid taps, Meteor early/perfect/late/no input, battle reset, both heroes together, and Knight alone. Test on an Android device as well as with a mouse.

The shared RPG UI overhaul also compiles. Its isolated capture command is **StepQuest → UI → Capture exploration previews**. Visual inspection of those Unity renders remains pending the editor launch issue.
