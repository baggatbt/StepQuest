# Buffmage sprite animation

The battle prefab now uses `Animations/BuffmageSheet-v2.png`: 48 distinct poses in an 8×6 grid (181×181 pixel cells), imported as 48 named sprites. His original concept-derived `BuffmageIdle.png` remains the portrait/reference. Hat, physique, bracers, orange cloth, boots, and palette follow that design.

The second revision replaces the frontal stance with a right-facing combat profile and coarser shading matched to the Knight. Median idle height is calibrated to 105% of the Knight's actual opaque battle height (3.9375 world units), rather than transparent cell dimensions. Target-based mirroring handles opponents on either side. The hitbox, book attachment positions and punch reach are calibrated with the larger art. Existing clips and gameplay timing remain intact. The previous sheet is retained for comparison.

## Clips and hooks

Fourteen Unity clips and the assigned `Buffmage.controller` cover:

- Idle: a smooth ping-pong breathing loop using all eight idle poses.
- BookWindup → BookHold while awaiting/charging input; BookThrow on release, then Idle. The held book is in the pose art; its flame remains a separate pixel effect. A separate projectile appears at the empty-hand release frame.
- PunchLeft/PunchRight: alternating jabs/crosses, with a short bounded queue so rapid taps do not cancel an earlier strike. Each accepted punch applies damage once at its contact pose; the result reports landed hits.
- MeteorReady → MeteorLaunch → MeteorAir → MeteorSlam → MeteorRecover → Idle. Flight remains world-space motion; the sprite changes to its impact pose on landing.
- Hurt from the existing `IsHurtTrigger`; a short Block response also preserves the existing defensive hook.
- Death: a nonlooping collapse, final-pose hold, then the shared fade/UI cleanup. The Dead parameter blocks hurt interruptions. Defeat waits for Buffmage's collapse before leaving the scene; Knight-only defeat keeps its existing immediate transition.

`BuffmageAnimationDriver` handles state playback, defeat cleanup, and revival/reset. Automatic lethal-HP detection is scoped to battle participants, so a zero-HP portrait/preview actor does not disappear in the home screen. `BuffmagePresentation` handles the book/flame/projectile and travel. Skills retain damage ownership; the clips contain no gameplay animation events.

The animation set, controller, material, first sprite, settings, and prefab component are assigned already. No manual inspector wiring is required.

## Timing and rendering

Contact markers are editable in `AnimationSet.asset`:

| Marker | Clip-local time |
| --- | --- |
| Book release | 1/12 s |
| Left punch contact | 2/24 s |
| Right punch contact | 1/24 s |
| Meteor contact | 1/12 s |
| Death hold after clip | 0.2 s |

Grounded poses use per-frame foot pivots; airborne poses retain their lift. All frames are point-filtered, uncompressed, and have mipmaps disabled. The dedicated pixel sprite material discards source alpha below 0.75, removing the generator's low-alpha matte without changing opaque colors. Renderer tint/fade still works. Existing book, flame, impact, and sound assets remain in use.

The original raster is preserved; packing/slicing, clip creation, and controller binding are Unity-native metadata operations. `Tools/Build-BuffmageAnimations.py` rebuilds those generated assets while preserving their GUIDs.

## Preview and verification

`output/buffmage/animation-preview.png` is a 120-frame animated PNG. It is an **offline pose preview**, using the actual atlas, clip frame lists, pivots, and pixel-alpha rule; it is not a Unity scene capture.

Executed checks:

- 596 animation checks passed: complete grid, opaque margins, grounded pivots, Knight-relative height, facing metadata, clip frame order, loop seams, exact clip durations, release/contact poses, controller references, and prefab assignments.
- 188 general hero/asset checks and 40 timing/cooldown/unlock regression checks passed.
- Runtime and editor source compiled with the installed Unity compiler and project references.

Unity itself still exits before opening the project (exit 1 in `Logs/BuffmageAnimationUnity.log`, with licensing-client connection errors). Actual Unity imports, shader compilation/GPU rendering, and live battle playback remain unverified here.

Once Unity opens, run **StepQuest → Heroes → Validate Buffmage animations**. This checks imported sprite/clip objects, loop flags, object-reference curves, material/shader errors, timing markers, and prefab bindings. Then test idle, early/perfect/overcharged throws, repeated punches, Meteor, hurt/block, death with another hero alive, final-party defeat, and battle reset/revival in Play mode.

## Artwork generation

Generated using the **built-in imagegen tool**, with the current Buffmage sprite as the identity reference. Final project asset: `Assets/Resources/Heroes/Buffmage/Animations/BuffmageSheet-v1.png`.

### Initial prompt

