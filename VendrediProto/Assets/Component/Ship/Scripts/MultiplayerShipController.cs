using QFSW.QC;
using UnityEngine;
using UnityEngine.Tilemaps;
using VComponent.Island;
using VComponent.Items.Merchandise;
using VComponent.Multiplayer.Deliveries;

namespace VComponent.Ship
{
    public class MultiplayerShipController : MonoBehaviour
    {
        [SerializeField] private MerchandiseType _currentMerchandiseCarriedType;
        [SerializeField] private ushort _currentMerchandiseCarriedNumber;

        private MultiplayerIslandController _dockedIsland;
        private Delivery _currentDelivery;
        private ushort _merchandiseAmountSellable;

        private void OnTriggerEnter(Collider other)
        {
            MultiplayerIslandController islandController = other.gameObject.GetComponent<MultiplayerIslandController>();
            if (islandController != null)
            {
                Debug.Log($"Entering island {islandController.IslandData.IslandName}");
                _dockedIsland = islandController;
                
                _currentDelivery = DeliveryManager.Instance.GetRequestedDeliveryBy(islandController);
                
                if (_currentDelivery != null)
                {
                    UpdateSellableState(_currentDelivery);
                    DeliveryManager.OnDeliveryUpdated += UpdateSellableState;
                }
                else
                {
                    Debug.Log("The current island don't request any delivery.");
                    DeliveryManager.OnDeliveryCreated += HandleDeliveryCreation;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            MultiplayerIslandController islandController = other.gameObject.GetComponent<MultiplayerIslandController>();
            if (islandController != null)
            {
                Debug.Log($"Exiting island {islandController.IslandData.IslandName}");
                
                _dockedIsland = null;
                _currentDelivery = null;
                _merchandiseAmountSellable = 0;
                DeliveryManager.OnDeliveryUpdated -= UpdateSellableState;
                DeliveryManager.OnDeliveryCreated -= HandleDeliveryCreation;
            }
        }

        #region SELL
        
        private void HandleDeliveryCreation(Delivery delivery)
        {
            // This delivery is not on the docked island
            if (delivery.Buyer != _dockedIsland)
            {
                return;
            }
            
            // The island has created a delivery, we can update the data to try to sell our merchandise.
            _currentDelivery = delivery;
            UpdateSellableState(_currentDelivery);
            DeliveryManager.OnDeliveryUpdated += UpdateSellableState;
        }

        private void UpdateSellableState(Delivery delivery)
        {
            // We do not want to update delivery that we are not tracking for this island.
            if (_currentDelivery != delivery)
            {
                return;
            }
            
            // The request il already done or we do not have the correct merchandise type.
            if (delivery.IsDone() || _currentMerchandiseCarriedType != delivery.Data.Merchandise)
            {
                _merchandiseAmountSellable = 0;
                return;
            }

            // Inform the UI that we can sell this merchandise. Maybe pass this instance to then sell bind the button listener to the sell goods method.
            _merchandiseAmountSellable = delivery.NeededAmount();
        }
        
        [Command]
        public void SellMerchandiseToDockedIsland()
        {
            if (_dockedIsland == null)
            {
                Debug.LogError("No docked island to sell merchandise to !");
                return;
            }

            if (_merchandiseAmountSellable > 0)
            {
                Debug.LogError($"The current merchandise: {_currentMerchandiseCarriedType} cannot be sell to the island: {_dockedIsland.IslandData.IslandName} !");
                return;
            }

            // Determine how many merchandise we can sell
            ushort merchandiseSellAmount = GetSellableAmount();

            _dockedIsland.UpdateDelivery(merchandiseSellAmount);

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