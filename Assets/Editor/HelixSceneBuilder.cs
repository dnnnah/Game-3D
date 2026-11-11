using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class HelixSceneBuilder
{
    private const string MainScenePath = "Assets/Scenes/Main.unity";
    private const string LevelOneScenePath = "Assets/Scenes/Level1.unity";
    private const string LevelTwoScenePath = "Assets/Scenes/Level2.unity";
    private const string LevelThreeScenePath = "Assets/Scenes/Level3.unity";
    private const string HelixPrefabPath = "Assets/Prefabs/HelixPartPrefab.prefab";
    private const string MaterialFolder = "Assets/Materials";

    static HelixSceneBuilder()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorApplication.delayCall += BuildActiveMainSceneIfNeeded;
    }

    [MenuItem("Tools/Helix Jump/Build Main Scene")]
    public static void BuildMainScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != MainScenePath)
            scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);

        BuildScene(scene);
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        if (scene.path == MainScenePath)
            EditorApplication.delayCall += BuildActiveMainSceneIfNeeded;
    }

    private static void BuildActiveMainSceneIfNeeded()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (string.IsNullOrEmpty(scene.path) && !scene.isDirty
            && AssetDatabase.LoadAssetAtPath<SceneAsset>(MainScenePath) != null)
            scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Single);

        if (scene.path != MainScenePath)
            return;

        BuildScene(scene);
    }

    private static void BuildScene(Scene scene)
    {
        GameSceneController controller = Object.FindAnyObjectByType<GameSceneController>();
        if (controller == null)
        {
            GameObject session = new GameObject("Game Session");
            controller = session.AddComponent<GameSceneController>();
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(HelixPrefabPath);
        if (prefab == null)
        {
            Debug.LogError($"No se encontró el prefab HelixPart en {HelixPrefabPath}.");
            return;
        }

        GameObject existingTower = GameObject.Find("Helix Tower");
        bool sceneAlreadyBuilt = existingTower != null;
        controller.SetHelixSceneDefaults();
        controller.SetScenePrefab(prefab);
        controller.ConfigureSceneFlow(true, false, 0, 0, "Level1", string.Empty, "Main");
        controller.SetVisualMaterials(
            LoadOrCreateMaterial("HelixDark.mat", new Color(0.035f, 0.045f, 0.055f)),
            LoadOrCreateMaterial("HelixAccent.mat", new Color(0.92f, 0.04f, 0.035f)),
            LoadOrCreateMaterial("HelixColumn.mat", new Color(0.92f, 0.93f, 0.94f)),
            LoadOrCreateMaterial("HelixBall.mat", new Color(0.92f, 0.04f, 0.035f)),
            LoadOrCreateBounceMaterial());

        if (sceneAlreadyBuilt)
        {
            GameObject player = GameObject.Find("Player Ball");
            TowerRotationController rotation = existingTower.GetComponent<TowerRotationController>();
            CameraFollowY follow = Object.FindAnyObjectByType<CameraFollowY>();
            if (player != null && rotation != null && follow != null)
                controller.SetSceneReferences(prefab, existingTower.transform, player.transform, rotation, follow);
        }

        controller.BuildWorldForEditor();
        ConfigurePlatformColliders(GameObject.Find("Helix Tower"));
        GameProgression progression = Object.FindAnyObjectByType<GameProgression>();
        if (progression == null)
            progression = controller.gameObject.AddComponent<GameProgression>();
        controller.SetProgression(progression);

        ProceduralLevelGenerator generator = controller.GetComponent<ProceduralLevelGenerator>();
        if (generator == null)
            generator = controller.gameObject.AddComponent<ProceduralLevelGenerator>();
        controller.SetProceduralLevelGenerator(generator);

        EditorUtility.SetDirty(controller);
        EditorUtility.SetDirty(progression);
        EditorUtility.SetDirty(generator);
        CameraFollowY cameraFollow = Object.FindAnyObjectByType<CameraFollowY>();
        if (cameraFollow != null)
            EditorUtility.SetDirty(cameraFollow);
        TowerRotationController towerRotation = Object.FindAnyObjectByType<TowerRotationController>();
        if (towerRotation != null)
            EditorUtility.SetDirty(towerRotation);
        EditorSceneManager.MarkSceneDirty(scene);
        AssetDatabase.SaveAssets();
        EditorSceneManager.SaveScene(scene);
        EnsureLevelScenes(prefab);
        GameObject tower = GameObject.Find("Helix Tower");
        if (tower != null)
        {
            Selection.activeGameObject = tower;
            SceneView.lastActiveSceneView?.FrameSelected();
        }
        Debug.Log("Helix Jump: escena vertical construida y guardada en Main.unity.");
    }

    private static void EnsureLevelScenes(GameObject prefab)
    {
        CreateOrUpdateLevelScene(LevelOneScenePath, prefab, 1, 6, "Level2");
        CreateOrUpdateLevelScene(LevelTwoScenePath, prefab, 2, 8, "Level3");
        CreateOrUpdateLevelScene(LevelThreeScenePath, prefab, 3, 12, string.Empty, 13.5f, 1.1f, 20.5f, 2.8f,
            new[]
            {
                new GameProgression.DifficultyStep { floorsRequired = 0, fallSpeedCap = 16f, hazardSectorCount = 2 },
                new GameProgression.DifficultyStep { floorsRequired = 4, fallSpeedCap = 18.5f, hazardSectorCount = 3 },
                new GameProgression.DifficultyStep { floorsRequired = 8, fallSpeedCap = 20.5f, hazardSectorCount = 4 }
            });

        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        AddBuildSceneIfMissing(buildScenes, MainScenePath);
        AddBuildSceneIfMissing(buildScenes, LevelOneScenePath);
        AddBuildSceneIfMissing(buildScenes, LevelTwoScenePath);
        AddBuildSceneIfMissing(buildScenes, LevelThreeScenePath);
        EditorBuildSettings.scenes = buildScenes.ToArray();
        AssetDatabase.SaveAssets();
    }

    private static void CreateOrUpdateLevelScene(string scenePath, GameObject prefab, int levelNumber,
        int floorsToComplete, string nextScene, float initialSpeed = 0f, float increasePerFloor = 0f,
        float speedCap = 0f, float changeRate = 0f, GameProgression.DifficultyStep[] difficultySteps = null)
    {
        Scene previousActiveScene = SceneManager.GetActiveScene();
        bool sceneExists = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) != null;
        Scene levelScene = sceneExists
            ? EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive)
            : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        try
        {
            SceneManager.SetActiveScene(levelScene);

            GameSceneController controller = FindControllerInScene(levelScene);
            if (controller == null)
            {
                GameObject session = new GameObject("Game Session");
                controller = session.AddComponent<GameSceneController>();
            }

            controller.SetScenePrefab(prefab);
            controller.ConfigureSceneFlow(false, true, levelNumber, floorsToComplete,
                "Level1", nextScene, "Main");

            if (difficultySteps != null)
            {
                GameProgression progression = controller.GetComponent<GameProgression>();
                if (progression == null)
                    progression = controller.gameObject.AddComponent<GameProgression>();
                progression.ConfigureLevelDifficulty(initialSpeed, increasePerFloor, speedCap, changeRate,
                    difficultySteps);
                controller.SetProgression(progression);
                EditorUtility.SetDirty(progression);
            }

            EditorUtility.SetDirty(controller);
            EditorSceneManager.MarkSceneDirty(levelScene);
            EditorSceneManager.SaveScene(levelScene, scenePath);
        }
        finally
        {
            if (levelScene.IsValid() && levelScene.isLoaded)
                EditorSceneManager.CloseScene(levelScene, true);
            if (previousActiveScene.IsValid() && previousActiveScene.isLoaded)
                SceneManager.SetActiveScene(previousActiveScene);
        }
    }

    private static GameSceneController FindControllerInScene(Scene scene)
    {
        GameObject[] roots = scene.GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            GameSceneController controller = roots[i].GetComponentInChildren<GameSceneController>(true);
            if (controller != null)
                return controller;
        }

        return null;
    }

    private static void AddBuildSceneIfMissing(List<EditorBuildSettingsScene> buildScenes, string scenePath)
    {
        for (int i = 0; i < buildScenes.Count; i++)
        {
            if (buildScenes[i].path == scenePath)
            {
                if (!buildScenes[i].enabled)
                    buildScenes[i] = new EditorBuildSettingsScene(scenePath, true);
                return;
            }
        }

        buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
    }

    private static void ConfigurePlatformColliders(GameObject tower)
    {
        if (tower == null)
            return;

        foreach (MeshCollider platformCollider in tower.GetComponentsInChildren<MeshCollider>(true))
        {
            MeshFilter meshFilter = platformCollider.GetComponent<MeshFilter>();
            Mesh platformMesh = meshFilter != null ? meshFilter.sharedMesh : null;
            if (platformMesh == null)
            {
                platformCollider.enabled = false;
                Debug.LogError($"El sector {platformCollider.name} no tiene una malla visual asignada.", platformCollider);
                continue;
            }

            platformCollider.sharedMesh = null;
            platformCollider.sharedMesh = platformMesh;
            platformCollider.convex = true;
            platformCollider.isTrigger = false;
            platformCollider.enabled = true;
            EditorUtility.SetDirty(platformCollider);
            PrefabUtility.RecordPrefabInstancePropertyModifications(platformCollider);
        }
    }

    private static Material LoadOrCreateMaterial(string fileName, Color color)
    {
        string path = Path.Combine(MaterialFolder, fileName).Replace('\\', '/');
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");

            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);
        else if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static PhysicsMaterial LoadOrCreateBounceMaterial()
    {
        const string path = MaterialFolder + "/HelixBall.physicMaterial";
        PhysicsMaterial material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(path);
        if (material == null)
        {
            material = new PhysicsMaterial("Helix Ball Bounce");
            AssetDatabase.CreateAsset(material, path);
        }

        material.bounciness = 0f;
        material.bounceCombine = PhysicsMaterialCombine.Minimum;
        material.frictionCombine = PhysicsMaterialCombine.Minimum;
        EditorUtility.SetDirty(material);
        return material;
    }
}
