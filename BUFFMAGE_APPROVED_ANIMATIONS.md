# Approved Buff Mage animation replacement

The approved design is `output/buffmage/idle-review/BuffmageIdle-bulk-v4.png`. Live artwork now uses six sibling sheets under `Assets/Resources/Heroes/Buffmage/Animations/`: `BuffmageApproved-Idle.png`, `BuffmageApproved-Book.png`, `BuffmageApproved-Punch.png`, `BuffmageApproved-Meteor.png`, `BuffmageApproved-Hurt.png`, and `BuffmageApproved-Death.png`. Each is a 4×2 source sheet containing eight poses. Native metadata slices 443×443 cells at rounded grid origins in each 1774×887 generated image; the unused half-pixel grid remainder lies in transparent margins. No source raster was resized or filtered. Earlier atlases remain as backups.

All fourteen existing clips/controller states are rebound to these 48 sprites. Clip GUIDs, frame rates, durations and contact/release markers are preserved. A forward/back idle loop avoids an end-to-start jump, and idle pivots use the bottom foot silhouette to stabilize horizontal placement; grounded frames align to their opaque foot bottom. Each action sheet's standing reference is calibrated against the same Knight-relative world stature to avoid size changes between sheets. Book anchors are measured per new book frame and follow the actual displayed sprite/pivot/mirroring. The release frame remains the forward empty-hand pose. Existing large book, flame-only tongues and burn-away impact are retained.

Runtime gameplay, stats, saves and skill input are unchanged. Editor validation loads all six sheets. Builder, offline validator, comparison and preview tools support per-frame sheet bindings and PPU. No manual inspector wiring is needed.

Checks executed: 620 animation/source/curve/controller/prefab/hand-anchor/idle-stability checks; 206 general hero asset checks; 40 combat logic checks; runtime and editor C# compilation. Offline preview: `output/buffmage/animation-preview.png` (animated PNG). This is not captured Unity gameplay. Play mode was left for the user as requested. Check planted idle over several loops, book grip while charging and release, punch contacts, Meteor landing/recovery, hurt/death, and return to idle after a battle.

Artwork generated using the built-in imagegen tool. Full 48-frame attempts were rejected before integration because they compressed the approved proportions. Separate action sheets were used instead. Book and death backgrounds were cleaned by additional built-in background-extraction calls; no scripted raster cleanup was used.

## Exact prompt: buffmageIdleSheetPrompt

Animate input1 EXACT tall outline-free muscular wizard. Output ONLY eight IDLE frames as4columns2rows squarecells. Transparentbackground. Preserve input1 proportions completely: LONGLEGS half fullheight, smallhead and compacthat, hugearms, simple3toneskin,NOdarkoutline. No chibi orshortlegs. Each frame at70%cellheight, samefixedfeet/head placement, generouspadding. Do not reinterpretmodel. Start exactlyinput1pose. Only tiny1pixel chest/shoulder breathing over8frames, plantedfeet no walk/noscalechanges. Allframes identicalbodywidth/palette/pixelgrid, no addeddetail. This is a clean subtle breathingloop, not eightdifferentcharacters. Desiredcanvas1024x512 cell256square. Fullbodyeverycell, noseRIGHT, no background,redmatte,glow,noisefringes,shadow,textorgrid. Retain originaltallshape EXACTLY.

## Exact prompt: approvedBookPrompt

