using Unity.Netcode;

namespace VComponent.Items.Merchandise
{
    public struct DeliveryNetworkPackage : INetworkSerializable
    {
        public ushort ID;
        public ResourceType Resource;
        public ushort MerchandiseDesiredAmount;
        public ushort MerchandiseCurrentAmount;
        public byte IslandIndex;
        public uint TimeAvailable;

        public DeliveryNetworkPackage(ushort id, ResourceType merchandise, ushort merchandiseDesiredAmount, ushort merchandiseCurrentAmount, byte islandIndex, uint timeAvailable)
        {
            ID = id;
            Resource = merchandise;
            MerchandiseDesiredAmount = merchandiseDesiredAmount;
            MerchandiseCurrentAmount = merchandiseCurrentAmount;
            IslandIndex = islandIndex;
            TimeAvailable = timeAvailable;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ID);
            serializer.SerializeValue(ref Resource);
            serializer.SerializeValue(ref MerchandiseDesiredAmount);
            serializer.SerializeValue(ref MerchandiseCurrentAmount);
            serializer.SerializeValue(ref IslandIndex);
            serializer.SerializeValue(ref TimeAvailable);
        }
    }
}