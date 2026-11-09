using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public enum MobileControlAction
{
    Left,
    Right,
    Forward,
    Backward,
    Jump
}

public sealed class MobileInputController : MonoBehaviour
{
    [SerializeField] private bool keyboardFallback = true;

    private bool leftHeld;
    private bool rightHeld;
    private bool forwardHeld;
    private bool backwardHeld;
    private bool jumpRequested;

    public Vector2 Movement
    {
        get
        {
            float x = (rightHeld ? 1f : 0f) - (leftHeld ? 1f : 0f);
            float z = (forwardHeld ? 1f : 0f) - (backwardHeld ? 1f : 0f);

            if (keyboardFallback && Keyboard.current != null)
            {
                Keyboard keyboard = Keyboard.current;
                x += IsPressed(keyboard.dKey, keyboard.rightArrowKey) ? 1f : 0f;
                x -= IsPressed(keyboard.aKey, keyboard.leftArrowKey) ? 1f : 0f;
                z += IsPressed(keyboard.wKey, keyboard.upArrowKey) ? 1f : 0f;
                z -= IsPressed(keyboard.sKey, keyboard.downArrowKey) ? 1f : 0f;
            }

            return Vector2.ClampMagnitude(new Vector2(x, z), 1f);
        }
    }

    private void Update()
    {
        if (keyboardFallback && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpRequested = true;
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            Clear();
    }

    public void SetHeld(MobileControlAction action, bool held)
    {
        switch (action)
        {
            case MobileControlAction.Left: leftHeld = held; break;
            case MobileControlAction.Right: rightHeld = held; break;
            case MobileControlAction.Forward: forwardHeld = held; break;
            case MobileControlAction.Backward: backwardHeld = held; break;
            case MobileControlAction.Jump:
                if (held) jumpRequested = true;
                break;
        }
    }

    public bool ConsumeJumpRequest()
    {
        bool requested = jumpRequested;
        jumpRequested = false;
        return requested;
    }

    public void Clear()
    {
        leftHeld = rightHeld = forwardHeld = backwardHeld = jumpRequested = false;
    }

    private static bool IsPressed(KeyControl primary, KeyControl alternate) =>
        primary.isPressed || alternate.isPressed;
}
