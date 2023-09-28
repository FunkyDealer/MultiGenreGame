using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_GameManager : MonoBehaviour
{
    private static RTS_GameManager _instance;
    public static RTS_GameManager inst { get { return _instance; } }

    [SerializeField]
    private GameObject _builderUnitPrefab;
    public static GameObject GetBuilderUnityPrefab => _instance._builderUnitPrefab;



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
