using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact();
    public void ShowInteractPopUp(string key);

    public void HideInteractPopUp();
}
