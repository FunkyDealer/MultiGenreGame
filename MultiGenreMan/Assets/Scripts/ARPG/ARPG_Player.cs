using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPG_Player : ARPG_Creature
{
    private ARPG_MainCamera _camera; 

    bool _moveOrder = false;
    private Vector3 _newTargetPosition;
    private Vector3 _newTargetDirection;
    private Quaternion _newTargetRotation;

    private ARPG_Enemy _currentTarget = null;

    private bool _attackOrder = false;

    private bool _canAttack = true;

    [SerializeField]
    private GameObject _projectilePreFab;
    [SerializeField]
    Transform _shootingPoint;

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


        _myNavMeshAgent.speed = _moveSpeed;
        _myNavMeshAgent.angularSpeed = _turnSpeed;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();





    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        AttackCycle();
    }

    public void ResetMovementOrder()
    {
        _moveOrder = false;
        _myNavMeshAgent.ResetPath();

        _newTargetDirection = transform.forward;
        _newTargetPosition = transform.position;
        _newTargetRotation = transform.rotation;

        //_myRigidBody.velocity = new Vector3(0, _myRigidBody.velocity.y, 0);
    }

    private void ManualMovement()
    {
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

    private void AttackCycle()
    {
        if (_attackOrder)
        {
            //check if target still exists
            if (_currentTarget != null)
            {

                _myNavMeshAgent.stoppingDistance = _attackRange;
                _myNavMeshAgent.SetDestination(_currentTarget.gameObject.transform.position);

                float distance = Vector3.Distance(transform.position, _currentTarget.gameObject.transform.position);

                //check if distance is less than weapon range
                if (distance <= _attackRange)
                {
                    //check rotation
                    _newTargetDirection = _currentTarget.gameObject.transform.position - transform.position;
                    _newTargetDirection.y = 0;

                    //Gradually rotate player towards target direction
                    Quaternion targetRotation = Quaternion.LookRotation(_newTargetDirection, transform.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, (_turnSpeed / 50) * Time.deltaTime);
                                        
                    if (Vector3.Angle(transform.forward, _newTargetDirection) < 1)
                    {
                        //attack
                        if (_canAttack)
                        {
                            Attack();


                        }
                        


                    }


                }
            }
            else
            {
                _attackOrder = false;
                _currentTarget = null;
            }

        }
    }

    private void Attack()
    {
        GameObject projectile = Instantiate(_projectilePreFab, _shootingPoint.position, Quaternion.identity);
        ARPG_Projectile p = projectile.GetComponent<ARPG_Projectile>();
        //p.GetInfo(this, _currentTarget);
        p._shooter = this;
        p._target = _currentTarget;

        if (_currentTarget != null)
        {
           
        }

        _canAttack = false;

        StartCoroutine(AttackCoolDown(1));
    }

    private IEnumerator AttackCoolDown(float time)
    {
        yield return new WaitForSeconds(time);

        _canAttack = true;
    }

    public override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);


    }

    public void GetCamera(ARPG_MainCamera mainCamera) => this._camera = mainCamera;


    public void ReceiveOrder(ARPG_PlayerOrder order) 
    {
        switch (order)
        {
            case ARPG_PlayerMoveOrder:
                MoveOrder(order as ARPG_PlayerMoveOrder);
                break;
            case ARPG_PlayerAttackOrder:
                AttackOrder(order as ARPG_PlayerAttackOrder);
                break;
            default:
                //do nothing
                break;
        }      


    }

    private void MoveOrder(ARPG_PlayerMoveOrder order)
    {
        EndAllOrders();

        _moveOrder = true;
        _newTargetPosition = order.Position;

        _myNavMeshAgent.stoppingDistance = 0;

        _myNavMeshAgent.SetDestination(_newTargetPosition);

    }

    private void AttackOrder(ARPG_PlayerAttackOrder order)
    {
        EndAllOrders();

        _currentTarget = order.Enemy as ARPG_Enemy;
        _attackOrder = true;

        _myNavMeshAgent.stoppingDistance = _attackRange;
        _myNavMeshAgent.SetDestination(_currentTarget.gameObject.transform.position);




    }

    private void EndAllOrders()
    {
        _attackOrder = false;
        _moveOrder = false;

        _currentTarget = null;

        _myNavMeshAgent.stoppingDistance = 0;
        _myNavMeshAgent.destination = transform.position;
    }
}
