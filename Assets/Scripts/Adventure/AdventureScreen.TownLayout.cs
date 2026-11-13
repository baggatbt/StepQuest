using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StepQuest.Adventure
{
    public partial class AdventureScreen
    {
        RectTransform townArtwork;
        // AdventureQuest-style scenic hub: the buildings themselves are tap targets.
        void BuildAdventureTown()
        {
            if (townView == null) return;
            var canvas = townView.GetComponentInParent<Canvas>();
            if (canvas != null) canvas.pixelPerfect = true;
            var scenery = Rect("Bramble village scenery", townView.transform);
            // Cover retained prototype artwork/controls; the four live service
            // buttons are moved above this surface below. Legacy objects stay intact.
            scenery.SetAsLastSibling();
            scenery.gameObject.AddComponent<RectMask2D>();
            var viewport = Rect("Village viewport", scenery);
            Place(viewport, new Vector2(0, .205f), new Vector2(1, .89f));
            viewport.gameObject.AddComponent<RectMask2D>();
            var backdrop = Image("Village panorama", viewport, Resources.Load<Sprite>("SQ Assets/UI/townPNG"));
            townArtwork = backdrop.rectTransform;
            backdrop.raycastTarget = true;
            if (backdrop.sprite != null)
            {
                var fit = backdrop.gameObject.AddComponent<AspectRatioFitter>();
                fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
                fit.aspectRatio = backdrop.sprite.rect.width / backdrop.sprite.rect.height;
            }
            else backdrop.color = new Color32(64, 77, 52, 255);
            TownBand(scenery, "Resource surround", .89f, 1);
            TownBand(scenery, "Gate and navigation surround", 0, .205f);

            var header = ui.Panel(scenery, "Village heading"); header.raycastTarget = false;
            Place(header.rectTransform, new Vector2(.035f, .83f), new Vector2(.965f, .885f));
            var title = Text(header.transform, "BRAMBLE VILLAGE", 44);
            StyleTownText(title, 44);
            Place(title.rectTransform, new Vector2(.025f, .10f), new Vector2(.975f, .90f));

            // Artwork-space coordinates keep signs attached to the same roofs
            // when the portrait viewport crops the illustration differently.
            TownBuilding(heroesBuilding, "GUILD HALL", new Vector2(.035f, .43f), new Vector2(.46f, .74f));
            TownBuilding(equipmentBuilding, "BLACKSMITH", new Vector2(.55f, .43f), new Vector2(.98f, .63f));
            TownBuilding(innBuilding, "THE INN", new Vector2(.64f, .10f), new Vector2(.99f, .365f));
            TownBuilding(travelBuilding, "WORLD MAP", new Vector2(.035f, .10f), new Vector2(.47f, .275f));

            if (randomBattleButton != null)
            {
                var r = randomBattleButton.GetComponent<RectTransform>();
                r.SetParent(townView.transform, false);
                r.SetAsLastSibling();
                Place(r, new Vector2(.035f, .105f), new Vector2(.965f, .195f));
                ui.ApplyButton(randomBattleButton, RpgButtonKind.Primary);
                ui.ApplyNameplate(randomBattleButton.image, true);
                StyleTownText(randomBattleLabel, 38);
                if (randomBattleLabel != null)
                {
                    RpgUi.Stretch(randomBattleLabel.rectTransform);
                    randomBattleLabel.rectTransform.offsetMin = new Vector2(12, 5) * UiScale;
                    randomBattleLabel.rectTransform.offsetMax = new Vector2(-12, -5) * UiScale;
                }
            }
        }

        void TownBand(Transform parent, string name, float min, float max)
        {
            var band = Image(name, parent, null); band.color = ui.Theme.inset; band.raycastTarget = false;
            Place(band.rectTransform, new Vector2(0, min), new Vector2(1, max));
        }

        void TownBuilding(Button button, string title, Vector2 min, Vector2 max)
        {
            if (button == null || button.image == null) return;
            foreach (var oldLabel in button.GetComponentsInChildren<TMP_Text>(true)) oldLabel.gameObject.SetActive(false);
            var rect = button.GetComponent<RectTransform>();
            rect.SetParent(townArtwork, false); Place(rect, min, max); rect.SetAsLastSibling();
            button.image.sprite = null; button.image.color = Color.clear; button.image.raycastTarget = true;
            var plaque = ui.Nameplate(rect, "Building title - " + title);
            // The plaque itself is clickable even where it extends above the
            // generous building hit area. The existing service opens directly.
            plaque.raycastTarget = true;
            plaque.rectTransform.anchorMin = plaque.rectTransform.anchorMax = new Vector2(.5f, 1);
            plaque.rectTransform.pivot = new Vector2(.5f, 0);
            plaque.rectTransform.anchoredPosition = new Vector2(0, Mathf.Round(4 * UiScale));
            var label = Text(plaque.transform, title, 36); StyleTownText(label, 36);
            label.color = new Color32(255, 235, 193, 255);
            label.textWrappingMode = TextWrappingModes.NoWrap;
            float availableWidth = rect.rect.width > 0 ? rect.rect.width * .98f : 205 * UiScale;
            plaque.rectTransform.sizeDelta = new Vector2(Mathf.Round(Mathf.Min(160 * UiScale, availableWidth)), Mathf.Round(30 * UiScale));
            label.rectTransform.offsetMin = new Vector2(Mathf.Round(5 * UiScale), Mathf.Round(UiScale));
            label.rectTransform.offsetMax = -label.rectTransform.offsetMin;
            button.targetGraphic = plaque; button.transition = Selectable.Transition.ColorTint;
            var colors = button.colors;
            colors.normalColor = Color.white; colors.highlightedColor = new Color(1, .94f, .75f);
            colors.pressedColor = new Color(.65f, .64f, .48f); colors.fadeDuration = 0; button.colors = colors;
        }

        void StyleTownText(TMP_Text label, float size)
        {
            if (label == null) return;
            if (font != null) label.font = font;
            label.fontStyle = FontStyles.Normal; label.color = textColor;
            label.alignment = TextAlignmentOptions.Midline;
            label.enableAutoSizing = false; label.fontSize = Mathf.Round(size * UiScale);
            label.characterSpacing = 0; label.lineSpacing = 3; label.raycastTarget = false;
        }
    }
}
