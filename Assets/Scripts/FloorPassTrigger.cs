using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public sealed class FloorPassTrigger : MonoBehaviour
{
    [SerializeField] private float floorHeight;
    [SerializeField, Min(0f)] private float clearanceBelowFloor = 0.25f;

    private GameProgression progression;
    private bool countedThisRun;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    public void Configure(GameProgression gameProgression, float height)
    {
        progression = gameProgression;
        floorHeight = height;
    }

    public void Configure(GameProgression gameProgression)
    {
        progression = gameProgression;
    }

    public void ResetForRun()
    {
        countedThisRun = false;
    }

    private void OnTriggerEnter(Collider other) => TryCountFloor(other);
    private void OnTriggerStay(Collider other) => TryCountFloor(other);

    private void TryCountFloor(Collider other)
    {
        if (countedThisRun || progression == null)
            return;

        PlayerMovement movement = other.GetComponentInParent<PlayerMovement>();
        if (movement == null || movement.VerticalSpeed >= -0.05f)
            return;

        if (movement.transform.position.y > floorHeight - clearanceBelowFloor)
            return;

        countedThisRun = true;
        progression.RegisterFloorPassed();
    }
}
