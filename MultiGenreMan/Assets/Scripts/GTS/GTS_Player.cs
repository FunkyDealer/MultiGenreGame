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

    float _distanceToground = 0;
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

    [SerializeField]
    private float _rotationSpeed = 4;

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

    [SerializeField, Range(0, 20)]
    private float _xOffset = 5;
    [SerializeField, Range(0, 20)]
    private float _yOffset = 5;
    [SerializeField, Range(0, 20)]
    private float _zOffset = 5;
    private Vector3 _offSet;
    public Vector3 OffSet { get { return _offSet; } }

    [SerializeField]
    private Transform _front;
    private float _colliderSize;

    private bool _canRotateUp = true;
    private bool _canRotateDown = true;
    [SerializeField]
    private float _rotateDirChangeDelay = 0.3f;

    [SerializeField]
    GameObject _topCamera;
    [SerializeField]
    GameObject _rightCamera;
    [SerializeField]
    GameObject _leftCamera;


    protected void Awake()
    {
        _myRigidBody = GetComponent<Rigidbody>();
        _myCollider = GetComponent<BoxCollider>();

        _offSet = new Vector3(_xOffset, _yOffset, -_zOffset);

        _colliderSize = Vector3.Distance(transform.position, _front.position);
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

        GTS_camera.inst.SetPosition(transform, _lookAtTarget);
    }

    // Update is called once per frame
    protected void Update()
    {
        ResetScene();

        Pause();

        SetCamera();

        SetTime();

        if (_alive && _inControl)
        {

            if (!_rotating)
            {
                PlayerInput();


            }

            MouseLook();



        }






    }

    float currentTimeScale = 1;

    private void SetTime()
    {
        if (!_inControl) return;
        currentTimeScale += Input.mouseScrollDelta.y * Time.deltaTime;

        if (currentTimeScale > 1) currentTimeScale = 1;
        if (currentTimeScale < 0) currentTimeScale = 0;

        Time.timeScale = currentTimeScale;
    }

    private void SetCamera()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1)) //main
        {
            _leftCamera.SetActive(false);
            _rightCamera.SetActive(false);
            _topCamera.SetActive(false);
            GTS_camera.inst.SwitchOn();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) //top
        {
            GTS_camera.inst.SwitchOff();
            _leftCamera.SetActive(false);
            _rightCamera.SetActive(false);
            _topCamera.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) //left
        {
            GTS_camera.inst.SwitchOff();
            _rightCamera.SetActive(false);
            _topCamera.SetActive(false);
            _leftCamera.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) //right
        {
            GTS_camera.inst.SwitchOff();
            _leftCamera.SetActive(false);
            _topCamera.SetActive(false);
            _rightCamera.SetActive(true);
        }



    }

    private void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_inControl) Time.timeScale = 0;
            else Time.timeScale = 1;

            _inControl = !_inControl;


        }
    }

    protected void FixedUpdate()
    {

        CheckForIsGrounded();

        if (_groundStatus != GroundStatus.isGrounded) FindGroundBellowMe();

        FindFloor();

        CheckUpright();

        BodyMovement();

        if (_groundStatus != GroundStatus.falling) FixRotation();
        if (_groundStatus == GroundStatus.falling || _upright) FixRotationForUpright();



        ResetInputs();


        GTS_DebugUi.inst.DebugLine("GroundedStatus", $"grounded: {_groundStatus.ToString()}");

        GTS_DebugUi.inst.DebugLine("Sprint", $"Sprint: {_sprinting} ");

        GTS_DebugUi.inst.DebugLine("Speed", $"CurrentSpeed: {_currentSpeed.ToString()}");

        GTS_DebugUi.inst.DebugLine("rotating", $"Rotating: {_rotating}");

        GTS_DebugUi.inst.DebugLine("canRotate", $"can rotate?: {_canRotate}");

        GTS_DebugUi.inst.DebugLine("gravity", $"gravity: {_myRigidBody.useGravity}");



    }


    private void LateUpdate()
    {
        //_direction = Vector3.zero;


        CameraUpdate();



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
        _movementInput = Vector2.ClampMagnitude(_movementInput, 1);

        


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

    void RotateDownDelay()
    {
        StopAllCoroutines();

        StartCoroutine(RotateDownCoroutine_());
    }

    IEnumerator RotateDownCoroutine_()
    {

        _canRotateDown = false;

        yield return new WaitForSeconds(_rotateDirChangeDelay);

        _canRotateDown = true;
    }

    void RotateUpDelay()
    {
        StopAllCoroutines();

        StartCoroutine(RotateUpCoroutine());
    }

    IEnumerator RotateUpCoroutine()
    {
        _canRotateUp = false;

        yield return new WaitForSeconds(_rotateDirChangeDelay);

        _canRotateUp = true;
    }

    //Player's Movement Input
    private void BodyMovement()
    {

        //if (!_isGrounded) _movementInput = Vector2.zero;
        //create movement vectors
        Vector3 horizontalDir = transform.right * _movementInput.x;
        Vector3 verticalDir = transform.forward * _movementInput.y;

        _direction = (horizontalDir + verticalDir);
        _flatDirection = _direction.normalized;

        bool rotate = false;

        if (_flatDirection.magnitude > 0f)
        {
            bool hole = CheckForFallingCorner();

            if (!hole) rotate = CheckForWallInFront();
            else rotate = true;
        }

        if (rotate) _direction = Vector3.zero;

        //conserve y speed so that fall speed is always the same
        Vector3 previousVelocity = _myRigidBody.velocity;

        _direction *= _currentSpeed;

        if (_groundStatus != GroundStatus.isGrounded)
        {
            _direction = Vector3.ClampMagnitude(_direction, _direction.magnitude / 2);
        }
        else
        {
           
        }

        if (_upright) _direction.y = previousVelocity.y; //if i'm upright, preserve fall speed like normal
        else
        {    //if I'm not upright and I can find a wall bellow me, try to auto connect with it (this serves to correct the player's position after rotating
            if (_groundStatus == GroundStatus.thereIsGround) _direction += _fallDir * 5;
            if (_groundStatus == GroundStatus.isGrounded) _direction += _fallDir;

            //if i'm not upright and i'm falling, then preserve fall speed to go back to ground
            if (_groundStatus == GroundStatus.falling) _direction.y = previousVelocity.y;
        }

        //_direction = new Vector3(_direction.x, previousVelocity.y, _direction.z);

        _myRigidBody.velocity = _direction;
    }

    private bool CheckForWallInFront() //Check if there is something blocking us in the direction we are going
    {
        if (!_canRotateUp) return false;
        //if (_groundStatus == GroundStatus.falling) return false;

        Vector3 direction = _flatDirection.normalized;
        direction = direction * _colliderSize;

        float extraHeight = _distanceToground / 4f;
        float lessRange = _distanceToground / 3;
        extraHeight = Mathf.Clamp(extraHeight, 0, 0.5f);
        lessRange = Mathf.Clamp(lessRange, 0, 1f);

        Vector3 place = (transform.position + transform.up * 0.1f) + direction - (transform.up * extraHeight) - (direction.normalized * lessRange);
        //direction.Normalize();

        GTS_DebugUi.inst.DebugLine("distanceToGround", $"distanceToGround: {_distanceToground}");

        //Vector3 pivot = transform.position - direction;
        Vector3 pivot = transform.TransformPoint(_myCollider.center);

        float rayDistance = Mathf.Clamp(_distanceToground * 2 + 0.5f, 0, 2);

        RaycastHit hit;
        if (Physics.Raycast(place, direction, out hit, rayDistance)) //There is something blocking us
        {
            FloatingTexTManager.inst.CreateText(_lookAtTarget.position, $"the is a wall", 0);
            Debug.DrawLine(place, place + direction * rayDistance, Color.green);

            RotateToWall(hit, pivot, true); //rotate into it
            RotateDownDelay();
            return true;
        }
        else
        {
            Debug.DrawLine(place, place + direction * .5f, Color.red);

            //if (_groundStatus == GroundStatus.isGrounded && !_sprinting) return CheckForFallingCorner();
            //else return false;            
        }

        return false;
    }

    private bool CheckForFallingCorner() //try to change into a wall bellow me
    {
        if (!_canRotateDown) return false;
        if (_groundStatus == GroundStatus.falling) return false; 
        //cast 2 rays

        Vector3 myCenter = transform.TransformPoint(_myCollider.center);

        Vector3 groundCheckTarget = transform.position + (_flatDirection.normalized * 0.55f);
        Vector3 groundCheckDir = groundCheckTarget - myCenter;

        float distance = 1.5f;

        RaycastHit groundCheck;
        if (Physics.Raycast(myCenter, groundCheckDir, out groundCheck, distance))
        {
            Debug.DrawLine(myCenter, myCenter + groundCheckDir * distance, Color.green);


        }
        else
        {
            Debug.DrawLine(myCenter, myCenter + groundCheckDir * distance, Color.red);

            Vector3 newTarget = transform.position - transform.up * 0.5f;
            Vector3 newDir = newTarget - groundCheckTarget;


            float newDistance = 1f;
            RaycastHit wallCheck;
            if (Physics.Raycast(groundCheckTarget, newDir, out wallCheck, newDistance))
            {
                Debug.DrawLine(groundCheckTarget, groundCheckTarget + newDir * newDistance, Color.green);

                RaycastHit SecondGroundCheck; //check if there is ground in front
                float groundCheckDistance = 1f;
                Vector3 direction = _flatDirection;
                if (Physics.Raycast(wallCheck.point, direction, out SecondGroundCheck, 1))
                {
                    Debug.DrawLine(wallCheck.point, wallCheck.point + direction * groundCheckDistance, Color.green);
                    StopAllCoroutines();
                    _canRotateUp = true;
                    return false;
                }
                else
                {
                    Debug.DrawLine(wallCheck.point, wallCheck.point + direction * groundCheckDistance, Color.red);

                    Vector3 vector = (transform.position + _flatDirection) - transform.position;
                    Vector3 vector2 = wallCheck.point - transform.position;

                    float angle = Vector3.Angle(vector, vector2);

                    vector2 = Quaternion.AngleAxis(angle, Vector3.Cross(_flatDirection, transform.up)) * vector2;

                    //Vector3 pivot = Vector3.Project(wallCheck.point, vector);
                    Vector3 pivot = transform.position + vector2;

                    //Debug.Log($"Pivot: {pivot}");
                    //pivot += transform.position;
                   // Debug.Log($"PivotAfter: {pivot}");

                    Debug.DrawLine(transform.position, transform.position + vector.normalized, Color.white, 1, false);
                    Debug.DrawLine(pivot, pivot + vector.normalized, Color.yellow, 5, false);
                    //Debug.Break();

                    //we found it! rotate!
                    FloatingTexTManager.inst.CreateText(transform.position, $"the is a hole", 0);
                    RotateToWall(wallCheck, wallCheck.point, false);

                    RotateUpDelay();
                    return true;
                }


            }
            else
            {
                Debug.DrawLine(groundCheckTarget, groundCheckTarget + newDir * newDistance, Color.red);
            }
        }



        return false;



    }



    private void RotateToWall(RaycastHit hit, Vector3 pivot, bool up)
    {
        //Vector3 rotationVector = Vector3.Cross(-hit.normal.normalized, transform.up);
        //rotationVector.Normalize();

        Vector3 rotationVector = Vector3.Cross(_flatDirection, transform.up);
        rotationVector.Normalize();

        //Debug.Log($"rotation vector: {rotationVector}");

        //float angle = Vector3.Angle(transform.up.normalized, hit.normal.normalized);
        //StartCoroutine(RotatePlayerByPoint(rotPoint, rotationVector, 0, angle));

        //rotate with player input
        RotatePlayerByInput(pivot, rotationVector, up);

        // _groundStatus = GroundStatus.isGrounded;
        //StartCoroutine(RotateDelay());
    }

    IEnumerator RotatePlayerByPoint(Vector3 point, Vector3 rotationVector, float currentAngleDone, float totalAngle)
    {
        _myRigidBody.useGravity = false;

        _rotating = true;

        _upright = false;

        _groundStatus = GroundStatus.isGrounded;


        float ammount = 25;
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

    void RotatePlayerByInput(Vector3 pivot, Vector3 rotationVector, bool up)
    {
        _myRigidBody.useGravity = false;
        // _rotating = true;
        _upright = false;

        float Up = -1;
        if (up) Up = 1;

        //_groundStatus = GroundStatus.isGrounded;

        float ammount = 90 * _rotationSpeed * Time.deltaTime;

        transform.RotateAround(pivot, rotationVector, ammount * Up);
    }

    private void CheckForIsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(_feet.position, -transform.up, out hit, 0.5f)) //Landing
        {
            _groundStatus = GroundStatus.isGrounded;
            Debug.DrawLine(_feet.position, _feet.position - transform.up * .5f, Color.green);

            _distanceToground = Vector3.Distance(transform.position, hit.point);

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
        if (Physics.Raycast(_feet.position, -transform.up, out hit, 2f)) //Found floor
        {
            if (_groundStatus != GroundStatus.isGrounded) _groundStatus = GroundStatus.thereIsGround;
            Debug.DrawLine(_feet.position, _feet.position - transform.up * 5f, Color.green);

            _distanceToground = Vector3.Distance(transform.position, hit.point);

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
            //Debug.DrawLine(_feet.position, _feet.position - Vector3.up * 5f, Color.white);
            _foundfloor = true;
        }
        else
        {
            //Debug.DrawLine(_feet.position, _feet.position - Vector3.up * 5f, Color.red);
            _foundfloor = false;
        }
    }

    private void CheckUpright()
    {
        Vector3 myUp = transform.up;

        float angle = Vector3.Angle(myUp, Vector3.up);

        if (angle < 10) _upright = true;
        else _upright = false;

        GTS_DebugUi.inst.DebugLine("Upright", $"Upright: {_upright}");

    }

    private void FixRotation()
    {
        if (_distanceToground < 2)
        {


            float ammount = 1 * Time.deltaTime;

            //Vector3 tanget = GetTangent(groundNormal);

            Vector3 forward = Vector3.ProjectOnPlane(transform.forward.normalized, groundNormal.normalized);

            Quaternion rotation = Quaternion.LookRotation(forward, groundNormal);

            //transform.RotateAround(transform.position, transform.right, ammount);

            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, ammount);

            //float angleAgain = Vector3.Angle(transform.up, groundNormal);
        }
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

        transform.rotation *= Quaternion.Euler(_mouseInput.x * _sensitivity * _horizontalRotationSpeed * Time.deltaTime * Vector3.up);

    }

    private void CameraUpdate()
    {
        GTS_camera.inst.UpdateCameraPosition(_mouseInput, _sensitivity, _verticalRotationSpeed, transform, _lookAtTarget);
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
