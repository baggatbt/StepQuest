# Buffmage combat art revision

Generated with the built-in imagegen tool, reference-image editing mode. References: the original Buffmage portrait, Knight sprite atlas, and reported battle screenshot. Project output: `Assets/Resources/Heroes/Buffmage/Animations/BuffmageSheet-v2.png`. Native atlas slicing and animation bindings are rebuilt by `Tools/Build-BuffmageAnimations.py`; the generated raster is preserved.

`output/buffmage/scale-facing-comparison.png` compares the actual assets at the same world scale. It is an offline comparison, not a Unity screenshot.

## Exact generation prompt

Use case: stylized-concept. Asset: a REPLACEMENT combat sprite atlas for Buffmage, matching an existing side-view pixel RPG.
Image 1 = Buffmage IDENTITY ONLY (hat, muscular build, bracers, orange cloth, purple band, dark pants/boots). Image 2 = EXISTING KNIGHT SPRITE SHEET and is the PRIMARY reference for pixel density, shading simplicity, perspective, battle stance, proportions and rendering style. Image 3 = game battle screenshot, overall style/context ONLY; the tiny frontal Buffmage in that screenshot is the ERROR being fixed.
Draw Buffmage as a proper SIDE-VIEW combat sprite facing SCREEN RIGHT in EVERY living pose. His nose, chin, eyes, forward fist, forward knee and attack direction must point RIGHT. Show a clear right-facing head profile with one visible eye under his hat; NOT a symmetrical front view, NOT a portrait looking at the viewer, NOT facing left. His torso is turned right in the same oblique side-combat perspective as the Knight. Maintain his large muscular chest and arms, but fit the style of the Knight, not a shiny detailed chibi collectible.
Art quality: authentic low-resolution 16-bit pixel art, approximately 40-44 native logical pixels of full character height (including hat) enlarged by a clean integer nearest-neighbor factor. CHUNKY coherent pixel clusters and a restrained earthy palette, 2-3 shade values per material, NO tiny speckle, NO gradients, NO glossy highlights, NO heavy decorative black contour, NO smooth antialias. Match the Knight's simple readable faces, hands, boots, body silhouettes and shadow density. Buffmage may be bulkier, not more finely detailed.
Exact sheet structure: 8 columns x 6 rows = 48 full-body frames, regular equal SQUARE cells. Requested canvas2048x1536, cell256x256. Transparent background with generous internal padding; keep every hat/fist/foot/corpse fully inside its cell. Clean binary alpha, no colored edge halos, no text, no grid lines, no scenery, no shadows.
Rows top to bottom, frames left to right:
1 IDLE: eight subtle breathing/guard poses, feet planted. Body and face clearly look RIGHT toward the enemy. Ready boxer-like stance, rear foot left, forward foot right, one large fist forward at chest height.
2 BOOK: 0 reach book, 1 grasp, 2 draw right-throwing arm back toward screen left, 3 hold book behind shoulder in windup, 4 RELEASE toward RIGHT with EMPTY forward hand, 5 follow through, 6 recover, 7 guard. The small held brown book has no baked flame; do not draw a detached projectile.
3 PUNCH: 0 guard,1 windup,2 LEFT JAB fully extended to screen RIGHT,3 recoil,4 cross windup,5 RIGHT CROSS fully extended toward screen RIGHT,6 recoil,7 guard. Keep RIGHT-facing profile throughout.
4 METEOR:0 crouch ready,1 jump takeoff,2 knees drawn up rising,3 curled cannonball facing right,4 fists raised diving pose,5 body slam contact,6 squashed landing crouch,7 recover guard RIGHT. Do not rotate to face the viewer. Unity handles flight trajectory.
5 HURT: eight flinch and recovery frames, recoiling backward toward LEFT while head remains pointed RIGHT, then return to right-facing guard.
6 DEATH: stagger, buckle knees, kneel, fall, ground contact, settle, still collapsed, final collapsed corpse. Fall to screen RIGHT, same outfit and hat, whole body inside cell, opaque final corpse.
Standing height and feet baseline identical across rows and frames, no pose changing to frontal. Retain Buffmage's identity from image1, while strictly adopting the Knight's combat-facing and coarse pixel rendering from image2.
