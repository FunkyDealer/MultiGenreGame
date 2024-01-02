using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_GameManager : MonoBehaviour
{
    private static GTS_GameManager _instance;
    public static GTS_GameManager inst { get { return _instance; } }

    [SerializeField]
    private GTS_Player _player;

    public GTS_Player Player { get { return _player; } }

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


    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
