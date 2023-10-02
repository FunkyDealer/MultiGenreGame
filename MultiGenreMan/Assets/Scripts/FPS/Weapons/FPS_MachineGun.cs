using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_MachineGun : FPS_Weapon
{




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


    public override void ShootPrimary(Vector3 eyePos, Vector3 eyeDirection)
    {
        base.ShootPrimary(eyePos, eyeDirection);
        Tuple<Vector3, FPS_Creature> tuple = CalculateHitScanTrajectory(eyePos, eyeDirection);
         GameObject projectile = Instantiate(_projectilePrefab, _shootingPoint.position, Quaternion.identity);
        FPS_LaserProjectile laser = projectile.GetComponent<FPS_LaserProjectile>();
        laser.Shooter = _owner;
        laser.HitPoint = tuple.Item1;
        laser.HitEntity = tuple.Item2;

        _recoil += _recoilIncreasePerShot;
        if (_recoil > _maxRecoil) _recoil = _maxRecoil;

        StartCoroutine(WeaponFireDelay(_weaponFireDelay));

        StopCoroutine(WeaponStabilizingDelay(0));
        StartCoroutine(WeaponStabilizingDelay(_timeToStartStabilizing));
    }

    public override void ShootSecondary()
    {
        base.ShootSecondary();


    }

}
