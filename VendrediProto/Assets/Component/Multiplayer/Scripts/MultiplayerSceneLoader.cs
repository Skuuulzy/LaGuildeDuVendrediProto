﻿using Eflatun.SceneReference;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Component.Multiplayer
{
    public static class MultiplayerSceneLoader
    {
        public static void LoadNetwork(SceneReference scene)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(scene.Name, LoadSceneMode.Single);
        }
    }
}