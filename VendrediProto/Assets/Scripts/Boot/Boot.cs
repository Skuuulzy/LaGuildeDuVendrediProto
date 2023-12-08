using VTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    [SerializeField] private LoadSceneDataSO _sceneToLoad;
    
    private void Awake()
    {
        SceneManager.LoadSceneAsync("PersistantManager", LoadSceneMode.Additive).completed += delegate
        {
            _sceneToLoad.Load();
        };
    }
}