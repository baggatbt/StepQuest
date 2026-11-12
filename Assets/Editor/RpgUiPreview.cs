using System;
using System.IO;
using System.Reflection;
using StepQuest.Adventure;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// Renders the real exploration builder in an isolated preview scene. It never
// starts gameplay, loads/saves player data, or changes the open scene.
public static class RpgUiPreview
{
    const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

    [MenuItem("StepQuest/UI/Capture exploration previews")]
    public static void Capture()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode before capturing previews.");
        CapturePortrait(1080, 1920);
        CapturePortrait(720, 1600);
        Debug.Log("Exploration previews saved to Logs/RpgUi-1080x1920.png and Logs/RpgUi-720x1600.png. The isolated preview has no saved party, so the battle action is disabled.");
    }

    static void Set(AdventureScreen screen, string name, object value)
    { typeof(AdventureScreen).GetField(name, Private).SetValue(screen, value); }

    static void CapturePortrait(int width, int height)
    {
        var scene = EditorSceneManager.NewPreviewScene();
        var pipeline = GraphicsSettings.defaultRenderPipeline;
        var qualityPipeline = QualitySettings.renderPipeline;
        RenderTexture target = null;
        Texture2D capture = null;
        RpgUi ui = null;
        AdventureScreen screen = null;
        var previousTarget = RenderTexture.active;
        try
        {
            // A private world-space Canvas has deterministic pixel dimensions
            // regardless of the current Game view size or the editor monitor.
            var canvasGo = new GameObject("UI capture canvas", typeof(RectTransform), typeof(Canvas));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(canvasGo, scene);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.pixelPerfect = true;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
            canvasGo.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            var cameraGo = new GameObject("UI capture camera", typeof(Camera));
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(cameraGo, scene);
            var camera = cameraGo.GetComponent<Camera>();
            camera.overrideSceneCullingMask = EditorSceneManager.GetSceneCullingMask(scene);
            camera.orthographic = true; camera.orthographicSize = height / 2f;
            camera.aspect = (float)width / height;
            camera.transform.position = new Vector3(0, 0, -100);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color32(29, 27, 25, 255);
            canvas.worldCamera = camera;

            var root = RpgUi.Rect("Adventure capture", canvasGo.transform);
            screen = root.gameObject.AddComponent<AdventureScreen>();
            ui = new RpgUi(width / 480f);
            Set(screen, "ui", ui);
            Set(screen, "dialogue", Resources.Load<AdventureDialogue>("Adventure/FirstAdventure"));
            Set(screen, "townHub", Resources.Load<TownHubDefinition>("Adventure/TownHub"));
            typeof(AdventureScreen).GetMethod("ShowRandomBattle", Private).Invoke(screen, null);
            var safe = root.GetComponentInChildren<RpgSafeWindow>();
            safe.enabled = false;
            var window = safe.GetComponent<RectTransform>();
            window.anchorMin = new Vector2(.035f, .14f); window.anchorMax = new Vector2(.965f, .855f);
            window.offsetMin = window.offsetMax = Vector2.zero;
            Canvas.ForceUpdateCanvases();
            foreach (var text in root.GetComponentsInChildren<TMP_Text>()) text.ForceMeshUpdate();
            LayoutRebuilder.ForceRebuildLayoutImmediate(window);
            Canvas.ForceUpdateCanvases();
            VerifyFooter(window);

            // Built-in rendering is used only for this off-screen UI capture;
            // restore both pipeline overrides even if rendering fails.
            GraphicsSettings.defaultRenderPipeline = null;
            QualitySettings.renderPipeline = null;
            target = new RenderTexture(width, height, 24) { antiAliasing = 1 };
            camera.targetTexture = target; camera.Render();
            RenderTexture.active = target;
            capture = new Texture2D(width, height, TextureFormat.RGB24, false);
            capture.ReadPixels(new Rect(0, 0, width, height), 0, 0); capture.Apply();
            Directory.CreateDirectory("Logs");
            File.WriteAllBytes("Logs/RpgUi-" + width + "x" + height + ".png", capture.EncodeToPNG());
        }
        finally
        {
            RenderTexture.active = previousTarget;
            QualitySettings.renderPipeline = qualityPipeline;
            GraphicsSettings.defaultRenderPipeline = pipeline;
            if (capture != null) Object.DestroyImmediate(capture);
            if (target != null) { target.Release(); Object.DestroyImmediate(target); }
            // Ownership belongs to this preview, not to the inactive controller.
            if (screen != null) Set(screen, "ui", null);
            EditorSceneManager.ClosePreviewScene(scene);
            ui?.Dispose();
        }
    }

    static void VerifyFooter(RectTransform window)
    {
        var footer = window.Find("Fixed actions") as RectTransform;
        if (footer == null) throw new InvalidOperationException("Exploration actions are missing.");
        var corners = new Vector3[4];
        foreach (var button in footer.GetComponentsInChildren<Button>())
        {
            button.GetComponent<RectTransform>().GetWorldCorners(corners);
            foreach (var corner in corners)
            {
                var point = footer.InverseTransformPoint(corner);
                if (point.y < footer.rect.yMin - 1 || point.y > footer.rect.yMax + 1)
                    throw new InvalidOperationException("Exploration action exceeds the fixed footer: " + button.name);
            }
        }
        if (footer.GetComponentsInChildren<Button>().Length != 3)
            throw new InvalidOperationException("Exploration must retain battle, heroes, and town actions.");
    }
}
