using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _movementTime;

    private Vector3 _newPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        _newPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
    }

    // TODO : Convert to the new input system 
    private void HandleMovementInput()
    {
        // Forward
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow))
        {
            _newPosition += (transform.forward * _movementSpeed);
        }
        // Backward
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _newPosition -= (transform.forward * _movementSpeed);
        }
        // Right
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            _newPosition += (transform.right * _movementSpeed);
        }
        // Left
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            _newPosition -= (transform.right * _movementSpeed);
        }

        transform.position = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * _movementTime);
    }
}
