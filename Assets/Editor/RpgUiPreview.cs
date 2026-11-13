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

    // Lets a developer request an isolated capture from the workspace while
    // Unity is already open. It never enters Play mode or changes the open scene.
    [InitializeOnLoadMethod]
    static void CaptureWorkspaceRequest()
    {
        EditorApplication.update += CaptureWhenReady;
        void CaptureWhenReady()
        {
            const string request = "Temp/StepQuestTownPreview.request";
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying) return;
            EditorApplication.update -= CaptureWhenReady;
            if (!File.Exists(request)) return;
            File.Delete(request);
            try
            {
                // The disabled preview controller cannot run Start/Update, even
                // if gameplay is open. Only its pure town builder is invoked.
                CapturePortrait(1080, 1920, true);
                CapturePortrait(720, 1600, true);
                File.WriteAllText("Logs/TownUiPreviewResult.txt", "PASS: both town resolutions rendered and validated.");
            }
            catch (Exception error)
            {
                Directory.CreateDirectory("Logs");
                File.WriteAllText("Logs/TownUiPreviewResult.txt", error.ToString());
                Debug.LogException(error);
            }
        }
    }

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

    [MenuItem("StepQuest/UI/Capture town previews")]
    public static void CaptureTown()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode before capturing previews.");
        CapturePortrait(1080, 1920, true);
        CapturePortrait(720, 1600, true);
        Debug.Log("Town previews saved to Logs/TownUi-1080x1920.png and Logs/TownUi-720x1600.png. Existing global HUD/navigation are outside this isolated town preview.");
    }

    static void CapturePortrait(int width, int height, bool town = false)
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
            screen.enabled = false;
            ui = new RpgUi(width / 480f);
            Set(screen, "ui", ui);
            Set(screen, "dialogue", Resources.Load<AdventureDialogue>("Adventure/FirstAdventure"));
            Set(screen, "townHub", Resources.Load<TownHubDefinition>("Adventure/TownHub"));
            RectTransform window;
            if (town)
            {
                Set(screen, "font", ui.Font); Set(screen, "textColor", ui.Theme.light);
                Set(screen, "townView", root.gameObject);
                foreach (string field in new[] { "heroesBuilding", "equipmentBuilding", "innBuilding", "travelBuilding", "randomBattleButton" })
                {
                    var button = ui.Button(root, field, () => { });
                    Set(screen, field, button);
                    if (field == "randomBattleButton") Set(screen, "randomBattleLabel", button.GetComponentInChildren<TMP_Text>());
                }
                typeof(AdventureScreen).GetMethod("BuildAdventureTown", Private).Invoke(screen, null);
                typeof(AdventureScreen).GetMethod("RefreshTownBalance", Private).Invoke(screen, null);
                window = root;
            }
            else
            {
                typeof(AdventureScreen).GetMethod("ShowRandomBattle", Private).Invoke(screen, null);
                var safe = root.GetComponentInChildren<RpgSafeWindow>();
                safe.enabled = false;
                window = safe.GetComponent<RectTransform>();
                window.anchorMin = new Vector2(.035f, .14f); window.anchorMax = new Vector2(.965f, .855f);
                window.offsetMin = window.offsetMax = Vector2.zero;
            }
            Canvas.ForceUpdateCanvases();
            foreach (var text in root.GetComponentsInChildren<TMP_Text>()) text.ForceMeshUpdate();
            LayoutRebuilder.ForceRebuildLayoutImmediate(window);
            Canvas.ForceUpdateCanvases();
            if (town) VerifyTown(window); else VerifyFooter(window);

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
            File.WriteAllBytes("Logs/" + (town ? "TownUi-" : "RpgUi-") + width + "x" + height + ".png", capture.EncodeToPNG());
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

    static void VerifyTown(RectTransform root)
    {
        var buttons = root.GetComponentsInChildren<Button>();
        if (root.Find("Bramble village scenery/Travel party") != null)
            throw new InvalidOperationException("The main village must not display a party plate.");
        if (buttons.Length != 5) throw new InvalidOperationException("Town must retain four directly accessible services and the gates.");
        if (root.Find("Bramble village scenery/Village viewport/Village panorama").GetComponent<Image>().sprite == null)
            throw new InvalidOperationException("Village artwork is missing.");
        foreach (var button in buttons)
        {
            var rect = button.GetComponent<RectTransform>();
            if (rect.rect.width < 100 || rect.rect.height < 60)
                throw new InvalidOperationException("Town tap target is too small: " + button.name);
        }
        foreach (var label in root.GetComponentsInChildren<TMP_Text>())
            if (label.isTextOverflowing) throw new InvalidOperationException("Town text overflows: " + label.text);
    }
}
