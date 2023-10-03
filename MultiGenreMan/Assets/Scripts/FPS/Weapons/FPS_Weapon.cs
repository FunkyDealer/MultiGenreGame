using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Weapon : MonoBehaviour
{
    protected FPS_Creature _owner;


    protected bool _canShoot = true;
    public bool CanShoot => _canShoot;

    [SerializeField]
    protected Transform _shootingPoint;
    [SerializeField]
    protected GameObject _projectilePrefab;


    [SerializeField]
    protected int _damage;
    protected float _recoil = 0; //recoil starts at 0
    [SerializeField]
    protected float _maxRecoil = 0.5f; //Max recoil the gun can have
    [SerializeField]
    protected float _recoilIncreasePerShot = 0.1f; //recoil increases per 0.1 per shot
    [SerializeField]
    protected float _timeToStartStabilizing = 0.1f; //Time it takes after a shot is fired before the player starts stabilizing their weapon
    protected bool _stabilize = true; //can the player start stabilizing their weapon after firing?
    [SerializeField]
    protected float _recoilStabilizationPerSecond = 0.5f; //Player stabilizer their weapon x value per second
    [SerializeField]
    protected float _weaponFireDelay = 0.1f; //how long till you can shoot again after last shot
    [SerializeField]
    protected float _maxRange = 100; //max range of weapon
    [SerializeField]
    protected float _spread = 5;

    protected virtual void Awake()
    {

    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_recoil > 0 && _stabilize)
        {
            _recoil -= _recoilStabilizationPerSecond * Time.deltaTime;
            if (_recoil < 0) _recoil = 0;
        }


    }

    protected virtual void FixedUpdate()
    {

    }

    public virtual void PickUpWeapon(FPS_Creature owner)
    {
        this._owner = owner;
    }

    public virtual void ShootPrimary(Transform eye)
    {



    }

    public virtual void ShootSecondary()
    {



    }

    protected IEnumerator WeaponStabilizingDelay(float time)
    {
        _stabilize = false;

        yield return new WaitForSeconds(time);

        _stabilize = true;
    }

    protected IEnumerator WeaponFireDelay(float time)
    {
        _canShoot = false;

        yield return new WaitForSeconds(time);


        _canShoot = true;
    }

    protected virtual Tuple<Vector3, FPS_Creature> CalculateHitScanTrajectory(Transform eye)
    {      
        Vector3 contactPoint = eye.position + eye.forward.normalized * _maxRange;

        Vector3 direction = eye.forward;        

        FPS_Creature hitEnemy = null;

        RaycastHit hit;

        if (Physics.Raycast(eye.position, direction, out hit, 100))
        {
            //Debug.Log("Something was hit");

            hitEnemy = hit.collider.gameObject.GetComponent<FPS_Creature>();
            contactPoint = hit.point;
            if (hitEnemy != null)
            {
                hitEnemy.ReceiveDamage(_damage);


            }
        }
        else
        {

            Debug.Log("Nothing was hit");
        }

        return new Tuple<Vector3, FPS_Creature>(contactPoint, hitEnemy);

    }

  

}
