using System;
using Cysharp.Threading.Tasks;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;
using VComponent.Items.Merchandise;
using VComponent.Multiplayer;
using VComponent.Tools.IDGenerators;
using VComponent.Tools.Timer;

namespace VComponent.Island
{
    /// <summary>
    /// Responsible for creating deliveries, and managing island data.
    /// </summary>
    public class MultiplayerFactionIslandController : NetworkBehaviour
    {
        [SerializeField] private byte _index;
        [SerializeField] private FactionIslandSO _islandData;
        
        private CountdownTimer _deliveryRequestTimer;
        
        private DeliveryNetworkPackage _currentNetworkDelivery;
        private bool _deliveryRequested;

        public static Action<DeliveryNetworkPackage> OnDeliveryRequested;
        public static Action<DeliveryNetworkPackage> OnDeliveryUpdated;
        public static Action<DeliveryNetworkPackage> OnDeliveryExpired;

        public byte Index => _index;
        public FactionIslandSO IslandData => _islandData;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                return;
            }
            
            // Init the timer with correct interval
            _deliveryRequestTimer = new CountdownTimer(_islandData.MerchandiseRequestedTimeInterval);
            
            // When the timer ends, we request another delivery and we restart the timer.
            _deliveryRequestTimer.OnTimerStop += () =>
            {
                RequestDelivery();
                _deliveryRequestTimer.Start();
            };
        }

        private void Update()
        {
            if (!IsServer)
            {
                return;
            }
            
            return;
            // Making clock tick
            _deliveryRequestTimer.Tick(Time.deltaTime);

            if (_deliveryRequested)
            {
                // Updating the time available on the request
                _currentNetworkDelivery.TimeAvailable = (uint)_deliveryRequestTimer.Time;
                //UpdateCurrentDeliveryClientRPC(_currentNetworkDeliveryPackage);
            }
        }

        public void StartRequestingDeliveries()
        {
            if (!IsServer)
            {
                return;
            }
            
            // Request the first delivery
            RequestDelivery();
            
            // Start timer for next delivery
            _deliveryRequestTimer.Start();
        }

        /// <summary>
        /// SERVER-SIDE
        /// Request a delivery and send a RPC to all the clients.
        /// </summary>
        [Command]
        private void RequestDelivery()
        {
            if (!IsServer)
            {
                return;
            }
            
            var randomRequest = _islandData.RequestRandomMerchandiseRequest();
            
            DeliveryNetworkPackage networkDeliveryNetworkPackagePackage = new DeliveryNetworkPackage(
                IDGenerator.RequestUniqueDeliveryID(),
                randomRequest.Item1, 
                randomRequest.Item2, 
                0, 
                _index, 
                randomRequest.Item3);
            
            RequestNewDeliveryClientRPC(networkDeliveryNetworkPackagePackage);
                        
            _deliveryRequested = true;
        }
        
        /// <summary>
        /// CLIENT-SIDE
        /// RPC received by all the client when the server create a new delivery request.
        /// </summary>
        [ClientRpc]
        private void RequestNewDeliveryClientRPC(DeliveryNetworkPackage networkDeliveryNetworkPackagePackage)
        {
            _currentNetworkDelivery = networkDeliveryNetworkPackagePackage;
            OnDeliveryRequested?.Invoke(networkDeliveryNetworkPackagePackage);
        }
        
        /// <summary>
        /// CLIENT-SIDE
        /// The server inform the client that a delivery has been updated.
        /// </summary>
        [ClientRpc]
        private void UpdateCurrentDeliveryClientRPC(DeliveryNetworkPackage networkDeliveryNetworkPackagePackage)
        {
            _currentNetworkDelivery = networkDeliveryNetworkPackagePackage;
            OnDeliveryUpdated?.Invoke(networkDeliveryNetworkPackagePackage);
        }
        
        /// <summary>
        /// SERVER-SIDE
        /// A client tell to the server that is updating the current deliveries. The server will then inform all other clients.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void UpdateCurrentDeliveryServerRPC(DeliveryNetworkPackage networkDeliveryNetworkPackagePackage, ushort amount, ServerRpcParams rpcParams = default)
        {
            if (amount == 0)
            {
                Debug.LogError("A delivery update with an amount of 0 has been called !");
                return;
            }
            
            // I'm not sure if we need to handle here the refund of player if the delivery is already completed.

            // Increase player currency
            var sellPrice = (ushort)(_islandData.GetResourceSellPrice(networkDeliveryNetworkPackagePackage.Resource) * amount);
            MultiplayerGameplayManager.Instance.IncreasePlayerCurrency(rpcParams.Receive.SenderClientId, sellPrice);
            
            // Update the delivery
            _currentNetworkDelivery.MerchandiseCurrentAmount += amount;
            
            // Inform all clients of the delivery update
            UpdateCurrentDeliveryClientRPC(_currentNetworkDelivery);
        }

        [Command]
        public void UpdateDelivery(ushort merchandiseAmount)
        {
            UpdateCurrentDeliveryServerRPC(_currentNetworkDelivery, merchandiseAmount);
        }
    }
}