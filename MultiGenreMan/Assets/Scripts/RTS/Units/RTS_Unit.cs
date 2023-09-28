using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RTS_Unit : RTS_ClickableEntity
{

    protected NavMeshAgent _myNavMesh;


    private RTS_ClickableEntity _currentTarget;
    private bool _moveOrder = false;
    private bool _attackOrder = false;
    private bool _canAttack = true;

    protected override void Awake()
    {
        base.Awake();

        _myNavMesh = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();


    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();


    }


    protected virtual void ReceiveOrder()
    {




    }

    public override void ReceiveMoveOrder(RTS_PlayerMoveOrder order)
    {
        base.ReceiveMoveOrder(order);

        _moveOrder = true;
        _myNavMesh.stoppingDistance = 0;


        _myNavMesh.SetDestination(order.Position);


    }

    private void EndAllOrders()
    {
        _attackOrder = false;
        _moveOrder = false;

        _currentTarget = null;

        _myNavMesh.stoppingDistance = 0;
        _myNavMesh.destination = transform.position;
    }


}
