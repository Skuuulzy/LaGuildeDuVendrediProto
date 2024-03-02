using Eflatun.SceneReference;
using MyGameDevTools.SceneLoading;
using MyGameDevTools.SceneLoading.UniTaskSupport;
using UnityEngine;
using UnityEngine.SceneManagement;
using VComponent.Tools.Singletons;
using SceneManager = MyGameDevTools.SceneLoading.SceneManager;

namespace GDV.SceneLoader
{
    public class HybridSceneManager : PersistentSingleton<HybridSceneManager>
    {
        [SerializeField] private SerializableDictionary<SceneIdentifier, SceneReference> _scenes;
        
        private readonly ISceneManager _sceneManager = new SceneManager();
        private ISceneLoaderUniTask _sceneLoader;

        private static Scene _originalScene;

        private LoadSceneInfoName LoadingScreenReference => GetSceneReference(SceneIdentifier.LOADING_SCREEN);
        
        #region DATA

        public enum SceneIdentifier
        {
            MAIN_MENU,
            LOADING_SCREEN,
            CHARACTER_CONTROL,
            INTERACTION_EXAMPLE,
            MAP_TEST,
            MULTIPLAYER_LOBBY
        }

        private LoadSceneInfoName GetSceneReference(SceneIdentifier identifier)
        {
            if (_scenes.ContainsKey(identifier))
            {
                return new LoadSceneInfoName(_scenes[identifier].Name);
            }
            
            Debug.LogError($"No scene associated with the identifier {identifier} found in the scenes.");
            return default;
        }

        #endregion

        #region MONO

        protected override void Awake()
        {
            base.Awake();
            
            _sceneLoader = new SceneLoaderUniTask(_sceneManager);
            _originalScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }

        #endregion

        #region SCENE LOAD METHODS

        public async void LoadScene(SceneIdentifier identifier)
        {
            var targetSceneReference = GetSceneReference(identifier);

            if (targetSceneReference.Reference == null)
            {
                return;
            }
            
            // Used for the first transition, the original scene is stored at the awake of the singleton.
            if (_originalScene != default)
            {
                _ = await _sceneLoader.TransitionToSceneAsync(targetSceneReference, LoadingScreenReference, _originalScene);
                _originalScene = default;
                return;
            }

            _ = await _sceneLoader.TransitionToSceneAsync(targetSceneReference, LoadingScreenReference);
        }

        #endregion
    }
}