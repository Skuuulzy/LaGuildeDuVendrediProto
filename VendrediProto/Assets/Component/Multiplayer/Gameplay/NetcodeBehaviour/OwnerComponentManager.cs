using System;
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


        public static Action<Transform> OnOwnerBoatSpawned;
        
        public TMP_Text PlayerNameTxt => _playerNameTxt;
        
        public override void OnNetworkSpawn()
        {
            SetPlayerName();
            
            if (!IsOwner)
            {
                foreach (var component in _ownerComponents)
                {
                    Destroy(component);
                }

                _ownerComponents = null;
            }
            else
            {
                OnOwnerBoatSpawned?.Invoke(transform);
            }
        }

        private void SetPlayerName()
        {
            _playerNameTxt.text = MultiplayerGameplayManager.Instance.GetPlayerNameFromId(GetComponent<NetworkObject>().OwnerClientId);
        }
    }
}