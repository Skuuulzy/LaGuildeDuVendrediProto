using Mono.Cecil;
using QFSW.QC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VComponent.Island;
using VComponent.Items.Merchandise;
using VComponent.Multiplayer.Deliveries;

namespace VComponent.Ship
{
    public class MultiplayerShipController : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<RessourceType, ushort> _currentRessourcesCarried;
		//[SerializeField] private RessourceType _currentRessourcesCarriedType;
  //      [SerializeField] private ushort _currentRessourceCarriedNumber;
        [SerializeField] private ushort _maxFreeSpace;

		

		private MultiplayerFactionIslandController _factionDockedIsland;
        private RessourcesIslandController _ressourcesDockedIsland;
        private Delivery _currentDelivery;
        private ushort _merchandiseAmountSellable;
        private int _maxDiffRessourcesTypeCarried = 2;

        private void OnTriggerEnter(Collider other)
        {
            MultiplayerFactionIslandController factionIslandController = other.gameObject.GetComponent<MultiplayerFactionIslandController>();
            if (factionIslandController != null)
            {
                Debug.Log($"Entering island {factionIslandController.IslandData.IslandName}");
                _factionDockedIsland = factionIslandController;
                
                _currentDelivery = DeliveryManager.Instance.GetRequestedDeliveryBy(factionIslandController);
                
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

			RessourcesIslandController ressourcesIslandController = other.gameObject.GetComponent<RessourcesIslandController>();
            if(ressourcesIslandController != null)
            {
				//Region RessourcesIsland
				Debug.Log($"Entering island {ressourcesIslandController.IslandData.IslandName}");
				_ressourcesDockedIsland = ressourcesIslandController;
				return;
            }

			PlayerIslandController playerIslandController = other.gameObject.GetComponent<PlayerIslandController>();
            if(playerIslandController != null)
            {
				//Region Player Island

				return;
            }

		}

		private void OnTriggerExit(Collider other)
        {
            MultiplayerFactionIslandController islandController = other.gameObject.GetComponent<MultiplayerFactionIslandController>();
            if (islandController != null)
            {
                Debug.Log($"Exiting island {islandController.IslandData.IslandName}");
                
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
            if (_currentDelivery.IsDone || _currentRessourcesCarried.ContainsKey(_currentDelivery.Data.Ressource) == false)
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
        public void SellMerchandiseToDockedIsland(RessourceType ressourceType)
        {
            if (_factionDockedIsland == null)
            {
                Debug.LogError("No docked island to sell merchandise to !");
                return;
            }

            if (_merchandiseAmountSellable <= 0)
            {
                Debug.LogError($"The current merchandise: {ressourceType} cannot be sell to the island: {_factionDockedIsland.IslandData.IslandName} !");
                return;
            }

            // Determine how many merchandise we can sell
            ushort merchandiseSellAmount = GetSellableAmount(ressourceType);

            _factionDockedIsland.UpdateDelivery(merchandiseSellAmount);

            _currentRessourcesCarried.ToDictionary()[ressourceType] -= merchandiseSellAmount;
            _currentRessourcesCarried.ToDictionary().Remove(ressourceType);

            _merchandiseAmountSellable -= merchandiseSellAmount;
        }

        private ushort GetSellableAmount(RessourceType ressourceType)
        {
            // The ship do not have enough or just enough merchandise in stock to complete the order just sell everything.
            if (_merchandiseAmountSellable >= _currentRessourcesCarried.ToDictionary()[ressourceType])
            {
                return _currentRessourcesCarried.ToDictionary()[ressourceType];
            }

            // The ship have too many resources in stock just sell the maximum asked by the delivery.
            return _merchandiseAmountSellable;
        }

		#endregion

		#region LOAD

        public void LoadRessource(RessourceType ressourceType, int amount)
        {
            Dictionary<RessourceType, ushort> currentRessources = _currentRessourcesCarried.ToDictionary();

            //We check if we have already the maximum differents ressources 
            if(currentRessources.Count >= _maxDiffRessourcesTypeCarried)
            {
                Debug.LogError("Cant add this ressource => already carriyng " + _maxDiffRessourcesTypeCarried);
                return;
            }

            //Check if we already contains the ressource
            if(currentRessources.ContainsKey(ressourceType))
            {
				_currentRessourcesCarried.ToDictionary()[ressourceType] += Convert.ToUInt16(amount);
            }
            else
            {
                //Adding the new ressource to the dictionary
                _currentRessourcesCarried.ToDictionary().Add(ressourceType, Convert.ToUInt16(amount));
            }
        }
		#endregion LOAD


		public int GetFreeSpace()
		{
            int currentSpace = 0;
            foreach(var kvp in _currentRessourcesCarried.ToDictionary())
            {
                currentSpace += kvp.Value;
            }
            return _maxFreeSpace - currentSpace; 
        }
	}
}