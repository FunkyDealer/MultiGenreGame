using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Creature : MonoBehaviour
{
    [SerializeField]
    protected int _maxHealth; //Max health that the creature can have
    protected int _currentHealth; //Current Health that the creature has
    [SerializeField]
    protected float _movSpeed; //creature's Movement Speed


    // Start is called before the first frame update
    protected virtual void Start()
    {
        _currentHealth = _maxHealth;


    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public virtual void ReceiveDamage(int damage)
    {


    }

}
