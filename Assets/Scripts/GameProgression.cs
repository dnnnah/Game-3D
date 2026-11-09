using UnityEngine;
using UnityEngine.UI;

public sealed class GameProgression : MonoBehaviour
{
    private const string BestFloorsKey = "HELIX_JUMP_BEST_FLOORS";

    [Header("Progresión por pisos")]
    [SerializeField, Min(0f)] private float initialMaximumFallSpeed = 12f;
    [SerializeField, Min(0f)] private float fallSpeedIncreasePerFloor = 0.8f;
    [SerializeField, Min(0f)] private float maximumFallSpeed = 20f;
    [SerializeField, Min(0f)] private float speedChangePerSecond = 2.5f;

    private PlayerMovement playerMovement;
    private Text floorCountLabel;
    private Text bestFloorLabel;
    private int floorsPassed;
    private int bestFloors;
    private float currentMaximumFallSpeed;
    private float targetMaximumFallSpeed;
    private bool runActive;
    private bool hasUnsavedRecord;

    public int FloorsPassed => floorsPassed;

    public void Configure(PlayerMovement movement, Text currentFloorText, Text bestFloorText)
    {
        playerMovement = movement;
        floorCountLabel = currentFloorText;
        bestFloorLabel = bestFloorText;
        bestFloors = Mathf.Max(0, PlayerPrefs.GetInt(BestFloorsKey, 0));
        UpdateFloorLabel();
        UpdateBestFloorLabel();
    }

    public void BeginRun()
    {
        if (playerMovement == null)
            return;

        floorsPassed = 0;
        currentMaximumFallSpeed = Mathf.Min(initialMaximumFallSpeed, maximumFallSpeed);
        targetMaximumFallSpeed = currentMaximumFallSpeed;
        playerMovement.SetMaximumFallSpeed(currentMaximumFallSpeed);
        runActive = true;
        UpdateFloorLabel();
    }

    public void RegisterFloorPassed()
    {
        if (!runActive)
            return;

        floorsPassed++;
        targetMaximumFallSpeed = Mathf.Min(
            initialMaximumFallSpeed + floorsPassed * fallSpeedIncreasePerFloor,
            maximumFallSpeed);
        UpdateFloorLabel();

        if (floorsPassed <= bestFloors)
            return;

        bestFloors = floorsPassed;
        hasUnsavedRecord = true;
        PlayerPrefs.SetInt(BestFloorsKey, bestFloors);
        UpdateBestFloorLabel();
    }

    public void EndRun()
    {
        runActive = false;
        SaveBestFloors();
    }

    private void Update()
    {
        if (!runActive || playerMovement == null)
            return;

        currentMaximumFallSpeed = Mathf.MoveTowards(
            currentMaximumFallSpeed,
            targetMaximumFallSpeed,
            speedChangePerSecond * Time.deltaTime);
        playerMovement.SetMaximumFallSpeed(currentMaximumFallSpeed);
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            SaveBestFloors();
    }

    private void OnApplicationQuit() => SaveBestFloors();
    private void OnDestroy() => SaveBestFloors();

    private void SaveBestFloors()
    {
        if (!hasUnsavedRecord)
            return;

        PlayerPrefs.Save();
        hasUnsavedRecord = false;
    }

    private void UpdateFloorLabel()
    {
        if (floorCountLabel != null)
            floorCountLabel.text = $"PISOS: {floorsPassed}";
    }

    private void UpdateBestFloorLabel()
    {
        if (bestFloorLabel != null)
            bestFloorLabel.text = $"RÉCORD: {bestFloors} pisos";
    }
}
