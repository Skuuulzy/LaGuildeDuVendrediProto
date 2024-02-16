#if UNITY_EDITOR
using GDV.SceneLoader;
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
        CheckPersistentManager();
    }

    /// <summary>
    /// Check if the persistent manager scene is loaded, if not load it.
    /// </summary>
    private void CheckPersistentManager()
    {
        if (!ExtensionMethods.LoadedSceneNames().Contains("PersistentManager"))
        {
            SceneManager.LoadScene("PersistentManager", LoadSceneMode.Additive);
            SceneLoader.SetOriginalScene();
        }
    }
}
#endif