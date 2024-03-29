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

namespace VComponent.Ship
{
    public class MultiplayerShipController : MonoBehaviour
    {
        [Header("Data")]
        [ShowInInspector] private Dictionary<ResourceType, ushort> _currentResourcesCarried = new ();
        
        [Header("Parameters")]
        [SerializeField] private ushort _capacity;
        [SerializeField] private int _maxResourcesTypeCarried = 2;

        private MultiplayerFactionIslandController _factionDockedIsland;
        private ResourcesIslandController _resourcesDockedIsland;
        private Delivery _currentDelivery;
        private ushort _merchandiseAmountSellable;

        public event Action<ResourcesSO, int> OnResourceAdded = delegate {};
        public event Action<ResourceType, int> OnResourceCarriedUpdated;
        public event Action<ResourceType> OnResourceCarriedDelivered;
        public event Action<bool,ResourcesIslandSO> OnResourceIslandDocked;
        public event Action<ShipState, string> OnShipStateUpdated;

        private CancellationTokenSource _cancellationTokenSource;

        private void OnTriggerEnter(Collider other)
        {
            MultiplayerFactionIslandController factionIslandController = other.gameObject.GetComponent<MultiplayerFactionIslandController>();
            if (factionIslandController != null)
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

                return;
            }

            ResourcesIslandController resourcesIslandController = other.gameObject.GetComponent<ResourcesIslandController>();
            if (resourcesIslandController != null)
            {
                Debug.Log($"Entering island {resourcesIslandController.IslandData.IslandName}");
                _resourcesDockedIsland = resourcesIslandController;
                OnResourceIslandDocked?.Invoke(true,resourcesIslandController.IslandData);
                OnShipStateUpdated?.Invoke(ShipState.DOCKED, _resourcesDockedIsland.IslandData.IslandName);
                return;
            }

            PlayerIslandController playerIslandController = other.gameObject.GetComponent<PlayerIslandController>();
            if (playerIslandController != null)
            {
                //Region Player Island
            }
        }

        private void OnTriggerExit(Collider other)
        {
            MultiplayerFactionIslandController islandController = other.gameObject.GetComponent<MultiplayerFactionIslandController>();
            if (islandController != null)
            {
                Debug.Log($"Exiting island {islandController.IslandData.IslandName}");
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
                if(_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
				{
                    _cancellationTokenSource.Cancel();
				}
            }
            
            ResourcesIslandController resourcesIslandController = other.gameObject.GetComponent<ResourcesIslandController>();
            if (resourcesIslandController != null)
            {
                Debug.Log($"Exiting island {resourcesIslandController.IslandData.IslandName}");
                OnShipStateUpdated?.Invoke(ShipState.IN_SEA, "");
                OnResourceIslandDocked?.Invoke(false,null);

                //Cancel the loading if we exit the island
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                }
                return;
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
            await SellResource(resourceType, _cancellationTokenSource);
            
            OnResourceCarriedDelivered?.Invoke(resourceType);
        }

        public async UniTask SellResource(ResourceType resourceType, CancellationTokenSource cancellationTokenSource)
		{
            // Determine how many merchandise we can sell
            ushort merchandiseSellAmount = GetSellableAmount(resourceType);
            ushort resourceAmount =  _factionDockedIsland.IslandData.ResourceAmountLoadAndSell; 

            //Sell resource 50 by 50. Each step lasts 2 seconds 
            for (int i = 0; i < merchandiseSellAmount / resourceAmount; i++)
			{
                await UniTask.Delay((int) _factionDockedIsland.IslandData.ResourceTimeLoadAndSell * 1000, cancellationToken: cancellationTokenSource.Token);
                //Log not working when cancel is called but cancel work 
				if (cancellationTokenSource.IsCancellationRequested)
				{
                    Debug.Log("CancelSelling");
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
            await LoadResource(resourceSO, amount, _cancellationTokenSource);
            
        }

        public async UniTask LoadResource(ResourcesSO resourceSO, int amount, CancellationTokenSource cancellationTokenSource)
		{
            int resourceAmount = _resourcesDockedIsland.IslandData.ResourceAmountLoadAndSell;
            //Load resource 50 by 50. Each step lasts 2 seconds 
            for (int i = 0; i < amount / resourceAmount; i++)
            {
                await UniTask.Delay((int) _resourcesDockedIsland.IslandData.ResourceTimeLoadAndSell * 1000, cancellationToken: cancellationTokenSource.Token);
                //Log not working when cancel is called but cancel work 
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    Debug.Log("CancelLoading");
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
    }
}