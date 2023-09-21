using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Enemy : FPS_Creature
{



    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;

            Destroy(gameObject);
        }
        


    }
    

}
