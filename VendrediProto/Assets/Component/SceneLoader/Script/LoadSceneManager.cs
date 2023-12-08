using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading.Tasks;

namespace VTools
{
    public static class LoadSceneManager
    {
        public static UnityAction<LoadSceneDataSO> LoadScenesEvent;
        public static UnityAction<LoadSceneDataSO> OnFinishedLoadingScenes;

        private static List<string> _loadedSceneNames = new ();
        private static List<string> _scenesToKeepLoaded = new ();

        private static bool _isLoading = false;
        public static bool IsLoading => _isLoading;

        public static void RestartGame()
        {
            _isLoading = false;
            _loadedSceneNames.Clear();
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        public static async Task LoadScenesAsync(LoadSceneDataSO loadSceneData)
        {
            _isLoading = true;

            string sceneNames = "";
            float startTime = Time.unscaledTime;

            foreach (string sceneName in loadSceneData.SceneNamesToUnload)
            {
                sceneNames += sceneName + ", ";
            }
            //Start unloading scenes
            
			float elapsedTime = Time.unscaledTime - startTime;
			float remainingTime = loadSceneData.MinimumLoadTime - elapsedTime;
			if (remainingTime > 0)
			{
				await Task.Delay(Mathf.CeilToInt(remainingTime * 1000));
			}

			List<AsyncOperation> scenesUnloading = new List<AsyncOperation>();
			foreach (string sceneNameToUnload in loadSceneData.SceneNamesToUnload)
            {
                scenesUnloading.Add(SceneManager.UnloadSceneAsync(sceneNameToUnload));
                _loadedSceneNames.Remove(sceneNameToUnload);
            }

            while (scenesUnloading.FirstOrDefault(x => x.isDone == false) != null)
            {
                await Task.Delay(100);
            }
            //Scenes are unloaded

            sceneNames = "";
            foreach (string sceneName in loadSceneData.SceneNamesToLoad)
            {
                sceneNames += sceneName + ", ";
            }

            //Start loading scenes
            List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
            foreach (string sceneNameToLoad in loadSceneData.SceneNamesToLoad)
            {
                AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneNameToLoad, LoadSceneMode.Additive);
                sceneLoad.allowSceneActivation = false;
                scenesLoading.Add(sceneLoad);
                _loadedSceneNames.Add(sceneNameToLoad);
            }

            foreach(string sceneNameToKeepLoaded in loadSceneData.SceneNamesToKeepLoaded)
            {
                _scenesToKeepLoaded.Add(sceneNameToKeepLoaded);
            }

            int scenesLoaded = 0;

            do
            {
                await Task.Delay(100);
                scenesLoaded = scenesLoading.Count(x => x.progress >= 0.9f);
            } while (scenesLoaded < scenesLoading.Count);

            //Scenes are loaded
            foreach(AsyncOperation op in scenesLoading)
            {
                op.allowSceneActivation = true;
            }
            while (scenesLoading.FirstOrDefault(x => x.isDone == false) != null)
            {
                await Task.Delay(100);
            }

            if (!string.IsNullOrEmpty(loadSceneData.SceneNameToActivate))
            {
                if (_loadedSceneNames.Contains(loadSceneData.SceneNameToActivate))
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadSceneData.SceneNameToActivate));
                }
            }

            _isLoading = false;
            OnFinishedLoadingScenes?.Invoke(loadSceneData);
        }

        #region static methods
        //Generic method that can load certain scenes while unloading others
        public static void LoadScenes(List<string> sceneNamesToLoad, List<string> sceneNamesToUnload, LoadTypeSO loadType, string sceneNameToActivate = null, float minimumLoadTime = 0f)
        {
            //Hack to enable scenes loading when persistent manager is not open
            if (LoadScenesEvent == null)
            {
                SceneManager.LoadSceneAsync("PersistentManager", LoadSceneMode.Additive).completed += delegate { LoadScenes(sceneNamesToLoad, sceneNamesToUnload, loadType, sceneNameToActivate, minimumLoadTime);};
                return;
            }
            //End of hack

            LoadSceneDataSO loadSceneData = ScriptableObject.CreateInstance<LoadSceneDataSO>();
            loadSceneData.SetInfo(sceneNamesToLoad, sceneNamesToUnload, loadType, sceneNameToActivate, minimumLoadTime);
            LoadScenesEvent?.Invoke(loadSceneData);
        }

        public static void LoadScenes(List<Scene> scenesToLoad, List<Scene> scenesToUnload, LoadTypeSO loadType, int sceneIndexToActivate = -1, float minimumLoadTime = 0f)
        {
            List<string> sceneNamesToLoad = new List<string>();
            List<string> sceneNamesToUnload = new List<string>();
            string sceneNameToActivate = null;
            if (scenesToLoad != null)
            {
                sceneNamesToLoad = scenesToLoad.Select(x => x.name).ToList();
            }
            if (scenesToUnload != null)
            {
                sceneNamesToUnload = scenesToUnload.Select(x => x.name).ToList();
            }
            if (sceneIndexToActivate >= 0 && sceneIndexToActivate < scenesToLoad.Count)
            {
                sceneNameToActivate = scenesToLoad[sceneIndexToActivate].name;
            }
            LoadScenes(sceneNamesToLoad, sceneNamesToUnload, loadType, sceneNameToActivate, minimumLoadTime);
        }

        public static void LoadScenes(LoadSceneDataSO loadSceneData)
        {
            LoadScenesEvent?.Invoke(loadSceneData);
        }

        //Loading scenes additively
        public static void LoadScenesAdditive(List<string> sceneNamesToLoad, LoadTypeSO loadType, string sceneNameToActivate = null, float minimumLoadTime = 0f)
        {
            LoadScenes(sceneNamesToLoad, new List<string>(), loadType, sceneNameToActivate, minimumLoadTime);
        }

        public static void LoadScenesAdditive(List<Scene> scenesToLoad, LoadTypeSO loadType, int sceneIndexToActivate = -1, float minimumLoadTime = 0f)
        {
            List<string> sceneNamesToLoad = new List<string>();
            string sceneNameToActivate = null;
            if (scenesToLoad != null)
            {
                sceneNamesToLoad = scenesToLoad.Select(x => x.name).ToList();
            }
            if (sceneIndexToActivate >= 0 && sceneIndexToActivate < scenesToLoad.Count)
            {
                sceneNameToActivate = scenesToLoad[sceneIndexToActivate].name;
            }
            LoadScenes(sceneNamesToLoad, new List<string>(), loadType, sceneNameToActivate, minimumLoadTime);
        }

        //Loading scenes in single mode : unloads all other scenes except the loading scene
        public static void LoadScenesSingle(List<string> sceneNamesToLoad, LoadTypeSO loadType, string sceneNameToActivate = null, float minimumLoadTime = 0f)
        {
            List<string> sceneNamesToUnload = new List<string>();
            sceneNamesToUnload.AddRange(_loadedSceneNames);
            sceneNamesToUnload.RemoveAll(x => _scenesToKeepLoaded.Contains(x));
            LoadScenes(sceneNamesToLoad, sceneNamesToUnload, loadType, sceneNameToActivate, minimumLoadTime);
        }

        public static void LoadScenesSingle(List<Scene> scenesToLoad, LoadTypeSO loadType, int sceneIndexToActivate = -1, float minimumLoadTime = 0f)
        {
            List<string> sceneNamesToLoad = new List<string>();
            string sceneNameToActivate = null;
            if (scenesToLoad != null)
            {
                sceneNamesToLoad = scenesToLoad.Select(x => x.name).ToList();
            }
            if (sceneIndexToActivate >= 0 && sceneIndexToActivate < scenesToLoad.Count)
            {
                sceneNameToActivate = scenesToLoad[sceneIndexToActivate].name;
            }
            LoadScenesSingle(sceneNamesToLoad, loadType, sceneNameToActivate, minimumLoadTime);
        }

        //Loading one scene
        public static void LoadSceneAdditive(string sceneNameToLoad, LoadTypeSO loadType, bool setActiveScene = false, float minimumLoadTime = 0f)
        {
            List<string> sceneNamesToLoad = new List<string>();
            sceneNamesToLoad.Add(sceneNameToLoad);
            LoadScenesAdditive(sceneNamesToLoad, loadType, setActiveScene ? sceneNameToLoad : null, minimumLoadTime);
        }

        public static void LoadSceneAdditive(Scene sceneToLoad, LoadTypeSO loadType, bool setActiveScene = false, float minimumLoadTime = 0f)
        {
            List<Scene> scenesToLoad = new List<Scene>();
            scenesToLoad.Add(sceneToLoad);
            LoadScenesAdditive(scenesToLoad, loadType, setActiveScene ? 0 : -1, minimumLoadTime);
        }

        public static void LoadSceneSingle(string sceneNameToLoad, LoadTypeSO loadType, bool setActiveScene = true, float minimumLoadTime = 0f)
        {
            List<string> sceneNamesToLoad = new List<string>();
            sceneNamesToLoad.Add(sceneNameToLoad);
            LoadScenesSingle(sceneNamesToLoad, loadType, setActiveScene ? sceneNameToLoad : null, minimumLoadTime);
        }

        public static void LoadSceneSingle(Scene sceneToLoad, LoadTypeSO loadType, bool setActiveScene = true, float minimumLoadTime = 0f)
        {
            List<Scene> scenesToLoad = new List<Scene>();
            scenesToLoad.Add(sceneToLoad);
            LoadScenesSingle(scenesToLoad, loadType, setActiveScene ? 0 : -1, minimumLoadTime);
        }

        //Unloading scenes
        public static void UnloadScenes(List<string> sceneNamesToUnload)
        {
            foreach (string sceneName in sceneNamesToUnload)
            {
                _loadedSceneNames.Remove(sceneName);
                SceneManager.UnloadSceneAsync(sceneName);
            }
        }

        public static void UnloadScenes(List<Scene> scenesToUnload)
        {
            List<string> sceneNamesToUnload = new List<string>();
            if (scenesToUnload != null)
            {
                sceneNamesToUnload = scenesToUnload.Select(x => x.name).ToList();
            }
            UnloadScenes(sceneNamesToUnload);
        }

        public static void UnloadScene(string sceneNameToUnload)
        {
            _loadedSceneNames.Remove(sceneNameToUnload);
            SceneManager.UnloadSceneAsync(sceneNameToUnload);
        }

        public static void UloadScene(Scene sceneToUnload)
        {
            _loadedSceneNames.Remove(sceneToUnload.name);
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }

        #endregion
    }
}