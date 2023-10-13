using Sirenix.OdinInspector;
using UnityEngine;

public class CrewMovementController : MonoBehaviour
{
    [TabGroup("Parameters")]
    [SerializeField] private float _maxSpeed = 5.0f;
    [TabGroup("Parameters")]
    [SerializeField] private float _acceleration = 10.0f;
    [TabGroup("Parameters")]
    [SerializeField] private float _deceleration = 20.0f;
    [TabGroup("Parameters")]
    [SerializeField] private float _intensity = 1.0f;
    [TabGroup("Inputs")]
    [SerializeField] private KeyCode _moveUpKey = KeyCode.W;
    [TabGroup("Inputs")]
    [SerializeField] private KeyCode _moveDownKey = KeyCode.S;
    [TabGroup("Inputs")]
    [SerializeField] private KeyCode _moveLeftKey = KeyCode.A;
    [TabGroup("Inputs")]
    [SerializeField] private KeyCode _moveRightKey = KeyCode.D;
    [TabGroup("Components")]
    [SerializeField] private Rigidbody _rb;

    private Vector3 _movement;

    private void Update()
    {
        ReadInput();
        MoveCharacter();
    }

    private void ReadInput()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        if (Input.GetKey(_moveUpKey))
        {
            verticalInput = 1.0f;
        }
        if (Input.GetKey(_moveDownKey))
        {
            verticalInput = -1.0f;
        }
        if (Input.GetKey(_moveLeftKey))
        {
            horizontalInput = -1.0f;
        }
        if (Input.GetKey(_moveRightKey))
        {
            horizontalInput = 1.0f;
        }

        _movement = new Vector3(horizontalInput, verticalInput, 0f).normalized * (_maxSpeed * _intensity);
    }

    private void MoveCharacter()
    {
        Vector3 targetVelocity = _movement;

        // Apply acceleration and deceleration
        _rb.velocity = Vector3.MoveTowards(_rb.velocity, targetVelocity, _acceleration * Time.deltaTime);

        // Deceleration when no input is given
        if (_movement == Vector3.zero)
        {
            _rb.velocity = Vector3.MoveTowards(_rb.velocity, Vector3.zero, _deceleration * Time.deltaTime);
        }
    }
}