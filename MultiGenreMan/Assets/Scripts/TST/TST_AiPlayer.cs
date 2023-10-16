using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_AiPlayer : TST_Controller
{
    [SerializeField]
    private int _team = 2;

    // Start is called before the first frame update
    protected override void Start()
    {
        Team = _team;

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (MyTurn && Input.GetButtonDown("Submit")) {
            StartCoroutine(EndMyTurn());
        }



    }


}
