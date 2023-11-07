using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    [SerializeField]
    protected int Ammount;

    [SerializeField]
    protected FPSPlayer.AMMOTYPE Type;




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           FPSPlayer p = other.gameObject.GetComponent<FPSPlayer>();

           if (GiveAmmo(p))
            {
                Destroy(gameObject);
            }



        }
    }

    protected bool GiveAmmo(FPSPlayer p)
    {
        switch (Type)
        {
            case FPSPlayer.AMMOTYPE.BULLET:

                if (p.CurrentBullets < p.MaxBullets)
                {
                    p.ReceiveBullets(Ammount);
                    return true;
                }
                else return false;

            case FPSPlayer.AMMOTYPE.PELLET:

                if (p.CurrentPellets < p.MaxPellets)
                {
                    p.ReceivePellets(Ammount);
                    return true;
                }
                else return false;

            case FPSPlayer.AMMOTYPE.LASER:

                if (p.CurrentLasers < p.MaxLasers)
                {
                    p.ReceiveLasers(Ammount);
                    return true;
                }
                else return false;

            default:
                return false;           
        }
    }
}
