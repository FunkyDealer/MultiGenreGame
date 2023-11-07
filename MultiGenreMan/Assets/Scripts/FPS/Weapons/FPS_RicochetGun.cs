using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_RicochetGun : FPS_Weapon
{
    [SerializeField]
    private int _maxRicochets = 5;


    protected override void Awake()
    {
        base.Awake();

        AmmoType = FPSPlayer.AMMOTYPE.LASER;
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
        if ((_owner as FPSPlayer).SpendLaser())
        { 

        base.ShootPrimary(eye);
        Tuple<List<Vector3>, FPS_Creature> tuple = CalculateLaserScanTrajectory(eye);
        GameObject projectile = Instantiate(_projectilePrefab, _shootingPoint.position, Quaternion.identity);
        FPS_LaserRicochetProjectile laser = projectile.GetComponent<FPS_LaserRicochetProjectile>();
        laser.Shooter = _owner;
        laser.HitPoints = tuple.Item1;
        laser.HitEntity = tuple.Item2;

        _currentRecoil += _recoilIncreasePerShot;
        if (_currentRecoil > _maxRecoil) _currentRecoil = _maxRecoil;

        StartCoroutine(WeaponFireDelay(_weaponFireDelay));

        StopCoroutine(WeaponStabilizingDelay(0));
        StartCoroutine(WeaponStabilizingDelay(_timeToStartStabilizing));
        }
    }

    protected Tuple<List<Vector3>, FPS_Creature> CalculateLaserScanTrajectory(Transform eye)
    {
        List<Vector3> contactPoints = new List<Vector3>();
         Vector3 firstContactPoint = eye.position + eye.forward.normalized * _maxRange;
        contactPoints.Add(firstContactPoint);

        Vector3 direction = eye.forward;

        FPS_Creature hitEnemy = null;

        RaycastHit hit;
        Tuple<List<Vector3>, FPS_Creature> result = new Tuple<List<Vector3>, FPS_Creature>(contactPoints, hitEnemy);

        if (Physics.Raycast(eye.position, direction, out hit, 1000))
        {
            //Debug.Log("Something was hit");

            hitEnemy = hit.collider.gameObject.GetComponent<FPS_Creature>();
            contactPoints[0] = hit.point;
            if (hitEnemy != null) //the shot hit an enemy
            {
                hitEnemy.ReceiveDamage(_damage, contactPoints[0]);
                result = new Tuple<List<Vector3>, FPS_Creature>(contactPoints, hitEnemy);
            }
            else //the shot didn't hit an enemy, try to ricochet it
            {
                int ricochetCount = 0;
                result = CalculateRicochet(hit.point, hit.normal, direction, contactPoints, ricochetCount);
            }
        }
        else //shot hit nothing (probly the skybox)
        {

            //Debug.Log("Nothing was hit");
        }

        return result;
    }

    private Tuple<List<Vector3>,FPS_Creature> CalculateRicochet(Vector3 hitPos, Vector3 hitNormal, Vector3 laserDir, List<Vector3> contactPoints, int ricochetCount)
    {
        FPS_Creature hitEnemy = null;
        Vector3 newContactPoint;

        float angle = Vector3.Angle(hitNormal, laserDir);
        Tuple<List<Vector3>, FPS_Creature> result = new Tuple<List<Vector3>, FPS_Creature>(contactPoints, hitEnemy);

        if (angle > 15f) //can ricochet
        {
            Vector3 newDir = Vector3.Reflect(laserDir, hitNormal);
            newDir.Normalize();

            newContactPoint = hitPos + newDir * 500;
            ricochetCount++;

            RaycastHit hit;

            if (Physics.Raycast(hitPos, newDir, out hit, 500))
            {                
                newContactPoint = hit.point;
                contactPoints.Add(newContactPoint);
                hitEnemy = hit.collider.gameObject.GetComponent<FPS_Creature>();
                

                if (hitEnemy != null) //the ricochet hit an enemy
                {
                    hitEnemy.ReceiveDamage(_damage, newContactPoint);                    
                    result = new Tuple<List<Vector3>, FPS_Creature>(contactPoints, hitEnemy);
                }
                else //the ricochet didn't hit an enemy, try to ricochet it again
                {
                   
                    if (ricochetCount < _maxRicochets) result = CalculateRicochet(hit.point, hit.normal, newDir, contactPoints, ricochetCount);
                }

            }
            else //ricochet hit nothing (probly the skybox)
            {
                contactPoints.Add(newContactPoint);
                result = new Tuple<List<Vector3>, FPS_Creature>(contactPoints, hitEnemy);
            }

        }

        return result;
    }

    public override void ShootSecondary()
    {
        base.ShootSecondary();


    }

    public override void PickUpWeapon(FPS_Creature owner)
    {
        base.PickUpWeapon(owner);

        (owner as FPSPlayer).ReceiveLasers(_ammoOnPickup);
    }

}

