using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_EnemyManager : MonoBehaviour
{
    private static GTS_EnemyManager _instance;
    public static GTS_EnemyManager inst { get { return _instance; } }

    private Dictionary<int, GTS_Enemy> _enemyList;

    private int _enemyTotalNr = 0;

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


        _enemyList = new Dictionary<int, GTS_Enemy>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void RegisterEnemy(int id, GTS_Enemy enemy)
    {
        if (!_enemyList.ContainsKey(id))
        {
            _enemyList.Add(id, enemy);
            _enemyTotalNr++;
        }


    }

    public void RemoveEnemy(int id)
    {
        if (_enemyList.ContainsKey(id))
        {
            _enemyList.Remove(id);
            _enemyTotalNr--;
        }


    }



}
