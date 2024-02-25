using GDV.SceneLoader;
using TMPro;
using UnityEngine;

namespace VComponent.Multiplayer
{
    public class ErrorView : MonoBehaviour
    {
        [SerializeField] private GameObject _window;
        [SerializeField] private TMP_Text _errorTitleTxt;
        [SerializeField] private TMP_Text _errorDetailsTxt;

        private void Awake()
        {
            MultiplayerManager.OnTaskFailed += HandleTaskFailed;
        }

        private void OnDestroy()
        {
            MultiplayerManager.OnTaskFailed -= HandleTaskFailed;
        }

        private void HandleTaskFailed(string errorTitle ,string errorDetails)
        {
            _window.SetActive(true);
            _errorTitleTxt.text = errorTitle;
            _errorDetailsTxt.text = errorDetails;
        }

        public void ToMainMenu()
        {
            SceneLoader.OnLoadScene?.Invoke(SceneLoader.SceneIdentifier.MAIN_MENU);
        }
    }
}