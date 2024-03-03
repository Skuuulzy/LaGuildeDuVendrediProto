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
        [SerializeField] private List<MonoBehaviour> _ownerComponents;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsOwner)
            {
                foreach (var component in _ownerComponents)
                {
                    component.enabled = false;
                }
            }
        }
    }
}