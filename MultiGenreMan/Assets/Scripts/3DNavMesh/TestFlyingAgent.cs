using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFlyingAgent : MonoBehaviour
{

    private Vector2Int _horizontalInput = Vector2Int.zero;
    private int _verticalInput = 0;

    private Vector3Int _currentPosition = Vector3Int.zero;

    // Start is called before the first frame update
    void Start()
    {

        MoveToNewPos(_currentPosition);

    }

    // Update is called once per frame
    void Update()
    {

        PlayerInput();



    }

    private void FixedUpdate()
    {
        Movement();

        ResetInputs();
    }



    private void PlayerInput()
    {
        Vector2 v = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        v = Vector2.ClampMagnitude(v, 1);
        _horizontalInput = new Vector2Int((int)v.x, (int)v.y);

        if (Input.GetButton("Jump"))
        {
            _verticalInput = +1;
        }
        else if (Input.GetButton("Crounch"))
        {
            _verticalInput -=1;
        }


        //transform.position += new Vector3(v.x, _verticalInput, v.y);
    }

    private void Movement()
    {
       

        Vector3Int newTarget = _currentPosition + new Vector3Int(_horizontalInput.x, _verticalInput, _horizontalInput.y);

        MoveToNewPos(newTarget);

    }

    private void ResetInputs()
    {
        _horizontalInput = Vector2Int.zero;
        _verticalInput = 0;
    }

    private void MoveToNewPos(Vector3Int newGridPos)
    {
        bool valid = FlyNavMeshGrid.ValidateSpace3D(newGridPos);

        if (valid)
        {
            Vector3 newTarget = FlyNavMeshGrid.GetSpace3D(newGridPos);

            transform.position = newTarget;

            _currentPosition = newGridPos;
        }

    }

}
