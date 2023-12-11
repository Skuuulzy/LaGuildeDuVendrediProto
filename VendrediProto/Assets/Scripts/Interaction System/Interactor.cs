using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    private bool _lock = false;
    private List<IInteractable> _interactables = new List<IInteractable>();


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            bool containInteractable = _interactables.Contains(interactable);
            if (!containInteractable)
            {
                _interactables.Add(interactable);
            }

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
            if (containInteractable)
            {
                interactable.HideInteractPopUp();
                _interactables.Remove(interactable);
                if (_interactables.Count() == 0)
                {
                    _lock = false;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_inputManager.Interact && _interactables.Count() > 0 && !_lock)
        {
            _lock = true;
            foreach (IInteractable interactable in _interactables)
            {
                interactable.Interact();
            }
        }
    }
}
