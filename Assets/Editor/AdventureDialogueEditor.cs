using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;
using System.Linq;
using StepQuest.Adventure;

[CustomEditor(typeof(AdventureDialogue))]
public class AdventureDialogueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Edit text and replies below. Keep node IDs stable. An empty Next Node returns to the village. Quest rewards are granted only once.", MessageType.Info);
        DrawDefaultInspector();
        if (GUILayout.Button("Validate conversation")) Validate((AdventureDialogue)target);
    }
    public static bool Validate(AdventureDialogue data)
    {
        bool valid = true;
        var ids = new HashSet<string>();
        foreach (var node in data.nodes)
            if (string.IsNullOrWhiteSpace(node.id) || !ids.Add(node.id))
            { Debug.LogError("Missing or duplicate dialogue node ID: " + node.id, data); valid = false; }
        foreach (var entry in new[] { data.offerNode, data.activeNode, data.victoryNode, data.completeNode })
            if (!ids.Contains(entry)) { Debug.LogError("Missing entry node: " + entry, data); valid = false; }
        foreach (var node in data.nodes)
            foreach (var reply in node.replies)
                if (!string.IsNullOrEmpty(reply.nextNode) && !ids.Contains(reply.nextNode))
                { Debug.LogError("Missing reply destination: " + reply.nextNode, data); valid = false; }
        if (data.copperReward < 0 || string.IsNullOrWhiteSpace(data.encounterStageId))
        { Debug.LogError("Reward must be nonnegative and encounter stage ID is required.", data); valid = false; }
        if (data.forestBackdrop == null || data.npcPortrait == null)
        { Debug.LogError("Assign the forest background and NPC portrait.", data); valid = false; }
        if (valid) Debug.Log("Adventure dialogue validation passed: " + data.name, data);
        return valid;
    }
    [MenuItem("StepQuest/Select first adventure")]
    public static void SelectAdventure()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath<AdventureDialogue>("Assets/Resources/Adventure/FirstAdventure.asset");
        EditorGUIUtility.PingObject(Selection.activeObject);
    }
    [MenuItem("StepQuest/Repair quest location wiring")]
    public static void RepairQuestWiring()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.name != "CharacterInfoPage")
            throw new System.InvalidOperationException("Open CharacterInfoPage before repairing quest wiring.");
        var objects = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<Transform>(true)).ToArray();
        Transform Find(string name) => objects.Single(t => t.name == name);
        var home = Find("HomePageCanvasGroup");
        var controller = home.GetComponent<AdventureScreen>();
        if (controller == null) controller = Undo.AddComponent<AdventureScreen>(home.gameObject);
        var map = Find("MapScrollView").GetComponent<ScrollRect>();
        var location = Find("LocationOnePanel");
        var town = objects.Single(t => t.name == "TownOne" && t.parent == location);
        var forest = Find("BattleNode1-1");
        var panel = Find("NodeInfoPanel1-1");
        var templateButton = panel.Find("StartBattleButton").GetComponent<Button>();
        var mayor = location.Find("Mayor Bramble");
        if (mayor == null)
        {
            var go = UnityEngine.UI.DefaultControls.CreateButton(new DefaultControls.Resources());
            Undo.RegisterCreatedObjectUndo(go, "Create mayor map location");
            go.name = "Mayor Bramble"; go.transform.SetParent(location, false); mayor = go.transform;
            // Use the existing travel button artwork; do not duplicate a map or Canvas.
            var image = go.GetComponent<Image>(); image.sprite = templateButton.image.sprite;
            image.type = Image.Type.Sliced; image.color = templateButton.image.color;
            foreach (var text in go.GetComponentsInChildren<UnityEngine.UI.Text>(true))
                Undo.DestroyObjectImmediate(text.gameObject);
            var portraitGo = new GameObject("Mayor portrait", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            Undo.RegisterCreatedObjectUndo(portraitGo, "Create mayor portrait"); portraitGo.transform.SetParent(mayor, false);
            var portrait = portraitGo.GetComponent<Image>();
            portrait.sprite = AssetDatabase.LoadAssetAtPath<AdventureDialogue>("Assets/Resources/Adventure/FirstAdventure.asset").npcPortrait;
            portrait.preserveAspect = true; portrait.raycastTarget = false;
            var portraitRect = portrait.rectTransform; portraitRect.anchorMin = new Vector2(.20f, .32f); portraitRect.anchorMax = new Vector2(.80f, .94f); portraitRect.offsetMin = portraitRect.offsetMax = Vector2.zero;
            var textGo = new GameObject("Mayor label", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(textGo, "Create mayor label"); textGo.transform.SetParent(mayor, false);
            var label = textGo.AddComponent<TextMeshProUGUI>(); label.text = "MAYOR\nBRAMBLE";
            label.font = TMP_Settings.defaultFontAsset; label.fontSize = 105; label.enableAutoSizing = true; label.fontSizeMin = 65; label.fontSizeMax = 105;
            label.color = Color.white; label.alignment = TextAlignmentOptions.Center; label.raycastTarget = false;
            var textRect = label.rectTransform; textRect.anchorMin = new Vector2(.02f, .02f); textRect.anchorMax = new Vector2(.98f, .32f); textRect.offsetMin = textRect.offsetMax = Vector2.zero;
        }
        var mayorImage = mayor.GetComponent<Image>();
        mayorImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Sprites/UI Icons/SquareWoodPanel.png");
        mayorImage.type = Image.Type.Simple;
        var mayorLabel = mayor.GetComponentInChildren<TMP_Text>(true);
        var fontSource = forest.GetComponentInChildren<TMP_Text>(true);
        if (fontSource != null) mayorLabel.font = fontSource.font;
        mayorLabel.color = new Color(.18f, .10f, .06f);
        EditorUtility.SetDirty(mayorImage); EditorUtility.SetDirty(mayorLabel);
        var rect = mayor.GetComponent<RectTransform>();
        Undo.RecordObject(rect, "Position mayor on travel map");
        rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f); rect.pivot = new Vector2(.5f, .5f);
        rect.anchoredPosition = new Vector2(-650, -1850); rect.sizeDelta = new Vector2(500, 1000); rect.localScale = Vector3.one;
        mayor.gameObject.SetActive(true);
        var mayorButton = mayor.GetComponent<Button>();
        mayorButton.onClick = new Button.ButtonClickedEvent();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(mayorButton.onClick, controller.OpenMayor);
        var data = new SerializedObject(controller);
        void Set(string field, UnityEngine.Object value) { data.FindProperty(field).objectReferenceValue = value; }
        Set("dialogue", AssetDatabase.LoadAssetAtPath<AdventureDialogue>("Assets/Resources/Adventure/FirstAdventure.asset"));
        Set("travelMap", map); Set("locationRoot", location.gameObject); Set("locationSelect", Find("LocationSelectPanel").gameObject);
        Set("townView", Find("MainUIGroup").gameObject); Set("townNode", town.GetComponent<Button>());
        Set("mayorButton", mayorButton); Set("forestNode", forest.GetComponent<Button>());
        Set("nodePanelTemplate", panel.gameObject); Set("buttonTemplate", templateButton);
        data.ApplyModifiedProperties();
        EditorUtility.SetDirty(controller); EditorUtility.SetDirty(mayorButton);
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        Selection.activeGameObject = mayor.gameObject;
        Debug.Log("Mayor Bramble is wired on the existing Location 1 travel map, above the town icon. Town view now references TownScrollView, not the off-screen TownBackground.");
    }
    [MenuItem("StepQuest/Configure playable town hub")]
    public static void ConfigureTownHub()
    {
        RepairQuestWiring();
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        var objects = scene.GetRootGameObjects().SelectMany(r => r.GetComponentsInChildren<Transform>(true)).ToArray();
        Transform Find(string name) => objects.Single(t => t.name == name);
        var controller = Find("HomePageCanvasGroup").GetComponent<AdventureScreen>();
        var town = Find("MainUIGroup");
        var assetPath = "Assets/Resources/Adventure/TownHub.asset";
        var hub = AssetDatabase.LoadAssetAtPath<TownHubDefinition>(assetPath);
        if (hub == null)
        {
            hub = ScriptableObject.CreateInstance<TownHubDefinition>();
            hub.shop.Add(new TownShopOffer { equipment = AssetDatabase.LoadAssetAtPath<Equipment>("Assets/Resources/Items/Equipment/WoodenSword.asset"), copperPrice = 15 });
            hub.shop.Add(new TownShopOffer { equipment = AssetDatabase.LoadAssetAtPath<Equipment>("Assets/Resources/Items/Equipment/LeatherCap.asset"), copperPrice = 10 });
            AssetDatabase.CreateAsset(hub, assetPath);
        }
        var frame = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Sprites/UI Icons/SquareWoodPanel.png");
        var random = town.Find("Random battle");
        if (random == null)
        {
            var go = DefaultControls.CreateButton(new DefaultControls.Resources());
            Undo.RegisterCreatedObjectUndo(go, "Create town battle button"); go.name = "Random battle"; go.transform.SetParent(town, false); random = go.transform;
            foreach (var text in go.GetComponentsInChildren<UnityEngine.UI.Text>()) Undo.DestroyObjectImmediate(text.gameObject);
            var textGo = new GameObject("Label", typeof(RectTransform)); textGo.transform.SetParent(random, false);
            var textRect = textGo.GetComponent<RectTransform>(); textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(18, 10); textRect.offsetMax = new Vector2(-18, -10);
            var label = textGo.AddComponent<TextMeshProUGUI>(); label.text = "RANDOM BATTLE\n100 steps";
            label.font = Find("BattleNode1-1").GetComponentInChildren<TMP_Text>(true).font;
            label.fontSize = 66; label.enableAutoSizing = true; label.fontSizeMin = 40; label.fontSizeMax = 66;
            label.color = new Color(.18f, .1f, .06f); label.alignment = TextAlignmentOptions.Center; label.raycastTarget = false;
        }
        var rect = random.GetComponent<RectTransform>(); rect.anchorMin = new Vector2(.12f, .73f); rect.anchorMax = new Vector2(.88f, .84f);
        rect.offsetMin = rect.offsetMax = Vector2.zero; rect.localScale = Vector3.one;
        random.GetComponent<Image>().sprite = frame; random.GetComponent<Image>().type = Image.Type.Simple;
        void Rename(Transform building, string title)
        {
            foreach (var label in building.GetComponentsInChildren<TMP_Text>(true))
            { Undo.RecordObject(label, "Name town service"); label.text = title; EditorUtility.SetDirty(label); }
        }
        Rename(Find("GuildHallButton"), "HEROES"); Rename(Find("GoldGenButton"), "EQUIPMENT");
        Rename(town.Find("CraftingButton"), "INN"); Rename(Find("TravelPortalButton"), "TRAVEL");
        var reset = town.Find("SAVE RESET"); if (reset != null) reset.gameObject.SetActive(false);
        foreach (var text in town.GetComponentsInChildren<TMP_Text>(true))
            if (text.text.StartsWith("Need crafting equipment")) text.gameObject.SetActive(false);
        var data = new SerializedObject(controller);
        void Set(string field, UnityEngine.Object value) { data.FindProperty(field).objectReferenceValue = value; }
        Set("townHub", hub); Set("randomBattleButton", random.GetComponent<Button>());
        Set("heroesBuilding", Find("GuildHallButton").GetComponent<Button>());
        Set("equipmentBuilding", Find("GoldGenButton").GetComponent<Button>());
        Set("innBuilding", town.Find("CraftingButton").GetComponent<Button>());
        Set("travelBuilding", Find("TravelPortalButton").GetComponent<Button>());
        var overlays = data.FindProperty("townOverlays");
        string[] overlayNames = { "VillageGatePanel", "CrafterBuildingPanel", "GoldGeneratorPanel", "BackgroundClosePanels" };
        overlays.arraySize = overlayNames.Length;
        for (int i = 0; i < overlayNames.Length; i++) overlays.GetArrayElementAtIndex(i).objectReferenceValue = town.Find(overlayNames[i]).gameObject;
        data.ApplyModifiedProperties();
        EditorUtility.SetDirty(controller); EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Town hub configured: repeatable step battles, equipment shop, heroes, inn, and original travel map.");
    }
    public static void ValidateSlice()
    {
        var asset = AssetDatabase.LoadAssetAtPath<AdventureDialogue>("Assets/Resources/Adventure/FirstAdventure.asset");
        if (asset == null || !Validate(asset)) throw new System.Exception("Adventure dialogue validation failed.");
        if (PlayerSettings.defaultInterfaceOrientation != UIOrientation.Portrait)
            throw new System.Exception("Portrait orientation is not configured.");
        Debug.Log("ADVENTURE_SLICE_VALIDATION_PASSED");
    }
}
