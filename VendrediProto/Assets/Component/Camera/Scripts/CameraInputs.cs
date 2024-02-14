using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Component.CameraSystem
{
    public class CameraInputs : MonoBehaviour
    {
        [ShowInInspector] public Vector2 Move { get; private set; }
        [ShowInInspector] public bool QuickMove { get; private set; }
        [ShowInInspector] public bool ClockwiseRotation { get; private set; }
        [ShowInInspector] public bool AntiClockwiseRotation { get; private set; }
        [ShowInInspector] public int Zoom { get; private set; }
        [ShowInInspector] public bool StartDrag { get; private set; }
        [ShowInInspector] public bool Drag { get; private set; }

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
        
        public void DragInput(InputAction.CallbackContext context)
        {
            //Debug.Log($"Start drag: {context.started}, Drag: {context.performed}");
            StartDrag = context.started;
            Drag = context.performed;
        }
    }
}