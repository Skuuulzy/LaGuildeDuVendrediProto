using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestInteraction : MonoBehaviour, IInteractable
{

    [SerializeField] private GameObject _popUpCanvas;
    [SerializeField] private TMP_Text _popUpText;
    public void Interact()
    {
        Debug.Log("OUi");
        //Do something
    }

    public void ShowInteractPopUp(string key)
    {
        _popUpText.text = key;
        _popUpCanvas.SetActive(true);
    }

    public void HideInteractPopUp()
    {
        _popUpCanvas.SetActive(false);
    }



}
