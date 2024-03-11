using VComponent.Island;

namespace VComponent.Items.Merchandise
{
    public class Delivery
    {
        public DeliveryNetworkPackage Data;
        public MultiplayerIslandController Buyer;

        public Delivery(DeliveryNetworkPackage data, MultiplayerIslandController buyer)
        {
            Data = data;
            Buyer = buyer;
        }
        
        public bool IsDone()
        {
            return Data.MerchandiseDesiredAmount == Data.MerchandiseCurrentAmount;
        }

        /// <summary>
        /// Return how many merchandise amount is needed to complete the delivery.
        /// </summary>
        public ushort NeededAmount()
        {
            if (IsDone())
            {
                return 0;
            }

            return (ushort)(Data.MerchandiseDesiredAmount - Data.MerchandiseCurrentAmount);
        }
    }
}