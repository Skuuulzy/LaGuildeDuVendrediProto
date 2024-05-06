using QFSW.QC;
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VComponent.Island;
using VComponent.Items.Merchandise;
using VComponent.Multiplayer.Deliveries;
using Cysharp.Threading.Tasks;
using System.Threading;
using VComponent.Multiplayer;

namespace VComponent.Ship
{
    public class MultiplayerShipController : MonoBehaviour, IDamageable
	{
        [Header("Data")]
        [ShowInInspector] private Dictionary<ResourceType, ushort> _currentResourcesCarried = new ();
        
        [Header("Parameters")]
        [SerializeField] private ushort _capacity;
        [SerializeField] private int _maxResourcesTypeCarried = 2;

        [Header("Ship Usefull Values")]
		[SerializeField] private ushort _lifeValue = 100;

		private MultiplayerFactionIslandController _factionDockedIsland;
        private ResourcesIslandController _resourcesDockedIsland;
        private Delivery _currentDelivery;
        private ushort _merchandiseAmountSellable;
		private CancellationTokenSource _cancellationTokenSource;

		public event Action<ResourcesSO, int> OnResourceAdded = delegate {};
        public event Action<ResourceType, int> OnResourceCarriedUpdated;
        public event Action<ResourceType> OnResourceCarriedDelivered;
        public event Action<bool,ResourcesIslandSO> OnResourceIslandDocked;
        public event Action<ShipState, string> OnShipStateUpdated;
        public event Action<MultiplayerShipMilitaryController> OnShipEncountered;
        
		public ushort LifeValue => _lifeValue;

        private void OnTriggerEnter(Collider other)
        {
            // FACTION ISLAND
            if (other.TryGetComponent(out MultiplayerFactionIslandController factionIslandController))
            {
                Debug.Log($"Entering island {factionIslandController.IslandData.IslandName}");
                _factionDockedIsland = factionIslandController;

                _currentDelivery = DeliveryManager.Instance.GetRequestedDeliveryBy(factionIslandController);
                OnShipStateUpdated?.Invoke(ShipState.DOCKED, _factionDockedIsland.IslandData.IslandName);
                if (_currentDelivery != null)
                {
                    UpdateSellableState();
                    _currentDelivery.OnDataUpdated += UpdateSellableState;
                }
                else
                {
                    Debug.Log("The current island don't request any delivery.");
                    DeliveryManager.OnDeliveryCreated += HandleDeliveryCreation;
                }
            }
            // RESOURCE ISLAND
            if (other.TryGetComponent(out ResourcesIslandController resourcesIslandController))
            {
                Debug.Log($"Entering island {resourcesIslandController.IslandData.IslandName}");
                _resourcesDockedIsland = resourcesIslandController;
                OnResourceIslandDocked?.Invoke(true, resourcesIslandController.IslandData);
                OnShipStateUpdated?.Invoke(ShipState.DOCKED, _resourcesDockedIsland.IslandData.IslandName);
            }
            // PLAYER ISLAND
            if (other.TryGetComponent(out PlayerIslandController playerIslandController))
            {
                
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // FACTION ISLAND
            if (other.TryGetComponent(out MultiplayerFactionIslandController factionIslandController))
            {
                // FACTION ISLAND
                Debug.Log($"Exiting island {factionIslandController.IslandData.IslandName}");
                OnShipStateUpdated?.Invoke(ShipState.IN_SEA, "");

                _factionDockedIsland = null;

                // Remove all data from the delivery
                if (_currentDelivery != null)
                {
                    _currentDelivery.OnDataUpdated -= UpdateSellableState;
                    _currentDelivery.RemoveSeller(this);
                    _currentDelivery = null;
                }

                _merchandiseAmountSellable = 0;

                DeliveryManager.OnDeliveryCreated -= HandleDeliveryCreation;

                //Cancel the selling if we exit the island
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;
                }
            }
            // RESOURCE ISLAND
            if (other.TryGetComponent(out ResourcesIslandController resourcesIslandController))
            {
                Debug.Log($"Exiting island {resourcesIslandController.IslandData.IslandName}");
                OnShipStateUpdated?.Invoke(ShipState.IN_SEA, "");
                OnResourceIslandDocked?.Invoke(false, null);

                //Cancel the loading if we exit the island
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource = null;
                }
            }

        }
        
        
        #region SELL

        private void HandleDeliveryCreation(Delivery delivery)
        {
            // This delivery is not on the docked island
            if (delivery.Buyer != _factionDockedIsland)
            {
                return;
            }

            // The island has created a delivery, we can update the data to try to sell our merchandise.
            _currentDelivery = delivery;
            UpdateSellableState();
            _currentDelivery.OnDataUpdated += UpdateSellableState;
        }

        private void UpdateSellableState()
        {
            // The request is already done or we do not have the correct merchandise type.
            if (_currentDelivery.IsDone || _currentResourcesCarried.ContainsKey(_currentDelivery.Data.Resource) == false)
            {
                _merchandiseAmountSellable = 0;
                _currentDelivery.RemoveSeller(this);
                return;
            }

            // Inform the delivery that we can sell this merchandise.
            _currentDelivery.AddNewSeller(this);
            _merchandiseAmountSellable = _currentDelivery.NeededAmount();
        }

