using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    private IInteractable _interactable;
    private bool _triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            _interactable = interactable;
            _triggered = true;
            _interactable.ShowInteractPopUp("E"); //Add key interaction from input manager
            //Display UI
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _triggered = false;
        _interactable = null;
        //remove UI

    }

    private void OnTriggerStay(Collider other)
    {
        if (_triggered && inputManager.Interact)
        {
            _interactable.Interact();
        }
        //interaction 
    }
}
