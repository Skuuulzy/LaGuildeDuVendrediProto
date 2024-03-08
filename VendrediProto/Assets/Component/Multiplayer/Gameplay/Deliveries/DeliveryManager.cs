using Unity.Netcode;
using VComponent.Tools.Singletons;

namespace VComponent.Multiplayer.Deliveries
{
    
    
    /// <summary>
    /// Handle the creation of delivery for all the player in the game.
    /// </summary>
    public class DeliveryManager : NetworkSingleton<DeliveryManager>
    {
        private void Update()
        {
            if (!IsServer)
            {
                return;
            }
            
            
        }

        /// <summary>
        /// Will spawn a delivery on all connected clients.
        /// <remarks>Be careful with late joining clients here.</remarks>
        /// </summary>
        [ClientRpc]
        private void SpawnNewDeliveryClientRpc()
        {
            
        }
    }
}