using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Teleporter : MonoBehaviour
{
    [SerializeField]
    private Transform _destination;



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

            if (p != null) p.TeleportMeTo(_destination);


        }
    }

}
