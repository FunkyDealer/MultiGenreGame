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
    protected int _maxMana = 50;
    protected int _currentMana;

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
        _currentMana = _maxMana;
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
 
        _currentHealth -= damage;
        FloatingTexTManager.inst.CreateText(this.transform.position, $"-{damage}", 0);
        if (_currentHealth <= 0)
        {
            FloatingTexTManager.inst.CreateText(this.transform.position, $"died", 0.5f);

            _currentHealth = 0;
            Destroy(gameObject);
        }



    }


}
