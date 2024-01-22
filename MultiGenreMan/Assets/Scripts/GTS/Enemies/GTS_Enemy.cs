using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_Enemy : GTS_Entity
{

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();


        
        GTS_EnemyManager.inst.RegisterEnemy(gameObject.GetInstanceID(), this);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ReceiveDamage(int damage)
    {
        _currentHealth = _currentHealth - damage;

        FloatingTexTManager.inst.CreateText(transform.position, $"-{damage}", 0f);

        if (_currentHealth < 0)
        {
            _currentHealth = 0;
            Die();
        }

    }

    public override void ReceiveDamage(int damage, Vector3 contactPoint)
    {
        _currentHealth = _currentHealth - damage;

        FloatingTexTManager.inst.CreateText(contactPoint, $"-{damage}", 0f);


        if (_currentHealth < 0)
        {
            _currentHealth = 0;
            Die();
        }

    }


    protected override void Die()
    {
        Vector3 pos = transform.position;

        FloatingTexTManager.inst.CreateText(pos, $"died", 0f);

        Destroy(gameObject);

    }
}
