using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class SceneTransitionController : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "Main";
    [SerializeField] private string firstLevelSceneName = "Level1";

    private bool loadInProgress;

    public void Configure(string menuScene, string firstLevelScene)
    {
        menuSceneName = menuScene;
        firstLevelSceneName = firstLevelScene;
    }

    public bool LoadFirstLevel() => LoadScene(firstLevelSceneName);

    public bool LoadMenu() => LoadScene(menuSceneName);

    public bool LoadNextLevel(string sceneName) => LoadScene(sceneName);

    private bool LoadScene(string sceneName)
    {
        if (loadInProgress)
            return false;

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("No se puede cambiar de escena: el nombre está vacío.", this);
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"La escena '{sceneName}' no está incluida en Build Settings.", this);
            return false;
        }

        try
        {
            loadInProgress = true;
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation != null)
                return true;

            loadInProgress = false;
            Debug.LogError($"Unity no pudo iniciar la carga de la escena '{sceneName}'.", this);
            return false;
        }
        catch (System.Exception exception)
        {
            loadInProgress = false;
            Debug.LogError($"Error al cargar la escena '{sceneName}': {exception.Message}", this);
            return false;
        }
    }
}
