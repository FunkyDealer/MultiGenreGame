using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_MachineGun : GTS_Weapon
{
    public override string ID
    {
        get {return "MachineGun"; }
    }


    protected override void Awake()
    {
        base.Awake();


    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();


    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();


    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();


    }

    public override void Shoot()
    {
            base.Shoot();

        if (_owner.SpendAmmo(_ammoType, 1))
        {
            Tuple<Vector3, GTS_Entity> tuple = CalculateHitScanTrajectory(_shootingPoint.position, transform.forward);
            GameObject projectile = Instantiate(_projectilePrefab, _shootingPoint.position, Quaternion.identity);
            GTS_LaserProjectile laser = projectile.GetComponent<GTS_LaserProjectile>();
            laser.Shooter = _owner;
            laser.HitPoint = tuple.Item1;
            laser.HitEntity = tuple.Item2;
        }
        else
        {

        }


        StartCoroutine(WeaponFireDelay(_weaponFireDelay));
    }
   

    public override GTS_Weapon PickUpWeapon(GTS_Entity owner)
    {
        return base.PickUpWeapon(owner);

    
    }


}
