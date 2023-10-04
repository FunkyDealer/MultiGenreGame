using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_MGPickUp : FPS_WeaponPickUp
{




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void GetPickUP(FPSPlayer player)
    {
        base.GetPickUP(player);

        player.PickUpWeapon(_weaponPrefab, _slot);

        Destroy(gameObject);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Player"))
        {
            FPSPlayer player = other.gameObject.GetComponent<FPSPlayer>();

            if (player != null) GetPickUP(player);
        }



    }
}
