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
    }
}