Use case: stylized-concept.
Asset type: production-ready 2D pixel-art CHARACTER ANIMATION SPRITE SHEET for Unity.
Input image 1 is the locked character design reference, NOT a background.
Create exactly ONE transparent sprite sheet with exactly 8 columns and 6 rows = 48 individual full-body character frames. Canvas 2048 by 1536, all cells exactly 256 by 256, equal grid, no gaps between cells. The transparent padding within each cell must prevent contact between frames. No visible grid, no labels, no text, no background, no ground shadows.
Preserve Buffmage's CURRENT design: massively muscular bare-chested would-be wizard, very broad shoulders, dark face under wide bent orange-brown witch/wizard hat with purple band, dark purple bracers, orange waist cloth, dark trousers and boots, silver belt buckle. Consistent chunky crisp SNES pixel art with the exact orange, purple, warm skin, and deep charcoal palette of the reference. No armor, no staff, no beard, no redesign. Front three-quarter battle pose, attacking toward screen RIGHT, same consistent character identity, scale, palette, anatomical proportions and pixel density across ALL 48 frames.
Eight frames per row, read LEFT TO RIGHT, rows TOP TO BOTTOM:
ROW 1 IDLE: 8-frame subtle breathing loop, slight chest rise and hat bob, fists relaxed. First and last match for a seamless loop.
ROW 2 BOOK THROW: 1 reach for belt, 2 grip an unlit closed brown book, 3 draw arm back in a strong throwing windup, 4 tense held anticipation, 5 snap arm forward to release book, 6 full powerful overhand follow-through with EMPTY hand, 7 recoil recover, 8 return toward idle. Book is small and held close; DO NOT draw a detached flying projectile or flame, those are separate game objects.
ROW 3 PUNCHING: 8-frame alternating straight jab and heavy cross to screen right. 1 guard, 2 left windup, 3 extended left fist, 4 recoil, 5 right windup, 6 huge extended right fist, 7 recoil, 8 guard. Large readable silhouette changes.
ROW 4 METEOR: 1 deep crouch preparation, 2 powerful upward takeoff, 3 knees drawn up rising, 4 curled cannonball in air, 5 fists overhead downward dive pose, 6 downward body slam contacting ground, 7 exaggerated squashed landing crouch, 8 recovery standing. Keep every airborne frame centered in its OWN cell, do not shift it vertically between cells; Unity supplies the flight trajectory.
ROW 5 HURT: 8-frame flinch reaction, torso recoils and face grimaces, hat tips, hands recoil, then return to guard. Keep ground foot position consistent.
ROW 6 DEATH: 8-frame collapse, 1 stagger, 2 buckle knees, 3 kneel, 4 fall sideways, 5 hit ground, 6 settle, 7 still collapsed, 8 final fully collapsed unconscious pose. Entire body and hat remain INSIDE each cell even for the wide lying pose. Keep final corpse opaque, do NOT fade it.
Strict technical constraints: exact regular 8x6 atlas, 48 frames, one character per cell, no overlaps, no extra character outside cells, no cropped hats/hands/feet, hard pixel edges with no smoothing, genuine alpha transparency. Standing frames grounded at local cell y=224 (28 pixels above cell bottom), centered local x=128; full standing height about 200px and constant scale. Air poses centered; corpse uses the same baseline. Clear motion progression, not copies of an idle pose.

### Cleanup prompt

Use case: precise-object-edit. Edit target: image 1, the Buffmage 48-frame animation sheet. Supporting identity reference: image 2, current Buffmage design.
Keep ALL 48 poses and their order: exact eight columns, six rows. Preserve the current hat, muscles, bracers, orange waistcloth, colors, and pixel art. Change ONLY the production problems:
1) REMOVE all red/orange matte fringe, ghost pixels, and colored semi-transparent speckle around silhouettes. Every silhouette must have a clean dark-purple pixel outline and crisp fully opaque pixels against FULLY transparent empty space. Binary alpha, no antialiasing or translucent colored halos. No red outline.
2) Give each character enough transparent space that hat, fists, boots and corpse are wholly visible. No pixels from any pose can cross into another cell. Do not crop hat tips.
3) Lay out on an exact regular 8 columns by 6 rows atlas, equal-size square cells, with generous internal padding, and a single consistent standing foot baseline. Do not trim transparent margins off the atlas. Full canvas requested 2048x1536, each cell256x256. Standing character about190 pixels high. Individual pixels remain chunky, not smooth.
Row1 idle, Row2 book throwing, Row3 alternating punches, Row4 Meteor crouch/jump/cannonball/dive/land/recover, Row5 hurt flinch, Row6 death collapse. No text, no visible grid, no background, no shadows. TRUE transparency; keep poses and character design unchanged.

