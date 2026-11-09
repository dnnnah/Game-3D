using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public sealed class FloorPassTrigger : MonoBehaviour
{
    [SerializeField] private float floorHeight;
    [SerializeField, Min(0f), Tooltip("Recicla el piso cuando el balón ya queda por debajo de su espesor.")]
    private float clearanceBelowFloor = 1f;

    private GameProgression progression;
    private Action passedCallback;
    private bool countedThisRun;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    public void Configure(GameProgression gameProgression, float height)
    {
        Configure(gameProgression, height, null);
    }

    public void Configure(GameProgression gameProgression, float height, Action onPassed)
    {
        progression = gameProgression;
        floorHeight = height;
        passedCallback = onPassed;
        countedThisRun = false;
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
        if (countedThisRun)
            return;

        PlayerMovement movement = other.GetComponentInParent<PlayerMovement>();
        if (movement == null || movement.VerticalSpeed >= -0.05f)
            return;

        if (movement.transform.position.y > floorHeight - clearanceBelowFloor)
            return;

        countedThisRun = true;
        progression?.RegisterFloorPassed();
        passedCallback?.Invoke();
    }
}
