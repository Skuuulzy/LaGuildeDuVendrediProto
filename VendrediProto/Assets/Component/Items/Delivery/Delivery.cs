using System;
using VComponent.Island;
using VComponent.Ship;

namespace VComponent.Items.Merchandise
{
    /// <summary>
    /// Contains all necessary data for a delivery.
    /// </summary>
    public class Delivery
    {
        public DeliveryNetworkPackage Data;
        public MultiplayerIslandController Buyer;
        public MultiplayerShipController Seller;
        
        public bool IsExpired { get; private set; }
        public bool IsDone => Data.MerchandiseDesiredAmount == Data.MerchandiseCurrentAmount;

        // TODO : It would be nice to make that only the delivery itself can raise these events.
        public Action OnUpdated;
        public Action OnExpired;
        
        public Delivery(DeliveryNetworkPackage data, MultiplayerIslandController buyer)
        {
            Data = data;
            Buyer = buyer;
        }

        /// <summary>
        /// Return how many merchandise amount is needed to complete the delivery.
        /// </summary>
        public ushort NeededAmount()
        {
            if (IsDone)
            {
                return 0;
            }

            return (ushort)(Data.MerchandiseDesiredAmount - Data.MerchandiseCurrentAmount);
        }

        #region DATA UPDATE METHODS

        public void UpdateNetworkData(DeliveryNetworkPackage newData)
        {
            Data = newData;
            OnUpdated?.Invoke();
        }

        public void AddNewSeller(MultiplayerShipController shipController)
        {
            if (Seller == shipController)
            {
                return;
            }
            
            Seller = shipController;
            OnUpdated?.Invoke();
        }

        public void RemoveSeller(MultiplayerShipController shipController)
        {
            Seller = null;
            OnUpdated?.Invoke();
        }
        
        public void SetHasExpired()
        {
            IsExpired = true;
            OnExpired?.Invoke();
        }

        #endregion
    }
}