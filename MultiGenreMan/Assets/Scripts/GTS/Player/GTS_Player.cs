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
        falling, //falling
        jumping //for jumping purposes
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

    bool _jump = false;
    [SerializeField]
    int _jumpPower = 7;
    Vector3 _jumpForce = Vector3.zero;

    private Rigidbody _myRigidBody;
    private BoxCollider _myCollider;

    private Vector2 _mouseInput = Vector2.zero;
    private Vector2 _movementInput = Vector2.zero;

    bool _paused = false;
    bool _canPause = true;

    private Vector3 groundNormal = Vector3.zero;

    [SerializeField]
    private Transform _lookAtTarget;
    private bool _fpsMode = false;
    [SerializeField] private Transform _FPSCameraTrans;

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

    [SerializeField]
    LayerMask _ignoreLayerMask;

    bool _cameraBlocked = false;

    //Weapons
    [SerializeField] Transform _weaponRotControl;
    [SerializeField] Transform _primaryArmRot;
    [SerializeField] Transform _secondaryArmRot;
    [SerializeField] Transform[] ShootingPos;
    private int _currentShootingPos = 0;
    private Vector3 _aimingPoint;
    private bool _firingPrimary = false;
    private bool _firingSecondary = false;

    private GTS_Weapon _currentlySelectedPrimary = null;
    private GTS_Weapon _currentlySelectedSecondary = null;

    [SerializeField]
    GTS_Weapon StartingPrimary;
    [SerializeField]
    GTS_Weapon StartingSecondary;

    private Dictionary<string,GTS_Weapon> _ownedWeapons;
    public Dictionary<string, GTS_Weapon> Weapons => _ownedWeapons;



    [SerializeField]
    private int _maxStoredBullets;
    public int _currentBullets { get; private set; } = 0;
    [SerializeField]
    private int _maxStoredRockets;
    public int _currentRockets { get; private set; } = 0;
    [SerializeField]
    private int _maxStoredGrenades;
    public int _currentGrenades { get; private set; } = 0;


    protected override void Awake()
    {
        base.Awake();

        _myRigidBody = GetComponent<Rigidbody>();
        _myCollider = GetComponent<BoxCollider>();

        _offSet = new Vector3(_xOffset, _yOffset, -_zOffset);

        _colliderSize = Vector3.Distance(transform.position, _front.position);
        _flatDirection = Vector3.zero;
        _direction = Vector3.zero;

        _fallDir = -Vector3.up;

        _ownedWeapons = new Dictionary< string, GTS_Weapon>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Cursor.lockState = CursorLockMode.Locked;

        _mouseInput = Vector2.zero;

        GTS_camera.inst.SetPosition(transform, _lookAtTarget);

        _aimingPoint = GTS_camera.inst.GetLookPoint();

        if (StartingPrimary != null)
        {
            _currentlySelectedPrimary = StartingPrimary;
            _currentlySelectedPrimary.PickUpWeapon(this);

            UpdatePrimaryAmmoUI();
        }

        if (StartingSecondary != null)
        {
            _currentlySelectedSecondary = StartingSecondary;
            _currentlySelectedSecondary.PickUpWeapon(this);
            UpdateSecondaryAmmoUI();
        }


        GTS_PlayerUI.inst.UpdateHealthTextDisplay(_currentHealth);
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
                BodyRotation();
            }

            _aimingPoint = GTS_camera.inst.GetLookPoint();
            AimWeapons();

            Shoot();

        }
    }

    protected void FixedUpdate()
    {
        if (_groundStatus != GroundStatus.jumping)
        {
            CheckForIsGrounded();

            if (_groundStatus != GroundStatus.isGrounded) FindGroundBellowMe();

        }
        FindFloor();

        CheckUpright();

        BodyMovement();

        if (_groundStatus != GroundStatus.falling) FixRotation();
        if (_groundStatus == GroundStatus.falling && !_upright) FixRotationForUpright();

        //Camera Fixed Update
        //_cameraBlocked = GTS_camera.inst.CheckIfBlocked(transform); //check if camera is being blocked
        bool tpsCamera = !_fpsMode;
        if (_cameraBlocked) tpsCamera = false;
        GTS_camera.inst.UpdateTPSCamera(tpsCamera); //update tps camera position, if in tps mode and not blocked, render it


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
        CameraUpdate();
    }



    //this function should always be in update
    //handles all inputs
    private void PlayerInput()
    {
        //Use GetAxisRaw to get a more responsive input
        _mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        _movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        _movementInput = Vector2.ClampMagnitude(_movementInput, 1);

        _firingPrimary = Input.GetButton("Fire1");
        _firingSecondary = Input.GetButton("Fire2");


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

        if (Input.GetButtonDown("Jump"))
        {
            _jump = true;
        }

        if (Input.GetButtonDown("ModeSelect"))
        {
            _fpsMode = !_fpsMode;
            GTS_camera.inst.ResetFPSCamera();
        }
    }

    private void ResetInputs()
    {
        _mouseInput = Vector2.zero;
        //_interact = false;
        _jump = false;

    }

    private void AimWeapons()
    {
        Vector3 myForward = _weaponRotControl.forward;
        Vector3 myPrimaryForward = _primaryArmRot.forward;
        Vector3 mySecondaryForward = _secondaryArmRot.forward;
        Vector3 myUp = _weaponRotControl.up;    

        if (!_fpsMode)
        {
            myForward = (_aimingPoint - _weaponRotControl.position).normalized;
            myPrimaryForward = (_aimingPoint - _primaryArmRot.position).normalized;
            mySecondaryForward = (_aimingPoint - _secondaryArmRot.position).normalized;
            myUp = Vector3.Cross(myForward, GTS_camera.inst.transform.right);
        }
        else
        {
            Vector3 lookPoint = GTS_camera.inst.transform.position + GTS_camera.inst.transform.forward * 1000;

            myForward = (lookPoint - _weaponRotControl.position).normalized;
            myPrimaryForward = (lookPoint - _primaryArmRot.position).normalized;
            mySecondaryForward = (lookPoint - _secondaryArmRot.position).normalized;
            myUp = Vector3.Cross(myForward, GTS_camera.inst.transform.right);
        }

        Quaternion newRot = Quaternion.LookRotation(myForward, myUp);
        Quaternion newPrimaryRot = Quaternion.LookRotation(myPrimaryForward, _primaryArmRot.up);
        Quaternion newSecondaryRot = Quaternion.LookRotation(mySecondaryForward, _secondaryArmRot.up);

        _weaponRotControl.rotation = newRot;
        _primaryArmRot.rotation = newPrimaryRot;
        _secondaryArmRot.rotation = newSecondaryRot;
    }

    private void Shoot()
    {
        if (_currentlySelectedPrimary != null)
        {
            if (_currentlySelectedPrimary.CanShoot && _firingPrimary)
            {
                _currentlySelectedPrimary.Shoot();

                UpdatePrimaryAmmoUI();
            }
        }

        if (_currentlySelectedSecondary != null) {
            if (_currentlySelectedSecondary.CanShoot && _firingSecondary)
            {

                _currentlySelectedSecondary.Shoot();

                UpdateSecondaryAmmoUI();
            }
        }

        _firingPrimary = false;
        _firingSecondary = false;
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

        if (_flatDirection.magnitude > 0f && !_rotating)
        {
            bool hole = CheckForFallingCorner();

            if (!hole) rotate = CheckForWallInFront();
            else rotate = true;
        }

        if (rotate) _direction = Vector3.zero;


        if (!rotate && _groundStatus == GroundStatus.isGrounded && _jump)
        {
            Jump();           
        }       

        //conserve y speed so that fall speed is always the same
        Vector3 previousVelocity = _myRigidBody.velocity;

        _direction *= _currentSpeed;

        if (_groundStatus != GroundStatus.jumping)
        {

            if (_groundStatus == GroundStatus.falling)
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
        }
        else
        {
            _direction.y = previousVelocity.y; //while jumping preserve y direction
        }

        //_direction = new Vector3(_direction.x, previousVelocity.y, _direction.z);

        _myRigidBody.velocity = _direction;

        
    }

    private void Jump()
    {
        _groundStatus = GroundStatus.jumping;

        // jumpForce = Vector3.up * _jumpPower;
        // jumpForce = sqrt(height * gravity * 2)
        float height = 8;
        if (_upright) _jumpForce = Vector3.up * (float)Math.Sqrt(2 * Math.Abs(Physics.gravity.y) * height);
        else
        {
            _jumpForce = Vector3.zero;
            StartCoroutine(MoveMeInDirection(0, transform.up, _SprintSpeed));
        }

        if (!_upright) RotateToUpright();

        //_myRigidBody.AddForce(_jumpForce, ForceMode.Impulse);
        _myRigidBody.velocity += _jumpForce;

        _jump = false;
        JumpDelay();
    }

    private void StopJumpCoroutines()
    {
        StopCoroutine(MoveMeInDirection(0, transform.up, 5));
        StopCoroutine(RotateTo(Quaternion.identity));

        _rotating = false;
        _myRigidBody.useGravity = true;
    }

    private void RotateToUpright()
    {
        Quaternion targetRotation;

        Vector3 targetUp = Vector3.up;
        Vector3 targetForward = transform.forward;
        // targetForward = new Vector3(targetForward.x, 0, targetForward.z);
        targetForward = Vector3.ProjectOnPlane(targetForward, Vector3.up);

        targetRotation = Quaternion.LookRotation(targetForward, targetUp);


        StartCoroutine(RotateTo(targetRotation));
    }

    private IEnumerator MoveMeInDirection(float time, Vector3 direction, float speed)
    {
        time += Time.deltaTime;

        //FloatingTexTManager.inst.CreateText(_lookAtTarget.position, "moving", 0);

        transform.position += direction * speed * Time.deltaTime;

        if (time < 0.2f) {
            yield return new WaitForFixedUpdate();
            StartCoroutine(MoveMeInDirection(time, direction, speed));
        }
        else yield break;
    }

    private bool CheckForWallInFront() //Check if there is something blocking us in the direction we are going
    {
        if (!_canRotateUp) return false;
        if (_groundStatus == GroundStatus.jumping) return false;
        //if (_groundStatus == GroundStatus.falling) return false;

        Vector3 direction = _flatDirection.normalized;
        direction = direction * _colliderSize;

        float extraHeight = _distanceToground / 3f;
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
        if (Physics.Raycast(place, direction, out hit, rayDistance, ~_ignoreLayerMask)) //There is something blocking us
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
        if (_groundStatus == GroundStatus.jumping) return false;
        if (_distanceToground > 1) return false;

        //cast 2 rays
        Vector3 myCenter = transform.TransformPoint(_myCollider.center);

        Vector3 groundCheckTarget = transform.position + (_flatDirection.normalized * 0.55f);
        Vector3 groundCheckDir = groundCheckTarget - myCenter;

        float distance = 1.5f;

        RaycastHit groundCheck;
        if (Physics.Raycast(myCenter, groundCheckDir, out groundCheck, distance, ~_ignoreLayerMask))
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
            if (Physics.Raycast(groundCheckTarget, newDir, out wallCheck, newDistance, ~_ignoreLayerMask))
            {
                Debug.DrawLine(groundCheckTarget, groundCheckTarget + newDir * newDistance, Color.green);

                RaycastHit SecondGroundCheck; //check if there is ground in front
                float groundCheckDistance = 1f;
                Vector3 direction = _flatDirection;
                if (Physics.Raycast(wallCheck.point, direction, out SecondGroundCheck, 1, ~_ignoreLayerMask))
                {
                    Debug.DrawLine(wallCheck.point, wallCheck.point + direction * groundCheckDistance, Color.green);
                    StopRotationCoroutines();
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
        StopJumpCoroutines();
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

    private IEnumerator RotateTo(Quaternion targetRotation)
    {
        _myRigidBody.useGravity = false;

        _rotating = true;

        float speed = 6;

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, speed * Time.deltaTime);

        CheckUpright();

        if (!_upright)
        {
            yield return new WaitForFixedUpdate();
            StartCoroutine(RotateTo(targetRotation));
        }

        else
        {
            _myRigidBody.useGravity = true;

            _rotating = false;
        }

    }

    void RotatePlayerByInput(Vector3 pivot, Vector3 rotationVector, bool up)
    {
        //_myRigidBody.useGravity = false;
        // _rotating = true;
        _upright = false;

        float Up = -1;
        if (up) Up = 1;

        //_groundStatus = GroundStatus.isGrounded;

        float ammount = 90 * _rotationSpeed * Time.deltaTime;

        transform.RotateAround(pivot, rotationVector, ammount * Up);
    }

    void RotateDownDelay()
    {
        StopRotationCoroutines();

        StartCoroutine(RotateDownCoroutine());
    }

    IEnumerator RotateDownCoroutine()
    {
        _canRotateDown = false;

        yield return new WaitForSeconds(_rotateDirChangeDelay);

        _canRotateDown = true;
    }

    void RotateUpDelay()
    {
        StopRotationCoroutines();

        StartCoroutine(RotateUpCoroutine());
    }

    IEnumerator RotateUpCoroutine()
    {
        _canRotateUp = false;

        yield return new WaitForSeconds(_rotateDirChangeDelay);

        _canRotateUp = true;
    }

    private void StopRotationCoroutines()
    {
        StopCoroutine(RotateUpCoroutine());
        StopCoroutine(RotateDownCoroutine());
    }

    private void CheckForIsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(_feet.position, -transform.up, out hit, 0.5f, ~_ignoreLayerMask)) //Landing
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
        if (Physics.Raycast(_feet.position, -transform.up, out hit, 1.3f, ~_ignoreLayerMask)) //Found floor
        {
            if (_groundStatus != GroundStatus.isGrounded) _groundStatus = GroundStatus.thereIsGround;
            Debug.DrawLine(_feet.position, _feet.position - transform.up * 1.3f, Color.green);

            _distanceToground = Vector3.Distance(transform.position, hit.point);

            groundNormal = hit.normal;
            _fallDir = -groundNormal;
        }
        else
        {
            if (_groundStatus != GroundStatus.isGrounded) _groundStatus = GroundStatus.falling;

            Debug.DrawLine(_feet.position, _feet.position - transform.up * 1.3f, Color.red);

            _fallDir = Vector3.zero;
        }
    }

    private void FindFloor()
    {
        RaycastHit hit;
        if (Physics.Raycast(_feet.position, -Vector3.up, out hit, 5f, ~_ignoreLayerMask)) //Found floor
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

    private void JumpDelay()
    {
        StartCoroutine(JumpDelayCoroutine());
    }

    private IEnumerator JumpDelayCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        _groundStatus = GroundStatus.falling;
    }

    private void BodyRotation()
    {
        transform.rotation *= Quaternion.Euler(_mouseInput.x * _sensitivity * _horizontalRotationSpeed * Time.deltaTime * Vector3.up);
    }

    private void CameraUpdate()
    {
        GTS_camera.inst.UpdateCameraValues(_mouseInput, _sensitivity, _verticalRotationSpeed, transform, _lookAtTarget);

        if (_fpsMode || (!_fpsMode && _cameraBlocked)) GTS_camera.inst.UpdateFPSCamera(transform, _FPSCameraTrans);
    }

    public override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);

        _currentHealth -= damage;

        if (_currentHealth < 0)
        {
            _currentHealth = 0;
            Die();
        }
        GTS_PlayerUI.inst.UpdateHealthTextDisplay(_currentHealth);
    }

    public override void ReceiveDamage(int damage, Vector3 contactPoint)
    {
        base.ReceiveDamage(damage, contactPoint);

        ReceiveDamage(damage);
    }

    protected override void Die()
    {
        base.Die();

        Debug.Log("You Died");

        _inControl = false;
        _alive = false;

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

    public bool AddHealth(int ammount)
    {
        if (_currentHealth >= _maxHealth) return false;

        _currentHealth += ammount;
        if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;
        GTS_PlayerUI.inst.UpdateHealthTextDisplay(_currentHealth);
        return true;
    }

    public void PickUpWeapon(GTS_Weapon weapon)
    {
        if (!_ownedWeapons.ContainsKey(weapon.ID))
        {
            _ownedWeapons.Add(weapon.ID, weapon);
        }

        switch (weapon.AmmoType)
        {
            case GTS_Weapon.AMMOTYPE.Bullets:
                AddBullets(weapon.AmmoOnPickup);
                break;
            case GTS_Weapon.AMMOTYPE.Rockets:
                AddRockets(weapon.AmmoOnPickup);
                break;
            case GTS_Weapon.AMMOTYPE.Grenades:
                AddGrenades(weapon.AmmoOnPickup);
                break;
            default:
                break;
        }

    }

    public bool AddAmmo(GTS_Weapon.AMMOTYPE type, int ammount)
    {
        switch (type)
        {
            case GTS_Weapon.AMMOTYPE.Bullets:
                return AddBullets(ammount);
            case GTS_Weapon.AMMOTYPE.Rockets:
                return AddRockets(ammount);
            case GTS_Weapon.AMMOTYPE.Grenades:
                return AddGrenades(ammount);
        }

        return false;
    }

    public bool AddBullets(int ammount)
    {
        if (_currentBullets >= _maxStoredBullets) return false;

        _currentBullets += ammount;

        if (_currentBullets >= _maxStoredBullets) _currentBullets = _maxStoredBullets;

        UpdateAmmoUI();
        return true;
    }

    public bool AddRockets(int ammount)
    {
        if (_currentRockets >= _maxStoredRockets) return false;

        _currentRockets += ammount;

        if (_currentRockets >= _maxStoredRockets) _currentRockets = _maxStoredRockets;
        UpdateAmmoUI();
        return true;
    }

    public bool AddGrenades(int ammount)
    {
        if (_currentGrenades >= _maxStoredGrenades) return false;

        _currentGrenades += ammount;

        if (_currentGrenades >= _maxStoredGrenades) _currentGrenades = _maxStoredGrenades;
        UpdateAmmoUI();
        return true;
    }

    private void UpdateAmmoUI()
    {
       if (_currentlySelectedPrimary != null) UpdatePrimaryAmmoUI();
       if (_currentlySelectedSecondary != null) UpdateSecondaryAmmoUI();
    }
    
    private void UpdatePrimaryAmmoUI()
    {
        int ammo = _currentBullets;

        switch (_currentlySelectedPrimary.AmmoType)
        {
            case GTS_Weapon.AMMOTYPE.Bullets:
                break;
            case GTS_Weapon.AMMOTYPE.Rockets:
                ammo = _currentRockets;
                break;
            case GTS_Weapon.AMMOTYPE.Grenades:
                ammo = _currentGrenades;
                break;
            default:
                break;
        }

        GTS_PlayerUI.inst.UpdatePrimaryAmmoDisplay(ammo);
    }

    private void UpdateSecondaryAmmoUI()
    {
        int ammo = _currentBullets;

        switch (_currentlySelectedSecondary.AmmoType)
        {
            case GTS_Weapon.AMMOTYPE.Bullets:
                break;
            case GTS_Weapon.AMMOTYPE.Rockets:
                ammo = _currentRockets;
                break;
            case GTS_Weapon.AMMOTYPE.Grenades:
                ammo = _currentGrenades;
                break;
            default:
                break;
        }
        GTS_PlayerUI.inst.UpdateSecondaryAmmoDisplay(ammo);
    }

    public override bool SpendAmmo(GTS_Weapon.AMMOTYPE type, int ammount)
    {        
        switch (type)
        {
            case GTS_Weapon.AMMOTYPE.Bullets:
                if (_currentBullets >= ammount)
                {
                    _currentBullets -= ammount;
                    return true;
                } 
                break;
            case GTS_Weapon.AMMOTYPE.Rockets:
                if (_currentRockets >= ammount)
                {
                    _currentRockets -= ammount;
                    return true;
                }

                break;
                case GTS_Weapon.AMMOTYPE.Grenades:
                if (_currentGrenades >= ammount)
                {
                    _currentGrenades -= ammount;
                    return true;
                }
                break;
            default:
                return false;
        }
        return base.SpendAmmo(type, ammount);
    }
}
