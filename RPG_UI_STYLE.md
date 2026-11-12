# Classic RPG UI

The adventure screens share `RpgUiTheme.asset` and the `RpgUi` component factory. The theme uses charcoal/brown surfaces, beige text, muted gold highlights, the existing gold-rimmed `RectangleBox_96x96` sprite with corrected nine-slice borders, and the existing Toriko pixel font. A serialized bitmap shader reference prevents stripping the runtime pixel font shader from builds.

Reusable components include sliced windows/insets, section headings, primary/secondary/navigation buttons with focused/pressed/disabled states, resource rows, framed artwork, item slots, and party status. Long dialogue and equipment actions grow vertically instead of shrinking the type. No new gameplay or save format is required.

The Town Gates exploration screen has a compact location header, a full-width aspect-preserved forest preview, cost/balance and reward rows, active party status, and a fixed action footer. The battle action reflects current steps, party health, and encounter state. Secondary hero management and town navigation remain accessible while longer content scrolls. A safe-window component intersects the home content area with the device safe area, preserving space for top resources and bottom navigation.

Town signs, hero management, equipment/shop, inn, travel quest dialogs, and the Buffmage combat prompt use the shared style. Existing building art, forest art, portraits, currency icon, and gameplay callbacks are retained. Battle scenery and combat character/enemy art were not restyled.

The changes are built by the existing adventure runtime controller, so they appear in Play mode. Full runtime/editor compilation passed. The Unity editor could not launch for visual inspection; use **StepQuest → UI → Capture exploration previews** once it opens. The command renders the real exploration builder at 1080×1920 and 720×1600 in an isolated scene, checks fixed-footer bounds, and saves PNGs under `Logs`. Its isolated no-save state intentionally shows an empty party and disabled battle action.
