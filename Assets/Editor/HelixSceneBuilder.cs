using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class HelixSceneBuilder
{
    private const string MainScenePath = "Assets/Scenes/Main.unity";
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

        if (GameObject.Find("Helix Tower") == null)
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

        if (scene.path != MainScenePath || GameObject.Find("Helix Tower") != null)
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

        controller.SetHelixSceneDefaults();
        controller.SetScenePrefab(prefab);
        controller.SetVisualMaterials(
            LoadOrCreateMaterial("HelixDark.mat", new Color(0.035f, 0.045f, 0.055f)),
            LoadOrCreateMaterial("HelixYellow.mat", new Color(1f, 0.78f, 0.08f)),
            LoadOrCreateMaterial("HelixColumn.mat", new Color(0.92f, 0.93f, 0.94f)),
            LoadOrCreateMaterial("HelixBall.mat", new Color(0.92f, 0.04f, 0.035f)),
            LoadOrCreateBounceMaterial());
        controller.BuildWorldForEditor();

        EditorUtility.SetDirty(controller);
        CameraFollowY cameraFollow = Object.FindAnyObjectByType<CameraFollowY>();
        if (cameraFollow != null)
            EditorUtility.SetDirty(cameraFollow);
        TowerRotationController towerRotation = Object.FindAnyObjectByType<TowerRotationController>();
        if (towerRotation != null)
            EditorUtility.SetDirty(towerRotation);
        EditorSceneManager.MarkSceneDirty(scene);
        AssetDatabase.SaveAssets();
        EditorSceneManager.SaveScene(scene);
        GameObject tower = GameObject.Find("Helix Tower");
        if (tower != null)
        {
            Selection.activeGameObject = tower;
            SceneView.lastActiveSceneView?.FrameSelected();
        }
        Debug.Log("Helix Jump: escena vertical construida y guardada en Main.unity.");
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

        material.bounciness = 0.72f;
        material.bounceCombine = PhysicsMaterialCombine.Maximum;
        material.frictionCombine = PhysicsMaterialCombine.Minimum;
        EditorUtility.SetDirty(material);
        return material;
    }
}
