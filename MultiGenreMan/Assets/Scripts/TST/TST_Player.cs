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

    // Start is called before the first frame update
    protected override void Start()
    {
        Team = 1;


        base.Start();

        




    }

    // Update is called once per frame
    void Update()
    {
        _movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


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
                _currentlySelectedSpace.Deselect();
                _currentlySelectedSpace = null;
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

    public bool IssueMovementOrder(GameObject g) //true if sucesseful, false if uncesseseful
    {
        TST_Space s = g.GetComponent<TST_Space>();

        if (s.IsOccupied()) return false;
        if (s.Space2D == _currentlySelectedSpace.Space2D) return false; //if old space is the same space

        TST_Unit u = _currentlySelectedSpace.GetUnit();
        if (!u.ValidateMovement(s.Space2D)) return false;

        if (u.MovesLeft > 0) u.TeleportToNewSpace(s.Space2D, s.Space3D);



        return true;
    }

    public void DeselectSpace()
    {
        _currentlySelectedSpace = null;
    }

    public bool IsUnitSelected()
    {
        if (_currentlySelectedSpace == null) return false;

        TST_Unit u = _currentlySelectedSpace.GetUnit();

        return u != null && u.Team == Team;
    } 




}
