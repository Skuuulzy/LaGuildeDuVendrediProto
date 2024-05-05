using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using VComponent.Ship;

public class PlayerIslandController : NetworkBehaviour
{
    public static Action<MultiplayerShipController> OnShipAddedToFleet;
    
    [Header("Ships")]
    [SerializeField] private MultiplayerShipController _standardShip;

    [Header("Component")] 
    [SerializeField] private Transform _dockTransform;

    private ulong _clientId;
    private List<MultiplayerShipController> _ships;
     
    /// <summary>
    /// SERVER - SIDE
    /// Change the ownership of the object to the correct client Id.
    /// </summary>
    public void SetUpForPlayer(ulong clientId)
    {
        GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }

    /// <summary>
    /// CLIENT - SIDE
    /// The server ask to the clients to spawn their first boat.
    /// </summary>
    [ClientRpc]
    public void SpawnFirstBoatClientRpc()
    {
        if (!IsOwner)
        {
            return;
        }

        _ships = new List<MultiplayerShipController>();
        _clientId = GetComponent<NetworkObject>().OwnerClientId;
        
        RequestSpawnShipServerRpc();
    }

    [ClientRpc]
    private void ConfirmBoatInstantiationClientRpc(bool instanced, NetworkObjectReference objectReference)
    {
        // Maybe later we want that all the clients know the the ships of the others
        if (!IsOwner)
        {
            return;
        }

        if (!instanced)
        {
            return;
        }

        if (objectReference.TryGet(out NetworkObject targetObject))
        {
            var ship = targetObject.GetComponent<MultiplayerShipController>();
            _ships.Add(ship);
            OnShipAddedToFleet?.Invoke(ship);
            Debug.Log($"Standard ship {ship.name} added to fleet of player {_clientId}");
        }
        else
        {
            Debug.LogError($"The instanced ship has not been found on the client {_clientId} instance !");
        }
    }
    
    /// <summary>
    /// SERVER - SIDE
    /// A client request the instantiation of a ship for this island.
    /// </summary>
    [ServerRpc]
    private void RequestSpawnShipServerRpc(ServerRpcParams rpcParams = default)
    {
        MultiplayerShipController standardShip = Instantiate(_standardShip, _dockTransform);
        standardShip.GetComponent<NetworkObject>().SpawnAsPlayerObject(rpcParams.Receive.SenderClientId, true);

        NetworkObjectReference reference = new NetworkObjectReference(standardShip.GetComponent<NetworkObject>());

        ConfirmBoatInstantiationClientRpc(true, reference);
    }
}