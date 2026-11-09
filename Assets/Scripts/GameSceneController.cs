using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public sealed class GameSceneController : MonoBehaviour
{
    [Header("Escenario")]
    [SerializeField] private GameObject helixPartPrefab;
    [SerializeField] private Vector3 playerStart = new Vector3(0f, 0.7f, 0f);
    [SerializeField, Min(0f)] private float cameraHeight = 8f;
    [SerializeField] private float cameraZOffset = -10f;

    private static readonly Color Ink = new Color(0.035f, 0.075f, 0.13f, 1f);
    private static readonly Color PanelInk = new Color(0.035f, 0.075f, 0.13f, 0.93f);
    private static readonly Color Accent = new Color(1f, 0.82f, 0.12f, 1f);
    private static readonly Color ButtonInk = new Color(0.11f, 0.19f, 0.29f, 0.9f);
    private static readonly Color Paper = new Color(0.94f, 0.97f, 1f, 1f);

    private MobileInputController mobileInput;
    private PlayerMovement playerMovement;
    private Rigidbody playerBody;
    private Text distanceText;
    private Text bestDistanceText;
    private GameProgression progression;
    private GameObject menuPanel;
    private GameObject gameplayPanel;
    private Font uiFont;

    private void Start()
    {
        Application.targetFrameRate = 60;
        mobileInput = GetComponent<MobileInputController>();
        if (mobileInput == null)
            mobileInput = gameObject.AddComponent<MobileInputController>();

        ConfigureLighting();
        CreateWorld();
        CreateInterface();
        progression = GetComponent<GameProgression>();
        if (progression == null)
            progression = gameObject.AddComponent<GameProgression>();
        progression.Configure(playerBody.transform, playerMovement, distanceText, bestDistanceText);
        ShowMenu();
    }

    private void ConfigureLighting()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.3f, 0.36f, 0.44f);

        Light keyLight = FindAnyObjectByType<Light>();
        if (keyLight == null)
        {
            GameObject lightObject = new GameObject("Key Light");
            keyLight = lightObject.AddComponent<Light>();
        }
        keyLight.type = LightType.Directional;
        keyLight.intensity = 1.25f;
        keyLight.transform.rotation = Quaternion.Euler(48f, -28f, 0f);
    }

    private void CreateWorld()
    {
        Transform world = new GameObject("World").transform;

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ground";
        ground.transform.SetParent(world, false);
        ground.transform.position = new Vector3(0f, -0.5f, 35f);
        ground.transform.localScale = new Vector3(18f, 1f, 90f);
        ground.GetComponent<Renderer>().sharedMaterial = CreateMaterial(new Color(0.14f, 0.21f, 0.3f));

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.name = "Player";
        player.transform.SetParent(world, false);
        player.transform.position = playerStart;
        player.GetComponent<Renderer>().sharedMaterial = CreateMaterial(new Color(0.12f, 0.78f, 0.84f));
        playerBody = player.AddComponent<Rigidbody>();
        playerMovement = player.AddComponent<PlayerMovement>();
        playerMovement.Configure(mobileInput, false);

        if (helixPartPrefab != null)
        {
            Material helixMaterial = CreateMaterial(Accent);
            PlaceHelixPart(world, new Vector3(4f, 0f, 7f), Quaternion.identity, helixMaterial);
            PlaceHelixPart(world, new Vector3(-4f, 0f, 17f), Quaternion.Euler(0f, 180f, 0f), helixMaterial);
        }

        Camera gameCamera = Camera.main;
        if (gameCamera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            gameCamera = cameraObject.GetComponent<Camera>();
        }
        GameObject cameraObjectInScene = gameCamera.gameObject;
        gameCamera.clearFlags = CameraClearFlags.SolidColor;
        gameCamera.backgroundColor = Ink;
        gameCamera.fieldOfView = 58f;
        cameraObjectInScene.transform.position = new Vector3(playerStart.x, cameraHeight, playerStart.z + cameraZOffset);
        cameraObjectInScene.transform.rotation = Quaternion.Euler(35f, 0f, 0f);
        CameraFollowXZ cameraFollow = cameraObjectInScene.GetComponent<CameraFollowXZ>();
        if (cameraFollow == null)
            cameraFollow = cameraObjectInScene.AddComponent<CameraFollowXZ>();
        cameraFollow.SetTarget(player.transform, cameraHeight, 0f, cameraZOffset);
    }

    private void PlaceHelixPart(Transform parent, Vector3 position, Quaternion rotation, Material material)
    {
        GameObject part = Instantiate(helixPartPrefab, position, rotation, parent);
        part.name = "Helix Part";
        Renderer partRenderer = part.GetComponent<Renderer>();
        if (partRenderer != null)
            partRenderer.sharedMaterial = material;
    }

    private static Material CreateMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material material = new Material(shader);
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);
        else if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);
        return material;
    }

    private void CreateInterface()
    {
        GameObject canvasObject = new GameObject("Game Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        GameObject safeAreaObject = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaFitter));
        RectTransform safeAreaRect = safeAreaObject.GetComponent<RectTransform>();
        safeAreaRect.SetParent(canvas.transform, false);
        safeAreaRect.anchorMin = Vector2.zero;
        safeAreaRect.anchorMax = Vector2.one;
        safeAreaRect.offsetMin = Vector2.zero;
        safeAreaRect.offsetMax = Vector2.zero;

        if (FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystemObject.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }

        menuPanel = CreatePanel("Main Menu", safeAreaRect, PanelInk);
        CreateText(menuPanel.transform, "HELIX 3D", new Vector2(0.5f, 0.72f), new Vector2(0f, 0f),
            new Vector2(900f, 180f), 96, Accent, FontStyle.Bold);
        CreateText(menuPanel.transform, "Desplázate en X y Z. Salta en Y.", new Vector2(0.5f, 0.58f), Vector2.zero,
            new Vector2(860f, 100f), 42, Paper, FontStyle.Normal);
        CreateButton(menuPanel.transform, "JUGAR", new Vector2(440f, 140f), new Vector2(0.5f, 0.43f),
            Vector2.zero, Accent, Ink, StartGame);
        bestDistanceText = CreateText(menuPanel.transform, "RÉCORD: 0 m", new Vector2(0.5f, 0.31f), Vector2.zero,
            new Vector2(800f, 90f), 38, Accent, FontStyle.Bold);
        CreateText(menuPanel.transform, "Controles: botones táctiles o WASD / flechas + espacio",
            new Vector2(0.5f, 0.25f), Vector2.zero, new Vector2(920f, 90f), 30, Paper, FontStyle.Normal);

        gameplayPanel = new GameObject("Gameplay UI", typeof(RectTransform));
        RectTransform gameplayRect = gameplayPanel.GetComponent<RectTransform>();
        gameplayRect.SetParent(safeAreaRect, false);
        Stretch(gameplayRect);
        CreateText(gameplayPanel.transform, "MOVER · X / Z                         SALTAR · Y",
            new Vector2(0.5f, 1f), new Vector2(0f, -56f), new Vector2(760f, 90f), 34, Paper, FontStyle.Bold);
        distanceText = CreateText(gameplayPanel.transform, "DISTANCIA: 0 m",
            new Vector2(0.5f, 1f), new Vector2(0f, -142f), new Vector2(760f, 80f), 42, Accent, FontStyle.Bold);
        CreateButton(gameplayPanel.transform, "MENÚ", new Vector2(180f, 116f), new Vector2(0f, 1f),
            new Vector2(105f, -70f), ButtonInk, Paper, ReturnToMenu);

        CreateControl(gameplayPanel.transform, "X−", MobileControlAction.Left, new Vector2(70f, 210f));
        CreateControl(gameplayPanel.transform, "X+", MobileControlAction.Right, new Vector2(330f, 210f));
        CreateControl(gameplayPanel.transform, "Z+", MobileControlAction.Forward, new Vector2(200f, 340f));
        CreateControl(gameplayPanel.transform, "Z−", MobileControlAction.Backward, new Vector2(200f, 70f));
        CreateButton(gameplayPanel.transform, "SALTAR", new Vector2(230f, 230f), new Vector2(1f, 0f),
            new Vector2(-140f, 140f), Accent, Ink, null, MobileControlAction.Jump, 34);
        gameplayPanel.SetActive(false);

        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    private void CreateControl(Transform parent, string label, MobileControlAction action, Vector2 position)
    {
        CreateButton(parent, label, new Vector2(140f, 140f), Vector2.zero, position,
            ButtonInk, Paper, null, action, 38);
    }

    private GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform), typeof(Image));
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        Stretch(rect);
        Image image = panel.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return panel;
    }

    private Button CreateButton(Transform parent, string label, Vector2 size, Vector2 anchor, Vector2 position,
        Color background, Color foreground, UnityEngine.Events.UnityAction onClick,
        MobileControlAction? controlAction = null, int fontSize = 44)
    {
        GameObject buttonObject = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Button));
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Image image = buttonObject.GetComponent<Image>();
        image.color = background;
        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        Navigation navigation = button.navigation;
        navigation.mode = Navigation.Mode.None;
        button.navigation = navigation;
        ColorBlock colors = button.colors;
        colors.normalColor = background;
        colors.highlightedColor = Color.Lerp(background, foreground, 0.14f);
        colors.pressedColor = Color.Lerp(background, foreground, 0.28f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;
        if (onClick != null)
            button.onClick.AddListener(onClick);

        if (controlAction.HasValue)
            buttonObject.AddComponent<MobileControlButton>().Configure(mobileInput, controlAction.Value);

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.SetParent(rect, false);
        Stretch(labelRect);
        Text text = labelObject.GetComponent<Text>();
        text.text = label;
        text.font = GetFont();
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.color = foreground;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        return button;
    }

    private Text CreateText(Transform parent, string value, Vector2 anchor, Vector2 position,
        Vector2 size, int fontSize, Color color, FontStyle style)
    {
        GameObject textObject = new GameObject(value, typeof(RectTransform), typeof(Text));
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        Text text = textObject.GetComponent<Text>();
        text.text = value;
        text.font = GetFont();
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        return text;
    }

    private Font GetFont()
    {
        if (uiFont == null)
            uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return uiFont;
    }

    private void StartGame()
    {
        mobileInput.Clear();
        menuPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        playerMovement.SetMovementEnabled(true);
        progression.BeginRun(playerBody.position.z);
    }

    private void ReturnToMenu()
    {
        mobileInput.Clear();
        progression.EndRun();
        playerMovement.SetMovementEnabled(false);
        playerBody.position = playerStart;
        playerBody.linearVelocity = Vector3.zero;
        menuPanel.SetActive(true);
        gameplayPanel.SetActive(false);
    }

    private void ShowMenu()
    {
        menuPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        playerMovement.SetMovementEnabled(false);
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
