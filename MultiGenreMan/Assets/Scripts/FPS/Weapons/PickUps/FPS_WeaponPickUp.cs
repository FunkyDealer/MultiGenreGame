using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_WeaponPickUp : MonoBehaviour
{
    [SerializeField]
    protected GameObject _weaponPrefab;

    [SerializeField]
    protected int _slot; //slot where the weapon goes

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    protected virtual void GetPickUP(FPSPlayer player)
    {

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        
    }
}
