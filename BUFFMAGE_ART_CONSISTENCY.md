# Original-design art conversion

The original identity reference survives at `Assets/Resources/Heroes/Buffmage/BuffmageIdle.png`. The rejected squat/chibi drafts are not used by the controller. The active replacement is `Assets/Resources/Heroes/Buffmage/Animations/BuffmageOriginalStyle-v5.png`, generated with built-in imagegen in reference-guided style-transfer mode. The original portrait and earlier sheets are retained.

## Reference findings

- Knight idle: 100×55 slice, 44×40 opaque bounds including sword, 17 opaque RGB colors; PPU 32 and prefab scale 3. Visible height 3.75 world units.
- Goblin idle, inspected during the initial pass: 33×36 slice and opaque bounds, 16 opaque colors; PPU 100 and prefab scale 8.5. Selective colored outlines and broad material shading.
- Mushroom, the final enemy reference: 23×37 idle slice, PPU 100, prefab scale 8. Detailed cap and body clusters, dark colored edges rather than a uniform black contour.
- Knight and Mushroom retain human/creature anatomy, small facial marks and layered material shading. They do not justify a mascot-style redesign. Original Buff Mage's hat, purple band/bracers/buckle, orange split cloth, defined chest/abdomen and full humanoid legs are the primary identity constraints.

## Integration and limits

The new atlas is 1448×1086, with 48 equal 181×181 slices in an 8×6 grid. It is an actual raster redraw; no raster resizing, blur or palette filter was applied. Existing point filtering and alpha material are unchanged. PPU is recalculated only to preserve the established Knight-relative world height. Per-frame grounded pivots are regenerated; airborne poses preserve the idle reference pivot. All 14 clips retain frame sequences, frame rates and contact/release timings. Controller/clip GUIDs remain stable. The material, prefab opening sprite, animation-set calibration and editor validator reference the new atlas. Stats, damage, cooldowns and skill input are unchanged.

`output/buffmage/scale-facing-comparison.png` compares Knight, Mushroom and the new Buff Mage using actual sprite rects, PPU and prefab scales at 26 pixels/world-unit (the portrait camera has orthographic size 15). This is an offline render, not a Unity screenshot. The generated work is an approximation of the reference artist's style, not a guarantee of identical hand-authored pixel technique. Unity import/GPU rendering and live effect alignment require Play mode validation if editor startup fails.

Executed: 596 animation checks, 188 hero asset checks, 40 combat logic checks, and runtime/editor C# compilation.

`output/buffmage/battle-art-overlay.png` adds the new idle sprite beside the original Buff Mage on the user's battle screenshot, at approximately 24 pixels/world-unit inferred from the Knight. Original screenshot characters are untouched. The NEW label identifies the offline candidate overlay; it is not captured gameplay and cannot validate animation/VFX alignment. A fresh Unity validation attempt exited with code 1 before opening the project (`Logs/BuffmageArtUnity.log`). The rejected chibi drafts live outside Unity assets in `output/buffmage/rejected-drafts`.

## Final exact generation prompt

Use case: style-transfer. Conservative pixel-art conversion of an existing character, NOT a character redesign.
INPUT1 = ORIGINAL BUFF MAGE, the mandatory identity, costume, silhouette and muscular anatomy reference. INPUT2 = actual HERO KNIGHT sprite atlas, rendering and human proportions reference. INPUT3 = actual MUSHROOM ENEMY atlas, secondary rendering reference. INPUT4 = actual BATTLE SCREENSHOT, relative scale, perspective and environment reference.
Return close to ORIGINAL BUFF MAGE. Tall muscular HUMANOID fantasy man, full-size legs, relatively realistic heroic proportions, recognizable pectorals, deltoids, biceps and abdominal definition. Broad brim crooked ORANGE WIZARD HAT with PURPLE band, shadowed eyes, dark purple wide wrist bracers, small purple/silver belt buckle, orange hanging split waist cloth, dark trousers and purple-brown boots. Preserve exact original outfit. Bare chest. Strong intimidating silhouette, ridiculous fake wizard concept. NO chibi, no oversized head, no mascot, no tiny legs, no inflated ball fists, no giant hat. Do NOT flatten anatomy into cartoon circles. Human anatomy and original identity take priority over simplification. Head excluding hat about1/6 of standing body, waist-to-ground roughly half body.
Convert FRONT original into Knight-like THREE-QUARTER SIDE COMBAT view facing SCREEN RIGHT: head,nose,forwardknee and actions right, torso oblique enough for visible defined chest. Match the Knight's planted human posture, not symmetrical front bodybuilder.
Rendering: detailed stylized16-bit fantasy RPG pixel art like input2 and3. Layered clean pixel clusters, selective thin dark plum/colored edges, no uniform black sticker contour. Defined muscles with natural volumes and3-4skin tones, neutral tan/pale peach lit skin and muted mauve-brown shadows, NOT orange skin. Enough facial/clothing detail to remain original, no noisy tiny dither or glossy gradients. Keep comparable apparent pixel size to Knight: approximately44-48 logical standing pixels high including hat (Knight40), not highly detailed miniature painting. Limited coherent earthy palette, no arbitrary16color reduction. Do NOT simply pixelate or resize the original: actually redraw it in the right perspective.
Production atlas: EXACT8columns x6rows,48 equal square cells, row-major, transparentbackground. Suggested logicalcanvas640x480/cells80square, or clean integer-enlarged equivalent. Character full standing height44-48 logical pixels, at least8 logical transparent pixels around EACH full pose including fists and corpses. Allframes same pixel density,palette,bodyproportions and outfit.
Row1 idle8: subtle breathing right-facing combatguard, feetplanted, original muscular build.
Row2 book8:reach,grasp,windup,holdbehindshoulder,releaseEMPTYhandRIGHT,followthrough,recover,guard. NO drawn book: Unity attaches separate large burning spellbook.
Row3 punches8:guard,windup,jabfullyextendedRIGHT,recoil,crosswindup,crossfullyextendedRIGHT,recoil,guard.
Row4 Meteor8:deepcrouch,takeoff,tuckedrising,cannonball,overheaddiving,groundslamcontact,landingcrouch,recover. Fullbody remains within owncell; Unity handles trajectory.
Row5 hurt8:flinch,recoilbackLEFT,brace,recover to originalrightguard. Never turnfrontal.
Row6 death8:stagger,buckleknees,kneel,fall,groundcontact,settle,collapsed,finalcorpse. Entirehat/bodyinsidecell.
Feet baseline consistent for standingposes. No frame overlaps. No text,grid,scenery,glow,particles or shadows. Genuine transparency. Preserve ORIGINALdesign and detailed humanoid build; change only perspective,clusterwork,edgecolors and shading needed to match Knight/Mushroom.
