using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_GameManager : MonoBehaviour
{
    private static TST_GameManager _instance;
    public static TST_GameManager inst { get { return _instance; } }

    private static List<TST_Unit> _units = new List<TST_Unit>();
    private static Dictionary<int,TST_Controller> _players = new Dictionary<int, TST_Controller>();

    public static int CurrentPlayer { get; private set; } = 0;

    public static int maxPlayers { get; private set; } = 2;

    private static List<GameObject> _movementIndicators = new List<GameObject>();

    [SerializeField]
    private GameObject _movementIndicatorObj;
    public GameObject GetMovementIndicatorObj => _movementIndicatorObj;


    [SerializeField]
    private GameObject _pauseMenuPrefab;

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

        Instantiate(_pauseMenuPrefab);
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

        CurrentPlayer = 0;

        Debug.Log($"starting units, count: {_units.Count}");
        foreach (var u in _units)
        {
            Vector2Int space = u.CurrentSpace2D;
            
            //sanitization
            if (space.x > TST_Field._totalWidth -1) space.x = TST_Field._totalWidth -1;
            if (space.y > TST_Field._totalLength -1) space.y = TST_Field._totalLength -1;
            if (space.x < 0) space.x = 0;
            if (space.y < 0) space.y = 0;   
            
            Vector3 newSpace3d = TST_Field.GetSpace(space).Space3D;

            u.TeleportToNewSpace(space, newSpace3d);

        }
        
        EndTurn();
    }

    public static void RegisterUnit(TST_Unit u)
    {
        _units.Add(u);

        _players[u.Team].RegisterUnit(u);
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
        Debug.Log($"it's now player {team}'s turn");

        TST_UIController.inst.SetTurnIndicator(team);

        foreach (var u in _units)
        {
            if (u.Team == team) u.resetTurn();
        }

        _players[team].StartMyTurn();
    }  



    public static void KillUnit(TST_Unit u)
    {
        _units.Remove(u);
        _players[u.Team].RemoveUnit(u);        

        Destroy(u.gameObject);

       if (_players[u.Team].CheckForDefeat()) _players[u.Team].Defeat();

    }

    public static void AddMovementIndicator(GameObject m)
    {
        _movementIndicators.Add(m);
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

    public static List<TST_Unit> GetListOfEnemies(int myTeam)
    {
        List<TST_Unit> enemyList = new List<TST_Unit>();

        foreach (TST_Unit u in _units)
        {
            if (u.Team != myTeam) enemyList.Add(u);
        }

        return enemyList;
    }

    public static void CheckForPlayerVictory()
    {
        bool playerVictory = true;

        foreach (var p in _players.Values)
        {
            if (p.Team == 1) continue;

            if (!p.Defeated) playerVictory = false;
        }

        if (playerVictory)
        {
            (_players[1] as TST_Player).Victory();
        }

    }

    public static void StopTheGame()
    {
        foreach (var p in _players.Values)
        {
            p.StopPlaying();
        }
    }

    public static void EndGame()
    {
        StopTheGame();

        _players.Clear();
        _units.Clear();
    }

    public static void PauseGame()
    {
        Time.timeScale = 0;

        foreach (var p in _players.Values)
        {
            p.Pause(true);
        }
    }

    public static void UnpauseGame()
    {
        Time.timeScale = 1;

        foreach (var p in _players.Values)
        {
            p.Pause(false);
        }
    }

}
