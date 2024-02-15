using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Component.CameraSystem
{
    public class CameraController : MonoBehaviour
    {
        [Header("Parameters")] [SerializeField]
        private float _normalMovementSpeed;

        [SerializeField] private float _fastMovementSpeed;
        [SerializeField] private float _movementTime;
        [SerializeField] private float _rotationAmount;
        [SerializeField] private Vector3 _zoomAmount;

        [Header("Components")] [SerializeField]
        private CameraInputs _inputs;

        [SerializeField] private Transform _cameraTransform;

        private Vector3 _newPosition;
        private Quaternion _newRotation;
        private Vector3 _newZoom;

        private Vector3 _dragStartPosition;
        private Vector3 _dragCurrentPosition;
        
        private Vector2 _rotateStartPosition;
        private Vector2 _rotateCurrentPosition;
        
        private bool _dragMovementInitialized;
        private bool _dragRotationInitialized;

        void Start()
        {
            _newPosition = transform.position;
            _newRotation = transform.rotation;
            _newZoom = _cameraTransform.localPosition;
        }

        private void Update()
        {
            HandleDragInput();
            HandleMovementInput();
        }

        // WIP : Do not work yet, start and current drag position are always the same
        // Checkout this video: https://www.youtube.com/watch?v=3Y7TFN_DsoI
        private void HandleDragInput()
        {
            if (_inputs.DragMovement)
            {
                // First frame input
                if (!_dragMovementInitialized)
                {
                    Plane plane1 = new Plane(Vector3.up, Vector3.zero);
                    Ray ray1 = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (plane1.Raycast(ray1, out var entry1))
                    {
                        _dragStartPosition = ray1.GetPoint(entry1);
                    }
                    
                    _dragMovementInitialized = true;
                }
                
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out var entry))
                {
                    _dragCurrentPosition = ray.GetPoint(entry);
                    _newPosition = transform.position + _dragStartPosition - _dragCurrentPosition;
                }
            }
            else
            {
                _dragMovementInitialized = false;
            }
            
            if (_inputs.DragRotation)
            {
                // First frame input
                if (!_dragRotationInitialized)
                {
                    _rotateStartPosition = Mouse.current.position.ReadValue();
                    
                    _dragRotationInitialized = true;
                }

                _rotateCurrentPosition = Mouse.current.position.ReadValue();
                var difference = _rotateStartPosition - _rotateCurrentPosition;

                _newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5));
            }
            else
            {
                _dragRotationInitialized = false;
            }
        }

        private void HandleMovementInput()
        {
            // Movement
            var movementSpeed = _inputs.QuickMove ? _fastMovementSpeed : _normalMovementSpeed;

            // Calculate movement direction relative to camera rotation
            var movementDirection = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * new Vector3(_inputs.Move.x, 0, _inputs.Move.y);
            _newPosition += movementDirection.normalized * movementSpeed;

            transform.position = Vector3.Lerp(transform.position, _newPosition, _movementTime * Time.deltaTime);

            // Rotation
            if (_inputs.ClockwiseRotation)
            {
                _newRotation *= Quaternion.Euler(Vector3.up * -_rotationAmount);
            }

            if (_inputs.AntiClockwiseRotation)
            {
                _newRotation *= Quaternion.Euler(Vector3.up * _rotationAmount);
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, _movementTime * Time.deltaTime);

            // Zoom
            _newZoom += _inputs.Zoom * _zoomAmount;

            _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _newZoom, _movementTime * Time.deltaTime);
        }
    }
}