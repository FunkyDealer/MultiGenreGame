using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_Controller : MonoBehaviour
{
    public int Team { get; protected set; }

    public bool MyTurn { get; protected set; } = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {


        TST_GameManager.RegisterPlayer(Team ,this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void StartMyTurn()
    {
        

        MyTurn = true;
    }

    protected virtual IEnumerator EndMyTurn()
    {
        MyTurn = false;

        yield return 0;

        
        TST_GameManager.EndTurn();

    }
}
