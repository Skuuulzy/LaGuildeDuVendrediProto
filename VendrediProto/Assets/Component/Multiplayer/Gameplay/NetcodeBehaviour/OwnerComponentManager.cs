using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace VComponent.Multiplayer
{
    /// <summary>
    /// This class destroy owner components when the network spawn on all instances where the client is not the owner.
    /// </summary>
    public class OwnerComponentManager : NetworkBehaviour
    {
        [SerializeField] private List<Component> _ownerComponents;
        
        public override void OnNetworkSpawn()
        {
            Debug.Log($"IsHost: {IsHost}, IsServer: {IsServer}, IsClient: {IsClient}");
            
            base.OnNetworkSpawn();
            
            if (!IsOwner)
            {
                foreach (var component in _ownerComponents)
                {
                    Destroy(component);
                }

                _ownerComponents = null;
            }
        }
    }
}