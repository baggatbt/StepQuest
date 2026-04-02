using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrafterRequestUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Image iconImage;
    [SerializeField] private CrafterEntityDatabase entityDatabase;

    private CrafterRequestSystem cachedSystem;

    private void OnEnable()
    {
        TryBindToRequestSystem();
        Refresh();
    }

    private void Start()
    {
        // Covers cases where UI enabled before request system finished Awake/Start
        TryBindToRequestSystem();
        Refresh();
    }

    private void OnDisable()
    {
        UnbindFromRequestSystem();
    }

    private void TryBindToRequestSystem()
    {
        if (cachedSystem != null)
            return;

        if (CrafterRequestSystem.Instance == null)
            return;

        cachedSystem = CrafterRequestSystem.Instance;
        cachedSystem.OnRequestUpdated -= Refresh;
        cachedSystem.OnRequestUpdated += Refresh;
    }

    private void UnbindFromRequestSystem()
    {
        if (cachedSystem == null)
            return;

        cachedSystem.OnRequestUpdated -= Refresh;
        cachedSystem = null;
    }

    public void Refresh()
    {
        TryBindToRequestSystem();

        var system = CrafterRequestSystem.Instance;
        if (system == null)
        {
            if (itemNameText != null)
                itemNameText.text = "No Request";

            if (progressText != null)
                progressText.text = "- / -";

            if (iconImage != null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }

            return;
        }

        if (entityDatabase == null)
        {
            Debug.LogWarning("[CrafterRequestUI] Missing CrafterEntityDatabase reference.", this);
            return;
        }

        var def = entityDatabase.Get(system.currentType);

        if (itemNameText != null)
            itemNameText.text = def != null ? def.displayName : system.currentType.ToString();

        if (iconImage != null)
        {
            if (def != null && def.iconSprite != null)
            {
                iconImage.sprite = def.iconSprite;
                iconImage.enabled = true;
                iconImage.color = Color.white;
                iconImage.preserveAspect = true;
            }
            else
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }
        }

        if (progressText != null)
            progressText.text = $"{system.currentProgress} / {system.currentRequired}";
    }
}