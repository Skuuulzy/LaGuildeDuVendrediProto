using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteraction : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("OUi");
    }

    public void ShowInteractPopUp(string key)
    {
        Debug.Log(key);
    }



}
