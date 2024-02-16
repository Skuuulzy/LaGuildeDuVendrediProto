using UnityEngine;
using UnityEngine.InputSystem;

namespace Component.CameraSystem
{
    public class CameraController : MonoBehaviour
    {
        [Header("Parameters")]
        [Tooltip("The standard speed of the camera")] [Range(0.1f,10)]
        [SerializeField] private float _normalMovementSpeed = 1;
        [Tooltip("The fats speed of the camera, when the fast speed key is pressed")] [Range(0.1f,10)]
        [SerializeField] private float _fastMovementSpeed = 2;
        [Tooltip("The rotation amount when the rotation key are pressed")] [Range(0.1f,10)]
        [SerializeField] private float _rotationAmount = 1;
        [Tooltip("How fast the camera will zoom")] [Range(1,20)]
        [SerializeField] private int _zoomAmount = 5;
        [Tooltip("The higher this value the higher the camera movement will be responsive")] [Range(0.1f,10)]
        [SerializeField] private float _movementResponsiveness = 5;
        
        [Header("Components")] 
        [SerializeField] private CameraInputs _inputs;
        [SerializeField] private Transform _cameraTransform;
        
        private Camera _mainCam;
        
        private Vector3 _newPosition;
        private Quaternion _newRotation;
        private Vector3 _newZoom;

        private Vector3 _dragMovementStartPosition;
        private Vector3 _dragMovementCurrentPosition;
        
        private Vector2 _dragRotateStartPosition;
        private Vector2 _dragRotateCurrentPosition;
        
        private bool _dragMovementInitialized;
        private bool _dragRotationInitialized;

        void Start()
        {
            _mainCam = Camera.main;
            
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
                    Ray ray1 = _mainCam.ScreenPointToRay(Input.mousePosition);

                    if (plane1.Raycast(ray1, out var entry1))
                    {
                        _dragMovementStartPosition = ray1.GetPoint(entry1);
                    }
                    
                    _dragMovementInitialized = true;
                }
                
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out var entry))
                {
                    _dragMovementCurrentPosition = ray.GetPoint(entry);
                    _newPosition = transform.position + _dragMovementStartPosition - _dragMovementCurrentPosition;
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
                    _dragRotateStartPosition = Mouse.current.position.ReadValue();
                    
                    _dragRotationInitialized = true;
                }

                _dragRotateCurrentPosition = Mouse.current.position.ReadValue();
                var difference = _dragRotateStartPosition- _dragRotateCurrentPosition;

                _newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 300));
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

            transform.position = Vector3.Lerp(transform.position, _newPosition, _movementResponsiveness * Time.deltaTime);

            // Rotation
            if (_inputs.ClockwiseRotation)
            {
                _newRotation *= Quaternion.Euler(Vector3.up * -_rotationAmount);
            }

            if (_inputs.AntiClockwiseRotation)
            {
                _newRotation *= Quaternion.Euler(Vector3.up * _rotationAmount);
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation, _movementResponsiveness * Time.deltaTime);

            // Zoom
            _newZoom += _inputs.Zoom * new Vector3(0, -_zoomAmount, _zoomAmount);

            _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _newZoom, _movementResponsiveness * Time.deltaTime);
        }
    }
}