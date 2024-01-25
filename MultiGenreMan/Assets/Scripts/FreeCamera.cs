using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    //camera stuff
    [SerializeField, Range(0.1f, 10), Min(0.1f)]
    private float _sensitivity = 10F; //Mouse Sensitivity
    private float _pitch = 0;
    private float _yaw = 0;
    private Vector2 _mouseInput = Vector2.zero;
    private Vector2 _movementInput = Vector2.zero;

    [SerializeField]
    private float _moveSpeed = 10;
    [SerializeField]
    private float _sprintSpeed = 20;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        transform.LookAt(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        //Use GetAxisRaw to get a more responsive input
        _mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        _movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        MouseLook();

    }

    private void FixedUpdate()
    {

        Movement();
    }

    void MouseLook()
    {
        _yaw = (_yaw + _sensitivity * _mouseInput.x) % 360f;
        _pitch = (_pitch - _sensitivity * _mouseInput.y) % 360f;

        _pitch = Mathf.Clamp(_pitch, -90, 90);

        transform.rotation = Quaternion.AngleAxis(_yaw, Vector3.up) * Quaternion.AngleAxis(_pitch, Vector3.right);



    }

    void Movement()
    {
        var speed = Time.deltaTime * (Input.GetButton("Crounch") ? _sprintSpeed : _moveSpeed);
        var forward = speed * _movementInput.y;
        var right = speed * _movementInput.x;
        var up = speed * ((Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f));

        transform.position += transform.forward * forward + transform.right * right + Vector3.up * up;
    }
}
