using FischlWorks_FogWar;
using UnityEngine;
using VComponent.Multiplayer;


public class FogOfWarRevealearsBinder : MonoBehaviour
{
    [SerializeField] private csFogWar _fogWar;
    [SerializeField] private int _boatSightRange = 200;

    private void Awake()
    {
        OwnerComponentManager.OnOwnerBoatSpawned += HandlePlayerBoatSpawned;
    }

    private void HandlePlayerBoatSpawned(Transform boatTransform)
    {
        _fogWar.AddFogRevealer(new csFogWar.FogRevealer(boatTransform, _boatSightRange, false));
    }
}