# Integrated first quest

## Play
Start CharacterInfoPage normally and use the existing travel navigation. No adventure screen opens at startup. The quest uses MapScrollView and its existing LocationOnePanel, TownOne, and BattleNode1-1 objects. Existing map artwork, panning, navigation, town services, and other encounters remain in place.

From the home town, use Gate > Dungeon > Location 1. Mayor Bramble is a saved portrait location above the existing town icon near the lower-left of the map. Tap his portrait to talk and accept the quest, then tap the first forest battle node to investigate. A quest badge marks the relevant town/forest location.

Story combat uses the existing victory panel; Return to map restores the original travel map and its scroll position. Return to the mayor to claim once. The forest node resumes its original battle click behavior after completion. Existing saved quest progress is retained.

## UI integration
AdventureScreen is now a scene-bound quest controller on HomePageCanvasGroup, not a screen bootstrap. Its serialized fields reference the existing map, town, node buttons, and node-panel/button style templates. Quest conversations use an additive scrolling panel under the existing Canvas, with the existing node-panel artwork, button artwork/colors, and font. There is no new Canvas, map background, map launcher, or separate location screen.

The forest Node pointer handler defers to the quest button while the quest is active, preventing its old node-info panel from opening behind the conversation. The town location opens the current TownScrollView; the obsolete TownBackground is parked off-screen and is no longer used by the quest.

## Edit content
Use StepQuest > Select first adventure to edit FirstAdventure.asset. Edit speaker text, response labels, node destinations, NPC portrait, forest preview, and encounter stage ID. Stable node IDs preserve links. Validate conversation checks entry/response links and required visual assets. Location placement is edited on the original scene map nodes.

## Battle completion
The hidden victory CanvasGroup bug is fixed by revealing the existing win panel and its ancestor groups. A terminal latch prevents duplicate rewards or a new planning round after victory/defeat. The story return button is explicitly routed through quest return handling. XP presentation tolerates missing/differently sized UI slots.

## Verify in Unity
- Startup shows the original UI; open the original map.
- Town location opens the original town; mayor conversation accepts the quest.
- Forest quest badge moves/pans with the existing node.
- Forest click opens one quest panel; cancel restores map interaction.
- Win: existing victory UI appears once, return restores map position, mayor reward claims once.
- Defeat and retry; multi-hit/status-effect final kills.
- Complete quest: forest node resumes normal battles; other nodes and town services continue working.
- Phone: scrolling dialogue, portrait ratios, touch targets, and existing navigation.

## Validation
Runtime/editor compilation and serialized scene-binding checks pass. The Gate > Dungeon > Location 1 route and opening the mayor conversation were verified in the running Unity editor using its portrait device simulator. Full battle/reward and physical Android-device testing are separate checks.
