using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VComponent.Island;
using VComponent.Items.Merchandise;
using VComponent.Tools.Singletons;

namespace VComponent.Multiplayer.Deliveries
{
    /// <summary>
    /// Hold the delivery generated by the islands and add data to them. Ensure correct data centralisation and integrity.
    /// </summary>
    public class DeliveryManager : Singleton<DeliveryManager>
    {
        [ShowInInspector, ReadOnly] private readonly List<Delivery> _activeDeliveries = new ();
        [ShowInInspector, ReadOnly] private readonly List<Delivery> _expiredDeliveries = new ();
        
        private MultiplayerFactionIslandController[] _islandControllers;

        public static Action<Delivery> OnDeliveryCreated;

        #region MONO

        protected override void Awake()
        {
            base.Awake();

            _islandControllers = FindObjectsOfType<MultiplayerFactionIslandController>();

            MultiplayerFactionIslandController.OnDeliveryRequested += HandleDeliveryRequested;
            MultiplayerFactionIslandController.OnDeliveryUpdated += HandleDeliveryUpdated;
            MultiplayerFactionIslandController.OnDeliveryExpired += HandleDeliveryExpired;
        }

        public void OnDestroy()
        {
            MultiplayerFactionIslandController.OnDeliveryRequested -= HandleDeliveryRequested;
            MultiplayerFactionIslandController.OnDeliveryUpdated -= HandleDeliveryUpdated;
            MultiplayerFactionIslandController.OnDeliveryExpired -= HandleDeliveryExpired;
        }

        #endregion MONO

        #region HANDLERS

        private void HandleDeliveryRequested(DeliveryNetworkPackage deliveryNetworkPackage)
        {
            if (DeliveryAlreadyExist(deliveryNetworkPackage.ID))
            {
                Debug.LogError($"Delivery with ID:{deliveryNetworkPackage.ID} already requested !");
                return;
            }

            Delivery delivery = new Delivery(deliveryNetworkPackage, FindIslandById(deliveryNetworkPackage.IslandIndex));
            _activeDeliveries.Add(delivery);
            
            OnDeliveryCreated?.Invoke(delivery);
        }

        private void HandleDeliveryUpdated(DeliveryNetworkPackage deliveryNetworkPackage)
        {
            int deliveryIndexToUpdate = GetDeliveryIndexByID(deliveryNetworkPackage.ID);
            
            // Delivery not found.
            if (deliveryIndexToUpdate == - 1)
            {
                Debug.LogWarning($"Delivery with index: {deliveryNetworkPackage.ID} not found in current deliveries. The delivery will be added but this behaviour is not supposed to happen !");
                HandleDeliveryRequested(deliveryNetworkPackage);
                return;
            }

            _activeDeliveries[deliveryIndexToUpdate].UpdateNetworkData(deliveryNetworkPackage);
        }
        
        private void HandleDeliveryExpired(DeliveryNetworkPackage deliveryNetworkPackage)
        {
            int deliveryIndexRemove = GetDeliveryIndexByID(deliveryNetworkPackage.ID);

            if (deliveryIndexRemove == -1)
            {
                Debug.LogWarning($"Delivery with index: {deliveryNetworkPackage.ID} not found in current deliveries. The delivery cannot be removed.");
                return;
            }

            // Removing the delivery from active deliveries.
            Delivery deliveryToRemove = _activeDeliveries[deliveryIndexRemove];
            _activeDeliveries.Remove(deliveryToRemove);
            
            // Adding it to expired deliveries.
            _expiredDeliveries.Add(deliveryToRemove);
            
            deliveryToRemove.SetHasExpired();
        }

        #endregion HANDLERS

        #region HELPERS METHODS

        private MultiplayerFactionIslandController FindIslandById(byte islandIndex)
        {
            foreach(MultiplayerFactionIslandController islandController in  _islandControllers)
            {
                if(islandController.Index == islandIndex)
                {
                    return islandController;
                }
            }

            Debug.LogError($"Unable to find any island controller with the index: {islandIndex}");
            return null;
        }

        private bool DeliveryAlreadyExist(ushort deliveryIndex)
        {
            for (int i = 0; i < _activeDeliveries.Count; i++)
            {
                if (_activeDeliveries[i].Data.ID == deliveryIndex)
                {
                    return true;
                }
            }

            return false;
        }

        private int GetDeliveryIndexByID(ushort deliveryIndex)
        {
            for (int i = 0; i < _activeDeliveries.Count; i++)
            {
                if (_activeDeliveries[i].Data.ID == deliveryIndex)
                {
                    return i;
                }
            }
            
            return -1;
        }

        #endregion HELPERS METHODS

        public Delivery GetRequestedDeliveryBy(MultiplayerFactionIslandController buyer)
        {
            for (int i = 0; i < _activeDeliveries.Count; i++)
            {
                if (_activeDeliveries[i].Buyer == buyer)
                {
                    return _activeDeliveries[i];
                }
            }

            return null;
        }
    }
}