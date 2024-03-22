using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VComponent.Items.Merchandise;
using VComponent.Ship;

public class ShipInfoController : MonoBehaviour
{
    [SerializeField] private LoadRessourcesInteraction _loadRessourcesInteraction;
    [SerializeField] private MultiplayerShipController _multiplayerShipController;

    [SerializeField] private SerializableDictionary<Image, int> _currentRessourcesCarried;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void UpdateExistingMerchandiseCarried(MerchandiseSO _merchandise, int number)
    {
        foreach(KeyValuePair<Image,int> kvp in _currentRessourcesCarried.ToDictionary())
        {
            if(kvp.Key.sprite == _merchandise.Sprite)
            {
                _currentRessourcesCarried.ToDictionary()[kvp.Key] = number;
                return;
			}
        }
    }
    public void OnRessourcesIslandDocked(RessourcesIslandSO islandSO)
    {
        _loadRessourcesInteraction.Show(islandSO.MerchandisesToSell, _multiplayerShipController.GetFreeSpace());
    }
}
