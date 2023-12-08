using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    private bool _triggered;

    private List<IInteractable> _interactables = new List<IInteractable>();
    // QUESTION : Comment savoir quel IInteractable est t-il

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            bool containInteractable = _interactables.Contains(interactable);
            if (!containInteractable)
            {
                _interactables.Add(interactable);
            }
            _triggered = true;
            foreach (IInteractable _interactable in _interactables)
            {
                _interactable.ShowInteractPopUp("A"); //Add key interaction from input manager
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            bool containInteractable = _interactables.Contains(interactable);
            if (_triggered && containInteractable)
            {
                interactable.HideInteractPopUp();
                if(_interactables.Count() == 0)
                {
                    _triggered = false;
                }
                _interactables.Remove(interactable);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_triggered && _inputManager.Interact)
        {
            //TODO : question si même bouton  et si bouton different!=

            foreach (IInteractable interactable in _interactables)
            {
                interactable.Interact();
            }
        }
    }
}
