using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_Controller : MonoBehaviour
{
    public int Team { get; protected set; }

    public bool MyTurn { get; protected set; } = false;

    protected List<TST_Unit> _unitList;

    public bool Defeated { get; protected set; } = false;

    protected virtual void Awake()
    {
        _unitList = new List<TST_Unit>();

        TST_GameManager.RegisterPlayer(Team, this);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void StartMyTurn()
    {
        

        MyTurn = true;
    }

    protected virtual IEnumerator EndMyTurn(float time)
    {
        MyTurn = false;

        yield return new WaitForSeconds(time);

        
        TST_GameManager.EndTurn();

    }

    public void RegisterUnit(TST_Unit u)
    {
        _unitList.Add(u);
    }

    public void RemoveUnit(TST_Unit u)
    {
        _unitList.Remove(u);
    }

    public virtual void StopPlaying()
    {

    }

    public virtual bool CheckForDefeat()
    {
        if (_unitList.Count <= 0) return true;

        return false;
    }

    public virtual void Defeat()
    {
        Defeated = true;
        EndGame();

    }

    public virtual void EndGame()
    {
        _unitList.Clear();
    }
}
