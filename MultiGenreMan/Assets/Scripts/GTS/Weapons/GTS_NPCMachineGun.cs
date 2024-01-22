using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_NPCMachineGun : GTS_Weapon
{
    public override string ID
    {
        get { return "NPCMachineGun"; }
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
            GameObject projectile = Instantiate(_projectilePrefab, _shootingPoint.position, Quaternion.identity);
            GTS_Bullet bullet = projectile.GetComponent<GTS_Bullet>();
            bullet.Shooter = _owner;
            bullet.Direction = transform.forward;
            bullet.Damage = _damage;
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
