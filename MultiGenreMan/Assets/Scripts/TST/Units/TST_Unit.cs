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
    protected int movement = 1;

    [SerializeField]
    public int Team { get; private set; } = 1;


    [SerializeField]
    protected int _startingWidth = 1;
    [SerializeField]
    protected int _startingLength = 1;

    public Vector2Int CurrentSpace { get; private set; }

    public int MovesLeft { get; private set; } = 1;




    protected virtual void Awake()
    {
        

        CurrentSpace = new Vector2Int(_startingWidth -1, _startingLength -1);

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
        TST_Field.RemoveUnitFromSpace(CurrentSpace);

        Debug.Log($"going to space {space2d}, in {space3d}");
        CurrentSpace = space2d;

        TST_Field.SetUnitInSpace(CurrentSpace, this);
        transform.position = new Vector3(space3d.x, transform.position.y, space3d.z);

        MovesLeft--;


    }

    //validating movement is done by calculating distance between the two points and checking if it greater or equal than the movement of the unit
    public bool ValidateMovement(Vector2Int s)
    {

        //calculate Cost
        //float dist = Vector2Int.Distance(s, CurrentSpace);
        float dist = Mathf.Sqrt(Mathf.Pow(CurrentSpace.x - s.x, 2) + Mathf.Pow(CurrentSpace.y - s.y, 2));

        if (dist <= movement) return true;

      

        return false;
    }

    public void resetTurn()
    {
        MovesLeft = 1;
    }

}