Use case: identity-preserve. Create EIGHT Book animationframes of EXACT input1 approvedtallmuscularwizard. Input2 approvednewIDLErow shows animationrendering and proportions to maintain. 4columns2rows,8equalSQUAREcells, lefttorightthentopbottom, canvasaspect2:1. Sameopaqueheight and modelproportions asidle reference. Fullbody inside eachcell with generouspaddingallaround includingfistsandcorpse.
LOCKEDdesign: tallhuman legs halfbodyheight, smallhead, compactorangefloppyhatpurpleband, barechest, VERYbulkyshoulders/biceps/forearms, purplebracers,silverbuckle,orange splitwaistcloth,darkpantsboots. Same3toneskincleanplanes. NO darkoutline, no extra muscleanatomy, no veins,dither,finedetailornewcolors. DoNOTchibify, shortenlegs,increaseheadorhat,orchangeoutfit. StandRIGHTfacing3quarterbattleview. Keep identity/pixelgrid/palette identical throughsequence.
Exacteightposes: 0reachbookatwaist,1grasp,2drawarmback,3holdwindupbehindshoulder,4drawbackforrelease,5throwRIGHTfullyextendedEMPTYhand,6followthrough,7recoverguard. NO bookorflame drawn; Unity adds separate book.
Feetbaselineconsistent forgroundedposes. Unityhandlesrootmotion. No particles,effects,drawnprops,glow,shadows,grid,labels,text,scenery orredmatte. Genuinecleantransparentbackground. Onlyonecharacter ineachcell. Talloutlinefreeapproveddesignisessential.

## Exact prompt: approvedPunchPrompt

Use case: identity-preserve. Create EIGHT Punch animationframes of EXACT input1 approvedtallmuscularwizard. Input2 approvednewIDLErow shows animationrendering and proportions to maintain. 4columns2rows,8equalSQUAREcells, lefttorightthentopbottom, canvasaspect2:1. Sameopaqueheight and modelproportions asidle reference. Fullbody inside eachcell with generouspaddingallaround includingfistsandcorpse.
LOCKEDdesign: tallhuman legs halfbodyheight, smallhead, compactorangefloppyhatpurpleband, barechest, VERYbulkyshoulders/biceps/forearms, purplebracers,silverbuckle,orange splitwaistcloth,darkpantsboots. Same3toneskincleanplanes. NO darkoutline, no extra muscleanatomy, no veins,dither,finedetailornewcolors. DoNOTchibify, shortenlegs,increaseheadorhat,orchangeoutfit. StandRIGHTfacing3quarterbattleview. Keep identity/pixelgrid/palette identical throughsequence.
Exacteightposes: 0guard,1leftwindup,2fullyextendedjabRIGHT,3recoil,4crosswindup,5fullyextendedcrossRIGHT,6recoil,7guard.
Feetbaselineconsistent forgroundedposes. Unityhandlesrootmotion. No particles,effects,drawnprops,glow,shadows,grid,labels,text,scenery orredmatte. Genuinecleantransparentbackground. Onlyonecharacter ineachcell. Talloutlinefreeapproveddesignisessential.

## Exact prompt: approvedMeteorPrompt

Use case: identity-preserve. Create EIGHT Meteor animationframes of EXACT input1 approvedtallmuscularwizard. Input2 approvednewIDLErow shows animationrendering and proportions to maintain. 4columns2rows,8equalSQUAREcells, lefttorightthentopbottom, canvasaspect2:1. Sameopaqueheight and modelproportions asidle reference. Fullbody inside eachcell with generouspaddingallaround includingfistsandcorpse.
LOCKEDdesign: tallhuman legs halfbodyheight, smallhead, compactorangefloppyhatpurpleband, barechest, VERYbulkyshoulders/biceps/forearms, purplebracers,silverbuckle,orange splitwaistcloth,darkpantsboots. Same3toneskincleanplanes. NO darkoutline, no extra muscleanatomy, no veins,dither,finedetailornewcolors. DoNOTchibify, shortenlegs,increaseheadorhat,orchangeoutfit. StandRIGHTfacing3quarterbattleview. Keep identity/pixelgrid/palette identical throughsequence.
Exacteightposes: 0deepcrouch,1takeoff,2kneestuckedrising,3curledcannonball,4overheaddivingfists,5groundslamcontact,6landingcrouch,7recoverguard.
Feetbaselineconsistent forgroundedposes. Unityhandlesrootmotion. No particles,effects,drawnprops,glow,shadows,grid,labels,text,scenery orredmatte. Genuinecleantransparentbackground. Onlyonecharacter ineachcell. Talloutlinefreeapproveddesignisessential.

## Exact prompt: approvedHurtPrompt

