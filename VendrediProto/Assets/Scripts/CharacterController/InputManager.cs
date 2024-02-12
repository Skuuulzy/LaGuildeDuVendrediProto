using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [ShowInInspector] public Vector2 Move { get; private set; }
    [ShowInInspector] public bool Jump { get; set; }
    [ShowInInspector] public bool Sprint { get; private set; }
    [ShowInInspector] public bool Interact { get; private set; }
    [ShowInInspector] public bool EndInteract { get; private set; }
    
    #region Action callbacks
	
    public void MoveInput(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        Jump = context.performed;
    }

    public void SprintInput(InputAction.CallbackContext context)
    {
        Sprint = context.performed;
    }

    public void InteractInput(InputAction.CallbackContext context)
    {
        Debug.Log(context.performed);
        Interact = context.performed;
        EndInteract = context.canceled;
    }
    
    #endregion
}