        [Command]
        public async void SellResourceToDockedIsland(ResourceType resourceType)
        {
            if (_factionDockedIsland == null)
            {
                Debug.LogError("No docked island to sell merchandise to !");
                return;
            }

            if (_merchandiseAmountSellable <= 0)
            {
                Debug.LogError($"The current merchandise: {resourceType} cannot be sell to the island: {_factionDockedIsland.IslandData.IslandName} !");
                return;
            }
            OnShipStateUpdated?.Invoke(ShipState.SELL_RESOURCES, "");

            //Create a cancel Token to cancel LoadResource if we exit the island area 
            _cancellationTokenSource = new CancellationTokenSource();
            
            //Sell the resource of the boat to the island during time
            await AsynchronouslySellResource(resourceType, _cancellationTokenSource.Token);
            
            OnResourceCarriedDelivered?.Invoke(resourceType);
        }

        private async UniTask AsynchronouslySellResource(ResourceType resourceType, CancellationToken cancellationTokenSource)
		{
            // Determine how many merchandise we can sell
            ushort merchandiseSellAmount = GetSellableAmount(resourceType);
            ushort resourceAmount =  _factionDockedIsland.IslandData.ResourceAmountLoadAndSell; 

            //Sell resource 50 by 50. Each step lasts 2 seconds 
            for (int i = 0; i < merchandiseSellAmount / resourceAmount; i++)
			{
                await UniTask.Delay((int) _factionDockedIsland.IslandData.ResourceTimeLoadAndSell * 1000);
                
                // Stop the selling if a cancellation is requested  
				if (cancellationTokenSource.IsCancellationRequested || _currentDelivery.IsDone)
				{
                    Debug.Log($"Selling resources on {name} stopped.");
                    return;
				}

                //Update the livery on the island 
                _factionDockedIsland.UpdateDelivery(resourceAmount);
                
                //Update the resource amount on the ship
                _currentResourcesCarried[resourceType] -= resourceAmount;
                _merchandiseAmountSellable -= resourceAmount;
                
                OnResourceCarriedUpdated?.Invoke(resourceType, _currentResourcesCarried[resourceType]);
            }

            //Remove the resource from the ship
            _currentResourcesCarried.Remove(resourceType);
            Debug.Log($"Loading resources on {name} done.");
        }

        private ushort GetSellableAmount(ResourceType resourceType)
        {
            // The ship do not have enough or just enough merchandise in stock to complete the order just sell everything.
            if (_merchandiseAmountSellable >= _currentResourcesCarried[resourceType])
            {
                return _currentResourcesCarried[resourceType];
            }

            // The ship have too many resources in stock just sell the maximum asked by the delivery.
            return _merchandiseAmountSellable;
        }

        #endregion

        #region LOAD

        public async void LoadResourceToShip(ResourcesSO resourceSO, int amount)
        {
            //We check if we have already the maximum different resources 
            if (_currentResourcesCarried.Count >= _maxResourcesTypeCarried)
            {
                Debug.LogError("Cant add this resource => already carrying " + _maxResourcesTypeCarried);
                return;
            }
            OnShipStateUpdated?.Invoke(ShipState.LOAD_RESOURCES, "");

            //Create a cancel Token to cancel LoadResource if we exit the island area 
            _cancellationTokenSource = new CancellationTokenSource();
            
            //Load the resource of the island on the boat during time
            await AsynchronouslyLoadResource(resourceSO, amount, _cancellationTokenSource.Token);
        }

        private async UniTask AsynchronouslyLoadResource(ResourcesSO resourceSO, int amount, CancellationToken cancellationTokenSource)
		{
            int resourceAmount = _resourcesDockedIsland.IslandData.ResourceAmountLoadAndSell;
            
            //Load resource 50 by 50. Each step lasts 2 seconds 
            for (int i = 0; i < amount / resourceAmount; i++)
            {
                await UniTask.Delay((int)_resourcesDockedIsland.IslandData.ResourceTimeLoadAndSell * 1000);
                
                // Stop the loading if a cancellation is requested 
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    Debug.Log($"Loading resources on {name} stopped.");
                    return;
                }

                //Check if we already contains the resource
                if (_currentResourcesCarried.ContainsKey(resourceSO.Type))
                {
                    _currentResourcesCarried[resourceSO.Type] += Convert.ToUInt16(resourceAmount);
                    OnResourceCarriedUpdated?.Invoke(resourceSO.Type, _currentResourcesCarried[resourceSO.Type]);
                }
                else
                {
                    //Adding the new resource to the dictionary
                    _currentResourcesCarried.Add(resourceSO.Type, Convert.ToUInt16(resourceAmount));
                    OnResourceAdded?.Invoke(resourceSO, resourceAmount);
                }
            }
            
            //Setting the state of the ship to docked to the island
            OnShipStateUpdated?.Invoke(ShipState.DOCKED, _resourcesDockedIsland.IslandData.IslandName);
            Debug.Log($"Loading resources on {name} done.");
        }

        public int GetFreeSpace()
        {
            int currentSpace = 0;
            foreach (var kvp in _currentResourcesCarried)
            {
                currentSpace += kvp.Value;
            }

            return _capacity - currentSpace;
        }

		#endregion LOAD

		#region LIFE 

		/// <summary>
		/// Taking dammage from a opponent weapon
		/// </summary>
		public void TakeDamage(int damage)
		{
			_lifeValue -= (ushort)damage;
            Debug.Log("Life = " + _lifeValue);
			if (_lifeValue <= 0)
			{
				OnDeath();
			}
		}

		public void OnDeath()
		{
            Debug.Log("This boat is dead dead " + this);
		}
		#endregion LIFE
	}
}