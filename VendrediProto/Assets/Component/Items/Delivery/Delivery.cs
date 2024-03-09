using Unity.Netcode;

namespace VComponent.Items.Merchandise
{
    public struct Delivery : INetworkSerializable
    {
        public MerchandiseType Merchandise;
        public int MerchandiseDesiredAmount;
        public int MerchandiseCurrentAmount;
        public int IslandIndex;
        public int TimeAvailable;

        public Delivery(MerchandiseType merchandise, int merchandiseDesiredAmount,int merchandiseCurrentAmount, int islandIndex, int timeAvailable)
        {
            Merchandise = merchandise;
            MerchandiseDesiredAmount = merchandiseDesiredAmount;
            MerchandiseCurrentAmount = merchandiseCurrentAmount;
            IslandIndex = islandIndex;
            TimeAvailable = timeAvailable;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Merchandise);
            serializer.SerializeValue(ref MerchandiseDesiredAmount);
            serializer.SerializeValue(ref MerchandiseCurrentAmount);
            serializer.SerializeValue(ref IslandIndex);
            serializer.SerializeValue(ref TimeAvailable);
        }
    }
}