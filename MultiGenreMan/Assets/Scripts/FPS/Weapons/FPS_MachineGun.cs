using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_MachineGun : FPS_Weapon
{




    protected override void Awake()
    {
        base.Awake();

        AmmoType = FPSPlayer.AMMOTYPE.BULLET;
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

    public override void ShootPrimary(Transform eye)
    {    

        if ((_owner as FPSPlayer).SpendBullet()) 
        {
            base.ShootPrimary(eye);

            Tuple<Vector3, FPS_Creature> tuple = CalculateHitScanTrajectory(eye);
            GameObject projectile = Instantiate(_projectilePrefab, _shootingPoint.position, Quaternion.identity);
            FPS_LaserProjectile laser = projectile.GetComponent<FPS_LaserProjectile>();
            laser.Shooter = _owner;
            laser.HitPoint = tuple.Item1;
            laser.HitEntity = tuple.Item2;

            _currentRecoil += _recoilIncreasePerShot;
            if (_currentRecoil > _maxRecoil) _currentRecoil = _maxRecoil;

            StartCoroutine(WeaponFireDelay(_weaponFireDelay));

            StopCoroutine(WeaponStabilizingDelay(0));
            StartCoroutine(WeaponStabilizingDelay(_timeToStartStabilizing));

        }
    }

    protected override Tuple<Vector3, FPS_Creature> CalculateHitScanTrajectory(Transform eye)
    {
        Vector3 contactPoint = eye.position + eye.forward.normalized * _maxRange;

        //this logic comes straight from quake3's machine gun source code
        float up;
        float right;

        contactPoint = Utils.Vector3MA(eye.position, _maxRange, eye.forward); //calculate contact point (where the shot lands)

        right = UnityEngine.Random.value * (float)Math.PI * 2.0f;
        up = (float)Math.Sin(right) * UnityEngine.Random.Range(-1, 1) * _spread; //calculate vertical spread
        right = (float)Math.Cos(right) * UnityEngine.Random.Range(-1, 1) * _spread; //calculate horizontal spread      

        contactPoint = Utils.Vector3MA(contactPoint, right, eye.right); //shift contact point horizontally
        contactPoint = Utils.Vector3MA(contactPoint, up, eye.up); //shift contact point vertically

        //direction is end - start
        Vector3 direction = contactPoint - eye.position;

        FPS_Creature hitEnemy = null;

        RaycastHit hit;

        if (Physics.Raycast(eye.position, direction, out hit, 1000))
        {
            //Debug.Log("Something was hit");

            hitEnemy = hit.collider.gameObject.GetComponent<FPS_Creature>();
            contactPoint = hit.point;
            if (hitEnemy != null)
            {
                hitEnemy.ReceiveDamage(_damage, contactPoint);


            }
        }
        else
        {

            //Debug.Log("Nothing was hit");
        }

        return new Tuple<Vector3, FPS_Creature>(contactPoint, hitEnemy);
    }

    public override void ShootSecondary()
    {
        base.ShootSecondary();


    }

    public override void PickUpWeapon(FPS_Creature owner)
    {
        base.PickUpWeapon(owner);

        (owner as FPSPlayer).ReceiveBullets(_ammoOnPickup);
    }

}
