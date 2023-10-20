using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_WarriorUnit : TST_Unit
{



    protected override void Awake()
    {
        base.Awake();

        gameObject.name = $"Warrior (Team {Team})";
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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();


    }
}
