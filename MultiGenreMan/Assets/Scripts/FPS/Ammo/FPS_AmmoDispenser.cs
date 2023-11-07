using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_AmmoDispenser : FPS_Interactible
{

    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private GameObject _pelletPrefab;
    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private Transform _dispensePoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public override void Interact(FPSPlayer p)
    {
        base.Interact(p);

        GameObject o = null;

        if (p.CurrentWeapon != null)
        {

            switch (p.CurrentWeapon.AmmoType)
            {
                case FPSPlayer.AMMOTYPE.BULLET:

                    o = Instantiate(_bulletPrefab, _dispensePoint.position, Quaternion.identity);

                    break;
                case FPSPlayer.AMMOTYPE.PELLET:

                    o = Instantiate(_pelletPrefab, _dispensePoint.position, Quaternion.identity);

                    break;
                case FPSPlayer.AMMOTYPE.LASER:

                    o = Instantiate(_laserPrefab, _dispensePoint.position, Quaternion.identity);

                    break;
                default:


                    break;
            }

        }
        else
        {
            o = Instantiate(_bulletPrefab, _dispensePoint.position, Quaternion.identity);
        }

        if (o != null) {
            Rigidbody rb = o.GetComponent<Rigidbody>();

            rb.AddForce(_dispensePoint.right * 5 + _dispensePoint.up * 3.5f, ForceMode.Impulse);
        }

    }


}
