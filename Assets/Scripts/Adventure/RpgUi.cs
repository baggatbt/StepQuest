using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace StepQuest.Adventure
{
    public enum RpgButtonKind { Primary, Secondary, Navigation }

    // Shared component factory: every adventure screen uses the same palette,
    // sliced windows, type scale, slots, information rows, and button states.
    public sealed class RpgUi : IDisposable
    {
        public readonly RpgUiTheme Theme;
        public readonly TMP_FontAsset Font;
        public readonly Sprite Window, Inset;
        readonly List<Sprite> sprites = new List<Sprite>();
        readonly List<Texture2D> textures = new List<Texture2D>();
        readonly Sprite primary, secondary, navigation, pressed, disabled, highlight;
        readonly bool ownsTheme;
        readonly bool usesExistingWindow;
        public float Scale { get; set; }

        public RpgUi(float scale)
        {
            Scale = scale;
            Theme = Resources.Load<RpgUiTheme>("Adventure/RpgUiTheme");
            if (Theme == null) { Theme = ScriptableObject.CreateInstance<RpgUiTheme>(); ownsTheme = true; }
            var source = Theme.pixelFont != null ? Theme.pixelFont : Resources.Load<Font>(
                "Sprites/GUI/Complete_UI_Essential_Pack_v2.3/Font/TTF (16px)/Toriko");
            // Raster atlas at its designed pixel size. Integer font sizes and point
            // filtering preserve the actual pixel glyphs instead of smoothing them.
            if (source != null)
            {
                Font = TMP_FontAsset.CreateFontAsset(source, 16, 1, GlyphRenderMode.RASTER_HINTED,
                    512, 512, AtlasPopulationMode.Dynamic);
                if (Theme.pixelFontShader != null) Font.material.shader = Theme.pixelFontShader;
                Font.atlasTextures[0].filterMode = FilterMode.Point;
            }
            var sourceFrame = Theme.windowFrame != null ? Theme.windowFrame : Resources.Load<Sprite>("Sprites/GUI/ui/RectangleBox_96x96");
            if (sourceFrame != null)
            {
                // Preserve the pack's pixel corner details while correcting its
                // original three-pixel slice, which cuts through the gold rim.
                Window = Sprite.Create(sourceFrame.texture, sourceFrame.rect, Vector2.one * .5f, 100,
                    0, SpriteMeshType.FullRect, Vector4.one * 9);
                Window.name = "RPG existing gold window"; Window.hideFlags = HideFlags.DontSave;
                sprites.Add(Window); usesExistingWindow = true;
            }
            else Window = Frame("RPG window", Theme.window, Theme.gold, true);
            Inset = Frame("RPG inset", Theme.inset, Theme.edge, false);
            primary = Frame("RPG primary", Theme.accent, Theme.light, true);
            secondary = Frame("RPG secondary", new Color32(66, 53, 42, 255), Theme.gold, true);
            navigation = Frame("RPG navigation", Theme.inset, Theme.edge, false);
            highlight = Frame("RPG focused", new Color32(91, 78, 53, 255), Theme.light, true);
            pressed = Frame("RPG pressed", new Color32(45, 45, 33, 255), Theme.gold, false);
            disabled = Frame("RPG disabled", new Color32(44, 42, 39, 255), new Color32(91, 82, 68, 255), false);
        }

        Sprite Frame(string name, Color fill, Color border, bool studs)
        {
            const int size = 24;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) {
                name = name, filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave
            };
            var pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                int distance = Mathf.Min(Mathf.Min(x, y), Mathf.Min(size - 1 - x, size - 1 - y));
                Color color = distance == 0 ? Theme.ink : distance == 1 ? border
                    : distance == 2 ? Theme.edge : distance == 3 ? Theme.ink : fill;
                if ((x == 0 || x == size - 1) && (y == 0 || y == size - 1)) color = Color.clear;
                if (studs && (x == 2 || x == size - 3) && (y == 2 || y == size - 3)) color = Theme.light;
                pixels[y * size + x] = color;
            }
            texture.SetPixels(pixels); texture.Apply(); textures.Add(texture);
            var sprite = Sprite.Create(texture, new Rect(0, 0, size, size), Vector2.one * .5f, 100,
                0, SpriteMeshType.FullRect, Vector4.one * 5);
            sprite.name = name; sprite.hideFlags = HideFlags.DontSave; sprites.Add(sprite);
            return sprite;
        }

        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = parent.gameObject.layer; go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>(); Stretch(rect); return rect;
        }
        public static void Stretch(RectTransform rect)
        { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }

        public Image Panel(Transform parent, string name, bool inset = false)
        {
            var image = Rect(name, parent).gameObject.AddComponent<Image>();
            ApplyFrame(image, inset ? Inset : Window); return image;
        }
        public void ApplyFrame(Image image, Sprite sprite)
        {
            image.sprite = sprite; image.color = sprite == Window && usesExistingWindow
                ? new Color(1f, .94f, .80f) : Color.white;
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 1f / ((sprite == Window && usesExistingWindow ? 1 : 2) * Scale);
        }
        public TMP_Text Text(Transform parent, string value, float size, Color? color = null)
        {
            var label = Rect("Text", parent).gameObject.AddComponent<TextMeshProUGUI>();
            if (Font != null) label.font = Font;
            label.text = value; label.fontSize = Mathf.Round(size * Scale);
            label.fontStyle = FontStyles.Normal; label.color = color ?? Theme.light;
            label.raycastTarget = false; label.lineSpacing = 5;
            label.textWrappingMode = TextWrappingModes.Normal;
            label.overflowMode = TextOverflowModes.Overflow;
            return label;
        }
        public VerticalLayoutGroup Stack(Transform parent, float padding = 12, float gap = 8)
        {
            var layout = parent.gameObject.AddComponent<VerticalLayoutGroup>();
            int p = Mathf.RoundToInt(padding * Scale);
            layout.padding = new RectOffset(p, p, p, p); layout.spacing = gap * Scale;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = true; layout.childForceExpandHeight = false;
            return layout;
        }
        public void Height(Component component, float height)
        {
            var sizing = component.GetComponent<LayoutElement>() ?? component.gameObject.AddComponent<LayoutElement>();
            sizing.minHeight = sizing.preferredHeight = height * Scale; sizing.flexibleHeight = 0;
            var buttonLayout = component.GetComponent<RpgButtonLayout>();
            if (buttonLayout != null) buttonLayout.minimumHeight = height * Scale;
        }
        public TMP_Text Heading(Transform parent, string value)
        {
            var label = Text(parent, value, Theme.titleSize); Height(label, 38); return label;
        }
        public TMP_Text Paragraph(Transform parent, string value)
        {
            var label = Text(parent, value, Theme.bodySize, Theme.muted);
            label.gameObject.AddComponent<LayoutElement>().minHeight = 30 * Scale;
            return label;
        }
        public Button Button(Transform parent, string title, UnityAction action, RpgButtonKind kind = RpgButtonKind.Secondary)
        {
            var panel = Panel(parent, title); var button = panel.gameObject.AddComponent<Button>();
            ApplyButton(button, kind); button.onClick.AddListener(action);
            Height(button, kind == RpgButtonKind.Primary ? 60 : kind == RpgButtonKind.Navigation ? 48 : 52);
            var label = Text(panel.transform, title, kind == RpgButtonKind.Primary ? 25 : 22);
            label.alignment = TextAlignmentOptions.Center;
            label.margin = new Vector4(14, 6, 14, 6) * Scale;
            var layout = panel.gameObject.AddComponent<RpgButtonLayout>();
            layout.label = label; layout.minimumHeight = panel.GetComponent<LayoutElement>().minHeight;
            layout.padding = 14 * Scale;
            var visual = panel.gameObject.AddComponent<RpgButtonState>();
            visual.Initialize(button, label, Theme.light, Theme.muted);
            return button;
        }
        public void ApplyButton(Button button, RpgButtonKind kind)
        {
            var graphic = button.GetComponent<Image>();
            ApplyFrame(graphic, kind == RpgButtonKind.Primary ? primary : kind == RpgButtonKind.Navigation ? navigation : secondary);
            graphic.raycastTarget = true; button.targetGraphic = graphic;
            button.transition = Selectable.Transition.SpriteSwap;
            button.spriteState = new SpriteState { highlightedSprite = highlight, selectedSprite = highlight,
                pressedSprite = pressed, disabledSprite = disabled };
            var navigationSettings = button.navigation; navigationSettings.mode = Navigation.Mode.Automatic;
            button.navigation = navigationSettings;
        }
        public Image Slot(Transform parent, Sprite icon, float size = 44)
        {
            var panel = Panel(parent, "Icon slot", true); Height(panel, size);
            panel.gameObject.AddComponent<LayoutElement>().preferredWidth = size * Scale;
            var image = Rect("Icon", panel.transform).gameObject.AddComponent<Image>();
            image.sprite = icon; image.preserveAspect = true; image.raycastTarget = false;
            image.rectTransform.offsetMin = Vector2.one * 6 * Scale;
            image.rectTransform.offsetMax = Vector2.one * -6 * Scale;
            if (icon == null) image.enabled = false;
            return panel;
        }
        public TMP_Text InfoRow(Transform parent, string title, string value, Sprite icon = null)
        {
            var panel = Panel(parent, title, true); Height(panel, 42);
            var label = Text(panel.transform, title, 20, Theme.muted);
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.rectTransform.anchorMax = new Vector2(.40f, 1);
            label.margin = new Vector4(icon == null ? 12 : 40, 4, 0, 4) * Scale;
            var detail = Text(panel.transform, value, 21);
            detail.alignment = TextAlignmentOptions.MidlineRight;
            detail.rectTransform.anchorMin = new Vector2(.40f, 0);
            detail.margin = new Vector4(0, 4, 12, 4) * Scale;
            if (icon != null)
            {
                var art = Rect("Resource icon", panel.transform).gameObject.AddComponent<Image>();
                art.sprite = icon; art.preserveAspect = true; art.raycastTarget = false;
                art.rectTransform.anchorMin = new Vector2(0, .15f); art.rectTransform.anchorMax = new Vector2(0, .85f);
                art.rectTransform.offsetMin = new Vector2(10 * Scale, 0); art.rectTransform.offsetMax = new Vector2(34 * Scale, 0);
            }
            return detail;
        }
        public void PartyMember(Transform parent, Sprite icon, string name, int level, int hp, int maxHp)
        {
            var row = Panel(parent, "Party member", true); Height(row, 60);
            var art = Slot(row.transform, icon, 48);
            art.rectTransform.anchorMin = art.rectTransform.anchorMax = new Vector2(0, .5f);
            art.rectTransform.pivot = new Vector2(0, .5f);
            art.rectTransform.anchoredPosition = new Vector2(8 * Scale, 0);
            art.rectTransform.sizeDelta = new Vector2(48, 48) * Scale;
            var label = Text(row.transform, name + "  <color=#B4A78F>Lv " + level + "</color>", 21);
            label.rectTransform.offsetMin = new Vector2(66, 30) * Scale;
            label.rectTransform.offsetMax = new Vector2(-10, -5) * Scale;
            var health = Text(row.transform, "HP " + hp + " / " + maxHp, 18, Theme.muted);
            health.rectTransform.offsetMin = new Vector2(66, 7) * Scale;
            health.rectTransform.offsetMax = new Vector2(-10, -32) * Scale;
        }
        public void Dispose()
        {
            foreach (var sprite in sprites) Destroy(sprite);
            foreach (var texture in textures) Destroy(texture);
            if (Font != null)
            {
                foreach (var atlas in Font.atlasTextures) Destroy(atlas);
                Destroy(Font.material); Destroy(Font);
            }
            if (ownsTheme) Destroy(Theme);
        }
        static void Destroy(UnityEngine.Object value)
        { if (Application.isPlaying) UnityEngine.Object.Destroy(value); else UnityEngine.Object.DestroyImmediate(value); }
    }
}
