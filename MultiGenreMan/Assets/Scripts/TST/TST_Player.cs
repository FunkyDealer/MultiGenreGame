using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_Player : TST_Controller
{
    private TST_MainCamera _camera;  

    [SerializeField]
    private int _cameraPanSpeed = 20;

    private Vector2 _movementInput = Vector2.zero;

    [SerializeField]
    GameObject _cursorIndicator;

    TST_Space _currentlySelectedSpace = null;

    protected override void Awake()
    {
        Team = 1;

        base.Awake();


    }

    // Start is called before the first frame update
    protected override void Start()
    {
        


        base.Start();

        




    }

    // Update is called once per frame
    void Update()
    {
        _movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));



        if (MyTurn && Input.GetButtonDown("Submit")) {
            StartCoroutine(EndMyTurn(0.1f));
        }

    }

    private void FixedUpdate()
    {
        Vector3 direction = Vector3.forward * _movementInput.x + Vector3.right * _movementInput.y;

        //transform.position += direction * _cameraPanSpeed * Time.deltaTime;

        MoveCameraHorizontally(_movementInput.x);

        MoveCameraVertically(_movementInput.y);

        //if (IsSomethingSelected())
        //{
        //    _cursorIndicator.transform.position = _currentlySelectedEntities.First().Value.transform.position + Vector3.up * 3;
        //}


    }


    protected override IEnumerator EndMyTurn(float time)
    {
        DeselectSpace();

        return base.EndMyTurn(time);

        
    }

    public void GetCamera(TST_MainCamera mainCamera) => this._camera = mainCamera;

    public void MoveCameraHorizontally(float input) => transform.position += Vector3.right * input * _cameraPanSpeed * Time.deltaTime;

    public void MoveCameraVertically(float input) => transform.position += Vector3.forward * input * _cameraPanSpeed * Time.deltaTime;

    //return true if sucessful in selecting something, false if otherwise
    public bool TryToSelectEntities(GameObject o, bool adding)
    {



        //SelectSingleEntity();

      

        return false;
    }



    public void SelectSpace(GameObject g)
    {
        TST_Space newSpace = g.GetComponent<TST_Space>();

        if (newSpace != null)
        {
            if (_currentlySelectedSpace != null)
            {
                DeselectSpace();
            }

            _currentlySelectedSpace = newSpace;
            newSpace.OnClick();
            SelectUnit();
            
        }
    }

    private void SelectUnit()
    {
        if (_currentlySelectedSpace.IsOccupied())
        {
            TST_GameManager.CreateMovementIndicators(_currentlySelectedSpace.GetUnit());
        }



    }

    public bool IssueOrder(GameObject g)
    {
        TST_Space s = g.GetComponent<TST_Space>();

        if (s.Space2D == _currentlySelectedSpace.Space2D) return false; //if old space is the same space

        if (s.IsOccupied())
        {
             return IssueAttackOrder(s, _currentlySelectedSpace.GetUnit(), s.GetUnit());

        }
         else return IssueMovementOrder(s);                   
    }

    private bool IssueAttackOrder(TST_Space s, TST_Unit attacker, TST_Unit defender)
    {
        return attacker.AttackEnemy(s, defender);
    }

    public bool IssueMovementOrder(TST_Space s) //true if sucesseful, false if uncesseseful
    {
        TST_Unit u = _currentlySelectedSpace.GetUnit();
        if (!u.ValidateMovementOrder(s.Space2D)) return false;


        u.TeleportToNewSpace(s.Space2D, s.Space3D);
        DeselectSpace();
        return true;

    }

    public void DeselectSpace()
    {
        if (_currentlySelectedSpace != null)
        {
            _currentlySelectedSpace.Deselect();
            _currentlySelectedSpace = null;
        }
    }

    public bool IsUnitSelected()
    {
        if (_currentlySelectedSpace == null) return false;

        TST_Unit u = _currentlySelectedSpace.GetUnit();

        return u != null && u.Team == Team;
    } 




}
