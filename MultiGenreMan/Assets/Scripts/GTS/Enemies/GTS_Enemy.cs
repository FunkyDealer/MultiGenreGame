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

    protected void FixedUpdate()
    {
        SendPosForLockOn();
    }

    protected override void ReceiveDamage(int damage)
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
        GTS_EnemyManager.inst.RemoveEnemy(gameObject.GetInstanceID());
        GTS_GameManager.inst.Player.RemoveFromLockOn(gameObject.GetInstanceID());

        Vector3 pos = transform.position;

        FloatingTexTManager.inst.CreateText(pos, $"died", 0f);

        Destroy(gameObject);

    }

    protected virtual void SendPosForLockOn()
    {
        float dist = Vector3.Distance(GTS_GameManager.inst.Player.MyCenter, this.MyCenter);
        GTS_GameManager.inst.Player.ReceivePosForLockOn(this, dist, this.gameObject.GetInstanceID());

    }
}
