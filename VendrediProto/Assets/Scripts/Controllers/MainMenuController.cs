using UnityEngine;
using VComponent.SceneLoader;

public class MainMenuController : MonoBehaviour
{
    public void LoadMapArtScene()
    {
        HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MAP_ART);
    }
    public void LoadMapDevScene()
    {
        HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MAP_DEV);
    }
    
    public void LoadMultiplayerLobby()
    {
        HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MULTIPLAYER_LOBBY);
    }
}