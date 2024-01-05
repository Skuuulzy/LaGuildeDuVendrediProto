using Cysharp.Threading.Tasks;
using GDV.SceneLoader;
using MyGameDevTools.SceneLoading;
using MyGameDevTools.SceneLoading.UniTaskSupport;
using UnityEngine;

public class Boot : MonoBehaviour
{
    private void Awake()
    {
        LoadFirstScenes();
    }

    private async void LoadFirstScenes()
    {
        // We need a first scene loader to load the persistent manager scene, then the scene loader will handle all scenes loading.
        ISceneManager sceneManager = new SceneManager();
        ISceneLoaderUniTask sceneLoader = new SceneLoaderUniTask(sceneManager);

        await sceneLoader.LoadSceneAsync(new LoadSceneInfoName("PersistentManager"));

        // We wait the end of frame for persistent manager initialisation.
        await UniTask.WaitForEndOfFrame(this);
        
        // Then we use the scene loader to load the main menu.
        SceneLoader.OnLoadScene?.Invoke(SceneLoader.SceneIdentifier.MAIN_MENU);
        
        // The we finally unload the boot
        _ = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Boot");
    }
}