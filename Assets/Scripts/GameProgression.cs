using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class GameProgression : MonoBehaviour
{
    private const string BestFloorsKey = "HELIX_JUMP_BEST_FLOORS";

    [Serializable]
    public struct DifficultyStep
    {
        [Min(0)] public int floorsRequired;
        [Min(0f)] public float fallSpeedCap;
        [Min(0)] public int hazardSectorCount;
    }

    [Header("Progresión por pisos")]
    [SerializeField, Min(0f)] private float initialMaximumFallSpeed = 12f;
    [SerializeField, Min(0f)] private float fallSpeedIncreasePerFloor = 0.8f;
    [SerializeField, Min(0f)] private float maximumFallSpeed = 20f;
    [SerializeField, Min(0f)] private float speedChangePerSecond = 2.5f;

    [Header("Niveles de dificultad")]
    [SerializeField] private DifficultyStep[] difficultySteps =
    {
        new DifficultyStep { floorsRequired = 0, fallSpeedCap = 15.2f, hazardSectorCount = 0 },
        new DifficultyStep { floorsRequired = 4, fallSpeedCap = 18.4f, hazardSectorCount = 1 },
        new DifficultyStep { floorsRequired = 8, fallSpeedCap = 20f, hazardSectorCount = 2 },
        new DifficultyStep { floorsRequired = 12, fallSpeedCap = 20f, hazardSectorCount = 3 }
    };

    [Header("Perfil de nivel 2")]
    [SerializeField, Min(0f)] private float levelTwoInitialFallSpeed = 10.5f;
    [SerializeField, Min(0f)] private float levelTwoFallSpeedIncrease = 0.55f;
    [SerializeField, Min(0f)] private float levelTwoMaximumFallSpeed = 16.5f;
    [SerializeField] private DifficultyStep[] levelTwoDifficultySteps =
    {
        new DifficultyStep { floorsRequired = 0, fallSpeedCap = 13f, hazardSectorCount = 0 },
        new DifficultyStep { floorsRequired = 6, fallSpeedCap = 15f, hazardSectorCount = 1 },
        new DifficultyStep { floorsRequired = 12, fallSpeedCap = 16.5f, hazardSectorCount = 2 }
    };

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
    public int HazardSectorsPerFloor => Mathf.Max(0, GetCurrentDifficultyStep().hazardSectorCount);

    public void ConfigureLevelDifficulty(float initialSpeed, float increasePerFloor, float speedCap,
        float changeRate, DifficultyStep[] steps)
    {
        initialMaximumFallSpeed = Mathf.Max(0f, initialSpeed);
        fallSpeedIncreasePerFloor = Mathf.Max(0f, increasePerFloor);
        maximumFallSpeed = Mathf.Max(initialMaximumFallSpeed, speedCap);
        speedChangePerSecond = Mathf.Max(0f, changeRate);

        difficultySteps = steps == null ? Array.Empty<DifficultyStep>() : (DifficultyStep[])steps.Clone();
        for (int i = 0; i < difficultySteps.Length; i++)
        {
            DifficultyStep step = difficultySteps[i];
            step.floorsRequired = Mathf.Max(0, step.floorsRequired);
            step.fallSpeedCap = Mathf.Max(0f, step.fallSpeedCap);
            step.hazardSectorCount = Mathf.Max(0, step.hazardSectorCount);
            difficultySteps[i] = step;
        }
    }

    public void Configure(PlayerMovement movement, Text currentFloorText, Text bestFloorText)
    {
        playerMovement = movement;
        floorCountLabel = currentFloorText;
        bestFloorLabel = bestFloorText;
        bestFloors = Mathf.Max(0, PlayerPrefs.GetInt(BestFloorsKey, 0));
        UpdateFloorLabel();
        UpdateBestFloorLabel();
    }

    public void ConfigureLevelProfile(int sceneLevelNumber)
    {
        if (sceneLevelNumber != 2)
            return;

        initialMaximumFallSpeed = levelTwoInitialFallSpeed;
        fallSpeedIncreasePerFloor = levelTwoFallSpeedIncrease;
        maximumFallSpeed = levelTwoMaximumFallSpeed;
        difficultySteps = levelTwoDifficultySteps;
    }

    public void BeginRun()
    {
        if (playerMovement == null)
            return;

        floorsPassed = 0;
        currentMaximumFallSpeed = Mathf.Min(initialMaximumFallSpeed, GetFallSpeedCap(GetCurrentDifficultyStep()));
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
        DifficultyStep step = GetCurrentDifficultyStep();
        targetMaximumFallSpeed = Mathf.Min(
            initialMaximumFallSpeed + floorsPassed * fallSpeedIncreasePerFloor,
            GetFallSpeedCap(step));
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

    private DifficultyStep GetCurrentDifficultyStep()
    {
        DifficultyStep current = new DifficultyStep
        {
            floorsRequired = 0,
            fallSpeedCap = maximumFallSpeed,
            hazardSectorCount = 0
        };

        if (difficultySteps == null)
            return current;

        for (int i = 0; i < difficultySteps.Length; i++)
        {
            DifficultyStep candidate = difficultySteps[i];
            if (candidate.floorsRequired <= floorsPassed && candidate.floorsRequired >= current.floorsRequired)
                current = candidate;
        }

        return current;
    }

    private float GetFallSpeedCap(DifficultyStep step)
    {
        return Mathf.Min(maximumFallSpeed, Mathf.Max(0f, step.fallSpeedCap));
    }
}
