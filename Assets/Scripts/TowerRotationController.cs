using UnityEngine;

public sealed class TowerRotationController : MonoBehaviour
{
    [SerializeField, Min(0f)] private float degreesPerPixel = 0.45f;
    [SerializeField, Min(0f)] private float maximumDegreesPerFrame = 90f;

    private MobileInputController input;
    private bool rotationEnabled;
    private Quaternion startingRotation;

    private void Awake()
    {
        startingRotation = transform.rotation;
    }

    public void Configure(MobileInputController inputController)
    {
        input = inputController;
        startingRotation = transform.rotation;
    }

    public void SetRotationEnabled(bool enabled)
    {
        rotationEnabled = enabled;
    }

    public void ResetRotation()
    {
        transform.rotation = startingRotation;
    }

    private void Update()
    {
        if (!rotationEnabled || input == null)
            return;

        float delta = input.ConsumeRotationDelta();
        float degrees = Mathf.Clamp(delta * degreesPerPixel, -maximumDegreesPerFrame, maximumDegreesPerFrame);
        if (Mathf.Abs(degrees) > 0.001f)
            transform.Rotate(Vector3.up, -degrees, Space.World);
    }
}
