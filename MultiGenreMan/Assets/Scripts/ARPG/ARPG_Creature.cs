using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPG_Creature : MonoBehaviour
{

    [SerializeField]
    protected int _maxHealth;
    protected int _currentHealth;

    [SerializeField]
    protected float _moveSpeed;

    protected Rigidbody _myRigidBody;


    protected virtual void Awake()
    {
        _myRigidBody = GetComponent<Rigidbody>();


    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {

    }


    protected virtual void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth < 0)
        {
            _currentHealth = 0;
            Destroy(gameObject);
        }



    }


}
