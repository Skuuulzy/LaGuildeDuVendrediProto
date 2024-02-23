using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VComponent.CameraSystem
{
    public class CameraInputs : MonoBehaviour
    {
        [ShowInInspector] public Vector2 Move { get; private set; }
        [ShowInInspector] public bool QuickMove { get; private set; }
        [ShowInInspector] public bool ClockwiseRotation { get; private set; }
        [ShowInInspector] public bool AntiClockwiseRotation { get; private set; }
        [ShowInInspector] public int Zoom { get; private set; }
        [ShowInInspector] public bool DragMovement { get; private set; }
        [ShowInInspector] public bool DragRotation { get; private set; }

        public void MoveInput(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }

        public void FastMoveInput(InputAction.CallbackContext context)
        {
            QuickMove = context.performed;
        }
        
        public void ClockwiseRotationInput(InputAction.CallbackContext context)
        {
            ClockwiseRotation = context.performed;
        }
        
        public void AntiClockwiseRotationInput(InputAction.CallbackContext context)
        {
            AntiClockwiseRotation = context.performed;
        }

        public void ScrollInput(InputAction.CallbackContext context)
        {
            var scrollValue = context.ReadValue<float>();
            if (scrollValue > 0)
            {
                Zoom = 1;
            }
            else if (scrollValue < 0)
            {
                Zoom = -1;
            }
            else
            {
                Zoom = 0;
            }
        }
        
        public void DragMovementInput(InputAction.CallbackContext context)
        {
            //Debug.Log($"Start drag: {context.started}, DragMovement: {context.performed}");
            DragMovement = context.performed;
        }
        
        public void DragRotationInput(InputAction.CallbackContext context)
        {
            //Debug.Log($"Start drag: {context.started}, DragMovement: {context.performed}");
            DragRotation = context.performed;
        }
    }
}