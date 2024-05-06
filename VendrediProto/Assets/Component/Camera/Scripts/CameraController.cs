using UnityEngine;
using UnityEngine.InputSystem;
using VComponent.InputSystem;

namespace VComponent.CameraSystem
{
    public class CameraController : MonoBehaviour
    {
        [Header("Main Parameters")]
        [Tooltip("The standard speed of the camera")] 
        [Range(0.1f,40)] [SerializeField] private float _normalMovementSpeed = 1;
        
        [Tooltip("The fats speed of the camera, when the fast speed key is pressed")] 
        [Range(0.1f,80)] [SerializeField] private float _fastMovementSpeed = 2;
        
        [Tooltip("The rotation amount when the rotation key are pressed")] 
        [Range(0.1f,10)] [SerializeField] private float _rotationAmount = 1;
        
        [Tooltip("How fast the camera will zoom")] 
        [Range(1,90)] [SerializeField] private int _zoomAmount = 5;
        
        [Tooltip("The higher this value the higher the camera movement will be responsive")]
        [Range(0.1f,10)] [SerializeField] private float _movementResponsiveness = 5;
        
        [Tooltip("The sensitivity for camera rotation when right click is pressed")]
        [Range(0.1f,10)] [SerializeField] private float _rotationSensitivity = 1;
        
        [Header("Pitch")]
        [Tooltip("Max angle for the pitching of the camera")] 
        [SerializeField] private float _maxPitchAngle = 10;
        
        [Tooltip("Min angle for the pitching of the camera")] 
        [SerializeField] private float _minPitchAngle = 75;
        
        [Tooltip("The screen ratio that the mouse need to travel before detecting a pitch")]
        [SerializeField] private float _pitchDeadZone = 0.05f;
        
        [SerializeField] private bool _inversePitch;
        
        
        [Header("Components")] 
        [SerializeField] private Transform _cameraTransform;
        
        private Camera _mainCam;
        
        private Vector3 _newPosition;
        private Quaternion _newYRotation;
        private Quaternion _newXRotation;
        private Vector3 _newZoom;

        private Vector3 _dragMovementStartPosition;
        private Vector3 _dragMovementCurrentPosition;
        
        private Vector2 _dragRotateStartPosition;
        private Vector2 _dragRotateCurrentPosition;
        
        private bool _dragMovementInitialized;
        private bool _dragRotationInitialized;

        void Awake()
        {
            _mainCam = Camera.main;
            
            _cameraTransform.localRotation = Quaternion.Euler(_minPitchAngle, 0, 0);
            
            _newPosition = transform.position;
            _newYRotation = transform.rotation;
            _newXRotation = _cameraTransform.localRotation;
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
            if (InputsManager.Instance.DragMovementCamera)
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
            
            if (InputsManager.Instance.DragRotationCamera)
            {
                // First frame input
                if (!_dragRotationInitialized)
                {
                    // Reset previous rotation since it may not be reached because of the lerp.
                    _newXRotation = _cameraTransform.localRotation;
                    _newYRotation = transform.rotation;
                    
                    _dragRotateStartPosition = Mouse.current.position.ReadValue();
                    _dragRotationInitialized = true;
                }

                _dragRotateCurrentPosition = Mouse.current.position.ReadValue();
                
                // Computing the drag horizontal input
                var differenceX = (_dragRotateStartPosition.x - _dragRotateCurrentPosition.x) / Screen.width;
                differenceX = Mathf.Clamp(differenceX, -1, 1);
                
                // Computing the drag vertical input
                var differenceY = (_dragRotateStartPosition.y - _dragRotateCurrentPosition.y) / Screen.height;
                differenceY = Mathf.Clamp(differenceY, -1, 1);
                
                if (differenceY != 0 && Mathf.Abs(differenceY) > _pitchDeadZone)
                {
                    differenceY = _inversePitch ? -differenceY : differenceY;
                    _newXRotation *= Quaternion.Euler(Vector3.right * (differenceY * _rotationSensitivity));
                    _newXRotation = _newXRotation.ClampAxis(ExtensionMethods.Axis.X, _maxPitchAngle, _minPitchAngle);
                }
                if (differenceX != 0)
                {
                    _newYRotation *= Quaternion.Euler(Vector3.up * (-differenceX * _rotationSensitivity));
                }
            }
            else
            {
                _dragRotationInitialized = false;
            }
        }

        private void HandleMovementInput()
        {
            // Movement
            var movementSpeed = InputsManager.Instance.QuickMoveCamera ? _fastMovementSpeed : _normalMovementSpeed;

            // Calculate movement direction relative to camera rotation
            var movementDirection = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * new Vector3(InputsManager.Instance.MoveCamera.x, 0, InputsManager.Instance.MoveCamera.y);
            _newPosition += movementDirection.normalized * movementSpeed;

            transform.position = Vector3.Lerp(transform.position, _newPosition, _movementResponsiveness * Time.deltaTime);

            // Rotation
            if (InputsManager.Instance.ClockwiseRotationCamera)
            {
                _newYRotation *= Quaternion.Euler(Vector3.up * -_rotationAmount);
            }

            if (InputsManager.Instance.AntiClockwiseRotationCamera)
            {
                _newYRotation *= Quaternion.Euler(Vector3.up * _rotationAmount);
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, _newYRotation, _movementResponsiveness * Time.deltaTime);

            // Zoom
            _newZoom += InputsManager.Instance.ZoomCamera * new Vector3(0, -_zoomAmount, _zoomAmount);

            _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, _newZoom, _movementResponsiveness * Time.deltaTime);
            
            // X Rotation (pitch)
            if (InputsManager.Instance.DragRotationCamera)
            {
                _cameraTransform.localRotation = Quaternion.Lerp(_cameraTransform.localRotation, _newXRotation, _movementResponsiveness * Time.deltaTime);
            }
        }

        /// <summary>
        /// Set the camera position by an extern call
        /// </summary>
        public void SetCameraPosition(Vector3 position)
		{
            _newPosition = position;
		}
    }
}