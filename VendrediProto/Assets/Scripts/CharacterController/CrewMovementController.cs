using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class CrewMovementController : MonoBehaviour
{
     #region Inspector Variables
     
    [TabGroup("Main Parameters")]
    [SerializeField] private float _moveSpeed = 2.0f;
    [TabGroup("Main Parameters")]
    [SerializeField] private float _sprintSpeed = 5.335f;
    [TabGroup("Main Parameters")]
    [SerializeField] private float _airMoveSpeed = 2.0f;
    [TabGroup("Main Parameters")]
    [SerializeField] private float _speedChangeRate = 10.0f;
    [TabGroup("Main Parameters")]
    [Tooltip("How fast the character turns to face movement direction")]
    [SerializeField, Range(0.0f, 0.3f)] private float _rotationSmoothTime = 0.12f;

    [TabGroup("Jump Parameters")]
    [SerializeField] private float _jumpHeight = 1.2f;
    [TabGroup("Jump Parameters")]
    [SerializeField] private float _gravity = -15.0f;
    [TabGroup("Jump Parameters")]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [SerializeField] private float _jumpTimeout = 0.50f;
    [TabGroup("Jump Parameters")]
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField] private float _fallTimeout = 0.15f;
    
    [TabGroup("Ground Parameters")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    [SerializeField] private bool _grounded = true;
    [TabGroup("Ground Parameters")]
    [SerializeField] private float _groundedOffset = -0.14f;
    [TabGroup("Ground Parameters")]
    [SerializeField] private float _groundedRadius = 0.28f;
    [TabGroup("Ground Parameters")]
    [SerializeField] private LayerMask _groundLayers;
    
    [TabGroup("Components")]
    [SerializeField] private GameObject _mainCamera;
    [TabGroup("Components")]
    [SerializeField] private CharacterController _controller;
    [TabGroup("Components")]
    [SerializeField] InputManager _inputManager;
    [TabGroup("Components")]
    [SerializeField] private Transform _footTransform;
    
    [TabGroup("Components")]
    [SerializeField] private bool _drawDebugGizmos;
    
    #endregion

    // Player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private readonly float _terminalVelocity = 53.0f;

    // Timeout delta time
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private void Start()
    {
        // reset our timeouts on start
        _jumpTimeoutDelta = _jumpTimeout;
        _fallTimeoutDelta = _fallTimeout;
    }

    private void LateUpdate()
    {
        JumpAndGravity();
        Move();
    }

    private void FixedUpdate()
    {
        GroundedCheck();
    }

    #region Methods

    private void GroundedCheck()
    {
        // The sphere will represent a zone where collisions will be detected. We are setting this zone to the position of our player.
        var position = _footTransform.position;
        Vector3 spherePosition = new(position.x, position.y - _groundedOffset, position.z);

        // Check if the sphere zone overlaps with any collider that has a ground layer
        _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
    }
    
    private void OnDrawGizmos()
    {
        if (!_drawDebugGizmos)
        {
            return;
        }
        
        // The sphere will represent a zone where collisions will be detected. We are setting this zone to the position of our player.
        var position = _footTransform.position;
        Vector3 spherePosition = new(position.x, position.y - _groundedOffset, position.z);

        // Check if the sphere zone overlaps with any collider that has a ground layer
        Gizmos.DrawSphere(spherePosition, _groundedRadius);
    }

    private void Move()
    {
        float targetSpeed;

        if (_grounded)
        {
            targetSpeed = _inputManager.Sprint ? _sprintSpeed : _moveSpeed;
        }
        else
        {
            targetSpeed = _airMoveSpeed;
        }
        
        if (_inputManager.Move == Vector2.zero)
        {
            targetSpeed = 0.0f;
        }

        var velocity = _controller.velocity;
        float currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;
        float speedOffset = 0.1f;

        // Accelerate or decelerate to target speed.
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * _speedChangeRate);

            // Round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // Normalize input direction so that the magnitude is always 1. This means rotation speed will be constant.
        Vector3 inputDirection = new Vector3(_inputManager.Move.x, 0.0f, _inputManager.Move.y).normalized;
        if (_inputManager.Move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);

            // Rotate to face input direction relative to camera position.
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private void JumpAndGravity()
    {
        if (_grounded)
        {
            // Reset the fall timeout timer.
            _fallTimeoutDelta = _fallTimeout;

            // Stop our velocity dropping infinitely when grounded.
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // If we're trying to jump and the time before being able to jump again has passed, jump.
            if (_inputManager.Jump && _jumpTimeoutDelta <= 0.0f)
            {
                // Calculate how much velocity is needed to reach desired height.
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }

            // Jump timeout.
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // Reset the jump timeout timer.
            _jumpTimeoutDelta = _jumpTimeout;

            // Fall timeout.
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }

            // If we are not grounded, do not jump.
            _inputManager.Jump = false;
        }

        // Apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time).
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }
    
    #endregion
}