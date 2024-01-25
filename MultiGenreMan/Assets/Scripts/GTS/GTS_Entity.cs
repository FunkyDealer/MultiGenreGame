using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_Entity : MonoBehaviour
{
    protected bool _alive = true;

    protected int _currentHealth;
    [SerializeField]
    protected int _maxHealth;

    [SerializeField]
    protected float _movSpeed;

    protected float _currentSpeed;

    Collider myMainCollider;

    public Vector3 MyCenter => myMainCollider.bounds.center;

    protected virtual void Awake()
    {
        myMainCollider = GetComponent<Collider>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
        _currentSpeed = _movSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void ReceiveDamage(int damage)
    {

    }

    public virtual void ReceiveDamage(int damage, Vector3 contactPoint)
    {


    }


    protected virtual void Die()
    {

    }

    public virtual bool SpendAmmo(GTS_Weapon.AMMOTYPE ammoType, int ammount)
    {


        return false;
    }
}
