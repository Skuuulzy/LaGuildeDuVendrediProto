using System;
using SebastianLague;
using Unity.Netcode;
using UnityEngine;

namespace VComponent.Ship
{
    /// <summary>
    /// This class control all ship sub controller.
    /// </summary>
    public class PlayerShipController : NetworkBehaviour, IDamageable
    {
        [Header("Components")]
        [SerializeField] private ShipUIController _uiController;
        [SerializeField] private ShipMilitaryController _militaryController;
        [SerializeField] private ResourcesShipController _resourcesController;
        [SerializeField] private Unit _movementController;
        [SerializeField] private NetworkObject _networkObject;
        
        public static Action<Transform> OnOwnerBoatSpawned;

        public ShipUIController UI => _uiController;
        public ShipMilitaryController Military => _militaryController;
        public ResourcesShipController Resources => _resourcesController;

        public override void OnNetworkSpawn()
        {
            _uiController.SetPlayerName(_networkObject.OwnerClientId);
            
            if (IsOwner)
            {
                // Initializing controllers                
                _movementController.Initialize(transform);
                _militaryController.Initialize();
                _resourcesController.Initialize();
                
                OnOwnerBoatSpawned?.Invoke(transform);
            }
            else
            {
                
            }
        }

        #region DAMAGE

        public void TakeDamage(int damage)
        {
            _militaryController.TakeDamage(damage);
            if (_militaryController.LifeValue <= 0)
            {
                OnDeath();
            }
        }

        public void OnDeath()
        {
            Debug.Log("The ship is dead.");
        }

        #endregion DAMAGE
    }
}