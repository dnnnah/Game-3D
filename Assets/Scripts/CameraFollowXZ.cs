using UnityEngine;

public sealed class CameraFollowXZ : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float fixedHeight = 8f;
    [SerializeField] private float xOffset;
    [SerializeField] private float zOffset = -10f;

    public void SetTarget(Transform followTarget, float height, float horizontalOffset, float depthOffset)
    {
        target = followTarget;
        fixedHeight = height;
        xOffset = horizontalOffset;
        zOffset = depthOffset;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // X and Z follow exactly; Y stays fixed and Z keeps the configured offset.
        transform.position = new Vector3(
            target.position.x + xOffset,
            fixedHeight,
            target.position.z + zOffset);
    }
}
