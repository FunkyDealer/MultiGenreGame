using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_ClickableEntity : MonoBehaviour
{
    public int Team; //team it belongs to
    [SerializeField]
    protected int _maxHealth;
    protected int _currentHealth;

    protected bool _invincible;

    protected virtual void Awake()
    {
        
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

    protected virtual void LateUpdate()
    {

    }

    public virtual void ReceiveDamage(int damage)
    {
        if (!_invincible)
        {



        }


    }

    public virtual void ReceiveMoveOrder(RTS_PlayerMoveOrder order)
    {

    }



}
