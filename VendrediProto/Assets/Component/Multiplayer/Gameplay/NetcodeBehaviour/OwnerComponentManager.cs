using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using VComponent.Island;

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
            base.OnNetworkSpawn();
            
            if (!IsOwner)
            {
                foreach (var component in _ownerComponents)
                {
                    Destroy(component);
                }

                _ownerComponents = null;
            }

            if (IsOwner)
            {
                MultiplayerFactionIslandController.OnDeliveryRequested += (delivery) =>
                {
                    Debug.Log($"Delivery Requested type: {delivery.Merchandise}");
                };
                
                MultiplayerFactionIslandController.OnDeliveryUpdated += (delivery) =>
                {
                    Debug.Log($"Delivery Updated type: {delivery.Merchandise}, current amount: {delivery.MerchandiseCurrentAmount}");
                };
            }
        }
    }
}