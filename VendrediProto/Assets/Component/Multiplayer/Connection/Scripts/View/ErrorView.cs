using TMPro;
using UnityEngine;
using VComponent.SceneLoader;

namespace VComponent.Multiplayer
{
    public class ErrorView : MonoBehaviour
    {
        [SerializeField] private GameObject _window;
        [SerializeField] private TMP_Text _errorTitleTxt;
        [SerializeField] private TMP_Text _errorDetailsTxt;

        private void Awake()
        {
            MultiplayerConnectionManager.OnTaskFailed += HandleTaskFailed;
        }

        private void OnDestroy()
        {
            MultiplayerConnectionManager.OnTaskFailed -= HandleTaskFailed;
        }

        private void HandleTaskFailed(string errorTitle ,string errorDetails)
        {
            _window.SetActive(true);
            _errorTitleTxt.text = errorTitle;
            _errorDetailsTxt.text = errorDetails;
        }

        public void ToMainMenu()
        {
            _ = HybridSceneLoader.Instance.TransitionTo(HybridSceneLoader.SceneIdentifier.MAIN_MENU);
        }
    }
}