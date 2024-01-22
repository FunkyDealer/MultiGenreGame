using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_Weapon : MonoBehaviour
{
    public enum AMMOTYPE
    {
        Bullets,
        Rockets,
        Grenades
    }

    protected bool pickedUp = false;

    protected GTS_Entity _owner;

    protected bool _canShoot = true;
    public bool CanShoot => _canShoot;

    [SerializeField]
    protected Transform _shootingPoint;
    [SerializeField]
    protected GameObject _projectilePrefab;

    [SerializeField]
    protected int _damage;
    [SerializeField]
    protected float _weaponFireDelay = 0.1f; //how long till you can shoot again after last shot
    [SerializeField]
    protected float _maxRange = 100; //max range of weapon

    [SerializeField]
    protected int _ammoOnPickup;
    public int AmmoOnPickup => _ammoOnPickup;


    [SerializeField]
    protected AMMOTYPE _ammoType;
    public AMMOTYPE AmmoType => _ammoType;

    public virtual string ID { get; }

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
        
    }

    protected virtual void FixedUpdate()
    {

    }

    public virtual GTS_Weapon PickUpWeapon(GTS_Entity owner)
    {
        this._owner = owner;

        if (!pickedUp)
        {
            pickedUp = true;
        }

        if (owner is GTS_Player) (owner as GTS_Player).PickUpWeapon(this);

        return this;
    }

    public virtual void Shoot()
    {
       


    }


    public virtual void SwitchOut()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);

    }

    public virtual void SwitchIn()
    {
        this.gameObject.SetActive(true);

        _canShoot = true;
    }

    protected IEnumerator WeaponFireDelay(float time)
    {
        _canShoot = false;

        yield return new WaitForSeconds(time);

        _canShoot = true;
    }

    protected virtual Tuple<Vector3, GTS_Entity> CalculateHitScanTrajectory(Vector3 shotOrigin, Vector3 shotDirection)
    {
        Vector3 contactPoint = shotOrigin + shotDirection * _maxRange;

        GTS_Entity hitEnemy = null;

        RaycastHit hit;

        if (Physics.Raycast(shotOrigin, shotDirection, out hit, 100))
        {
            //Debug.Log("Something was hit");

            hitEnemy = hit.collider.gameObject.GetComponent<GTS_Entity>();
            contactPoint = hit.point;
            if (hitEnemy != null)
            {
                hitEnemy.ReceiveDamage(_damage, contactPoint);


            }
        }
        else
        {

           
        }

        return new Tuple<Vector3, GTS_Entity>(contactPoint, hitEnemy);

    }

}
