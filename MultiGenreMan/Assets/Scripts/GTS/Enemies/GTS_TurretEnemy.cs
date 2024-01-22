using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_TurretEnemy : GTS_Enemy
{
    [SerializeField]
    private Transform _turretRot;

    [SerializeField]
    private float _timeToStartFiring = 0.5f;
    private float _timeToSetSafety = 1f;

    [SerializeField]
    private float _rotationSpeed = 10;

    private enum AIStatus
    {
        Sleep,
        Attacking
    }
    private AIStatus _status = AIStatus.Sleep;

    //private GTS_Entity _target = null;

    private Vector3 _targetPosition; //target's position
    private Vector3 _targetForward;
    private Vector3 _restForward;

    private Quaternion _targetRotation = Quaternion.identity; 

    [SerializeField]
    GameObject _target;

    [SerializeField]
    private LayerMask _targetingIgnore;

    [SerializeField]
    private float _aimCompensation = 3; //how much will the turret compensate for the target's movement

    [SerializeField]
    GTS_Weapon _myWeapon;


    protected override void Awake()
    {
        _restForward = transform.forward;
        _targetForward = transform.forward;

        _myWeapon.PickUpWeapon(this);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _status = AIStatus.Sleep;

        _target = GTS_GameManager.inst.Player.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        



    }

    private void FixedUpdate()
    {
        RotateToTarget();

        if (_status == AIStatus.Sleep)
        {
            //rotate to front
            _targetForward = _restForward;
            _targetRotation = Quaternion.identity;

        }
        else if (_status == AIStatus.Attacking)
        {
            //if (_target == null) _target = GTS_GameManager.inst.Player;
            //rotate to target
            GetTargetPosition();

            CreateTargetRotation();

            if (_myWeapon.CanShoot) TryToShoot();


        }        
    }

    private void GetTargetPosition()
    {
        Vector3 center = _target.GetComponent<Collider>().bounds.center;
        _targetPosition = center;

        Rigidbody rb = _target.GetComponent<Rigidbody>();
        if (rb != null)
        { //try to compensate position with velocity
            Vector3 velocity = rb.velocity;

            _targetPosition += (velocity.normalized * _aimCompensation);
        }
    }

    private void CreateTargetRotation()
    {
        _targetForward = _turretRot.forward;

        _targetForward = (_targetPosition - _turretRot.position).normalized;
        Vector3 targetUp = Vector3.Cross(_targetForward, _turretRot.right);

        //_targetForward = Vector3.RotateTowards()

        _targetRotation = Quaternion.LookRotation(_targetForward, targetUp);

    }

    private void RotateToTarget()
    {

        

        _turretRot.rotation = Quaternion.Lerp(_turretRot.rotation, _targetRotation, _rotationSpeed * Time.deltaTime);

        //_turretRot.LookAt(_target.position, transform.up);
    }

    private void TryToShoot()
    {       
        bool canShoot = true;

        //check if my forward and target forward match by a fault of 5 degrees
        float angle = Vector3.Angle(_turretRot.forward, _targetForward);
        if (angle > 30) canShoot = false;
        Debug.Log(angle);

        if (canShoot)
        {
            //check if shot not blocked
            RaycastHit hit;
            float distance = Vector3.Distance(_turretRot.position, _targetPosition);
            if (Physics.Raycast(_turretRot.position, _turretRot.forward, out hit, distance, ~_targetingIgnore, QueryTriggerInteraction.Ignore))
            {
                float newDistance = Vector3.Distance(_turretRot.position, hit.point);

                if (!hit.collider.gameObject.CompareTag("Player"))
                {
                    if (newDistance < distance * 0.8f)
                    {
                        Debug.DrawLine(_turretRot.position, _turretRot.position + _targetForward * newDistance, Color.red);
                        canShoot = false;
                    }
                    else
                    {
                        Debug.DrawLine(_turretRot.position, _turretRot.position + _targetForward * newDistance, Color.green);
                    }


                }
                else
                {
                    Debug.DrawLine(_turretRot.position, _turretRot.position + _targetForward * newDistance, Color.green);
                }

            }
            else
            {
                Debug.DrawLine(_turretRot.position, _turretRot.position + _targetForward * distance, Color.green);
            }


            //shoot
            if (canShoot) Shoot();

        }
    }

    private void Shoot()
    {

        _myWeapon.Shoot();


    }

    public override bool SpendAmmo(GTS_Weapon.AMMOTYPE ammoType, int ammount)
    {
        return true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _status = AIStatus.Attacking;

           
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _status = AIStatus.Sleep;

          
        }
    }

}
