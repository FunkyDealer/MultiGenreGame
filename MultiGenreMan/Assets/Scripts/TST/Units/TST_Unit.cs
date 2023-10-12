using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_Unit : MonoBehaviour
{
    [SerializeField]
    protected int _maxHealth = 100;
    protected int _currentHealth;

    [SerializeField]
    protected int _maxMana = 100;
    protected int _currentMana;

    [SerializeField]
    protected int power = 1;
    [SerializeField]
    protected int magic = 1;

    [SerializeField]
    protected int team = 1;


    [SerializeField]
    private int _startingWidth = 1;
    [SerializeField]
    private int _startingLength = 1;
    
    public Vector2Int _currentSpace { get; private set; }



    protected virtual void Awake()
    {
        

        _currentSpace = new Vector2Int(_startingWidth -1, _startingLength -1);

        _currentHealth = _maxHealth;
        _currentMana = _maxMana;


    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        TST_GameManager.RegisterUnit(this);


    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {

    }

    public void TeleportToNewSpace(Vector2Int space2d, Vector3 space3d)
    {
        _currentSpace = space2d;


        transform.position = new Vector3(space3d.x, transform.position.y, space3d.z);




    }

}
