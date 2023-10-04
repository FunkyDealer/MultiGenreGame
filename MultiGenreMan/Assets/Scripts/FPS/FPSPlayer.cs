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
    private bool _isGrounded;
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
    private Transform _weaponPos;
    private List<FPS_Weapon> _heldWeapons;
    

    private FPS_Weapon _currentlySelectedWeapon;
    [SerializeField]
    GameObject _laserProjectilePrefab;
    private bool _canShoot = true;

    

    private bool _jump = false;
    private bool _firePrimary = false;
    private bool _fireSecondary = false;
    private Vector2 _movementInput = Vector2.zero;
    private Vector2 _mouseInput = Vector2.zero;

    private List<FPS_Weapon>[] _slots;
    private int _currentlySelectedSlot = 0;
    private int _currentlySelectedLocalSlot = 1;

    private void Awake()
    {
        _myRigidBody = GetComponent<Rigidbody>();
        _heldWeapons = new List<FPS_Weapon>();

        _slots = new List<FPS_Weapon>[9];

        for (int i = 0; i < 9; i++)
        {
            _slots[i] = new List<FPS_Weapon>();
        }


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
        CheckForIsGrounded();

        BodyMovement();


        ResetInputs();

       

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
        //Use GetAxisRaw to get a more responsive input
        _mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        _movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (_isGrounded && Input.GetButton("Jump"))
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
            _myRigidBody.velocity += jumpForce;
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

    private void CheckForIsGrounded()
    {
            RaycastHit hit;
            if (Physics.Raycast(_feet.position, -_feet.up, out hit, 0.3f)) //Landing
            {
                _isGrounded = true;
             Debug.DrawLine(_feet.position, _feet.position - Vector3.up * .3f, Color.green);
             }
            else 
            {
                _isGrounded = false;

                Debug.DrawLine(_feet.position, _feet.position - Vector3.up * .3f, Color.red);
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

        Debug.Log("received a hit");
    }



}
