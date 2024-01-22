using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_AmmoPickUp : GTS_PickUp
{
    [SerializeField]
    private int _ammount;
    [SerializeField]
    protected GTS_Weapon.AMMOTYPE ammoType;



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
        if (other.gameObject.CompareTag("Player"))
        {
            GTS_Player p = other.gameObject.GetComponent<GTS_Player>();

            if (p.AddAmmo(ammoType,_ammount)) Destroy(gameObject);
        }
    }
}
