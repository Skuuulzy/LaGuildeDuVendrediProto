using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CannonInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _popUpCanvas;
    [SerializeField] private TMP_Text _popUpText;
    [SerializeField] private Item _item;
    [SerializeField] private bool _isReloaded;

    public void GetName()
    {
        Debug.Log("I am : " + gameObject.name);
    }

    public void GetPriority()
    {
    }

    public void HideInteractPopUp()
    {
        _popUpCanvas.SetActive(false);
    }

    public void Interact()
    {
        if (this.gameObject.activeInHierarchy == false)
        {
            return;
        }
        if (_isReloaded)
        {
            Debug.Log("Fire ! ");
            _isReloaded = false;
        }
        else
        {
            //TODO : if cannonball, reload
            if(_item.ItemType == ItemsType.CANNONBALL)
            {
                Debug.Log("Reloading");
                _isReloaded = true;
            }
            Debug.Log("No ammo pls recharge");
        }
    }

    public void ShowInteractPopUp(string key)
    {
        _popUpText.text = key;
        _popUpCanvas.SetActive(true);
        GetName();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
