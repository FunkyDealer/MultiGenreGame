using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_MainCamera : MonoBehaviour
{
    [SerializeField, Range(-30, 30)]
    private float _xOffset = 5;
    [SerializeField, Range(0, 30)]
    private float _yOffset = 5;
    [SerializeField, Range(-30, 30)]
    private float _zOffset = 5;

    [SerializeField]
    private float _zoomSpeed = 5;

    [SerializeField]
    private float _distanceFromTarget;

    private Vector3 _offset = Vector3.zero;

    [SerializeField]
    private Transform _playerObject;
    private RTS_Player _player;

    private Camera _myCamera;

    [SerializeField]
    GameObject _cursorIndicator;    

    private void Awake()
    {
        _player = _playerObject.gameObject.GetComponent<RTS_Player>();
        _myCamera = gameObject.GetComponent<Camera>();

    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _playerObject.position;

        _offset = new Vector3(-_xOffset, _yOffset, -_zOffset);

        transform.position += _offset;
        _distanceFromTarget = _offset.magnitude;

        transform.LookAt(_playerObject.position);

        _player.GetCamera(this);

    }


    // Update is called once per frame
    void Update()
    {
        UpdateCameraPositon();


        ZoomCamera();

        
        ClickEnvironment();
    }

    private void FixedUpdate()
    {


        MouseMapNavigation();


    }

    //interaction by mouse with the environment
    private void ClickEnvironment()
    {
        //selecting
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 newPos = _playerObject.position;            

            //float distance;
            Ray ray = _myCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
           
            if (Physics.Raycast(ray, out hit, 100))
            {
                newPos = hit.point;

                RTS_ClickableEntity entity = null;

                entity = hit.collider.gameObject.GetComponent<RTS_ClickableEntity>();

                //Debug.DrawLine(ray.origin, newPos, Color.red, 1);

                if (entity != null) //if we clicked a clickable entity
                {
                    _cursorIndicator.transform.position = entity.transform.position + Vector3.up * 3;

                    _player.SelectEntity(entity); //select it
                }
                else //else deselect
                {
                    _player.DeselectEntity();
                    _cursorIndicator.transform.position = newPos;
                }


            }
            else
            {
                Debug.Log("Nothing was clicked on");
            }  




        }
        //order issuing
        if (Input.GetButton("Fire2") && _player.IsSomethingSelected())
        {
            Ray ray = _myCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit, 100))
            {
                RTS_PlayerMoveOrder moveOrder = new RTS_PlayerMoveOrder(hit.point);
                //Debug.Log("Movement order Issued");
                _player.IssueOrder(moveOrder);
            }
            else
            {
                Debug.Log("Nothing was clicked on");
            }



        }

    }

    //moving the camera with the mouse by dragging to the edges
    private void MouseMapNavigation()
    {
        Vector3 mouseScreenPos = Input.mousePosition;


        //if (mouseScreenPos.x <= 0) _player.MoveCameraHorizontally(-1); //move Left 

        //if (mouseScreenPos.x >= Screen.width) _player.MoveCameraHorizontally(1); //move right

        //if (mouseScreenPos.y < 0) _player.MoveCameraVertically(-1); //move down

        //if (mouseScreenPos.y > Screen.height) _player.MoveCameraVertically(1); //move up

    }

    //Update camera position to the pivot
    private void UpdateCameraPositon()
    {

        //_offset = new Vector3(-_xOffset, _yOffset, -_zOffset);

        // transform.position = _playerObject.position + _offset;
        transform.position = _playerObject.position + _offset;
        transform.LookAt(_playerObject.position);
    }


    //Zoom the camera up and down
    private void ZoomCamera()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            _distanceFromTarget = Vector3.Distance(_playerObject.position, transform.position);

            Vector3 directionToTarget = _playerObject.position - transform.position;


            _offset *= (1 - (scrollInput * _zoomSpeed) / _offset.magnitude); //shortens or makes the offset vector bigger


            if (_distanceFromTarget >= 5 && _distanceFromTarget <= 40) transform.position = _playerObject.position + _offset;
            if (_distanceFromTarget > 40)
            {
                _offset = _offset.normalized * 40;
                transform.position = _playerObject.position + _offset;
            }
            if (_distanceFromTarget < 5)
            {
                _offset = _offset.normalized * 5;
                transform.position = _playerObject.position + _offset;
            }
        }
    }


}
