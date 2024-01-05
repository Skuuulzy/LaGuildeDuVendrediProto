using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    //Unused : Enum GetName() pour savoir quel type de IInteractable
    public void GetName();

    //Unused : Get Priority pour savoir la priorité 
    public void GetPriority();
    public void Interact();
    public void ShowInteractPopUp(string key);

    public void HideInteractPopUp();

}
