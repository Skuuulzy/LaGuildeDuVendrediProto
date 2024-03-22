using QFSW.QC;
using UnityEngine;
using VComponent.Island;
using VComponent.Items.Merchandise;
using VComponent.Multiplayer.Deliveries;

namespace VComponent.Ship
{
    public class MultiplayerShipController : MonoBehaviour
    {
        [SerializeField] private MerchandiseType _currentMerchandiseCarriedType;
        [SerializeField] private ushort _currentMerchandiseCarriedNumber;

        private MultiplayerFactionIslandController _factionDockedIsland;
        private RessourcesIslandController _ressourcesDockedIsland;
        private Delivery _currentDelivery;
        private ushort _merchandiseAmountSellable;

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
            // The request il already done or we do not have the correct merchandise type.
            if (_currentDelivery.IsDone || _currentMerchandiseCarriedType != _currentDelivery.Data.Merchandise)
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
        public void SellMerchandiseToDockedIsland()
        {
            if (_factionDockedIsland == null)
            {
                Debug.LogError("No docked island to sell merchandise to !");
                return;
            }

            if (_merchandiseAmountSellable <= 0)
            {
                Debug.LogError($"The current merchandise: {_currentMerchandiseCarriedType} cannot be sell to the island: {_factionDockedIsland.IslandData.IslandName} !");
                return;
            }

            // Determine how many merchandise we can sell
            ushort merchandiseSellAmount = GetSellableAmount();

            _factionDockedIsland.UpdateDelivery(merchandiseSellAmount);

            _currentMerchandiseCarriedNumber -= merchandiseSellAmount;
            _currentMerchandiseCarriedType = MerchandiseType.NONE;

            _merchandiseAmountSellable -= merchandiseSellAmount;
        }

        private ushort GetSellableAmount()
        {
            // The ship do not have enough or just enough merchandise in stock to complete the order just sell everything.
            if (_merchandiseAmountSellable >= _currentMerchandiseCarriedNumber)
            {
                return _currentMerchandiseCarriedNumber;
            }

            // The ship have too many resources in stock just sell the maximum asked by the delivery.
            return _merchandiseAmountSellable;
        }

        #endregion
    }
}