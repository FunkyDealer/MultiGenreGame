using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_Unit : MonoBehaviour
{
    [SerializeField]
    protected int _maxHealth = 100;
    protected int _currentHealth;
    public int GetHealth => _currentHealth;

    [SerializeField]
    protected int _maxMana = 100;
    protected int _currentMana;

    [SerializeField]
    protected int attackPower =50;
    [SerializeField]
    protected int magic = 1;
    [SerializeField]
    protected int movement = 1;
    [SerializeField]
    protected float attackRange = 1;

    [SerializeField]
    private int _team = 1;

    [SerializeField]
    public int Team { get; private set; } = 1;


    [SerializeField]
    protected int _startingWidth = 1;
    [SerializeField]
    protected int _startingLength = 1;

    public Vector2Int CurrentSpace { get; private set; }

    [SerializeField]
    protected int _maxMoves = 1;
    [SerializeField]
    protected int _maxAttacks = 1;
    public int MovesLeft { get; private set; } = 1;
    public int AttacksLeft { get; private set; } = 1;


    protected virtual void Awake()
    {
        Team = _team;

        gameObject.name = $"Unit (Team {Team}";

        CurrentSpace = new Vector2Int(_startingWidth -1, _startingLength -1);

        _currentHealth = _maxHealth;
        _currentMana = _maxMana;

       MovesLeft = _maxMoves;
       AttacksLeft = _maxAttacks;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {

        TST_GameManager.RegisterUnit(this);

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {

    }

    public void TeleportToNewSpace(Vector2Int space2d, Vector3 space3d)
    {
        TST_Field.RemoveUnitFromSpace(CurrentSpace);

        Debug.Log($"going to space {space2d}, in {space3d}");
        CurrentSpace = space2d;

        TST_Field.SetUnitInSpace(CurrentSpace, this);

        Vector3 target = new Vector3(space3d.x, transform.position.y, space3d.z);
        transform.LookAt(target);

        transform.position = target;

        MovesLeft--;


    }

    public bool AttackEnemy(TST_Space enemySpace,TST_Unit defender)
    {
        if (ValidateAttack(enemySpace, defender))
        {
            Vector3 target = new Vector3(enemySpace.Space3D.x, transform.position.y, enemySpace.Space3D.z);
            transform.LookAt(target);


            Debug.Log("Attacking");
            AttacksLeft--;
            defender.ReceiveDamage(attackPower);

            return true;
        }
        else return false;
    }

    //validating movement is done by calculating distance between the two points and checking if it greater or equal than the movement of the unit
    public bool ValidateMovementOrder(Vector2Int targetSpace)
    {
        if (MovesLeft < 1)
        {
            Debug.Log("No moves Left");
            return false;
        }

        return ValidateMovement(targetSpace);
    }

    public bool ValidateMovement(Vector2Int targetSpace)
    {
        //calculate Cost
        //float dist = Vector2Int.Distance(s, CurrentSpace);
        float dist = Mathf.Sqrt(Mathf.Pow(CurrentSpace.x - targetSpace.x, 2) + Mathf.Pow(CurrentSpace.y - targetSpace.y, 2));

        if (dist <= movement) return true;

        return false;
    }

    public bool ValidateAttack(TST_Space targetSpace, TST_Unit defender)
    {
        if (AttacksLeft < 1)
        {
            Debug.Log("no attacks left");
            return false;
        }

        if (defender.Team == Team)
        {
            Debug.Log("Thats the same team");
            return false;
        }

        //calculate cost
        //float dist = Vector2Int.Distance(s, CurrentSpace);
        float dist = Mathf.Sqrt(Mathf.Pow(CurrentSpace.x - targetSpace.Space2D.x, 2) + Mathf.Pow(CurrentSpace.y - targetSpace.Space2D.y, 2));

        if (dist <= attackRange) return true;

        return false;
    }

    public void resetTurn()
    {
        MovesLeft = _maxMoves;
        AttacksLeft = _maxAttacks;
    }

    public void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            TST_GameManager.KillUnit(this);
        }
    }

}
