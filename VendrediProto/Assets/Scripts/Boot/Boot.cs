using Cysharp.Threading.Tasks;
using UnityEngine;
using VComponent.SceneLoader;

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
        HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MAIN_MENU);
    }
}