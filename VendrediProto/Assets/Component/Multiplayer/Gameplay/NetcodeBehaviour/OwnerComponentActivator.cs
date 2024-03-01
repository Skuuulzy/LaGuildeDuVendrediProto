using System;
using System.Collections.Generic;
using SebastianLague;
using Unity.Netcode;
using UnityEngine;

namespace VComponent.Multiplayer
{
    public class OwnerComponentActivator : NetworkBehaviour
    {
        [SerializeField] private List<MonoBehaviour> _ownerComponents;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsOwner)
            {
                foreach (var component in _ownerComponents)
                {
                    component.enabled = true;
                }
            }
        }
    }
}