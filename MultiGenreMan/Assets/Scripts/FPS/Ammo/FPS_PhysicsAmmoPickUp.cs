using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_PhysicsAmmoPickUp : AmmoPickUp
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            FPSPlayer p = collision.gameObject.GetComponent<FPSPlayer>();

            if (GiveAmmo(p))
            {
                Destroy(gameObject);
            }
        }
    }
}
