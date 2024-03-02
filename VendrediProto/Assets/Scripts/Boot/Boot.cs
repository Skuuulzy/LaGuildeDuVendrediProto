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
        // We wait the end of frame for persistent manager initialisation.
        await UniTask.WaitForEndOfFrame(this);
        
        // Then we use the scene loader to load the main menu.
        HybridSceneManager.Instance.LoadScene(HybridSceneManager.SceneIdentifier.MAIN_MENU);
    }
}