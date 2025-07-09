using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public static PanelManager I { get; private set; }

    [Header("Links")]
    [Tooltip("Full-screen Image that dims the game.\nAdd a Button component so OnClick() works.")]
    [SerializeField] private Image backdrop;

    // runtime
    readonly Stack<UIPanel> stack = new();

    //───────────────────────────────────────────────────────────────
    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        backdrop.gameObject.SetActive(false);
        backdrop.GetComponent<Button>().onClick.AddListener(CloseTop);
    }

    //───────────────────────────────────────────────────────────────
    public void Toggle(UIPanel panel)
    {
        if (stack.Contains(panel)) Close(panel);
        else                       Open (panel);
    }

    public void Open(UIPanel panel)
    {
        if (stack.Count == 0) Time.timeScale = 0; // pause game optionally
        stack.Push(panel);
        panel.Show();
        backdrop.gameObject.SetActive(true);
        backdrop.transform.SetAsLastSibling(); // under the new panel
        panel.transform.SetAsLastSibling();    // topmost
    }

    public void CloseTop()
    {
        if (stack.Count > 0) Close(stack.Peek());
    }

    public void Close(UIPanel panel)
    {
        // Pop until we remove the requested one
        while (stack.Count > 0)
        {
            var top = stack.Pop();
            top.Hide();
            if (top == panel) break;
        }

        backdrop.gameObject.SetActive(stack.Count > 0);
        if (stack.Count == 0) Time.timeScale = 1;
    }

    //───────────────────────────────────────────────────────────────
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseTop();
    }
}
