using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveStripUI : MonoBehaviour
{
    public ObjectiveManager objectiveManager;   // drag your ObjectiveManager object
    public Transform content;                   // Content with VerticalLayoutGroup
    public GameObject rowPrefab;                // ObjectiveRow prefab

    void OnEnable()
    {
        ObjectiveManager.ObjectivesChanged += Refresh;
        Refresh();
    }
    void OnDisable()
    {
        ObjectiveManager.ObjectivesChanged -= Refresh;
    }

    public void Refresh()
    {
        if (objectiveManager == null || content == null || rowPrefab == null) return;

        // Clear existing rows
        foreach (Transform c in content) Destroy(c.gameObject);

        foreach (var o in objectiveManager.active.Take(3)) // show top 3
        {
            var go = Instantiate(rowPrefab, content);
            var row = go.GetComponent<ObjectiveRowUI>();
            if (row == null) row = go.AddComponent<ObjectiveRowUI>();
            row.Bind(o);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(content as RectTransform);
    }
}
