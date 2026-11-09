using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public sealed class SafeAreaFitter : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea;
    private Vector2Int lastScreenSize;

    private void Awake() => rectTransform = GetComponent<RectTransform>();

    private void OnEnable() => ApplyIfChanged(true);

    private void Update() => ApplyIfChanged(false);

    private void ApplyIfChanged(bool force)
    {
        if (Screen.width <= 0 || Screen.height <= 0)
            return;

        Rect safeArea = Screen.safeArea;
        if (safeArea.width <= 0f || safeArea.height <= 0f)
            safeArea = new Rect(0f, 0f, Screen.width, Screen.height);

        Vector2Int screenSize = new Vector2Int(Screen.width, Screen.height);
        if (!force && safeArea == lastSafeArea && screenSize == lastScreenSize)
            return;

        lastSafeArea = safeArea;
        lastScreenSize = screenSize;
        rectTransform.anchorMin = new Vector2(safeArea.xMin / Screen.width, safeArea.yMin / Screen.height);
        rectTransform.anchorMax = new Vector2(safeArea.xMax / Screen.width, safeArea.yMax / Screen.height);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
