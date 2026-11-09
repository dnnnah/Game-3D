using UnityEngine;

public sealed class CameraFollowY : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float heightOffset = 4f;
    [SerializeField, Min(0f)] private float smoothTime = 0.12f;

    private float fixedX;
    private float fixedZ;
    private float verticalVelocity;

    private void Awake()
    {
        fixedX = transform.position.x;
        fixedZ = transform.position.z;
    }

    public void SetTarget(Transform followTarget, float followHeightOffset)
    {
        target = followTarget;
        heightOffset = followHeightOffset;
        fixedX = transform.position.x;
        fixedZ = transform.position.z;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        float targetY = target.position.y + heightOffset;
        float followY = Mathf.SmoothDamp(transform.position.y, targetY, ref verticalVelocity, smoothTime);
        transform.position = new Vector3(fixedX, followY, fixedZ);
    }
}
