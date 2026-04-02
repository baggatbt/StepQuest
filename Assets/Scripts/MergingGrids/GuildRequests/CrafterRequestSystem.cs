using System;
using UnityEngine;

public class CrafterRequestSystem : MonoBehaviour
{
    public static CrafterRequestSystem Instance { get; private set; }

    [Header("Data")]
    [SerializeField] private CrafterRequestDatabase requestDatabase;

    [Header("Current")]
    public CrafterEntityType currentType;
    public int currentRequired;
    public int currentProgress;

    public Action OnRequestUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (requestDatabase == null)
        {
            Debug.LogWarning("[CrafterRequestSystem] No request database assigned.", this);
            return;
        }

        if (currentType == CrafterEntityType.None || currentRequired <= 0)
            GenerateNewRequest();
    }

    public void OnItemRecycled(CrafterEntityType type)
    {
        if (type != currentType)
            return;

        currentProgress++;
        OnRequestUpdated?.Invoke();

        if (currentProgress >= currentRequired)
            CompleteRequest();
    }

    private void CompleteRequest()
    {
        CrafterRequestDefinition def = GetCurrentDefinition();

        if (def != null && PlayerData.Instance != null)
        {
            PlayerData.Instance.AddCopper(def.rewardCopper);

            // Only keep this if your PlayerData actually has AddXP()
            // PlayerData.Instance.AddXP(def.rewardXP);
        }

        GenerateNewRequest();
    }

    public void GenerateNewRequest()
    {
        if (requestDatabase == null)
        {
            Debug.LogWarning("[CrafterRequestSystem] Cannot generate request, database missing.", this);
            return;
        }

        CrafterRequestDefinition def = requestDatabase.GetRandom();

        if (def == null)
        {
            Debug.LogWarning("[CrafterRequestSystem] No request definitions found.", this);
            currentType = CrafterEntityType.None;
            currentRequired = 0;
            currentProgress = 0;
            OnRequestUpdated?.Invoke();
            return;
        }

        currentType = def.requestedType;
        currentRequired = def.GetRandomQuantity();
        currentProgress = 0;

        OnRequestUpdated?.Invoke();
    }

    private CrafterRequestDefinition GetCurrentDefinition()
    {
        if (requestDatabase == null || requestDatabase.requests == null)
            return null;

        for (int i = 0; i < requestDatabase.requests.Count; i++)
        {
            CrafterRequestDefinition def = requestDatabase.requests[i];
            if (def == null)
                continue;

            if (def.requestedType == currentType)
                return def;
        }

        return null;
    }
}