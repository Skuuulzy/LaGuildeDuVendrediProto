using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanonInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _popUp;
    [SerializeField] private TMP_Text _popUpText;
    public void Interact()
    {
        Debug.Log("Canon");
    }

    public void ShowInteractPopUp(string key)
    {
        _popUpText.text = key;
        _popUp.SetActive(true);
    }
    public void HideInteractPopUp()
    {
        _popUp.SetActive(false);
    }

    public void OnTriggerStay(Collider other)
    {
        Debug.Log("Canon");
    }

    public void GetName()
    {
    }

    public void GetPriority()
    {
    }
}
