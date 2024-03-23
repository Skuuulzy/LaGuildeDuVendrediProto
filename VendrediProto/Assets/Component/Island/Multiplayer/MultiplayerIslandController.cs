using System;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;
using VComponent.Items.Merchandise;
using VComponent.Tools.IDGenerators;
using VComponent.Tools.Timer;

namespace VComponent.Island
{
    /// <summary>
    /// Responsible for creating deliveries, and managing island data.
    /// </summary>
    public class MultiplayerIslandController : NetworkBehaviour
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
        
        private void Start()
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
            // Request the first delivery
            RequestDelivery();
            
            // Start timer for next delivery
            //_deliveryRequestTimer.Start();
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
        private void UpdateCurrentDeliveryServerRPC(DeliveryNetworkPackage networkDeliveryNetworkPackagePackage)
        {
            // I think we need to do something safer here.
            // We need to handle here the race conditions when deliver things.
            
            _currentNetworkDelivery = networkDeliveryNetworkPackagePackage;
            
            UpdateCurrentDeliveryClientRPC(networkDeliveryNetworkPackagePackage);
        }

        [Command]
        public void UpdateDelivery(ushort merchandiseAmount)
        {
            _currentNetworkDelivery.MerchandiseCurrentAmount += merchandiseAmount;
            UpdateCurrentDeliveryServerRPC(_currentNetworkDelivery);
        }
    }
}