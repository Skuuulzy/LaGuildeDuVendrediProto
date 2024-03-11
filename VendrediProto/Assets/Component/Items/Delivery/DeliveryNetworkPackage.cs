using Unity.Netcode;

namespace VComponent.Items.Merchandise
{
    public struct DeliveryNetworkPackage : INetworkSerializable
    {
        public ushort ID;
        public MerchandiseType Merchandise;
        public ushort MerchandiseDesiredAmount;
        public ushort MerchandiseCurrentAmount;
        public byte IslandIndex;
        public uint TimeAvailable;

        public DeliveryNetworkPackage(ushort id, MerchandiseType merchandise, ushort merchandiseDesiredAmount, ushort merchandiseCurrentAmount, byte islandIndex, uint timeAvailable)
        {
            ID = id;
            Merchandise = merchandise;
            MerchandiseDesiredAmount = merchandiseDesiredAmount;
            MerchandiseCurrentAmount = merchandiseCurrentAmount;
            IslandIndex = islandIndex;
            TimeAvailable = timeAvailable;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ID);
            serializer.SerializeValue(ref Merchandise);
            serializer.SerializeValue(ref MerchandiseDesiredAmount);
            serializer.SerializeValue(ref MerchandiseCurrentAmount);
            serializer.SerializeValue(ref IslandIndex);
            serializer.SerializeValue(ref TimeAvailable);
        }
    }
}