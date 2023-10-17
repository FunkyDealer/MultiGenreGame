using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_AiPlayer : TST_Controller
{
    [SerializeField]
    private int _team = 2;

    TST_Unit _currentlySelectedUnit = null;

    bool _possibleMoves = true;

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        Team = _team;

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

        _possibleMoves = true;

        StartCoroutine(EndMyTurn());


    }

    private void AILogic()
    {
        //
        // find closest enemy -> check if in attack range -> yes -> attack
        //                                  |
        //                                  v
        //                                  no -> find furthest possible place to move to -> move
        //

        //Get unit




        //get unit that still has moves or attacks




        
    }

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
                Vector2Int s = new Vector2Int(myUnit.CurrentSpace.x + w, myUnit.CurrentSpace.y + l);
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

    

    


}
