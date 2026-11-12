using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StepQuest.Adventure
{
    // Adventure windows share the home Canvas and retain existing gameplay bindings.
    public partial class AdventureScreen : MonoBehaviour
    {
        [SerializeField] AdventureDialogue dialogue;
        [SerializeField] ScrollRect travelMap;
        [SerializeField] GameObject locationRoot;
        [SerializeField] GameObject locationSelect;
        [SerializeField] GameObject townView;
        [SerializeField] Button mayorButton;
        [SerializeField] Button townNode;
        [SerializeField] Button forestNode;
        [SerializeField] GameObject nodePanelTemplate;
        [SerializeField] Button buttonTemplate;
        static AdventureScreen current;
        static Vector2 savedMapPosition;
        static bool hasMapPosition;
        Button.ButtonClickedEvent originalForestClick;
        GameObject modal;
        Transform content;
        Transform actionFooter;
        RpgUi ui;
        TMP_Text townBadge, forestBadge;
        float UiScale => Mathf.Max(1f, ((RectTransform)transform).rect.width / 480f);
        TMP_FontAsset font;
        Color textColor = Color.white;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetRuntime() { current = null; hasMapPosition = false; }
        IEnumerator Start()
        {
            current = this;
            yield return null;
            if (dialogue == null || travelMap == null || townNode == null || forestNode == null || nodePanelTemplate == null)
            { Debug.LogError("Quest integration needs its existing travel UI references assigned.", this); yield break; }
            ui = new RpgUi(UiScale);
            font = ui.Font; textColor = ui.Theme.light;
            townBadge = Badge(townNode.transform, OpenTown);
            forestBadge = Badge(forestNode.transform, OpenForest);
            originalForestClick = forestNode.onClick;
            forestNode.onClick = new Button.ButtonClickedEvent();
            forestNode.onClick.AddListener(() =>
            {
                if (AdventureProgress.State == StoryQuestState.Complete) originalForestClick.Invoke();
                else OpenForest();
            });
            townNode.gameObject.SetActive(true);
            if (mayorButton != null)
            {
                mayorButton.gameObject.SetActive(true);
                mayorButton.onClick = new Button.ButtonClickedEvent();
                mayorButton.onClick.AddListener(OpenMayor);
            }
            else Debug.LogError("The mayor map location is not assigned. Run StepQuest > Repair quest location wiring.", this);
            townNode.onClick = new Button.ButtonClickedEvent();
            townNode.onClick.AddListener(OpenTown);
            RefreshMarkers();
            InitializeTownHub();
            if (AdventureProgress.ConsumeTravelReturn())
            {
                OpenTravelMap();
                yield return null;
                Canvas.ForceUpdateCanvases();
                if (hasMapPosition) travelMap.content.anchoredPosition = savedMapPosition;
            }
        }
        void OnDestroy() { if (current == this) current = null; ui?.Dispose(); }
        void Update()
        {
            if (modal != null && !travelMap.gameObject.activeInHierarchy && !townView.activeInHierarchy) Close();
        }
        public static bool OwnsQuestNode(Node node)
        {
            return current != null && current.forestNode != null && node.gameObject == current.forestNode.gameObject
                && AdventureProgress.State != StoryQuestState.Complete;
        }
        public static void RememberTravelMap()
        {
            if (current == null || current.travelMap == null) return;
            savedMapPosition = current.travelMap.content.anchoredPosition; hasMapPosition = true;
        }
        public static bool OpenTravelMap()
        {
            if (current == null || current.travelMap == null) return false;
            current.Close();
            if (current.townView != null) current.townView.SetActive(false);
            current.CloseTownOverlays();
            current.travelMap.gameObject.SetActive(true);
            current.locationRoot.SetActive(true);
            current.locationSelect.SetActive(false);
            current.RefreshMarkers();
            return true;
        }
        void OpenTown()
        {
            Close();
            travelMap.gameObject.SetActive(false);
            townView.SetActive(true);
            CloseTownOverlays();
        }
        public void OpenMayor()
        {
            ShowNode(EntryNode());
        }
        TMP_Text Badge(Transform location, UnityEngine.Events.UnityAction onClick)
        {
            var button = MakeButton(location, "!", onClick);
            var rect = button.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(.85f, .88f); rect.pivot = new Vector2(.5f, .5f); rect.anchoredPosition = Vector2.zero; rect.sizeDelta = new Vector2(100, 100);
            return button.GetComponentInChildren<TMP_Text>();
        }
        void RefreshMarkers()
        {
            if (townBadge == null || forestBadge == null) return;
            townBadge.text = AdventureProgress.State == StoryQuestState.Won ? "!" : "?";
            townBadge.transform.parent.gameObject.SetActive(AdventureProgress.State == StoryQuestState.Available || AdventureProgress.State == StoryQuestState.Won);
            forestBadge.text = "!";
            forestBadge.transform.parent.gameObject.SetActive(AdventureProgress.State == StoryQuestState.Accepted);
        }
        void OpenForest()
        {
            OpenPanel(); Label("Forest Road", 32);
            Art(dialogue.forestBackdrop, 130);
            Label(dialogue.questTitle, 26);
            if (AdventureProgress.State == StoryQuestState.Accepted)
            {
                Label("Investigate the missing road signs with your party.", 25);
                MakeButton(content, "Investigate", () =>
                {
                    string error;
                    if (!AdventureProgress.Launch(dialogue, out error)) Label(error, 24);
                });
            }
            else Label(AdventureProgress.State == StoryQuestState.Won ? "The road is clear. Return to the mayor in town." : "Ask Mayor Bramble in town about the missing road signs.", 25);
            MakeButton(content, "Back", Close);
        }
        string EntryNode()
        {
            switch (AdventureProgress.State)
            {
                case StoryQuestState.Accepted: return dialogue.activeNode;
                case StoryQuestState.Won: return dialogue.victoryNode;
                case StoryQuestState.Complete: return dialogue.completeNode;
                default: return dialogue.offerNode;
            }
        }
        void ShowNode(string id)
        {
            var node = dialogue.Find(id);
            if (node == null) { Debug.LogError("Missing dialogue node: " + id); return; }
            OpenPanel(); Art(dialogue.npcPortrait, 110); Label(node.speaker, 30); Label(node.text, 26);
            foreach (var reply in node.replies)
            {
                var choice = reply;
                MakeButton(content, choice.text, () =>
                {
                    if (choice.action == DialogueAction.AcceptQuest) AdventureProgress.Accept();
                    if (choice.action == DialogueAction.ClaimReward && !AdventureProgress.Claim(dialogue.copperReward))
                    { Label("Reward is not available yet.", 24); return; }
                    RefreshMarkers();
                    if (string.IsNullOrEmpty(choice.nextNode)) Close(); else ShowNode(choice.nextNode);
                });
            }
            MakeButton(content, "Close", Close);
        }
        void OpenPanel(float footerHeight = 0)
        {
            Close();
            ui.Scale = UiScale;
            // Block taps through the window while retaining the existing home Canvas.
            modal = Image("Adventure overlay", transform, null).gameObject;
            modal.GetComponent<Image>().color = new Color(0.06f, .05f, .05f, .82f);
            modal.transform.SetAsLastSibling();
            var window = ui.Panel(modal.transform, "RPG window");
            window.gameObject.AddComponent<RpgSafeWindow>();
            var viewport = Rect("Viewport", window.transform);
            viewport.offsetMin = new Vector2(12, 12 + footerHeight) * UiScale;
            viewport.offsetMax = new Vector2(-12, -12) * UiScale;
            viewport.gameObject.AddComponent<RectMask2D>();
            var hit = viewport.gameObject.AddComponent<Image>(); hit.color = Color.clear;
            var scroll = viewport.gameObject.AddComponent<ScrollRect>(); scroll.viewport = viewport; scroll.horizontal = false; scroll.movementType = ScrollRect.MovementType.Clamped;
            var body = Rect("Content", viewport); body.anchorMin = new Vector2(0, 1); body.anchorMax = Vector2.one; body.pivot = new Vector2(.5f, 1); body.sizeDelta = Vector2.zero;
            ui.Stack(body, 5, ui.Theme.spacing);
            body.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.content = body; content = body;
            if (footerHeight > 0)
            {
                var footer = Rect("Fixed actions", window.transform);
                footer.anchorMax = new Vector2(1, 0);
                footer.offsetMin = new Vector2(17, 12) * UiScale;
                footer.offsetMax = new Vector2(-17, 12 + footerHeight) * UiScale;
                ui.Stack(footer, 0, 6); actionFooter = footer;
            }
        }
        void Close()
        {
            explorationButton = null; explorationBalance = null; explorationStatus = null;
            actionFooter = null;
            if (modal != null) { modal.SetActive(false); Destroy(modal); modal = null; }
        }
        void Art(Sprite sprite, float height, bool fillWidth = false)
        {
            var frame = ui.Panel(content, "Artwork", true); frame.raycastTarget = false;
            ui.Height(frame, height);
            if (fillWidth && sprite != null)
            {
                var aspect = frame.gameObject.AddComponent<RpgArtworkLayout>();
                aspect.aspect = sprite.rect.width / sprite.rect.height;
                aspect.padding = 16 * UiScale;
            }
            var image = Image("Portrait", frame.transform, sprite); image.preserveAspect = true; image.raycastTarget = false;
            if (sprite == null)
            {
                image.enabled = false;
                var unknown = ui.Text(frame.transform, "?", 30, ui.Theme.gold);
                unknown.alignment = TextAlignmentOptions.Center;
            }
            image.rectTransform.offsetMin = Vector2.one * 8 * UiScale;
            image.rectTransform.offsetMax = Vector2.one * -8 * UiScale;
        }
        void Label(string value, float size)
        {
            var t = Text(content, value, Mathf.Min(size, 30));
            t.gameObject.AddComponent<LayoutElement>().minHeight = (Mathf.Min(size, 30) + 8) * UiScale;
            if (size < 28) t.color = ui.Theme.muted;
        }
        Button MakeButton(Transform parent, string title, UnityEngine.Events.UnityAction action)
        {
            return ui.Button(parent, title, action,
                title.StartsWith("Back") || title == "Close" ? RpgButtonKind.Navigation : RpgButtonKind.Secondary);
        }
        TMP_Text Text(Transform parent, string value, float size)
        {
            return ui.Text(parent, value, size);
        }
        static Image Image(string name, Transform parent, Sprite sprite)
        { var rect = Rect(name, parent); var image = rect.gameObject.AddComponent<Image>(); image.sprite = sprite; return image; }
        static RectTransform Rect(string name, Transform parent)
        { return RpgUi.Rect(name, parent); }
        static void Place(RectTransform r, Vector2 min, Vector2 max)
        { r.anchorMin = min; r.anchorMax = max; r.offsetMin = r.offsetMax = Vector2.zero; r.localScale = Vector3.one; }
    }
}
