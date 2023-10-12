using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_Player : MonoBehaviour
{
    private TST_MainCamera _camera;

    [SerializeField]
    private int _team = 1;

    public int Team { get; private set; }
    private int _unitCount = 0;
    private int _buildingCount = 0;

    [SerializeField]
    private int _cameraPanSpeed = 20;

    private Vector2 _movementInput = Vector2.zero;


    [SerializeField]
    GameObject _cursorIndicator;



    // Start is called before the first frame update
    void Start()
    {
  

        Team = _team;




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

    TST_Space _currentlySelectedSpace = null;

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


        }


    }

   // public bool IsSomethingSelected() => _SelectedEntitiesAmmount > 0;
}
