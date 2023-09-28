using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayer : FPS_Creature
{

    [SerializeField, Min(0.01f)]
    private float _movSmoothLerp = 0.03f;
    private Vector3 _direction;

    [SerializeField]
    Transform _feet;
    private bool _canJump;
    private bool _inAir = true;
    [SerializeField]
    int _jumpPower = 7;

    private Rigidbody _myRigidBody;

    //camera stuff
    [SerializeField, Min(0.1f)]
    private float _sensitivity = 10F; //Mouse Sensitivity
    private float _rotationX = 0F;
    private float _rotationY = 0F;
    private float _rotArrayX;
    private float _rotAverageX = 0F;
    private float _rotArrayY;
    private float _rotAverageY = 0F;
    private Quaternion _cameraOriginalRotation;
    private Quaternion _bodyOriginalRotation;

    
    //Shooting
    [SerializeField]
    private Transform _cameraTransform;
    [SerializeField]
    Transform _shootingPoint;
    [SerializeField]
    GameObject _laserProjectilePrefab;
    private bool _canShoot = true;

    private float _currentRecoil = 0; //recoil starts at 0
    private float _maxRecoil = 0.5f; //Max recoil the gun can have
    private float _recoilIncreasePerShot = 0.1f; //recoil increases per 0.1 per shot
    private float _timeToStartStabilizing = 0.1f; //Time it takes after a shot is fired before the player starts stabilizing their weapon
    private bool _stabilize = true; //can the player start stabilizing their weapon after firing?
    private float _recoilStabilizationPerSecond = 0.5f; //Player stabelizer their weapon x value per second

    private bool _jump = false;
    private bool _firePrimary = false;
    private bool _fireSecondary = false;
    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _mouseInput = Vector2.zero;

    private void Awake()
    {
        _myRigidBody = GetComponent<Rigidbody>();


    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        

        _cameraOriginalRotation = _cameraTransform.localRotation;
        _bodyOriginalRotation = transform.localRotation;

        Cursor.lockState = CursorLockMode.Locked;

    }





    // Update is called once per frame
    void Update()
    {

        PlayerInput();
       

        CameraAndBodyRotation();


        Shoot();


    }

    private void FixedUpdate()
    {
        CheckForJumpClear();

        BodyMovement();


        ResetInputs();

        if (_currentRecoil > 0 && _stabilize)
        {
            _currentRecoil -= 0.5f * Time.deltaTime;
            if (_currentRecoil < 0) _currentRecoil = 0;
        }

    }

    private void LateUpdate()
    {
        _direction = Vector3.zero;



        
    }

    private void ResetInputs()
    {
        _jump = false;
        _firePrimary = false;
        _fireSecondary = false;
        _mouseInput = Vector2.zero;
    }

    //this function should always be in update
    //handles all inputs
    private void PlayerInput()
    {
        _mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        _movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (_canJump && Input.GetButton("Jump"))
        {
            _jump = true;
        }

        if (Input.GetButton("Fire1"))
        {
            _firePrimary = true;
        }
        else
        {
            _firePrimary = false;            
        }

        if (Input.GetButtonDown("Fire2"))
        {
            _fireSecondary = true;
        }

        
    }


    //shooting Function
    private void Shoot()
    {
        if (_canShoot && _firePrimary)
        {

            GameObject projectile = Instantiate(_laserProjectilePrefab, _shootingPoint.position, Quaternion.identity);
            FPS_LaserProjectile laser = projectile.GetComponent<FPS_LaserProjectile>();
            laser.Shooter = this;
            laser.EyePoint = _cameraTransform.position;
            laser.ShootingDirection = _cameraTransform.forward;
            laser.Recoil = _currentRecoil;
            laser.Spread = _myRigidBody.velocity.magnitude;

            _currentRecoil += _recoilIncreasePerShot;
            if (_currentRecoil > _maxRecoil) _currentRecoil = _maxRecoil;

            StartCoroutine(WeaponFireDelay(0.2f));

            StopCoroutine(WeaponStabilizingDelay(0));
            StartCoroutine(WeaponStabilizingDelay(_timeToStartStabilizing));

            //_firePrimary = false;
        }

        if (_fireSecondary)
        {
            //Nothing yet
        }


    }
    private IEnumerator WeaponFireDelay(float time)
    {
        _canShoot = false;

        yield return new WaitForSeconds(time);


        _canShoot = true;
    }
    private IEnumerator WeaponStabilizingDelay(float time)
    {
        
        _stabilize = false;

        yield return new WaitForSeconds(time);

        _stabilize = true;
    }

    //Player's Movement Input
    private void BodyMovement()
    {
        _movementInput = Vector2.ClampMagnitude(_movementInput, 1);

        //Jumping
        Vector3 jumpForce = Vector3.zero;

        if (_canJump && _jump)
        {
            _canJump = false;
            _jump = false;

            jumpForce = Vector3.up * _jumpPower;
            _myRigidBody.AddForce(jumpForce, ForceMode.VelocityChange);
        }

        //turn the camera's forward into a flat vector
        Vector3 cameraForward = _cameraTransform.forward; 
        cameraForward.y = 0;

        float dirY = _myRigidBody.velocity.y; //conserve y speed so that fall speed is always the same
        //create movement vectors
        Vector3 horizontalDir = _cameraTransform.right * _movementInput.x;
        Vector3 verticalDir =  cameraForward * _movementInput.y;

        _direction = (horizontalDir + verticalDir) * _movSpeed;

        _direction.y = dirY;

        _myRigidBody.velocity = _direction;

    }

    //Camera and Body rotation input
    private void CameraAndBodyRotation()
    {
        //Resets the average rotation
        _rotAverageY = 0f;
        _rotAverageX = 0f;

        //Gets rotational input from the mouse
        _rotationX += (_mouseInput.x * _sensitivity) * 100 * Time.deltaTime;
        _rotationY += (_mouseInput.y * _sensitivity) * 100 * Time.deltaTime;

        _rotationY = Mathf.Clamp(_rotationY, -90, 90);

        //Adds the rotation values to their relative array
        _rotArrayY = _rotationY;
        _rotArrayX = _rotationX;

        //Adding up all the rotational input values from each array
        _rotAverageY += _rotArrayY;
        _rotAverageX += _rotArrayX;

        //Get the rotation you will be at next as a Quaternion
        Quaternion yQuaternion = Quaternion.AngleAxis(_rotAverageY, Vector3.left);
        Quaternion xQuaternion = Quaternion.AngleAxis(_rotAverageX, Vector3.up);

        //Rotate
        _cameraTransform.localRotation = _cameraOriginalRotation * yQuaternion;
        transform.localRotation = _bodyOriginalRotation * xQuaternion;
    }

    private void CheckForJumpClear()
    {

            RaycastHit hit;
            if (Physics.Raycast(_feet.position, -_feet.up, out hit, 0.3f)) //Landing
            {
                _inAir = false;
                _canJump = true;
             Debug.DrawLine(_feet.position, _feet.position - Vector3.up * .3f, Color.green);
             }
            else 
            {
                _inAir = true;
                _canJump = false;

                Debug.DrawLine(_feet.position, _feet.position - Vector3.up * .3f, Color.red);
            }

    }



}