Use case: identity-preserve. Create EIGHT Hurt animationframes of EXACT input1 approvedtallmuscularwizard. Input2 approvednewIDLErow shows animationrendering and proportions to maintain. 4columns2rows,8equalSQUAREcells, lefttorightthentopbottom, canvasaspect2:1. Sameopaqueheight and modelproportions asidle reference. Fullbody inside eachcell with generouspaddingallaround includingfistsandcorpse.
LOCKEDdesign: tallhuman legs halfbodyheight, smallhead, compactorangefloppyhatpurpleband, barechest, VERYbulkyshoulders/biceps/forearms, purplebracers,silverbuckle,orange splitwaistcloth,darkpantsboots. Same3toneskincleanplanes. NO darkoutline, no extra muscleanatomy, no veins,dither,finedetailornewcolors. DoNOTchibify, shortenlegs,increaseheadorhat,orchangeoutfit. StandRIGHTfacing3quarterbattleview. Keep identity/pixelgrid/palette identical throughsequence.
Exacteightposes: 0flinch,1strongerbackwardrecoilLEFT,2brace,3brace,4recover,5recover,6nearguard,7originalguard. FaceRIGHTthroughout.
Feetbaselineconsistent forgroundedposes. Unityhandlesrootmotion. No particles,effects,drawnprops,glow,shadows,grid,labels,text,scenery orredmatte. Genuinecleantransparentbackground. Onlyonecharacter ineachcell. Talloutlinefreeapproveddesignisessential.

## Exact prompt: approvedDeathPrompt

Use case: identity-preserve. Create EIGHT Death animationframes of EXACT input1 approvedtallmuscularwizard. Input2 approvednewIDLErow shows animationrendering and proportions to maintain. 4columns2rows,8equalSQUAREcells, lefttorightthentopbottom, canvasaspect2:1. Sameopaqueheight and modelproportions asidle reference. Fullbody inside eachcell with generouspaddingallaround includingfistsandcorpse.
LOCKEDdesign: tallhuman legs halfbodyheight, smallhead, compactorangefloppyhatpurpleband, barechest, VERYbulkyshoulders/biceps/forearms, purplebracers,silverbuckle,orange splitwaistcloth,darkpantsboots. Same3toneskincleanplanes. NO darkoutline, no extra muscleanatomy, no veins,dither,finedetailornewcolors. DoNOTchibify, shortenlegs,increaseheadorhat,orchangeoutfit. StandRIGHTfacing3quarterbattleview. Keep identity/pixelgrid/palette identical throughsequence.
Exacteightposes: 0stagger,1buckleknees,2kneel,3fall,4groundcontact,5settle,6collapsed,7finalstillcorpse. Entirehat/bodyinsidecell.
Feetbaselineconsistent forgroundedposes. Unityhandlesrootmotion. No particles,effects,drawnprops,glow,shadows,grid,labels,text,scenery orredmatte. Genuinecleantransparentbackground. Onlyonecharacter ineachcell. Talloutlinefreeapproveddesignisessential.

## Exact prompt: approvedBookCutoutPrompt

Use case: background-extraction. Remove ONLY the white/gray background and drop shadows from this eight-frame book-throw sprite sheet. Every pixel outside the eight character silhouettes must be genuinely zero-alpha transparent, including gaps between legs and arms. Preserve the exact sprite colors, proportions, pixel clusters, frame positions, canvas dimensions, eight poses and 4x2 grid unchanged. No glow, no haze, no red matte, no replacement color background, no new drawing. Actual transparent PNG cutouts of all eight fullbody sprites. Do not crop or rearrange frames.

## Exact prompt: approvedDeathCutoutPrompt

Use case: background-extraction. Remove ONLY white/gray background, colored glow and ground shadows from this eight-frame death sprite sheet. Every pixel outside the eight full character silhouettes must be genuinely zero-alpha transparent, including gaps between limbs. Preserve exact sprite colors, proportions, pixel clusters, positions, canvas dimensions and 4x2 grid. No drawing edits, no cropping or rearrangement, no matte or replacement background. Actual transparent PNG cutouts of eight fullbody poses.
