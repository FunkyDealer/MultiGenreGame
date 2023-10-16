using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_GameManager : MonoBehaviour
{
    private static TST_GameManager _instance;
    public static TST_GameManager inst { get { return _instance; } }

    private static List<TST_Unit> _units;
    private static Dictionary<int,TST_Controller> _players; 

    public static int CurrentPlayer { get; private set; } = 0;

    public static int maxPlayers { get; private set; } = 2;

    private static List<GameObject> _movementIndicators;

    [SerializeField]
    private GameObject _movementIndicatorObj;

    private void Awake()
    {       

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        _units = new List<TST_Unit>();
        _players = new Dictionary<int, TST_Controller>();
        _movementIndicators = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public static IEnumerator StartUnits()
    {
        yield return new WaitForFixedUpdate();

        Debug.Log($"starting units, count: {_units.Count}");
        foreach (var u in _units)
        {
            Vector2Int space = u.CurrentSpace;
            
            //sanitization
            if (space.x > TST_Field._width -1) space.x = TST_Field._width -1;
            if (space.y > TST_Field._length -1) space.y = TST_Field._length -1;
            if (space.x < 0) space.x = 0;
            if (space.y < 0) space.y = 0;   
            
            TST_SpaceInfo newSpace = TST_Field.GetSpaceInfo(space);
            Vector3 newSpace3d = newSpace._pos;

            u.TeleportToNewSpace(space, newSpace3d);

        }

        EndTurn();
    }

    public static void RegisterUnit(TST_Unit u)
    {
        _units.Add(u);
    }

    public static void RegisterPlayer(int team, TST_Controller c)
    {
        _players.Add(team,c);
    }

    public static void EndTurn()
    {
        CurrentPlayer++;

        if (CurrentPlayer > maxPlayers) CurrentPlayer = 1;

        StartTurn(CurrentPlayer);
    }

    public static void StartTurn(int team)
    {
        foreach (var u in _units)
        {
            if (u.Team == team) u.resetTurn();
        }
    }

    public static void CreateMovementIndicators(TST_Unit u)
    {
        for (int w = -10; w <= 10; w++)
        {
            for (int l = -10; l <= 10; l++)
            {
                Vector2Int s = new Vector2Int(u.CurrentSpace.x + w,  u.CurrentSpace.y + l);


                    if (TST_Field.ValidateSpace2D(s) && u.ValidateMovement(s))
                    {
                        Vector3 pos = TST_Field.GetSpace3D(s);

                        GameObject g = Instantiate(inst._movementIndicatorObj, pos, inst._movementIndicatorObj.transform.rotation);
                        g.transform.localScale = new Vector3(TST_Field.GetSpaceSize(), TST_Field.GetSpaceSize(), g.transform.localScale.z);
                        _movementIndicators.Add(g);
                    }
                

            }
        }
    }

    public static void DestroyMovementIndicators()
    {
        if (_movementIndicators.Count > 0)
        {
            foreach (var a in _movementIndicators)
            {
                Destroy(a);
            }

            _movementIndicators.Clear();
        }



    }

}
