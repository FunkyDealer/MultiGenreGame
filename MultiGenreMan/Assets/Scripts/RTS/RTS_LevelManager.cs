using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_LevelManager : MonoBehaviour
{
    private static RTS_LevelManager _instance;
    public static RTS_LevelManager inst { get { return _instance; } }

    [SerializeField]
    private List<GameObject> _spawnPoints = new List<GameObject>();
    [SerializeField]
    private static List<GameObject> _spawnP = new List<GameObject>();
    


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

        _spawnP = _spawnPoints;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void ChooseSpawnPoints(RTS_Player player)
    {

        if (_spawnP.Count > 0)
        {
            Vector3 spawnPoint = _spawnP[0].transform.position;

            player.transform.position = spawnPoint;
        }
         else
        {
            player.transform.position = Vector3.zero;
        }

    }

    public static Vector3 GetSpawnPoint(int team)
    {
        return _spawnP[team-1].transform.position;


    }
}
