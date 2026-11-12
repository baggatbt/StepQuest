using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// One pointer owns a hold. Unity sends its release to the pressed object even
// outside the button; hardware polling also recovers a lost release event.
public sealed class CombatActionControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, ICancelHandler
{
    public bool IsHeld { get; private set; }
    public bool Cancelled { get; private set; }
    public int PressCount { get; private set; }
    public int ReleaseCount { get; private set; }
    public bool Accepting { get; set; }
    int pointer, touchId, pressFrame;
    bool touchPointer;

    public void ResetInput()
    { IsHeld = Cancelled = false; PressCount = ReleaseCount = 0; Accepting = true; }

    public void OnPointerDown(PointerEventData data)
    {
        if (!Accepting || Cancelled || IsHeld || data.button != PointerEventData.InputButton.Left) return;
        var button = GetComponent<Button>();
        if (button != null && !button.IsInteractable()) return;
        pointer = data.pointerId; pressFrame = Time.frameCount;
        var extended = data as ExtendedPointerEventData;
        touchPointer = extended != null ? extended.device is Touchscreen : data.pointerId >= 0;
        touchId = extended != null ? extended.touchId : -1;
        IsHeld = true; PressCount++;
    }
    public void OnPointerUp(PointerEventData data)
    { if (IsHeld && data.pointerId == pointer) Release(); }
    void Release() { IsHeld = false; ReleaseCount++; }
    public void Cancel() { Cancelled = true; Accepting = false; IsHeld = false; }
    public void OnCancel(BaseEventData data) { Cancel(); }
    void OnDisable() { Cancel(); }
    void OnApplicationFocus(bool focused) { if (!focused) Cancel(); }
    void OnApplicationPause(bool paused) { if (paused) Cancel(); }
    void Update()
    {
        if (!IsHeld || Time.frameCount <= pressFrame) return;
        if (touchPointer)
        {
            bool down = false;
            if (Touchscreen.current != null)
                foreach (var touch in Touchscreen.current.touches)
                    if (touch.press.isPressed && (touchId < 0 || touch.touchId.ReadValue() == touchId)) { down = true; break; }
            if (!down) Release();
        }
        else if (Mouse.current == null || !Mouse.current.leftButton.isPressed) Release();
    }
}
