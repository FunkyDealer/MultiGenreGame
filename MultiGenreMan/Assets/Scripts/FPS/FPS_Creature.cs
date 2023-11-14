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

    [SerializeField]
    protected int _maxArmour = 100;
    protected int _currentArmour = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
        _currentArmour = 0;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void ReceiveDamage(int damage)
    {

    }

    public virtual void ReceiveDamage(int damage, Vector3 contactPoint)
    {


    }



}
