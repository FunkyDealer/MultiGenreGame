using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPG_MovementHelper : MonoBehaviour
{
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
            ARPG_Player player = other.gameObject.GetComponent<ARPG_Player>();
            player.ResetMovementOrder();

        }
    }
}
