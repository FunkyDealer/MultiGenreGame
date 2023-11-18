using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_AiPlayer : TST_Controller
{
    [SerializeField]
    private int _team = 2;

    TST_Unit _currentlySelectedUnit = null;


    bool _commanding = false;


    protected override void Awake()
    {
        Team = _team;

        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //if (MyTurn && Input.GetButtonDown("Submit")) {
        //    StartCoroutine(EndMyTurn());
        //}



    }

    public override void StartMyTurn()
    {
        base.StartMyTurn();


        _commanding = true;

        AILogic();

        StartCoroutine(EndMyTurn(1));

    }

    private void AILogic()
    {
        //
        // start by checking if the unit can attack right after the turn starts
        //
        // find closest enemy -> check if in attack range -> yes -> attack
        //                                  |
        //                                  v
        //                                  no -> find furthest possible place to move to -> move
        //
            

       StartCoroutine(AttackOrder());

       StartCoroutine(MoveOrder());
        
    }

    private IEnumerator AttackOrder()
    {
        Tuple<TST_Unit, TST_Unit> pairing = CheckForPossibleAttacks();

        if (pairing != null)
        {
            pairing.Item1.AttackEnemy(pairing.Item2.GetSpace(), pairing.Item2);

            yield return new WaitForSeconds(1);

            StartCoroutine(AttackOrder()); //try to attack with another unit
        }

        yield return 0;
    }

    private IEnumerator MoveOrder()
    {
        TST_Unit unit = GetUnitThatCanMove();

        if (unit != null)
        {            
            //move unit          
            MoveUnit(unit);
            unit.MakeImmobile(); //this is an hack so that this function doesn't loop forever when the unit can move but doesn't need to (or is stuck and can't move)

            Debug.Log("after moving");

            yield return new WaitForSeconds(1);

            //try to attack with this unit
            if (TryToAttackWithUnit(unit))
            {
                //unit can attack
                yield return new WaitForSeconds(1);
            }//else continue logic

            StartCoroutine(MoveOrder()); //try to move another unit
        }

        _commanding = false;
        yield return 0;
    }

    protected override IEnumerator EndMyTurn(float time)
    {
        yield return new WaitUntil( () => !_commanding );

        MyTurn = false;

        yield return new WaitForSeconds(time);


        TST_GameManager.EndTurn();

    }

    private TST_Unit GetUnitThatCanMove()
    {
        TST_Unit unit = null;

        foreach (var u in _unitList)
        {
            if (u.MovesLeft > 0) unit = u;
        }

        return unit;
    }

    private bool MoveUnit(TST_Unit unit)
    {
        //search closest enemy
        TST_Unit closest = GetClosestEnemyTo(unit);

        if (closest == null) return false; //there are no enemies

        //check if closest isn't right next to our unit
        float dist = Vector2Int.Distance(unit.CurrentSpace2D, closest.CurrentSpace2D);
        if (dist <= unit.GetAttackRange) return false; //don't move if enemy is in attack range   

        List<TST_Space> possibleSpaces = new List<TST_Space>();
        List<int> SeenSpaces = new List<int>();
        SeenSpaces.Add(unit.GetSpace().GetInstanceID());
        Debug.Log("right before recursive");

    
        foreach (TST_Space s in unit.GetSpace().Neighbours)
        {
            RecursiveAICheckMovement(s, ref possibleSpaces, ref SeenSpaces,unit);           
        }
        SeenSpaces.Clear();
        Debug.Log("right after recursive");
        Debug.Log($"Possible spaces for {unit.gameObject.name} is {possibleSpaces.Count}");

        if (possibleSpaces.Count > 0)
        {
            //search closest space to enemy
            TST_Space targetSpace = GetClosestSpaceTo(possibleSpaces, closest);

            //move unit
            if (unit.ValidateMovement(targetSpace.Space2D))
            {
                unit.TeleportToNewSpace(targetSpace.Space2D, targetSpace.Space3D);
            }
            else
            {
                Debug.Log($"Error, for some reason {unit.gameObject.name} could not be moved by AI to target {targetSpace.Space2D}");
                return false;
            }

            return true;
        }

        Debug.Log($"Couldn't Move Unit {unit.gameObject.name} anywhere");
        return false; //cannot move unit anywhere
    }

    //recursive function that goes to each space and checks if it's a valid move
    //seen spaces are the spaces that were already seen
    public void RecursiveAICheckMovement(TST_Space space, ref List<TST_Space> spaces, ref List<int> SeenSpaces, TST_Unit u)
    {
        if (!SeenSpaces.Contains(space.GetInstanceID()))
        { 
        SeenSpaces.Add(space.GetInstanceID());

        if (u.ValidateMovement(space.Space2D))
        {
            if (!space.IsOccupied()) spaces.Add(space);

            foreach (TST_Space s in space.Neighbours)
            {
                if (!SeenSpaces.Contains(s.GetInstanceID())) RecursiveAICheckMovement(s, ref spaces, ref SeenSpaces, u);

            }
        }
    }
    }

    private TST_Space GetClosestSpaceTo(List<TST_Space> spaces, TST_Unit target)
    {
        TST_Space closest = spaces[0];
        float closestDist = Vector2Int.Distance(closest.Space2D, target.CurrentSpace2D);

        if (spaces.Count > 1)
        {
            foreach (var s in spaces)
            {
                float dist = Vector2Int.Distance(s.Space2D, target.CurrentSpace2D);

                if (dist < closestDist)
                {
                    closest = s;
                    closestDist = dist;
                }
            }
        }

        return closest;
    }

    private TST_Unit GetClosestEnemyTo(TST_Unit MyUnit)
    {
        List<TST_Unit> enemies = TST_GameManager.GetListOfEnemies(MyUnit.Team);

        if (enemies.Count == 0) return null;

        TST_Unit closest = enemies[0];
        
        float closestDist = Vector2Int.Distance(MyUnit.CurrentSpace2D, closest.CurrentSpace2D);

        if (enemies.Count > 1)
        {
            foreach (TST_Unit e in enemies)
            {
                if (e.GetInstanceID() == closest.GetInstanceID()) continue;
                float dist = Vector2Int.Distance(MyUnit.CurrentSpace2D, e.CurrentSpace2D);

                if (dist < closestDist)
                {
                    closest = e;
                    closestDist = dist;
                }
            }
        }
        return closest;
    }

    private bool TryToAttackWithUnit(TST_Unit myUnit)
    {
        if (myUnit.AttacksLeft > 0)
        {
            List<TST_Unit> possibleAttacks = new List<TST_Unit>(); //list of possible attacks
            List<TST_Unit> enemyList = TST_GameManager.GetListOfEnemies(Team); //list of enemies

            if (enemyList.Count == 0) return false; //there are no enemies

            foreach (var u in enemyList)
            {
                if (myUnit.ValidateAttackFast(u)) //validate attack
                {
                    possibleAttacks.Add(u);
                }
            }

            if (possibleAttacks.Count > 0)
            {
                if (possibleAttacks.Count == 1) myUnit.AttackEnemy(possibleAttacks[0].GetSpace(), possibleAttacks[0]); //possible attacks are only 1
                else
                {
                    //possible attacks are more than 1, attack one that's weakest
                    TST_Unit enemy = GetWeakestEnemy(possibleAttacks);

                    myUnit.AttackEnemy(enemy.GetSpace(), enemy);

                }
            }
        }
        return false; //couldn't attack anyone
    }

    private Tuple<TST_Unit, TST_Unit> CheckForPossibleAttacks()
    {
        List<TST_Unit> enemyList = TST_GameManager.GetListOfEnemies(Team);

        if (enemyList.Count == 0) return null;

        foreach (var u in _unitList) //go through each of my units
        {
            if (u.AttacksLeft == 0) continue;

            List<TST_Unit> possibleAttacks = new List<TST_Unit>();

            foreach (TST_Unit v in enemyList) //and see if they can attack this unit
            {
               if (u.ValidateAttackFast(v)) //validate attack
                {
                   possibleAttacks.Add(v);
                }
            }

            if (possibleAttacks.Count > 0)
            {
                if (possibleAttacks.Count == 1) return Tuple.Create(u, possibleAttacks[0]); //possible attacks are only 1
                else return Tuple.Create(u, GetWeakestEnemy(possibleAttacks)); //possible attacks are more than 1, attack one that's weakest
            }
        }
        return null; //no possible attacks were found
    }

    private TST_Unit GetWeakestEnemy(List<TST_Unit> possibleAttacks)
    {
        TST_Unit weakest = possibleAttacks[0];

        foreach (var u in possibleAttacks)
        {
            if (weakest == u) continue;
            if (u.GetHealth < weakest.GetHealth) weakest = u;
        }
        return weakest;
    }


    //old scripts 
    private TST_Unit CheckForValidMoves()
    {
        foreach (var u in _unitList)
        {
            if (u.MovesLeft > 0) return u;
        }

        return null;
    }
    
    private Tuple<TST_Unit, TST_Unit> TryToAttackWithOneOfMyUnits()
    {
        foreach (var u in _unitList)
        {
            if (u.AttacksLeft > 0)
            {
                return new Tuple<TST_Unit, TST_Unit>(u,GetWeakestEnemyUnitThatCanBeAttacked(u));
            }
        }

        return null;
    }

    private TST_Unit GetWeakestEnemyUnitThatCanBeAttacked(TST_Unit myUnit)
    {
        List<TST_Unit> list = new List<TST_Unit>(); //list of possible enemies to attack

        for (int w = -3; w <= 3; w++)
        {
            for (int l = -3; l <= 3; l++)
            {
                Vector2Int s = new Vector2Int(myUnit.CurrentSpace2D.x + w, myUnit.CurrentSpace2D.y + l);
                if (!TST_Field.ValidateSpace2D(s)) continue; //check if space is valid (is not outside the grid)
                TST_Space space = TST_Field.GetSpace(s); //get valied space

                if (space.IsOccupied()) //if there is a unit on the space
                {
                    TST_Unit enemy = space.GetUnit(); //get that unit
                    if (enemy.Team == myUnit.Team) continue; //check if that unit isn't of the same team
                    else
                    {
                        list.Add(enemy); //add the enemy to list of possible units that can be attacked
                    }

                }
                else continue;
            }
        }

        if (list.Count > 0) //find the weakest unit
        {
            TST_Unit weakestUnit = list[0];

            foreach (var e in list)
            {
                if (e.GetHealth < weakestUnit.GetHealth) weakestUnit = e;
            }

            return weakestUnit;
        }
        else { return null; }

        //isntead of doing this it should just get a list of enemies and check if any of them are in range
    }

    public override void Defeat()
    {
        base.Defeat();

        TST_GameManager.CheckForPlayerVictory();
    }

    public override void StopPlaying()
    {
        StopAllCoroutines();
    }
    


}
