using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public sealed class GameSceneController : MonoBehaviour
{
    private enum RunState
    {
        Menu,
        Playing,
        GameOver
    }

    [Header("Helix Jump")]
    [SerializeField] private GameObject helixPartPrefab;
    [SerializeField, Min(3)] private int platformFloorCount = 24;
    [SerializeField, Min(3), Tooltip("El sector importado cubre 30 grados; 12 piezas completan la vuelta antes de abrir un hueco.")]
    private int sectorsPerFloor = 12;
    [SerializeField, Min(1)] private int gapWidthSectors = 2;
    [SerializeField, Min(0)] private int firstGapSector;
    [SerializeField] private int gapShiftPerFloor = 3;
    [SerializeField, Min(0.5f)] private float floorSpacing = 1.7f;
    [SerializeField] private float sectorRotationPerFloor = 9f;
    [SerializeField, Min(0f)] private float passTriggerDepth = 0.95f;
    [SerializeField] private Vector3 passTriggerSize = new Vector3(8f, 0.25f, 8f);
    [SerializeField, Min(0f)] private float columnEndPadding = 1.4f;
    [SerializeField, Tooltip("Rojo brillante para distinguir los sectores letales del acento seguro.")]
    private Color hazardColor = new Color(1f, 0.08f, 0.02f, 1f);
    [SerializeField, Min(0f)] private float playerSpawnClearance = 3f;
    [SerializeField, Min(0.5f)] private float playerOrbitRadius = 1.1f;
    [SerializeField, Min(0f)] private float cameraHeightOffset = 4.5f;
    [SerializeField] private Material darkPlatformMaterial;
    [FormerlySerializedAs("yellowPlatformMaterial")]
    [SerializeField] private Material safeAccentMaterial;
    [SerializeField] private Material columnMaterial;
    [SerializeField] private Material playerMaterial;
    [SerializeField] private PhysicsMaterial ballBounceMaterial;
    [Header("Iluminación y fondo")]
    [SerializeField] private Color backgroundColor = new Color(0.48f, 0.61f, 0.7f, 1f);
    [SerializeField] private Color ambientLightColor = new Color(0.3f, 0.36f, 0.44f);
    [SerializeField] private Color directionalLightColor = Color.white;
    [SerializeField, Min(0f)] private float directionalLightIntensity = 1.25f;
    [SerializeField] private Vector3 directionalLightRotation = new Vector3(48f, -28f, 0f);
    [Header("Apariencia del personaje")]
    [SerializeField] private Color playerColor = new Color(0.92f, 0.04f, 0.035f, 1f);
    [SerializeField, Range(0f, 1f)] private float playerMetallic = 0.12f;
    [SerializeField, Range(0f, 1f)] private float playerSmoothness = 0.78f;
    [Header("Flujo de escenas")]
    [SerializeField] private bool isMainMenuScene = true;
    [SerializeField] private bool autoStartLevel;
    [SerializeField, Min(0)] private int levelNumber;
    [SerializeField, Min(0)] private int floorsToCompleteLevel;
    [SerializeField] private string firstLevelSceneName = "Level1";
    [SerializeField] private string nextLevelSceneName;
    [SerializeField] private string mainMenuSceneName = "Main";
    [SerializeField] private Transform towerRoot;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private TowerRotationController towerRotation;
    [SerializeField] private CameraFollowY cameraFollow;
    [SerializeField] private GameProgression progression;

    private static readonly Color Ink = new Color(0.68f, 0.7f, 0.73f, 1f);
    private static readonly Color PanelInk = new Color(0.035f, 0.055f, 0.075f, 0.92f);
    private static readonly Color Accent = new Color(0.92f, 0.04f, 0.035f, 1f);
    private static readonly Color ButtonInk = new Color(0.11f, 0.19f, 0.29f, 0.9f);
    private static readonly Color Paper = new Color(0.94f, 0.97f, 1f, 1f);
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int MetallicId = Shader.PropertyToID("_Metallic");
    private static readonly int SmoothnessId = Shader.PropertyToID("_Smoothness");
    private static readonly int GlossinessId = Shader.PropertyToID("_Glossiness");

    private MobileInputController mobileInput;
    private PlayerMovement playerMovement;
    private Text floorCountText;
    private Text bestFloorText;
    private GameObject menuPanel;
    private GameObject gameplayPanel;
    private GameObject gameOverPanel;
    private Text gameOverFloorText;
    private Text gameOverTitleText;
    private Font uiFont;
    private Vector3 playerStart;
    private MaterialPropertyBlock playerAppearanceBlock;
    private SceneTransitionController sceneTransition;
    [SerializeField] private ProceduralLevelGenerator levelGenerator;
    private RunState runState = RunState.Menu;

    private void Start()
    {
        Application.targetFrameRate = 60;
        SetHelixSceneDefaults();
        mobileInput = GetComponent<MobileInputController>();
        if (mobileInput == null)
            mobileInput = gameObject.AddComponent<MobileInputController>();
        sceneTransition = GetComponent<SceneTransitionController>();
        if (sceneTransition == null)
            sceneTransition = gameObject.AddComponent<SceneTransitionController>();
        sceneTransition.Configure(mainMenuSceneName, firstLevelSceneName);

        ConfigureLighting();
        EnsureVisualMaterials();
        CreateWorld();
        ApplyBackgroundColor();
        ApplyPlayerAppearance();
        playerMovement = playerTransform.GetComponent<PlayerMovement>();
        playerMovement.SetMovementEnabled(false);
        towerRotation.Configure(mobileInput);
        cameraFollow.SetTarget(playerTransform, cameraHeightOffset);
        CreateInterface();
        progression = GetComponent<GameProgression>();
        if (progression == null)
            progression = gameObject.AddComponent<GameProgression>();
        progression.Configure(playerMovement, floorCountText, bestFloorText);
        foreach (FloorPassTrigger trigger in towerRoot.GetComponentsInChildren<FloorPassTrigger>(true))
            trigger.Configure(progression);
        levelGenerator = GetComponent<ProceduralLevelGenerator>();
        if (levelGenerator == null)
            levelGenerator = gameObject.AddComponent<ProceduralLevelGenerator>();
        levelGenerator.Configure(helixPartPrefab, towerRoot, towerRoot.Find("Central Column"), progression,
            platformFloorCount, sectorsPerFloor, gapWidthSectors, firstGapSector, gapShiftPerFloor,
            floorSpacing, sectorRotationPerFloor, passTriggerDepth, passTriggerSize, columnEndPadding,
            darkPlatformMaterial, safeAccentMaterial, hazardColor, EndRun);
        levelGenerator.Initialize();
        if (!isMainMenuScene && autoStartLevel)
            StartGame();
        else
            ShowMenu();
    }

    public void SetSceneReferences(GameObject prefab, Transform tower, Transform player, TowerRotationController rotation,
        CameraFollowY followCamera)
    {
        helixPartPrefab = prefab;
        towerRoot = tower;
        playerTransform = player;
        towerRotation = rotation;
        cameraFollow = followCamera;
    }

    public void SetProgression(GameProgression gameProgression)
    {
        progression = gameProgression;
    }

    public void SetProceduralLevelGenerator(ProceduralLevelGenerator generator)
    {
        levelGenerator = generator;
    }

    public void ConfigureSceneFlow(bool mainMenuScene, bool startAutomatically, int sceneLevelNumber,
        int floorGoal, string firstLevelScene, string nextLevelScene, string menuScene)
    {
        isMainMenuScene = mainMenuScene;
        autoStartLevel = startAutomatically;
        levelNumber = Mathf.Max(0, sceneLevelNumber);
        floorsToCompleteLevel = Mathf.Max(0, floorGoal);
        firstLevelSceneName = firstLevelScene;
        nextLevelSceneName = nextLevelScene;
        mainMenuSceneName = menuScene;
        if (sceneTransition != null)
            sceneTransition.Configure(mainMenuSceneName, firstLevelSceneName);
    }

    public void SetScenePrefab(GameObject prefab)
    {
        helixPartPrefab = prefab;
    }

    public void SetHelixSceneDefaults()
    {
        platformFloorCount = Mathf.Max(platformFloorCount, 3);
        sectorsPerFloor = Mathf.Max(sectorsPerFloor, 3);
        gapWidthSectors = Mathf.Clamp(gapWidthSectors, 1, sectorsPerFloor - 2);
        floorSpacing = Mathf.Max(floorSpacing, 1.7f);
        playerSpawnClearance = Mathf.Max(playerSpawnClearance, 3f);
        playerOrbitRadius = Mathf.Max(playerOrbitRadius, 1.1f);
        cameraHeightOffset = Mathf.Max(cameraHeightOffset, 4.5f);
        RefreshPlayerStartPosition();
    }

    public void SetVisualMaterials(Material dark, Material accent, Material white, Material red, PhysicsMaterial bounce)
    {
        darkPlatformMaterial = dark;
        safeAccentMaterial = accent;
        columnMaterial = white;
        playerMaterial = red;
        ballBounceMaterial = bounce;
    }

    public void BuildWorldForEditor()
    {
        ConfigureLighting();
        EnsureVisualMaterials();
        CreateWorld();
        ApplyBackgroundColor();
        ApplyPlayerAppearance();
    }

    public void SetBackgroundColor(Color color)
    {
        backgroundColor = color;
        ApplyBackgroundColor();
    }

    public void SetAmbientLightColor(Color color)
    {
        ambientLightColor = color;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambientLightColor;
    }

    public void SetPlayerAppearance(Color color, float metallic, float smoothness)
    {
        playerColor = color;
        playerMetallic = Mathf.Clamp01(metallic);
        playerSmoothness = Mathf.Clamp01(smoothness);
        ApplyPlayerAppearance();
    }

    private void Update()
    {
        if (runState == RunState.Playing && floorsToCompleteLevel > 0
            && progression != null && progression.FloorsPassed >= floorsToCompleteLevel)
            CompleteLevel();
    }

    private void ConfigureLighting()
    {
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambientLightColor;

        Light keyLight = FindAnyObjectByType<Light>();
        if (keyLight == null)
        {
            GameObject lightObject = new GameObject("Key Light");
            keyLight = lightObject.AddComponent<Light>();
        }
        keyLight.type = LightType.Directional;
        keyLight.color = directionalLightColor;
        keyLight.intensity = directionalLightIntensity;
        keyLight.transform.rotation = Quaternion.Euler(directionalLightRotation);
    }

    private void EnsureVisualMaterials()
    {
        if (darkPlatformMaterial == null)
            darkPlatformMaterial = CreateMaterial(new Color(0.035f, 0.045f, 0.055f));

        if (safeAccentMaterial == null)
            safeAccentMaterial = CreateMaterial(playerColor);

        if (columnMaterial == null)
            columnMaterial = CreateMaterial(new Color(0.92f, 0.93f, 0.94f));

        if (playerMaterial == null)
            playerMaterial = CreateMaterial(playerColor);
    }

    private void CreateWorld()
    {
        if (towerRoot != null && playerTransform != null && towerRotation != null && cameraFollow != null)
        {
            RefreshPlayerStartPosition();
            if (!Application.isPlaying)
                EnsurePlatformLayout();
            UpdateCentralColumn();
            playerTransform.position = playerStart;
            if (Application.isPlaying)
                towerRotation.EnsurePhysicsSetup();
            else
                EnsureTowerPhysicsAndPlatformColliders();
            if (!Application.isPlaying)
                EnsureFloorPassTriggers();
            return;
        }

        if (darkPlatformMaterial == null)
            darkPlatformMaterial = CreateMaterial(new Color(0.035f, 0.045f, 0.055f));
        if (safeAccentMaterial == null)
            safeAccentMaterial = CreateMaterial(playerColor);
        if (columnMaterial == null)
            columnMaterial = CreateMaterial(new Color(0.92f, 0.93f, 0.94f));
        if (playerMaterial == null)
            playerMaterial = CreateMaterial(playerColor);

        towerRoot = new GameObject("Helix Tower").transform;
        towerRotation = towerRoot.gameObject.AddComponent<TowerRotationController>();
        towerRotation.EnsurePhysicsSetup();

        GameObject column = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        column.name = "Central Column";
        column.transform.SetParent(towerRoot, false);
        column.transform.localPosition = new Vector3(0f, (platformFloorCount - 1) * floorSpacing * 0.5f, 0f);
        column.transform.localScale = new Vector3(1.35f, platformFloorCount * floorSpacing * 0.5f + 1.4f, 1.35f);
        column.GetComponent<Renderer>().sharedMaterial = columnMaterial;
        Collider columnCollider = column.GetComponent<Collider>();
        if (columnCollider != null)
            columnCollider.enabled = false;

        if (!Application.isPlaying)
            BuildPlatformFloors();
        RefreshPlayerStartPosition();
        UpdateCentralColumn();
        EnsureTowerPhysicsAndPlatformColliders();

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.name = "Player Ball";
        player.transform.position = playerStart;
        player.transform.localScale = Vector3.one * 0.72f;
        player.GetComponent<Renderer>().sharedMaterial = playerMaterial;
        playerTransform = player.transform;
        playerMovement = player.AddComponent<PlayerMovement>();
        SphereCollider ballCollider = player.GetComponent<SphereCollider>();
        if (ballBounceMaterial == null)
            ballBounceMaterial = CreateBounceMaterial();
        ballCollider.sharedMaterial = ballBounceMaterial;

        Camera gameCamera = Camera.main;
        if (gameCamera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            gameCamera = cameraObject.GetComponent<Camera>();
        }
        gameCamera.clearFlags = CameraClearFlags.SolidColor;
        gameCamera.backgroundColor = backgroundColor;
        gameCamera.fieldOfView = 42f;
        gameCamera.transform.position = new Vector3(0f, playerStart.y + cameraHeightOffset, -12f);
        gameCamera.transform.LookAt(new Vector3(0f, playerStart.y - 1f, 0f));
        cameraFollow = gameCamera.GetComponent<CameraFollowY>();
        if (cameraFollow == null)
            cameraFollow = gameCamera.gameObject.AddComponent<CameraFollowY>();

        MobileInputController sceneInput = GetComponent<MobileInputController>();
        if (sceneInput == null)
            sceneInput = gameObject.AddComponent<MobileInputController>();
        towerRotation.Configure(sceneInput);
        cameraFollow.SetTarget(playerTransform, cameraHeightOffset);
    }

    private void BuildPlatformFloors()
    {
        if (helixPartPrefab == null)
            return;

        int gapWidth = Mathf.Clamp(gapWidthSectors, 1, sectorsPerFloor - 2);
        for (int floor = 0; floor < platformFloorCount; floor++)
        {
            float floorY = (platformFloorCount - 1 - floor) * floorSpacing;
            int gapStart = (firstGapSector + floor * gapShiftPerFloor) % sectorsPerFloor;
            if (gapStart < 0)
                gapStart += sectorsPerFloor;
            int accentSector = (gapStart + gapWidth + sectorsPerFloor / 2) % sectorsPerFloor;

            for (int sector = 0; sector < sectorsPerFloor; sector++)
            {
                int distanceFromGap = (sector - gapStart + sectorsPerFloor) % sectorsPerFloor;
                if (distanceFromGap < gapWidth)
                    continue;

                GameObject part = Instantiate(helixPartPrefab, towerRoot);
                part.name = $"Floor {floor + 1} · Sector {sector + 1}";
                part.transform.localPosition = new Vector3(0f, floorY, 0f);
                part.transform.localRotation = Quaternion.Euler(0f,
                    sector * (360f / sectorsPerFloor) + floor * sectorRotationPerFloor, 0f)
                    * helixPartPrefab.transform.localRotation;
                part.transform.localScale = helixPartPrefab.transform.localScale;

                MeshCollider partCollider = part.GetComponent<MeshCollider>();
                if (partCollider != null)
                    ConfigurePlatformCollider(partCollider);

                Renderer partRenderer = part.GetComponentInChildren<Renderer>();
                if (partRenderer != null)
                    partRenderer.sharedMaterial = sector == accentSector ? safeAccentMaterial : darkPlatformMaterial;
            }
        }

        if (!Application.isPlaying)
            EnsureFloorPassTriggers();
    }

    private void EnsureFloorPassTriggers()
    {
        if (towerRoot == null)
            return;

        for (int floor = 0; floor < platformFloorCount; floor++)
        {
            float floorY = (platformFloorCount - 1 - floor) * floorSpacing;
            string triggerName = $"Floor Pass Trigger {floor + 1}";
            Transform triggerTransform = towerRoot.Find(triggerName);
            if (triggerTransform == null)
            {
                GameObject triggerObject = new GameObject(triggerName, typeof(BoxCollider), typeof(FloorPassTrigger));
                triggerTransform = triggerObject.transform;
                triggerTransform.SetParent(towerRoot, false);
            }

            triggerTransform.localPosition = new Vector3(0f, floorY - passTriggerDepth, 0f);
            triggerTransform.localRotation = Quaternion.identity;
            BoxCollider box = triggerTransform.GetComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = passTriggerSize;
            triggerTransform.GetComponent<FloorPassTrigger>().Configure(null, floorY);
        }
    }

    private void EnsureTowerPhysicsAndPlatformColliders()
    {
        if (towerRoot == null)
            return;

        if (towerRotation == null)
            towerRotation = towerRoot.GetComponent<TowerRotationController>();
        if (towerRotation != null)
            towerRotation.EnsurePhysicsSetup();

        foreach (MeshCollider platformCollider in towerRoot.GetComponentsInChildren<MeshCollider>(true))
            ConfigurePlatformCollider(platformCollider);
    }

    private void EnsurePlatformLayout()
    {
        if (towerRoot == null || helixPartPrefab == null)
            return;

        int gapWidth = Mathf.Clamp(gapWidthSectors, 1, sectorsPerFloor - 2);
        int expectedParts = platformFloorCount * (sectorsPerFloor - gapWidth);
        int existingParts = 0;
        foreach (MeshCollider platformCollider in towerRoot.GetComponentsInChildren<MeshCollider>(true))
        {
            string partName = platformCollider.gameObject.name;
            if (partName.StartsWith("Floor ", System.StringComparison.Ordinal)
                && partName.Contains(" · Sector "))
                existingParts++;
        }

        if (existingParts == expectedParts)
            return;

        List<Transform> oldParts = new List<Transform>();
        foreach (Transform child in towerRoot)
        {
            if (child.name.StartsWith("Floor ", System.StringComparison.Ordinal)
                && child.name.Contains(" · Sector ")
                && child.GetComponent<MeshCollider>() != null)
                oldParts.Add(child);
        }

        foreach (Transform oldPart in oldParts)
        {
            if (Application.isPlaying)
                Destroy(oldPart.gameObject);
            else
                DestroyImmediate(oldPart.gameObject);
        }

        BuildPlatformFloors();
    }

    private void UpdateCentralColumn()
    {
        if (towerRoot == null)
            return;

        Transform column = towerRoot.Find("Central Column");
        if (column == null)
            return;

        column.localPosition = new Vector3(0f, (platformFloorCount - 1) * floorSpacing * 0.5f, 0f);
        column.localScale = new Vector3(1.35f, platformFloorCount * floorSpacing * 0.5f + 1.4f, 1.35f);
    }

    private void RefreshPlayerStartPosition()
    {
        float topFloorY = (platformFloorCount - 1) * floorSpacing;
        playerStart = new Vector3(0f, topFloorY + playerSpawnClearance, -playerOrbitRadius);
    }

    private static void ConfigurePlatformCollider(MeshCollider platformCollider)
    {
        MeshFilter meshFilter = platformCollider.GetComponent<MeshFilter>();
        Mesh platformMesh = meshFilter != null ? meshFilter.sharedMesh : null;
        if (platformMesh != null && platformCollider.sharedMesh != platformMesh)
        {
            platformCollider.sharedMesh = null;
            platformCollider.sharedMesh = platformMesh;
        }

        platformCollider.convex = true;
        platformCollider.isTrigger = false;
        platformCollider.enabled = platformMesh != null;
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

    private void ApplyBackgroundColor()
    {
        Camera targetCamera = Camera.main;
        if (targetCamera == null && cameraFollow != null)
            targetCamera = cameraFollow.GetComponent<Camera>();
        if (targetCamera == null)
            return;

        targetCamera.clearFlags = CameraClearFlags.SolidColor;
        targetCamera.backgroundColor = backgroundColor;
    }

    private void ApplyPlayerAppearance()
    {
        if (playerTransform == null)
            return;

        Renderer playerRenderer = playerTransform.GetComponentInChildren<Renderer>();
        Material playerSharedMaterial = playerRenderer != null ? playerRenderer.sharedMaterial : null;
        if (playerSharedMaterial == null)
            return;

        if (playerAppearanceBlock == null)
            playerAppearanceBlock = new MaterialPropertyBlock();

        playerRenderer.GetPropertyBlock(playerAppearanceBlock);
        if (playerSharedMaterial.HasProperty(BaseColorId))
            playerAppearanceBlock.SetColor(BaseColorId, playerColor);
        else if (playerSharedMaterial.HasProperty(ColorId))
            playerAppearanceBlock.SetColor(ColorId, playerColor);

        if (playerSharedMaterial.HasProperty(MetallicId))
            playerAppearanceBlock.SetFloat(MetallicId, playerMetallic);
        if (playerSharedMaterial.HasProperty(SmoothnessId))
            playerAppearanceBlock.SetFloat(SmoothnessId, playerSmoothness);
        else if (playerSharedMaterial.HasProperty(GlossinessId))
            playerAppearanceBlock.SetFloat(GlossinessId, playerSmoothness);

        playerRenderer.SetPropertyBlock(playerAppearanceBlock);
    }

    private static PhysicsMaterial CreateBounceMaterial()
    {
        return new PhysicsMaterial("Helix Ball Bounce")
        {
            bounciness = 0f,
            bounceCombine = PhysicsMaterialCombine.Minimum,
            frictionCombine = PhysicsMaterialCombine.Minimum
        };
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
        CreateText(menuPanel.transform, "HELIX JUMP", new Vector2(0.5f, 0.72f), new Vector2(0f, 0f),
            new Vector2(900f, 180f), 96, Accent, FontStyle.Bold);
        CreateText(menuPanel.transform, "Gira la torre y guía el balón por los huecos.", new Vector2(0.5f, 0.58f), Vector2.zero,
            new Vector2(860f, 100f), 42, Paper, FontStyle.Normal);
        CreateButton(menuPanel.transform, "JUGAR", new Vector2(440f, 140f), new Vector2(0.5f, 0.43f),
            Vector2.zero, Accent, Ink, StartGame);
        bestFloorText = CreateText(menuPanel.transform, "RÉCORD: 0 pisos", new Vector2(0.5f, 0.32f),
            Vector2.zero, new Vector2(800f, 80f), 38, Accent, FontStyle.Bold);
        CreateText(menuPanel.transform, "Desliza horizontalmente · A / D en el Editor",
            new Vector2(0.5f, 0.22f), Vector2.zero, new Vector2(920f, 90f), 30, Paper, FontStyle.Normal);

        gameplayPanel = new GameObject("Gameplay UI", typeof(RectTransform));
        RectTransform gameplayRect = gameplayPanel.GetComponent<RectTransform>();
        gameplayRect.SetParent(safeAreaRect, false);
        Stretch(gameplayRect);
        CreateText(gameplayPanel.transform, "DESLIZA PARA GIRAR LA TORRE",
            new Vector2(0.5f, 1f), new Vector2(0f, -56f), new Vector2(760f, 90f), 34, Paper, FontStyle.Bold);
        floorCountText = CreateText(gameplayPanel.transform, "PISOS: 0",
            new Vector2(0.5f, 1f), new Vector2(0f, -142f), new Vector2(760f, 80f), 42, Accent, FontStyle.Bold);
        if (levelNumber > 0)
            CreateText(gameplayPanel.transform, $"NIVEL {levelNumber}",
                new Vector2(1f, 1f), new Vector2(-140f, -70f), new Vector2(250f, 72f), 32, Paper, FontStyle.Bold);
        CreateButton(gameplayPanel.transform, "MENÚ", new Vector2(180f, 116f), new Vector2(0f, 1f),
            new Vector2(105f, -70f), ButtonInk, Paper, ReturnToMenu);
        gameplayPanel.SetActive(false);

        gameOverPanel = CreatePanel("Game Over", safeAreaRect, PanelInk);
        gameOverTitleText = CreateText(gameOverPanel.transform, "FIN DE LA PARTIDA", new Vector2(0.5f, 0.67f), Vector2.zero,
            new Vector2(900f, 150f), 76, Accent, FontStyle.Bold);
        gameOverFloorText = CreateText(gameOverPanel.transform, "PISOS: 0", new Vector2(0.5f, 0.54f),
            Vector2.zero, new Vector2(800f, 90f), 44, Paper, FontStyle.Bold);
        CreateButton(gameOverPanel.transform, "REINTENTAR", new Vector2(520f, 140f),
            new Vector2(0.5f, 0.41f), Vector2.zero, Accent, Ink, StartGame);
        CreateButton(gameOverPanel.transform, "MENÚ", new Vector2(440f, 120f),
            new Vector2(0.5f, 0.29f), Vector2.zero, ButtonInk, Paper, ReturnToMenu);
        gameOverPanel.SetActive(false);

        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
        Color background, Color foreground, UnityEngine.Events.UnityAction onClick, int fontSize = 44)
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
        if (isMainMenuScene)
        {
            sceneTransition.LoadFirstLevel();
            return;
        }

        if (runState == RunState.Playing)
            return;

        mobileInput.Clear();
        towerRotation.SetRotationEnabled(false);
        towerRotation.ResetRotation();
        playerMovement.ResetBall(playerStart);
        progression.BeginRun();
        levelGenerator?.ResetForRun();
        runState = RunState.Playing;
        menuPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        playerMovement.SetMovementEnabled(true);
        towerRotation.SetRotationEnabled(true);
    }

    private void ReturnToMenu()
    {
        if (!isMainMenuScene)
        {
            mobileInput.Clear();
            progression.EndRun();
            playerMovement.SetMovementEnabled(false);
            towerRotation.SetRotationEnabled(false);
            if (sceneTransition.LoadMenu())
                return;
        }

        mobileInput.Clear();
        progression.EndRun();
        runState = RunState.Menu;
        towerRotation.SetRotationEnabled(false);
        towerRotation.ResetRotation();
        playerMovement.SetMovementEnabled(false);
        playerMovement.ResetBall(playerStart);
        levelGenerator?.ResetForRun();
        menuPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    private void ShowMenu()
    {
        runState = RunState.Menu;
        menuPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        playerMovement.SetMovementEnabled(false);
        playerMovement.ResetBall(playerStart);
        towerRotation.SetRotationEnabled(false);
        towerRotation.ResetRotation();
    }

    private void EndRun()
    {
        if (runState != RunState.Playing)
            return;

        runState = RunState.GameOver;
        progression.EndRun();
        mobileInput.Clear();
        playerMovement.SetMovementEnabled(false);
        towerRotation.SetRotationEnabled(false);
        gameOverTitleText.text = "FIN DE LA PARTIDA";
        gameOverFloorText.text = $"PISOS: {progression.FloorsPassed}";
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    private void CompleteLevel()
    {
        if (runState != RunState.Playing)
            return;

        runState = RunState.GameOver;
        progression.EndRun();
        mobileInput.Clear();
        playerMovement.SetMovementEnabled(false);
        towerRotation.SetRotationEnabled(false);

        if (!string.IsNullOrWhiteSpace(nextLevelSceneName)
            && sceneTransition.LoadNextLevel(nextLevelSceneName))
            return;

        gameOverTitleText.text = "¡NIVEL COMPLETADO!";
        gameOverFloorText.text = $"PISOS: {progression.FloorsPassed}";
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
