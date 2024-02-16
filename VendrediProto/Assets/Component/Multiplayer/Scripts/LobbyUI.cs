using Eflatun.SceneReference;
using UnityEngine;

namespace Component.Multiplayer
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] SceneReference _multiScene;

        public async void CreateGame()
        {
            await Multiplayer.Instance.CreateLobby();
            MultiplayerSceneLoader.LoadNetwork(_multiScene);
        }

        public async void QuickJoinGame()
        {
            await Multiplayer.Instance.QuickJoinLobby();
        }
    }
}