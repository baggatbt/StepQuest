using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StepQuest.Adventure
{
    public partial class AdventureScreen
    {
        // Normalized town coordinates keep the hub usable on portrait aspect ratios.
        void BuildAdventureTown()
        {
            if (townView == null) return;
            var scenery = Rect("Town paths", townView.transform);
            scenery.SetAsFirstSibling();
            Path(scenery, new Vector2(.46f, .24f), new Vector2(.54f, .77f));
            Path(scenery, new Vector2(.23f, .49f), new Vector2(.77f, .54f));
            Path(scenery, new Vector2(.23f, .50f), new Vector2(.29f, .63f));
            Path(scenery, new Vector2(.71f, .40f), new Vector2(.77f, .53f));
            Path(scenery, new Vector2(.39f, .47f), new Vector2(.61f, .57f));

            var header = ui.Panel(scenery, "Village heading"); header.raycastTarget = false;
            Place(header.rectTransform, new Vector2(.08f, .78f), new Vector2(.92f, .875f));
            var title = Text(header.transform, "BRAMBLE VILLAGE", 28);
            Place(title.rectTransform, new Vector2(.02f, .42f), new Vector2(.98f, .93f));
            title.alignment = TextAlignmentOptions.Center;
            var hint = Text(header.transform, "Tap a building to enter", 19);
            hint.color = ui.Theme.muted;
            Place(hint.rectTransform, new Vector2(.02f, .08f), new Vector2(.98f, .46f));
            hint.alignment = TextAlignmentOptions.Center;

            TownBuilding(heroesBuilding, "GUILD HALL", "Heroes & gear", new Vector2(.10f, .54f), new Vector2(.41f, .75f));
            TownBuilding(equipmentBuilding, "BLACKSMITH", "Equipment shop", new Vector2(.59f, .54f), new Vector2(.90f, .75f));
            TownBuilding(innBuilding, "THE INN", "Rest your party", new Vector2(.59f, .29f), new Vector2(.90f, .49f));
            TownBuilding(travelBuilding, "TRAVEL", "Explore the world", new Vector2(.10f, .29f), new Vector2(.41f, .49f));

            if (randomBattleButton != null)
            {
                var r = randomBattleButton.GetComponent<RectTransform>();
                r.SetParent(townView.transform, false);
                Place(r, new Vector2(.19f, .16f), new Vector2(.81f, .25f));
                ui.ApplyButton(randomBattleButton, RpgButtonKind.Primary);
                StyleTownText(randomBattleLabel, 25);
            }
        }

        void Path(Transform parent, Vector2 min, Vector2 max)
        {
            var edge = Image("Path edge", parent, null);
            edge.color = new Color(.48f, .38f, .20f); edge.raycastTarget = false;
            Place(edge.rectTransform, min, max);
            var fill = Image("Packed earth", edge.transform, null);
            fill.color = new Color(.76f, .65f, .39f); fill.raycastTarget = false;
            fill.rectTransform.offsetMin = Vector2.one * 4 * UiScale;
            fill.rectTransform.offsetMax = Vector2.one * -4 * UiScale;
        }

        void TownBuilding(Button button, string title, string subtitle, Vector2 min, Vector2 max)
        {
            if (button == null || button.image == null) return;
            foreach (var oldLabel in button.GetComponentsInChildren<TMP_Text>(true)) oldLabel.gameObject.SetActive(false);
            var rect = button.GetComponent<RectTransform>();
            rect.SetParent(townView.transform, false); Place(rect, min, max);
            // A transparent button covers both the sprite and its sign, giving a generous tap target.
            var sprite = button.image.sprite;
            button.image.sprite = null; button.image.color = Color.clear;
            var art = Image("Building artwork", rect, sprite);
            art.preserveAspect = true; art.raycastTarget = false;
            Place(art.rectTransform, new Vector2(.12f, .30f), new Vector2(.88f, 1));
            var sign = ui.Panel(rect, "Service sign"); sign.raycastTarget = false;
            Place(sign.rectTransform, new Vector2(-.04f, 0), new Vector2(1.04f, .32f));
            var label = Text(sign.transform, title + "\n<size=75%>" + subtitle + "</size>", 23);
            StyleTownText(label, 23);
            label.rectTransform.offsetMin = new Vector2(6, 4) * UiScale;
            label.rectTransform.offsetMax = new Vector2(-6, -4) * UiScale;
            button.targetGraphic = art;
            button.transition = Selectable.Transition.ColorTint;
            var colors = button.colors;
            colors.highlightedColor = new Color(1, .94f, .75f);
            colors.pressedColor = new Color(.75f, .65f, .45f);
            button.colors = colors;
        }

        void StyleTownText(TMP_Text label, float size)
        {
            if (label == null) return;
            if (font != null) label.font = font;
            label.fontStyle = FontStyles.Normal; label.color = textColor;
            label.alignment = TextAlignmentOptions.Center;
            label.enableAutoSizing = false; label.fontSize = size * UiScale;
            label.characterSpacing = 0; label.lineSpacing = 6;
            label.raycastTarget = false;
        }
    }
}
