using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public sealed class MobileInputController : MonoBehaviour
{
    [SerializeField] private bool keyboardFallback = true;
    [SerializeField, Min(0f)] private float keyboardRotationSpeed = 180f;

    private float rotationDelta;

    private void Update()
    {
        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
        {
            rotationDelta += touchscreen.primaryTouch.delta.ReadValue().x;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            rotationDelta += Mouse.current.delta.ReadValue().x;
        }

        if (keyboardFallback && Keyboard.current != null)
        {
            Keyboard keyboard = Keyboard.current;
            float direction = (IsPressed(keyboard.dKey, keyboard.rightArrowKey) ? 1f : 0f)
                - (IsPressed(keyboard.aKey, keyboard.leftArrowKey) ? 1f : 0f);
            rotationDelta += direction * keyboardRotationSpeed * Time.deltaTime;
        }

    }

    public float ConsumeRotationDelta()
    {
        float delta = rotationDelta;
        rotationDelta = 0f;
        return delta;
    }

    public void Clear()
    {
        rotationDelta = 0f;
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            Clear();
    }

    private static bool IsPressed(KeyControl primary, KeyControl alternate) =>
        primary.isPressed || alternate.isPressed;
}
