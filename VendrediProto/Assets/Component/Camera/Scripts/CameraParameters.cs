using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VComponent.CameraSystem
{
    [CreateAssetMenu(menuName = "VComponents/CameraSystem/Camera Parameters")]
    public class CameraParameters : ScriptableObject
    {
        [TextArea]
        [SerializeField] private string _description;
        
        [Header("MOVEMENT PARAMETERS")]
        [Tooltip("The standard speed of the camera.")] 
        [Range(0.1f,40)] [SerializeField] private float _normalMovementSpeed = 1;
        [Tooltip("The fats speed of the camera, when the fast speed key is pressed.")] 
        [Range(0.1f,80)] [SerializeField] private float _fastMovementSpeed = 2;
        [Tooltip("How fast the camera will zoom")]
        [Range(1,150)] [SerializeField] private int _zoomSpeed = 5;
        [Tooltip("The higher this value the higher the camera movement will be responsive.")]
        [Range(0.1f,10)] [SerializeField] private float _movementResponsiveness = 5;
        
        [Header("ROTATION PARAMETERS")]
        [Tooltip("The higher this value the higher the camera rotation will be sensitive.")] 
        [Range(0.1f,10)] [SerializeField] private float _rotationSensitivity = 1;
        [Tooltip("The higher this value the higher the camera rotation will be responsive.")]
        [Range(0.1f,10)] [SerializeField] private float _rotationResponsiveness = 5;
        [Tooltip("Max angle for the pitching of the camera.")]
        [SerializeField] private Vector2 _pitchRange = new (10, 75);
        [Tooltip("The screen ratio that the mouse need to travel before detecting a pitch.")]
        [SerializeField] private float _pitchDeadZone = 0.05f;
        [Tooltip("If pitch inverted, when dragging mouse down the cam look up.")]
        [SerializeField] private bool _inversePitch;

        public float NormalMovementSpeed => _normalMovementSpeed;
        public float FastMovementSpeed => _fastMovementSpeed;
        public int ZoomSpeed => _zoomSpeed;
        public float MovementResponsiveness => _movementResponsiveness;

        
        public float RotationSensitivity => _rotationSensitivity;
        public float RotationResponsiveness => _rotationResponsiveness;
        public Vector2 PitchRange => _pitchRange;
        public float PitchDeadZone => _pitchDeadZone;
        public bool InversePitch => _inversePitch;
    }
}