using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayer : FPS_Creature
{
    private bool _inControl = true;

    [SerializeField, Min(0.01f)]
    private float _movSmoothLerp = 0.03f;
    private Vector3 _direction;

    [SerializeField]
    Transform _feet;
    private bool _isGrounded;
    private bool _crouch = false;
    [SerializeField]
    int _jumpPower = 7;

    private Rigidbody _myRigidBody;
    private CapsuleCollider _myCollider;

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
    private Transform _crouchPos;
    [SerializeField]
    private Transform _standUpPos;
    [SerializeField]
    private Transform _eyesTransform;

    [SerializeField]
    private Transform _weaponPos;
    private List<FPS_Weapon> _heldWeapons;

    private FPS_Weapon _currentlySelectedWeapon;
    public FPS_Weapon CurrentWeapon => _currentlySelectedWeapon;
    [SerializeField]
    GameObject _laserProjectilePrefab;
    private bool _canShoot = true;

    [SerializeField]
    private float _crouchSpeed = 10;

    private bool _tryingToGrabWall = false;
    private bool _grabingWall = false;

    private bool _interact = false;
    private bool _canInteract = true;
    private bool _crouchOrder = false;
    private bool _jump = false;
    private bool _firePrimary = false;
    private bool _fireSecondary = false;
    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _mouseInput = Vector2.zero;

    private List<FPS_Weapon>[] _slots;
    private int _currentlySelectedSlot = 0;
    private int _currentlySelectedLocalSlot = 1;

    public readonly int MaxBullets = 200;
    public readonly int MaxPellets = 50;
    public readonly int MaxLasers = 40;

    private int _currentBullets = 0;
    public int CurrentBullets => _currentBullets;

    private int _currentPellets = 0;
    public int CurrentPellets => _currentPellets;

    private int _currentLasers = 0;
    public int CurrentLasers => _currentLasers;

    [SerializeField] bool _invincible = false;

    public enum AMMOTYPE {
        BULLET,
        PELLET,
        LASER
     }

    private float _lastGroundPos;
    private float _highestPositon;
    private bool falling = false;

    [SerializeField]
    private GameObject _playerDeathObject;
    [SerializeField]
    private GameObject _DeathHUD;

    bool _paused = false;
    bool _canPause = true;
    [SerializeField]
    private GameObject _pauseMenu;

    private void Awake()
    {
        _myRigidBody = GetComponent<Rigidbody>();
        _myCollider = GetComponent<CapsuleCollider>();
        _heldWeapons = new List<FPS_Weapon>();

        _slots = new List<FPS_Weapon>[9];

        for (int i = 0; i < 9; i++)
        {
            _slots[i] = new List<FPS_Weapon>();
        }

        GameObject p = Instantiate(_pauseMenu);
        FPS_UIPauseController ui = p.GetComponent<FPS_UIPauseController>();
        ui.Player = this;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();      

        _cameraOriginalRotation = _cameraTransform.localRotation;
        _bodyOriginalRotation = transform.localRotation;

        Cursor.lockState = CursorLockMode.Locked;

        FPS_UIController.inst.UpdateArmour(_currentArmour);
        FPS_UIController.inst.UpdateHealth(_currentHealth);

        _lastGroundPos = this.transform.position.y;
        _highestPositon = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_alive && _inControl)
        {
            PlayerInput();


            //CameraAndBodyRotation();
            MouseLook();
        }


        Shoot();


    }

    private void FixedUpdate()
    {
        CheckForIsGrounded();

        JumpFromWall();

        BodyMovement();

        GrabOnToWall();

        Crouch();

        CheckInteract();
        Interact();

        ResetInputs();

        if (_isGrounded)
        {
            _lastGroundPos = transform.position.y;
            _highestPositon = 0;
        }
        else
        {
            if (transform.position.y > _highestPositon) _highestPositon = transform.position.y;

            
            float fallenDistance = _highestPositon - transform.position.y;
            if (fallenDistance > 4) falling = true;


        }

    }




    private void LateUpdate()
    {
        _direction = Vector3.zero;



        
    }

    private void ResetInputs()
    {
        _interact = false;
        _jump = false;
        _firePrimary = false;
        _fireSecondary = false;
        _mouseInput = Vector2.zero;
    }

    //this function should always be in update
    //handles all inputs
    private void PlayerInput()
    {
        if (_canPause && Input.GetButtonDown("Escape"))
        {
            PauseGame();
            return;
        }

        //Use GetAxisRaw to get a more responsive input
        _mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        _movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;      

        if (Input.GetButton("Interact"))
        {
            _interact = true;
        }

        if (Input.GetButtonDown("Jump"))
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

        if (!_firePrimary && !_fireSecondary && Input.inputString != "")
        {
            int number;
            bool isNumber = Int32.TryParse(Input.inputString, out number);
            if (isNumber && number > 0 && number < 10)
            {
                SelectSlot(number);
            }
        }

        if (!_isGrounded && Input.GetButtonDown("Crounch")) TryToGrabWall();

        if (Input.GetButton("Crounch")) _crouchOrder = true;
        else _crouchOrder = false;

        if (_grabingWall && Input.GetButtonUp("Crounch")) ReleaseWall();
    }

    private void SelectSlot(int slot)
    {
        int slotCount = _slots[slot - 1].Count;

        if (_currentlySelectedSlot != slot)
        {         
            if (slotCount > 0) //get weapon in the first local slot
            {
                _currentlySelectedSlot = slot;
                _currentlySelectedLocalSlot = 0;
                SelectWeaponFromSlot(slot, _currentlySelectedLocalSlot);
            }
            else
            {
                Debug.Log($"there are no weapons in slot {slot}");
            }
        }
        else
        {
            if (slotCount > _currentlySelectedLocalSlot + 2) //get the weapon in the next local slot
            {
                _currentlySelectedLocalSlot++;
                SelectWeaponFromSlot(slot, _currentlySelectedLocalSlot);
            }
             else if (slotCount == _currentlySelectedLocalSlot + 1) //get the first local slot again
            {
                if (_currentlySelectedLocalSlot == 0)
                {
                    Debug.Log("tried to selected the weapon that you are already holding");
                }
                else
                {
                    _currentlySelectedLocalSlot = 0;
                    SelectWeaponFromSlot(slot, _currentlySelectedLocalSlot);
                }
            }            
        }
    }

    private void SelectWeaponFromSlot(int weaponSlot, int currentlySelectedLocalSlot)
    {
        _currentlySelectedSlot = weaponSlot;
        _currentlySelectedLocalSlot = currentlySelectedLocalSlot;

        if (_currentlySelectedWeapon != null)
        {
            _currentlySelectedWeapon.SwitchOut();
            _currentlySelectedWeapon = null;            
        }
        //the local slot is already in array format (starts at 0)
        //the weapon slot is in normal format (starts at 1)
        _currentlySelectedWeapon = _slots[weaponSlot - 1][currentlySelectedLocalSlot];
        _currentlySelectedWeapon?.SwitchIn();

        //Debug.Log($"selecting weapon {currentlySelectedLocalSlot+1} from slot {weaponSlot}");

        UpdateAmmoCounter();
    }

    //shooting Function
    private void Shoot()
    {
        if (_currentlySelectedWeapon != null)
        {

            if (_currentlySelectedWeapon.CanShoot && _firePrimary)
            {
                _currentlySelectedWeapon.ShootPrimary(_cameraTransform);

            }

            if (_fireSecondary)
            {
                //Nothing yet
            }
        }
    }

    //Player's Movement Input
    private void BodyMovement()
    {
        _movementInput = Vector2.ClampMagnitude(_movementInput, 1);      

        //if (!_isGrounded) _movementInput = Vector2.zero;

        //Jumping
        Vector3 jumpForce = Vector3.zero;

        if (_isGrounded && _jump)
        {
            _isGrounded = false;
            _jump = false;

            // jumpForce = Vector3.up * _jumpPower;
            // jumpForce = sqrt(height * gravity * 2)
            float height = 3;
            jumpForce = Vector3.up * (float)Math.Sqrt(2 * Math.Abs(Physics.gravity.y) * height);
            //_myRigidBody.AddForce(jumpForce, ForceMode.Impulse);
            _myRigidBody.velocity += new Vector3(0, jumpForce.y, 0);
        }

        //turn the camera's forward into a flat vector
        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0;

        //conserve y speed so that fall speed is always the same
        Vector3 previousVelocity = _myRigidBody.velocity;

        //create movement vectors
        Vector3 horizontalDir = _cameraTransform.right * _movementInput.x;
        Vector3 verticalDir = cameraForward * _movementInput.y;

        _direction = (horizontalDir + verticalDir);      
        _direction = AdjustDirectionToGround(_direction);
        _direction *= _movSpeed;

        _direction.y += previousVelocity.y;        
        //_direction = new Vector3(_direction.x, previousVelocity.y, _direction.z);

        _myRigidBody.velocity = _direction;

    }

    private Vector3 groundNormal = Vector3.zero;

    private Vector3 AdjustDirectionToGround(Vector3 direction)
    {
        if (_isGrounded)
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
            Vector3 adjustedVelocity = slopeRotation * direction;

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }

        return direction;
    }

    private void Crouch()
    {
        if (_crouchOrder)
        {
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _crouchPos.position, _crouchSpeed * Time.deltaTime);

            _eyesTransform.position = Vector3.Lerp(_eyesTransform.position,new Vector3(_eyesTransform.position.x, _crouchPos.position.y, _eyesTransform.position.z), _crouchSpeed * Time.deltaTime);

            _myCollider.height = 1.2f;
            _myCollider.center = new Vector3(0, -0.4f, 0);

            _crouch = true;
        }
        else 
        {
            //check if player can uncrouch
            if (_crouch)
            {
                RaycastHit hit;
                if (Physics.Raycast(_cameraTransform.position, Vector3.up, out hit, 0.3f)) //raycast up
                {
                    //something is blocking
                    Debug.DrawLine(_cameraTransform.position, _cameraTransform.position + Vector3.up * .3f, Color.green);

                    _crouch = true;
                }
                else
                {
                    _crouch = false;
                }


            }
            else {
                
                _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _standUpPos.position, _crouchSpeed * Time.deltaTime);

                _eyesTransform.position = Vector3.Lerp(_eyesTransform.position, new Vector3(_eyesTransform.position.x, _standUpPos.position.y, _eyesTransform.position.z), _crouchSpeed * Time.deltaTime);

                _myCollider.height = 2f;
                _myCollider.center = Vector3.zero;
            }

        }
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

    private float _pitch = 0;

    private void MouseLook()
    {
        _pitch += _mouseInput.y * -_sensitivity * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, -90, 90);

        _cameraTransform.localRotation = Quaternion.Euler(Vector3.right * _pitch);

        transform.rotation *= Quaternion.Euler(_mouseInput.x * _sensitivity * Time.deltaTime * Vector3.up);


        //half life camera tilt effect
        Vector3 eulerRotation = _cameraTransform.localRotation.eulerAngles;
        if (_movementInput.x > 0)
        {
            eulerRotation.z = Mathf.Lerp(eulerRotation.z, -0.5f, Mathf.Abs(_movementInput.x));            
        }
        if (_movementInput.x < 0)
        {
            eulerRotation.z = Mathf.Lerp(eulerRotation.z, 0.5f, Mathf.Abs(_movementInput.x));
        }
        if (_movementInput.x == 0)
        {
            eulerRotation.z = 0;
        }
        _cameraTransform.localRotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, eulerRotation.z);
    }

    private void CheckForIsGrounded()
    {
            RaycastHit hit;
            if (Physics.Raycast(_feet.position, -_feet.up, out hit, 0.3f)) //Landing
            {
                _isGrounded = true;
             Debug.DrawLine(_feet.position, _feet.position - Vector3.up * .3f, Color.green);

                groundNormal = hit.normal;

                if (falling) FallDamage();
                ResetFalling();
             }
            else 
            {
                _isGrounded = false;
                Debug.DrawLine(_feet.position, _feet.position - Vector3.up * .3f, Color.red);
            }
    }

    private void FallDamage()
    {
        float fallenDistance = _highestPositon - transform.position.y;

        ReceiveDamage((int)fallenDistance * 2);       
    }

    private void GrabOnToWall()
    {
        if (_grabingWall)
        {
            _myRigidBody.velocity = Vector3.zero;      
        }
    }

    private void ReleaseWall()
    {
        _grabingWall = false;
        Debug.Log("released the wall");

        _myRigidBody.useGravity = true;
    }

    private void JumpFromWall()
    {
        if (_grabingWall && _jump)
        {
            ReleaseWall();

            _isGrounded = false;
            _jump = false;

            float power = 15;
            Vector3 jumpForce = Vector3.up * power;

            _myRigidBody.AddForce(jumpForce, ForceMode.Impulse);           
        }
    }
    private void TryToGrabWall()
    {
        Vector3 dir = _crouchPos.right;
        for (int i = 0; i < 8; i++)
        {
            RaycastHit hit;
            
            if (Physics.Raycast(_crouchPos.position, dir, out hit, 1.5f))
            {
                    FPS_Creature c = null;
                    c = hit.collider.gameObject.GetComponent<FPS_Creature>();

                    if (c == null)
                    {
                        _grabingWall = true;
                        _myRigidBody.useGravity = false;
                        _myRigidBody.velocity = Vector3.zero;

                        Debug.Log("grabbed a wall");

                        break;
                    }
            }
            //if (_grabingWall) Debug.DrawRay(_crouchPos.position, dir * 3, Color.red, 1f);
            //else Debug.DrawRay(_crouchPos.position, dir * 1.5f, Color.blue, 1f);

            dir = Quaternion.AngleAxis(45, _crouchPos.up) * dir;
        }
    }

    public void PickUpWeapon(GameObject weaponPrefab, int slot)
    {
        FPS_Weapon weapon = Instantiate(weaponPrefab, Vector3.zero, Quaternion.identity).GetComponent<FPS_Weapon>();
        weapon.gameObject.transform.SetParent(_weaponPos, true);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        _heldWeapons.Add(weapon);
        weapon.PickUpWeapon(this);
        _slots[slot - 1].Add(weapon);

        weapon.gameObject.SetActive(false);

        if (_currentlySelectedWeapon == null)
        {
            SelectWeaponFromSlot(slot, 0);
        }
        //_currentlySelectedWeapon = weapon;         

    }

    public override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }  

        FPS_UIController.inst.UpdateHealth(_currentHealth);

        Debug.Log("received a hit");
    }

    public override void ReceiveDamage(int damage, Vector3 contactPoint)
    {
        base.ReceiveDamage(damage, contactPoint);

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }

        FPS_UIController.inst.UpdateHealth(_currentHealth);

        Debug.Log("received a hit");
    }

    public override void ReceiveDamageFromHazard(int damage, float delay)
    {
        base.ReceiveDamageFromHazard(damage, delay);

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }

        FPS_UIController.inst.UpdateHealth(_currentHealth);

        StartCoroutine(ReloadHazardDamage(delay));
    }

    protected override void Die()
    {
        base.Die();
        _alive = false;
        _inControl = false;
        StopAllCoroutines();

        Cursor.lockState = CursorLockMode.None;

        SpawnDeathHead();
    }

    public Tuple<int, int> GetHealthStatus() => new Tuple<int, int>(_maxHealth, _currentHealth);

    public void ReceiveHealth(int ammount)
    {
        if (_currentHealth < _maxHealth)
        {
            _currentHealth += ammount;
            if (_currentHealth > _maxHealth) _currentHealth = _maxHealth;

            FPS_UIController.inst.UpdateHealth(_currentHealth);
        }
    }

    public void ReceiveBullets(int quantity)
    {
        _currentBullets += quantity;

        if (_currentBullets > MaxBullets) _currentBullets = MaxBullets;

        UpdateAmmoCounter();
    }

    public void ReceivePellets(int quantity)
    {
        _currentPellets += quantity;

        if (_currentPellets > MaxPellets) _currentPellets = MaxPellets;

        UpdateAmmoCounter();
    }

    public void ReceiveLasers(int quantity)
    {
        _currentLasers += quantity;

        if (_currentLasers > MaxLasers) _currentLasers = MaxLasers;

        UpdateAmmoCounter();
    }

    public bool SpendBullet()
    {
        if (_currentBullets <= 0) return false;

        _currentBullets--;
        FPS_UIController.inst.UpdateAmmoCounter(_currentBullets);

        return true;
    }

    public bool SpendPellet()
    {
        if (_currentPellets <= 0) return false;

        _currentPellets--;
        FPS_UIController.inst.UpdateAmmoCounter(_currentPellets);

        return true;
    }

    public bool SpendLaser()
    {
        if (_currentLasers <= 0) return false;

        _currentLasers--;
        FPS_UIController.inst.UpdateAmmoCounter(_currentLasers);

        return true;
    }

    private void UpdateAmmoCounter()
    {
        if (_currentlySelectedWeapon != null)
        {

            switch (_currentlySelectedWeapon.AmmoType)
            {
                case AMMOTYPE.BULLET:
                    FPS_UIController.inst.StartAmmoCounter(_currentBullets);
                    break;
                case AMMOTYPE.PELLET:
                    FPS_UIController.inst.StartAmmoCounter(_currentPellets);
                    break;
                case AMMOTYPE.LASER:
                    FPS_UIController.inst.StartAmmoCounter(_currentLasers);
                    break;
                default:
                    FPS_UIController.inst.StartAmmoCounter(_currentBullets);
                    break;
            }
        }
    }

    private void Interact()
    {
        if (_interact && _canInteract)
        {
            RaycastHit hit;
            if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hit, 3))
            {
                if (hit.collider != null)
                {
                    FPS_Interactible[] I = hit.collider.gameObject.GetComponents<FPS_Interactible>();

                    foreach (var i in I)
                    {
                        i.Interact(this);
                    }

                    StartCoroutine(IntereractDelay());
                }
            }

                _interact = false;
        }
    }
    private IEnumerator IntereractDelay()
    {
        _canInteract = false;
        yield return new WaitForSeconds(0.5f);

        _canInteract = true;
    }

    private void CheckInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out hit, 3))
        {
            if (hit.collider != null)
            {
                FPS_Interactible I = hit.collider.gameObject.GetComponent<FPS_Interactible>();

                if (I != null)
                {
                    FPS_UIController.inst.UpdatePromtDisplayer(I.Prompt);
                }
                else
                {
                    FPS_UIController.inst.UpdatePromtDisplayer(null);
                }

            }
            else
            {
                FPS_UIController.inst.UpdatePromtDisplayer(null);
            }
        }
        else
        {
            FPS_UIController.inst.UpdatePromtDisplayer(null);
        }
    }

    private void ResetFalling()
    {
        _highestPositon = 0;
        _lastGroundPos = transform.position.y;
        falling = false;
    }

    public void TeleportMeTo(Transform newPos)
    {
        this.transform.position = newPos.position;

        ResetFalling();
    }

    private void SpawnDeathHead()
    {
        GameObject g = Instantiate(_playerDeathObject, _cameraTransform.transform.position, Quaternion.identity);
        FPS_UIController.inst.DeactivateHUD();

        GameObject h = Instantiate(_DeathHUD);
        h.SetActive(true);

        this.gameObject.SetActive(false);
        
    }

    public void PauseGame()
    {
        _paused = !_paused;

        switch (_paused)
        {
            case true:
                _canPause = false;
                _inControl = false;
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                FPS_UIController.inst.DeactivateHUD();
                FPS_UIPauseController.inst.EnableMenu();
                break;
            case false:
                _inControl = true;
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                FPS_UIController.inst.ActivateHUD();
                StartCoroutine(PauseUnlock());
                break;
        }


    }

    private IEnumerator PauseUnlock()
    {
        yield return 0;

        _canPause = true;
    }

}
