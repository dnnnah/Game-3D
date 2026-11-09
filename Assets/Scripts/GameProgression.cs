using UnityEngine;
using UnityEngine.UI;

public sealed class GameProgression : MonoBehaviour
{
    private const string BestDistanceKey = "HELIX3D_BEST_DISTANCE";

    [Header("Progresión")]
    [SerializeField, Min(0f)] private float initialRunSpeed = 5f;
    [SerializeField, Min(0f)] private float speedIncreasePerMeter = 0.025f;
    [SerializeField, Min(0f)] private float maximumRunSpeed = 12f;
    [SerializeField, Min(0f)] private float speedChangePerSecond = 1.5f;
    [SerializeField, Min(0f)] private float pointsPerMeter = 1f;

    private Transform player;
    private PlayerMovement playerMovement;
    private Text distanceLabel;
    private Text bestDistanceLabel;
    private float runStartZ;
    private float greatestDistance;
    private float currentRunSpeed;
    private int currentScore;
    private int bestDistance;
    private bool runActive;
    private bool hasUnsavedRecord;

    public void Configure(Transform playerTransform, PlayerMovement movement, Text scoreText, Text recordText)
    {
        player = playerTransform;
        playerMovement = movement;
        distanceLabel = scoreText;
        bestDistanceLabel = recordText;
        bestDistance = Mathf.Max(0, PlayerPrefs.GetInt(BestDistanceKey, 0));
        UpdateDistanceLabel();
        UpdateBestDistanceLabel();
    }

    public void BeginRun(float startZ)
    {
        if (player == null || playerMovement == null)
            return;

        runStartZ = startZ;
        greatestDistance = 0f;
        currentScore = 0;
        currentRunSpeed = Mathf.Min(initialRunSpeed, maximumRunSpeed);
        runActive = true;
        playerMovement.SetAutomaticForwardSpeed(currentRunSpeed);
        UpdateDistanceLabel();
    }

    public void EndRun()
    {
        runActive = false;
        SaveBestDistance();
    }

    private void Update()
    {
        if (!runActive || player == null || playerMovement == null)
            return;

        greatestDistance = Mathf.Max(greatestDistance, player.position.z - runStartZ);
        int nextScore = Mathf.FloorToInt(greatestDistance * pointsPerMeter);
        if (nextScore != currentScore)
        {
            currentScore = nextScore;
            UpdateDistanceLabel();
        }

        if (currentScore > bestDistance)
        {
            bestDistance = currentScore;
            hasUnsavedRecord = true;
            PlayerPrefs.SetInt(BestDistanceKey, bestDistance);
            UpdateBestDistanceLabel();
        }

        float targetSpeed = Mathf.Min(initialRunSpeed + greatestDistance * speedIncreasePerMeter, maximumRunSpeed);
        currentRunSpeed = Mathf.MoveTowards(currentRunSpeed, targetSpeed, speedChangePerSecond * Time.deltaTime);
        playerMovement.SetAutomaticForwardSpeed(currentRunSpeed);
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
            SaveBestDistance();
    }

    private void OnApplicationQuit() => SaveBestDistance();
    private void OnDestroy() => SaveBestDistance();

    private void SaveBestDistance()
    {
        if (!hasUnsavedRecord)
            return;

        PlayerPrefs.Save();
        hasUnsavedRecord = false;
    }

    private void UpdateDistanceLabel()
    {
        if (distanceLabel != null)
            distanceLabel.text = $"DISTANCIA: {currentScore} m";
    }

    private void UpdateBestDistanceLabel()
    {
        if (bestDistanceLabel != null)
            bestDistanceLabel.text = $"RÉCORD: {bestDistance} m";
    }
}
