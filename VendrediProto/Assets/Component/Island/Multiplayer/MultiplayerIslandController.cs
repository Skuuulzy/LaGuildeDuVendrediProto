using System;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;
using VComponent.Items.Merchandise;
using VComponent.Tools.Timer;

namespace VComponent.Island
{
    /// <summary>
    /// Responsible for creating deliveries, and managing island data.
    /// </summary>
    public class MultiplayerIslandController : NetworkBehaviour
    {
        [SerializeField] private int _index;
        [SerializeField] private IslandSO _islandData;

        private CountdownTimer _deliveryRequestTimer;

        public static Action<Delivery> OnDeliveryRequested;
        public static Action<Delivery> OnDeliveryUpdated;
        public static Action<Delivery> OnDeliveryExpired;

        private Delivery _currentDelivery;
        
        private void Start()
        {
            if (!IsServer)
            {
                return;
            }
            
            // Init the timer with correct interval
            _deliveryRequestTimer = new CountdownTimer(_islandData.MerchandiseToSellTimeInterval);
            
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
            
            // Making clock tick
            _deliveryRequestTimer.Tick(Time.deltaTime);
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
            var randomRequest = _islandData.RequestRandomMerchandiseRequest();
            Delivery delivery = new Delivery(randomRequest.Item1, randomRequest.Item2, 0, _index, randomRequest.Item3);
            
            RequestNewDeliveryClientRPC(delivery);
        }
        
        /// <summary>
        /// CLIENT-SIDE
        /// RPC received by all the client when the server create a new delivery request.
        /// </summary>
        [ClientRpc]
        private void RequestNewDeliveryClientRPC(Delivery delivery)
        {
            _currentDelivery = delivery;
            OnDeliveryRequested?.Invoke(delivery);
        }
        
        /// <summary>
        /// CLIENT-SIDE
        /// The server inform the client that a delivery has been updated.
        /// </summary>
        [ClientRpc]
        private void UpdateCurrentDeliveryClientRPC(Delivery delivery)
        {
            _currentDelivery = delivery;
            OnDeliveryUpdated?.Invoke(delivery);
        }
        
        /// <summary>
        /// SERVER-SIDE
        /// A client tell to the server that is updating the current deliveries. The server will then inform all other clients.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        private void UpdateCurrentDeliveryServerRPC(Delivery delivery)
        {
            // I think i need to do something safer here.
            // We need to handle here the race conditions when deliver things.
            
            _currentDelivery = delivery;
            
            UpdateCurrentDeliveryClientRPC(delivery);
        }

        [Command]
        private void UpdateDelivery()
        {
            if (_currentDelivery.IsDone())
            {
                Debug.LogWarning("Delivery already completed.");
                return;
            }
            
            _currentDelivery.MerchandiseCurrentAmount++;
            UpdateCurrentDeliveryServerRPC(_currentDelivery);
        }
    }
}