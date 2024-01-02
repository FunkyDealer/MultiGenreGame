using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GTS_Player : GTS_Entity
{
    private bool _inControl = true;

    private bool _rotating = false;
    private bool _upright = true;
    private bool _foundfloor = false;

    private enum GroundStatus
    {
        isGrounded, //there is ground Right Bellow
        thereIsGround, //there is ground bellow, Try to connect with it;
        falling //falling
    }
    private GroundStatus _groundStatus = GroundStatus.falling;

    //private bool _isGrounded = false; 
    //private bool _thereIsGround = false; 

    bool _canRotate = true;

    [SerializeField, Min(0.1f)]
    private float _sensitivity = 10F; //Mouse Sensitivity
    [SerializeField, Min(0.01f)]
    private float _movSmoothLerp = 0.03f;
    private Vector3 _direction;
    private Vector3 _flatDirection;
    private Vector3 _fallDir; //direct to where to fall

    private float _rotationSpeed = 2;

    [SerializeField]
    private float _horizontalRotationSpeed = 30;
    [SerializeField]
    private float _verticalRotationSpeed = 10;

    [SerializeField]
    Transform _feet;

    bool _sprinting = false;
    [SerializeField]
    float _SprintSpeed = 20;
    [SerializeField]
    int _jumpPower = 7;

    private Rigidbody _myRigidBody;
    private BoxCollider _myCollider;

    private Vector2 _mouseInput = Vector2.zero;
    private Vector2 _movementInput = Vector2.zero;
    private Quaternion _bodyOriginalRotation;

    private float _lastGroundPos;
    private float _highestPositon;
    private bool falling = false;

    bool _paused = false;
    bool _canPause = true;

    private Vector3 groundNormal = Vector3.zero;

    [SerializeField]
    private Transform _lookAtTarget;
    public Transform LookAtTarget { get { return _lookAtTarget; } }

    [SerializeField, Range(0, 20)]
    private float _xOffset = 5;
    [SerializeField, Range(0, 20)]
    private float _yOffset = 5;
    [SerializeField, Range(0, 20)]
    private float _zOffset = 5;
    private Vector3 _offSet;
    public Vector3 OffSet { get { return _offSet; } }

    [SerializeField]
    private Transform front;
    private float _colliderSize;

    protected void Awake()
    {
        _myRigidBody = GetComponent<Rigidbody>();
        _myCollider = GetComponent<BoxCollider>();

        _offSet = new Vector3(_xOffset, _yOffset, -_zOffset);

        _colliderSize = Vector3.Distance(transform.position, front.position);
        _flatDirection = Vector3.zero;
        _direction = Vector3.zero;

        _fallDir = -Vector3.up;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _bodyOriginalRotation = transform.localRotation;

        Cursor.lockState = CursorLockMode.Locked;

        _mouseInput = Vector2.zero;
    }

    // Update is called once per frame
    protected void Update()
    {
        ResetScene();

        if (_alive && _inControl)
        {

            if (!_rotating)
            {
                PlayerInput();

                

                
            }

            MouseLook();

        }



       


    }

    protected void FixedUpdate()
    {
        if (!_rotating)
        {
            BodyMovement();

            CheckForIsGrounded();

            if (_groundStatus != GroundStatus.isGrounded) FindGroundBellowMe();

            FindFloor();

            CheckUpright();

            if (_groundStatus != GroundStatus.falling) FixRotation();
            if (_groundStatus == GroundStatus.falling || _upright) FixRotationForUpright();

        }

        ResetInputs();


        GTS_DebugUi.inst.DebugLine("GroundedStatus",$"grounded: {_groundStatus.ToString()}");

        GTS_DebugUi.inst.DebugLine("Sprint", $"Sprint: {_sprinting} ");

        GTS_DebugUi.inst.DebugLine("Speed", $"CurrentSpeed: {_currentSpeed.ToString()}");

        GTS_DebugUi.inst.DebugLine("rotating", $"Rotating: {_rotating}");

        GTS_DebugUi.inst.DebugLine("canRotate", $"can rotate?: {_canRotate}");

        GTS_DebugUi.inst.DebugLine("gravity", $"gravity: {_myRigidBody.useGravity}");



    }


    private void LateUpdate()
    {
        _direction = Vector3.zero;

        




    }

    private void ResetInputs()
    {
        _mouseInput = Vector2.zero;
        //_interact = false;
        //_jump = false;
        //_firePrimary = false;
        //_fireSecondary = false;
    }

    //this function should always be in update
    //handles all inputs
    private void PlayerInput()
    {
        //Use GetAxisRaw to get a more responsive input
        _mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        _movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetButtonDown("Crounch"))
        {
            _sprinting = true;
            _currentSpeed = _SprintSpeed;
        }
        if (Input.GetButtonUp("Crounch"))
        {
            _sprinting = false;
            _currentSpeed = _movSpeed;
        }
    }

    
    IEnumerator RotateDelay()
    {
        _canRotate = false;

        yield return new WaitForSeconds(0.5f);

        _canRotate = true;
    }

    IEnumerator RotatePlayerByPoint(Vector3 point, Vector3 rotationVector, float currentAngleDone, float totalAngle)
    {
        _myRigidBody.useGravity = false;

        _rotating = true;

        _upright = false;

        _groundStatus = GroundStatus.isGrounded;


        float ammount = 90 * _rotationSpeed * Time.deltaTime;
        currentAngleDone += ammount;

        if (currentAngleDone < totalAngle)
        {
            transform.RotateAround(point, rotationVector, ammount);

            yield return new WaitForFixedUpdate();

            StartCoroutine(RotatePlayerByPoint(point, rotationVector, currentAngleDone, totalAngle));
        }
        else
        {
            _myRigidBody.useGravity = true;
             _rotating = false;
        }
    }

    //Player's Movement Input
    private void BodyMovement()
    {
        _movementInput = Vector2.ClampMagnitude(_movementInput, 1);

        //if (!_isGrounded) _movementInput = Vector2.zero;

        Vector3 myForward = transform.forward;

        //conserve y speed so that fall speed is always the same
        Vector3 previousVelocity = _myRigidBody.velocity;

        //create movement vectors
        Vector3 horizontalDir = transform.right * _movementInput.x;
        Vector3 verticalDir = transform.forward * _movementInput.y;

        _direction = (horizontalDir + verticalDir);
        _flatDirection = _direction;
        //_direction = AdjustDirectionToGround(_direction);
        _direction *= _currentSpeed;        

        
        if (_upright) _direction.y = previousVelocity.y; //if i'm upright, preserve fall speed like normal
        else
        {    //if I'm not upright and I can find a wall bellow me, try to auto connect with it (this serves to correct the player's position after rotating
            if (_groundStatus == GroundStatus.thereIsGround || _groundStatus == GroundStatus.isGrounded) _direction += _fallDir * 3;

            //if i'm not upright and i'm falling, then preserve fall speed to go back to ground
            if (_groundStatus == GroundStatus.falling) _direction.y = previousVelocity.y;
        }
        

        //_direction = new Vector3(_direction.x, previousVelocity.y, _direction.z);

        _myRigidBody.velocity = _direction;

        if (_movementInput.magnitude > 0.2f && _canRotate) CheckForSurfaceChange();

    }

    private void CheckForSurfaceChange() //try to change into a wall in front of me
    {
        Vector3 direction = _flatDirection.normalized;

        direction = direction * _colliderSize;

        Vector3 place = _feet.position + direction + transform.up * 0.1f;
        direction.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(place, direction, out hit, 0.5f)) //There is a wall
        {
            Debug.DrawLine(place, place + direction * .5f, Color.green);

            RotateToWall(hit);
        }
        else
        {
            Debug.DrawLine(place, place + direction * .5f, Color.red);

            if (_groundStatus == GroundStatus.isGrounded && !_sprinting) CheckForFallingCorner();
        }
    }

    private void CheckForFallingCorner() //try to change into a wall bellow me
    {
        Vector3 CheckPos = transform.position - transform.up * 0.2f;

        Vector3 checkDirection = -_flatDirection.normalized;

        RaycastHit hit;
        if (Physics.Raycast(CheckPos, checkDirection, out hit, 0.5f)) //There is a wall
        {
            Debug.DrawLine(CheckPos, CheckPos + checkDirection * .3f, Color.green);
            RotateToWall(hit, false);

            FloatingTexTManager.inst.CreateText(hit.point, $"Found wall on this point {hit.point}", 0);
            Debug.Log($"Found wall on this point {hit.point}");            

            return;
        }     

    }

    private void RotateToWall(RaycastHit hit, bool bycenter = true)
    {
        Vector3 rotPoint = transform.position;
        if (bycenter) rotPoint = transform.TransformPoint(_myCollider.center);

        Vector3 rotationVector = Vector3.Cross(-hit.normal.normalized, transform.up);
        rotationVector.Normalize();

        //Debug.Log($"rotation vector: {rotationVector}");

        float angle = Vector3.Angle(transform.up.normalized, hit.normal.normalized);

        FloatingTexTManager.inst.CreateText(LookAtTarget.position, $"Angle: {angle}", 0);
        
        StartCoroutine(RotatePlayerByPoint(rotPoint, rotationVector, 0, angle));

        _myRigidBody.velocity = Vector3.zero;


        _groundStatus = GroundStatus.isGrounded;


        StartCoroutine(RotateDelay());
    }

    private void CheckForIsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(_feet.position, -transform.up, out hit, 0.5f)) //Landing
        {
            _groundStatus = GroundStatus.isGrounded;
            Debug.DrawLine(_feet.position, _feet.position - transform.up * .5f, Color.green);

            groundNormal = hit.normal;
            
            if (_upright) _myRigidBody.useGravity = true;
            else
            {
                _fallDir = -groundNormal;
                _myRigidBody.useGravity = false;
            } 

        }
        else
        {
            _groundStatus = GroundStatus.falling;
            Debug.DrawLine(_feet.position, _feet.position - transform.up * .5f, Color.red);

            _myRigidBody.useGravity = true;
            _fallDir = Vector3.zero;
        }

        
    }

    private void FindGroundBellowMe()
    {
        RaycastHit hit;
        if (Physics.Raycast(_feet.position, -transform.up, out hit, 5f)) //Found floor
        {
            if (_groundStatus != GroundStatus.isGrounded) _groundStatus = GroundStatus.thereIsGround;
            Debug.DrawLine(_feet.position, _feet.position - transform.up * 5f, Color.green);

            groundNormal = hit.normal;
            _fallDir = -groundNormal;
        }
        else
        {
            if (_groundStatus != GroundStatus.isGrounded) _groundStatus = GroundStatus.falling;

            Debug.DrawLine(_feet.position, _feet.position - transform.up * 5f, Color.red);

            _fallDir = Vector3.zero;
        }
    }

    private void FindFloor()
    {
        RaycastHit hit;
        if (Physics.Raycast(_feet.position, -Vector3.up, out hit, 5f)) //Found floor
        {
            Debug.DrawLine(_feet.position, _feet.position - Vector3.up * 5f, Color.green);
            _foundfloor = true;
        }
        else
        {
            Debug.DrawLine(_feet.position, _feet.position - Vector3.up * 5f, Color.red);
            _foundfloor = false;
        }
    }

    private void CheckUpright()
    {
        Vector3 myUp = transform.up;

        float angle = Vector3.Angle(myUp, Vector3.up);

        if (angle < 10) _upright = true;
        else _upright = false;

        GTS_DebugUi.inst.DebugLine( "Upright",$"Upright: {_upright}");

    }

    private void FixRotation()
    {

             float ammount = 1 * Time.deltaTime;

             //Vector3 tanget = GetTangent(groundNormal);

            Vector3 forward = Vector3.ProjectOnPlane(transform.forward.normalized, groundNormal.normalized);

             Quaternion rotation = Quaternion.LookRotation(forward, groundNormal);

            //transform.RotateAround(transform.position, transform.right, ammount);

            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, ammount);

            //float angleAgain = Vector3.Angle(transform.up, groundNormal);
    }

    private void FixRotationForUpright()
    {

        float ammount = 3 * Time.deltaTime;
        
        Vector3 v = new Vector3(transform.forward.x, 0, transform.forward.z);
        


        Quaternion rotation = Quaternion.LookRotation(v, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, ammount);


    }

    private void MouseLook()
    { 
        GTS_camera.inst.UpdateCameraPosition(_mouseInput, _sensitivity, _verticalRotationSpeed, transform, LookAtTarget);

        transform.rotation *= Quaternion.Euler(_mouseInput.x * _sensitivity * _horizontalRotationSpeed * Time.deltaTime * Vector3.up);        
    }

    void ResetScene()
    {
        if (Input.GetKeyDown(KeyCode.P)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    Vector3 GetTangent(Vector3 normal)
    {
        Vector3 tangent = Vector3.Cross(normal, Vector3.forward); ;

        Vector3 t2 = Vector3.Cross(normal, Vector3.up);

        if (tangent.magnitude > t2.magnitude)
        {
            return tangent;
        }
        else
        {
            return t2;
        }


    }
}
