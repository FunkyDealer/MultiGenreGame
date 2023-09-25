using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPG_Player : ARPG_Creature
{
    private ARPG_MainCamera _camera;

    private float _turnSpeed = 4f;


    bool _moveOrder = false;
    private Vector3 _newTargetPosition;
    private Vector3 _newTargetDirection;
    private Quaternion _newTargetRotation;

    protected override void Awake()
    {
        base.Awake();



    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        _newTargetPosition = transform.position;
        _newTargetRotation = transform.rotation;

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();





    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();


        if (_moveOrder)
        {
            _newTargetDirection = _newTargetPosition - transform.position;
            _newTargetDirection.y = 0;
            _newTargetDirection.Normalize();   
            
            
            //Gradually rotate player towards target direction
            Quaternion targetRotation = Quaternion.LookRotation(_newTargetDirection, transform.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _turnSpeed * Time.deltaTime);


            //Move player to target position
            float dirY = _myRigidBody.velocity.y; //conserve y speed so that fall speed is always the same
            Vector3 moveDirection = _newTargetDirection * _moveSpeed;
            moveDirection.y = dirY;            

            _myRigidBody.velocity = moveDirection;


        }
    }

    public void ResetMovementOrder()
    {
        _moveOrder = false;

        _newTargetDirection = transform.forward;
        _newTargetPosition = transform.position;
        _newTargetRotation = transform.rotation;

        _myRigidBody.velocity = new Vector3(0, _myRigidBody.velocity.y, 0);
    }


    protected override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);


    }

    public void GetCamera(ARPG_MainCamera mainCamera)
    {
        this._camera = mainCamera;


    }


    public void ReceiveOrder(ARPG_PlayerOrder order) 
    {

        MoveOrder(order);


    }

    private void MoveOrder(ARPG_PlayerOrder order)
    {

        _moveOrder = true;
        _newTargetPosition = order._position;
        


    }
}
