using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ARPG_Creature : MonoBehaviour
{

    [SerializeField]
    protected int _maxHealth;
    protected int _currentHealth;

    [SerializeField]
    protected float _moveSpeed;
    [SerializeField]
    protected float _turnSpeed = 200f;

    [SerializeField]
    protected float _attackRange;

    protected Rigidbody _myRigidBody;
    protected NavMeshAgent _myNavMeshAgent;

    protected virtual void Awake()
    {
        _myRigidBody = GetComponent<Rigidbody>();
        _myNavMeshAgent = GetComponent<NavMeshAgent>(); 


    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {

    }


    public virtual void ReceiveDamage(int damage)
    {
        Debug.Log($"Current health: {_currentHealth}");
        Debug.Log($"received {damage} damage");
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Debug.Log($"Current health: {_currentHealth}");
            _currentHealth = 0;
            Destroy(gameObject);
        }



    }


}
