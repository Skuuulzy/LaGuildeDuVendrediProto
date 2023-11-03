using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    [SerializeField] private LoadSceneDataSO _sceneToLoad;

    private void Awake()
    {
        SceneManager.LoadScene("PersistantManager", LoadSceneMode.Additive);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        _sceneToLoad.Load();
    }
}
