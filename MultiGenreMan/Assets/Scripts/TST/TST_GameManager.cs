using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_GameManager : MonoBehaviour
{
    private static TST_GameManager _instance;
    public static TST_GameManager inst { get { return _instance; } }

    [SerializeField]
    private static TST_Field _field;

    [SerializeField]
    private static TST_Player _player;

    [SerializeField]
    private static List<TST_Unit> _units;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public static void StartUnits()
    {

        foreach (var u in _units)
        {
            Vector2Int space = u._currentSpace;
            
            //sanitization
            if (space.x > TST_Field._width -1) space.x = TST_Field._width -1;
            if (space.y > TST_Field._length -1) space.y = TST_Field._length -1;
            if (space.x < 0) space.x = 0;
            if (space.y < 0) space.y = 0;   
            
            TST_SpaceInfo newSpace = TST_Field.GetSpaceInfo(space);
            Vector3 newSpace3d = newSpace._pos;

            u.TeleportToNewSpace(space, newSpace3d);




        }


    }

    public static void RegisterUnit(TST_Unit u)
    {
        _units.Add(u);
    }


}
