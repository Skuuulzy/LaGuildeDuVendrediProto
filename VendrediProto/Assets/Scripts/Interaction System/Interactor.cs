using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    private IInteractable _interactable;
    private bool _triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            _interactable = interactable;
            _triggered = true;
            _interactable.ShowInteractPopUp("A"); //Add key interaction from input manager
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(_triggered && _interactable != null)
        {
            _interactable.HideInteractPopUp();
            _triggered = false;
            _interactable = null;
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (_triggered && _inputManager.Interact)
        {
            _interactable.Interact();
        } 
    }
}
