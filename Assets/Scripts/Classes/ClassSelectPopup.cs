using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassSelectPopup : MonoBehaviour
{
    [Header("Wiring")]
    public HeroClassSwitcher switcher;         // drag your HeroClassSwitcher
    public Button classIconButton;             // the small icon button on the stats panel
    public Image  classIconImage;              // the image on that button
    public GameObject popupRoot;               // the popup panel (set inactive by default)
    public Transform popupContent;             // Grid/Vertical Layout group under the popup
    public GameObject classButtonPrefab;       // a small prefab with Button + Image (and optional label)
    public Button backdropCloseButton;         // transparent full-screen button behind popup (optional)

    void Awake()
    {
        if (classIconButton) classIconButton.onClick.AddListener(TogglePopup);
        if (backdropCloseButton) backdropCloseButton.onClick.AddListener(HidePopup);
        HidePopup();
    }

    void OnEnable()
    {
        RefreshCurrentIcon();
    }

    // Called when the class icon is tapped
    public void TogglePopup()
    {
        if (popupRoot == null) return;
        if (popupRoot.activeSelf) HidePopup();
        else ShowPopup();
    }

    public void ShowPopup()
    {
        if (popupRoot == null || switcher == null) return;

        // Clear old
        foreach (Transform c in popupContent) Destroy(c.gameObject);

        // Build one button per unlocked class
        foreach (var entry in switcher.UnlockedEntries())
        {
            var go = Instantiate(classButtonPrefab, popupContent);
            var btn = go.GetComponent<Button>();
            var icon = go.transform.GetComponentInChildren<Image>(); // first Image in prefab
            if (icon != null)
            {
                // Prefer CharacterData.heroIcon if present
                if (entry.data != null && entry.data.heroIcon != null)
                    icon.sprite = entry.data.heroIcon;
            }

            // Optional label (works with Text or TMP)
            var tmp = go.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp) tmp.text = entry.data != null ? entry.data.heroID : entry.classType.ToString();
            var txt = go.GetComponentInChildren<Text>(true);
            if (txt) txt.text = entry.data != null ? entry.data.heroID : entry.classType.ToString();

            // Grey out the currently active
            bool isActive = (entry.classType == switcher.CurrentType);
            if (btn) btn.interactable = !isActive;

            // Wire click
            if (btn)
            {
                var target = entry.classType;
                btn.onClick.AddListener(() =>
                {
                    switcher.SwitchTo(target);
                    RefreshCurrentIcon();
                    HidePopup();
                });
            }
        }

        // Show popup + backdrop
        popupRoot.SetActive(true);
        if (backdropCloseButton) backdropCloseButton.gameObject.SetActive(true);
    }

    public void HidePopup()
    {
        if (popupRoot) popupRoot.SetActive(false);
        if (backdropCloseButton) backdropCloseButton.gameObject.SetActive(false);
    }

    // Updates the small icon on the hero panel to match the current class
    public void RefreshCurrentIcon()
    {
        if (switcher == null || classIconImage == null) return;

        var current = switcher.UnlockedEntries()
                              .FirstOrDefault(e => e.classType == switcher.CurrentType);

        if (current != null && current.data != null && current.data.heroIcon != null)
            classIconImage.sprite = current.data.heroIcon;
    }
}
