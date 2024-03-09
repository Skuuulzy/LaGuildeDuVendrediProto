using System;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;
using VComponent.Items.Merchandise;
using VComponent.Tools.Timer;

namespace VComponent.Island
{
    /// <summary>
    /// Responsible fore creating deliveries, and managing island data.
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

        [Command]
        private void RequestDelivery()
        {
            var randomRequest = _islandData.RequestRandomMerchandiseRequest();
            Delivery delivery = new Delivery(randomRequest.Item1, randomRequest.Item2, 0, _index, randomRequest.Item3);
            
            RequestNewDeliveryClientRPC(delivery);
        }
        
        [ClientRpc]
        private void RequestNewDeliveryClientRPC(Delivery delivery) {
            
            OnDeliveryRequested?.Invoke(delivery);
        }
        
        [ClientRpc]
        private void UpdateCurrentDeliveryClientRPC(Delivery delivery) {
            
            OnDeliveryUpdated?.Invoke(delivery);
        }

        [Command]
        private void UpdateDelivery()
        {
            _currentDelivery.MerchandiseCurrentAmount++;
            UpdateCurrentDeliveryClientRPC(_currentDelivery);
        }
    }
}