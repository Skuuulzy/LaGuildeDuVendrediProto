using System.Collections.Generic;
using TMPro;
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
        [SerializeField] private TMP_Text _playerNameTxt;
        
        [SerializeField] private List<Component> _ownerComponents;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            SetPlayerName();
            
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
                    Debug.Log($"Delivery Requested type: {delivery.Ressource}");
                };
                
                MultiplayerFactionIslandController.OnDeliveryUpdated += (delivery) =>
                {
                    Debug.Log($"Delivery Updated type: {delivery.Ressource}, current amount: {delivery.MerchandiseCurrentAmount}");
                };

            }
        }

        private void SetPlayerName()
        {
            if (IsOwner)
            {
                _playerNameTxt.text = MultiplayerConnectionManager.Instance.GetPlayerName();
            }
            
            // Find the player name with is ID
            var ownerID = GetComponent<NetworkObject>().OwnerClientId;
            Debug.Log($"Owner ID: {ownerID}");
        }
    }
}