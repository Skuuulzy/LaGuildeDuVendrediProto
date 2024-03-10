using UnityEngine;
using VComponent.Island;
using VComponent.Items.Merchandise;
using VComponent.Multiplayer.Deliveries;

public class MultiplayerShipController : MonoBehaviour
{
    [SerializeField] private MerchandiseType _currentMerchandiseCarriedType;
    [SerializeField] private ushort _currentMerchandiseCarriedNumber;

    private MultiplayerIslandController _dockedIsland;
    private bool _merchandiseSellable;
    
    private void OnTriggerEnter(Collider other)
    {
        MultiplayerIslandController islandController = other.gameObject.GetComponent<MultiplayerIslandController>();
        if (islandController != null)
        {
            Debug.Log($"Entering island {islandController.IslandData.IslandName}");
            _dockedIsland = islandController;

            Delivery requestedDelivery = DeliveryManager.Instance.GetRequestedDeliveryBy(islandController);
            
            if (requestedDelivery != null)
            {
                // The request il already done or we do not have the correct merchandise type.
                if (requestedDelivery.Data.IsDone() || _currentMerchandiseCarriedType != requestedDelivery.Data.Merchandise)
                {
                    return;
                }
                
                // Inform the UI that we can sell this merchandise. Maybe pass this instance to then sell bind the button listener to the sell goods method.
                _merchandiseSellable = true;
            }
            else
            {
                Debug.Log("The current island don't request any delivery.");
                // Do something here to check if the island generate a delivery while we are in the zone.
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MultiplayerIslandController islandController = other.gameObject.GetComponent<MultiplayerIslandController>();
        if (islandController != null)
        {
            Debug.Log($"Exiting island {islandController.IslandData.IslandName}");
            _dockedIsland = null;
            _merchandiseSellable = false;
        }
    }

    public void SellMerchandiseToDockedIsland()
    {
        if (_dockedIsland == null)
        {
            Debug.LogError("No docked island to sell merchandise to !");
            return;
        }

        if (!_merchandiseSellable)
        {
            Debug.LogError($"The current merchandise: {_currentMerchandiseCarriedType} cannot be sell to the island: {_dockedIsland.IslandData.IslandName} !");
            return;
        }
        
        // Do something to check how many units of merchandise can be sell here.
        _dockedIsland.UpdateDelivery(_currentMerchandiseCarriedNumber);
        _currentMerchandiseCarriedNumber = 0;
        _currentMerchandiseCarriedType = MerchandiseType.NONE;
        _merchandiseSellable = false;
    }
}