using UnityEngine;

namespace StepQuest.Adventure
{
    [CreateAssetMenu(menuName = "StepQuest/UI/RPG theme")]
    public sealed class RpgUiTheme : ScriptableObject
    {
        public Color ink = new Color32(22, 19, 20, 255);
        public Color window = new Color32(43, 36, 33, 255);
        public Color inset = new Color32(31, 29, 28, 255);
        public Color edge = new Color32(111, 87, 60, 255);
        public Color gold = new Color32(180, 150, 95, 255);
        public Color light = new Color32(225, 204, 161, 255);
        public Color muted = new Color32(194, 182, 158, 255);
        public Color accent = new Color32(91, 108, 72, 255);
        public Color warning = new Color32(227, 162, 125, 255);
        public Font pixelFont;
        public Shader pixelFontShader;
        public Sprite windowFrame;
        public Sprite copperIcon;
        [Tooltip("Logical dimensions use a 480-pixel-wide portrait layout.")]
        public float bodySize = 22;
        public float titleSize = 30;
        public float spacing = 8;
    }
}
