using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Automatic setup of the scene with all required components to play any scene independently.
/// </summary>
[DefaultExecutionOrder(-70)]
public class EditorSceneUtility : MonoBehaviour
{
    private void Awake()
    {
        CheckPersistantManager();
    }

    /// <summary>
    /// Check if the persistant manager scene is loaded, if not load it.
    /// </summary>
    private void CheckPersistantManager()
    {
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            string sceneName = SceneManager.GetSceneAt(i).name;
            if (sceneName == "PersistantManager")
            {
                return;
            }
        }

        SceneManager.LoadScene("PersistantManager", LoadSceneMode.Additive);
    }
}