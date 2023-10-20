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
    public float GetAttackRange => attackRange;

    [SerializeField]
    private int _team = 1;

    [SerializeField]
    public int Team { get; private set; } = 1;


    [SerializeField]
    protected int _startingWidth = 1;
    [SerializeField]
    protected int _startingLength = 1;

    public Vector2Int CurrentSpace2D { get; private set; }

    [SerializeField]
    protected int _maxMoves = 1;
    [SerializeField]
    protected int _maxAttacks = 1;
    public int MovesLeft { get; private set; } = 1;
    public int AttacksLeft { get; private set; } = 1;


    protected virtual void Awake()
    {
        Team = _team;

        gameObject.name = $"Unit (Team {Team})";

        CurrentSpace2D = new Vector2Int(_startingWidth -1, _startingLength -1);

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
    public void CreateMovementIndicators()
    {
        List<int> visited = new List<int>();
        visited.Add(GetSpace().GetInstanceID());

        foreach (var s in GetSpace().Neighbours)
        {
            RecursiveIndicatorBuilder(s, visited);
        }
        visited.Clear();

    }

    //recursive function that goes to each space and checks if it's a valid move
    //seen spaces are the spaces that were already seen
    private void RecursiveIndicatorBuilder(TST_Space space, List<int> visited)
    {
        if (!visited.Contains(space.GetInstanceID()))
        {
            visited.Add(space.GetInstanceID());

            if (ValidateMovement(space.Space2D))
            {
                if (!space.IsOccupied())
                {
                    GameObject g = Instantiate(TST_GameManager.inst.GetMovementIndicatorObj, space.Space3D, TST_GameManager.inst.GetMovementIndicatorObj.transform.rotation);
                    g.transform.localScale = new Vector3(TST_Field.GetSpaceSize(), TST_Field.GetSpaceSize(), g.transform.localScale.z);
                    TST_GameManager.AddMovementIndicator(g);
                }

                foreach (TST_Space s in space.Neighbours)
                {
                    if (!visited.Contains(s.GetInstanceID()))
                    {
                        RecursiveIndicatorBuilder(s, visited);
                    }
                }
            }
        }
    }


    public void TeleportToNewSpace(Vector2Int space2d, Vector3 space3d)
    {
        TST_Field.RemoveUnitFromSpace(CurrentSpace2D);

        Debug.Log($"going to space {space2d}, in {space3d}");
        CurrentSpace2D = space2d;

        TST_Field.SetUnitInSpace(CurrentSpace2D, this);

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
            if (defender.ReceiveDamage(attackPower))
            {
                //exp?
            }

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
        //float dist = Vector2Int.Distance(CurrentSpace2D, targetSpace);
        float dist = Mathf.Sqrt(Mathf.Pow(CurrentSpace2D.x - targetSpace.x, 2) + Mathf.Pow(CurrentSpace2D.y - targetSpace.y, 2));

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
        //float dist = Vector2Int.Distance(CurrentSpace2D, targetSpace.Space2D);
        float dist = Mathf.Sqrt(Mathf.Pow(CurrentSpace2D.x - targetSpace.Space2D.x, 2) + Mathf.Pow(CurrentSpace2D.y - targetSpace.Space2D.y, 2));

        if (dist <= attackRange) return true;

        return false;
    }

    public bool ValidateAttackFast(TST_Unit enemy)
    {
        if (AttacksLeft < 1) return false;
        if (enemy.Team == Team) return false;

        //calculate cost
        //float dist = Vector2Int.Distance(CurrentSpace2D, enemy.CurrentSpace2D);
        float dist = Mathf.Sqrt(Mathf.Pow(CurrentSpace2D.x - enemy.CurrentSpace2D.x, 2) + Mathf.Pow(CurrentSpace2D.y - enemy.CurrentSpace2D.y, 2));

        if (dist <= attackRange) return true;

        return false;
    }

    public void resetTurn()
    {
        MovesLeft = _maxMoves;
        AttacksLeft = _maxAttacks;
    }

    public bool ReceiveDamage(int damage)
    {
        _currentHealth -= damage;

        FloatingTexTManager.inst.CreateText(transform.position, new string($"-{damage}"), 0f);

        if (_currentHealth <= 0)
        {
            FloatingTexTManager.inst.CreateText(transform.position, new string($"Killed {gameObject.name}"), 0.5f);
            TST_GameManager.KillUnit(this);
            return true;
        }

        return false;
    }

    public TST_Space GetSpace() => TST_Field.GetSpace(CurrentSpace2D);

    public void MakeImmobile()
    {
        MovesLeft = 0;
    }
    
}
