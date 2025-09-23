using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BasicMapGenerator : MonoBehaviour
{
    [Header("Output")]
    public int textureSize = 512;      // 256–1024 is fine
    public int seed = 12345;           // change for a different map
    public float noiseScale = 2.2f;    // bigger = larger continents
    public float detailScale = 8f;     // small features overlay
    public Gradient terrainGradient;   // colors across biomes (0..1)
    public float waterLevel = 0.38f;   // threshold for water
    public float beachBand = 0.03f;    // sand band above water
    public Color forestTint = new Color(0.08f, 0.18f, 0.08f, 0.55f);
    public float forestThreshold = 0.68f;  // where to start tinting darker “forest”

    [Header("Assignment")]
    public bool assignToRenderer = true;   // set material.mainTexture
    public bool setPointFilter = true;     // crisp pixels

    Texture2D generated;
    MeshRenderer mr;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        if (terrainGradient == null || terrainGradient.colorKeys.Length == 0)
            terrainGradient = DefaultGradient();

        generated = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false, false);
        generated.wrapMode = TextureWrapMode.Repeat;
        if (setPointFilter) generated.filterMode = FilterMode.Point;

        FillTextureTiled(generated, seed, noiseScale, detailScale, terrainGradient, waterLevel, beachBand, forestThreshold, forestTint);

        if (assignToRenderer && mr)
        {
            var mat = mr.material; // instance
            mat.mainTexture = generated;
            if (mat.mainTexture) mat.mainTexture.wrapMode = TextureWrapMode.Repeat;
        }
    }

    static void FillTextureTiled(
        Texture2D tex, int seed, float scale, float detail, Gradient grad,
        float water, float beach, float forestT, Color forestTint)
    {
        int W = tex.width, H = tex.height;
        var prng = new System.Random(seed);
        float ox = (float)prng.NextDouble() * 10000f;
        float oy = (float)prng.NextDouble() * 10000f;
        float ox2 = (float)prng.NextDouble() * 10000f;
        float oy2 = (float)prng.NextDouble() * 10000f;

        // precompute for speed
        Color[] pixels = new Color[W * H];

        for (int y = 0; y < H; y++)
        {
            for (int x = 0; x < W; x++)
            {
                // Tileable perlin: blend 4 samples so edges match (torus trick)
                float u = (float)x / W;
                float v = (float)y / H;

                float nBase = TileableNoise(u, v, ox, oy, scale);
                float nDetail = TileableNoise(u, v, ox2, oy2, detail);

                // combine noises
                float n = Mathf.Clamp01(nBase * 0.85f + nDetail * 0.15f);

                // Choose terrain color
                Color c;
                if (n < water)
                {
                    // deeper water = darker
                    float t = Mathf.InverseLerp(0f, water, n);
                    c = Color.Lerp(new Color(0.02f, 0.08f, 0.18f), new Color(0.07f, 0.2f, 0.35f), t);
                }
                else if (n < water + beach)
                {
                    // beach band
                    float t = Mathf.InverseLerp(water, water + beach, n);
                    c = Color.Lerp(new Color(0.80f, 0.72f, 0.45f), new Color(0.88f, 0.82f, 0.55f), t);
                }
                else
                {
                    // land via gradient
                    float t = Mathf.InverseLerp(water + beach, 1f, n);
                    c = grad.Evaluate(t);

                    // darker tint for “forest” highlands
                    if (n > forestT)
                    {
                        float ft = Mathf.InverseLerp(forestT, 1f, n);
                        c = Color.Lerp(c, new Color(
                            Mathf.Clamp01(c.r + forestTint.r * ft),
                            Mathf.Clamp01(c.g + forestTint.g * ft),
                            Mathf.Clamp01(c.b + forestTint.b * ft),
                            1f), 0.6f * ft);
                    }
                }

                pixels[y * W + x] = c;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply(false, false);
    }

    // Blend 4 offset perlin samples so the tile’s edges are seamless
    static float TileableNoise(float u, float v, float ox, float oy, float scale)
    {
        // Scale UVs to world space
        float x = (u * scale) + ox;
        float y = (v * scale) + oy;

        // Period = 1 in uv-space; blend edges
        float n00 = Mathf.PerlinNoise(x, y);
        float n10 = Mathf.PerlinNoise(x + scale, y);
        float n01 = Mathf.PerlinNoise(x, y + scale);
        float n11 = Mathf.PerlinNoise(x + scale, y + scale);

        float sx = Smoothstep(u);
        float sy = Smoothstep(v);

        float nx0 = Mathf.Lerp(n00, n10, sx);
        float nx1 = Mathf.Lerp(n01, n11, sx);
        return Mathf.Lerp(nx0, nx1, sy);
    }

    static float Smoothstep(float t) => t * t * (3f - 2f * t);

    static Gradient DefaultGradient()
    {
        var g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.40f, 0.65f, 0.35f), 0.0f), // grass
                new GradientColorKey(new Color(0.45f, 0.70f, 0.38f), 0.35f),
                new GradientColorKey(new Color(0.50f, 0.75f, 0.42f), 0.6f),
                new GradientColorKey(new Color(0.55f, 0.68f, 0.46f), 0.8f),
                new GradientColorKey(new Color(0.62f, 0.62f, 0.55f), 1.0f), // rocky
            },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
        );
        return g;
    }
}
