using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace VComponent.Multiplayer
{
    public class OwnerComponentActivator : NetworkBehaviour
    {
        [SerializeField] private List<MonoBehaviour> _componentsToDeactivate;
        
        private void Awake()
        {
            if (!IsOwner)
            {
                foreach (var component in _componentsToDeactivate)
                {
                    component.enabled = false;
                }
            }
        }
    }
}