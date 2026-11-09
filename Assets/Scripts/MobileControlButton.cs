using UnityEngine;
using UnityEngine.EventSystems;

public sealed class MobileControlButton : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private MobileInputController input;
    [SerializeField] private MobileControlAction action;

    public void Configure(MobileInputController inputController, MobileControlAction controlAction)
    {
        input = inputController;
        action = controlAction;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (input != null)
            input.SetHeld(action, true);
    }

    public void OnPointerUp(PointerEventData eventData) => Release();
    public void OnPointerExit(PointerEventData eventData) => Release();
    private void OnDisable() => Release();

    private void Release()
    {
        if (input != null)
            input.SetHeld(action, false);
    }
}
