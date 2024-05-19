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
        public MultiplayerFactionIslandController Buyer;
        public ResourcesShipController Seller;
        
        public bool IsExpired { get; private set; }
        public bool IsDone => Data.MerchandiseDesiredAmount == Data.MerchandiseCurrentAmount;

        // TODO : It would be nice to make that only the delivery itself can raise these events.
        public Action OnDataUpdated;
        public Action OnClockUpdated;
        public Action OnExpired;

        public bool HasSeller;
        
        public Delivery(DeliveryNetworkPackage data, MultiplayerFactionIslandController buyer)
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
            OnDataUpdated?.Invoke();
        }

        public void AddNewSeller(ResourcesShipController shipController)
        {
            if (HasSeller)
            {
                return;
            }

            HasSeller = true;
            Seller = shipController;
            OnDataUpdated?.Invoke();
        }

        public void RemoveSeller(ResourcesShipController shipController)
        {
            if (!HasSeller)
            {
                return;
            }

            HasSeller = false;
            Seller = null;
            OnDataUpdated?.Invoke();
        }
        
        public void SetHasExpired()
        {
            IsExpired = true;
            OnExpired?.Invoke();
        }

        #endregion
    }
}