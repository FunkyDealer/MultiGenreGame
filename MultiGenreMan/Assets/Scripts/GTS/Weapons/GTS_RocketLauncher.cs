using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_RocketLauncher : GTS_Weapon
{
    public override string ID
    {
        get { return "RocketLauncher"; }
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
            GTS_Rocket rocket = projectile.GetComponent<GTS_Rocket>();
            rocket.Shooter = _owner;
            rocket.Direction = transform.forward;
            rocket.Damage = _damage;
        }

        StartCoroutine(WeaponFireDelay(_weaponFireDelay));
    }


    public override GTS_Weapon PickUpWeapon(GTS_Entity owner)
    {
        return base.PickUpWeapon(owner);


    }
}
