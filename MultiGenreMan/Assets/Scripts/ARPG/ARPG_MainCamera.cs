using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPG_MainCamera : MonoBehaviour
{
    [SerializeField, Range(5, 20)]
    private float _xOffset = 5;
    [SerializeField, Range(5, 20)]
    private float _yOffset = 5;
    [SerializeField, Range(5, 20)]
    private float _zOffset = 5;

    [SerializeField]
    private float _zoomSpeed = 5;

    [SerializeField]
    private float _distanceFromTarget;

    private Vector3 _offset = Vector3.zero;

    [SerializeField]
    private Transform _playerObject;
    private ARPG_Player _player;

    private Camera _myCamera;


    [SerializeField]
    GameObject _cursorIndicator;

    private void Awake()
    {
        _player = _playerObject.gameObject.GetComponent<ARPG_Player>();
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





        IssuePlayerOrder();



        ZoomCamera();





    }

    private void UpdateCameraPositon()
    {

        //_offset = new Vector3(-_xOffset, _yOffset, -_zOffset);

        // transform.position = _playerObject.position + _offset;
        transform.position = _playerObject.position + _offset;
        transform.LookAt(_playerObject.position);
    }

    private void IssuePlayerOrder()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            Vector3 newPos = _playerObject.position;

            // Vector3 mouseScreenPos = Input.mousePosition;
            //mouseScreenPos.z = _myCamera.nearClipPlane;
            //Vector3 mouseWorldPos = _myCamera.ScreenToWorldPoint(mouseScreenPos);

            //float distance;
            Ray ray = _myCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            ARPG_Creature creature = null;
            if (Physics.Raycast(ray, out hit, 100))
            {
                newPos = hit.point;

                creature = hit.collider.gameObject.GetComponent<ARPG_Creature>();

            } 
            else
            {

            }
            ARPG_PlayerOrder order = new ARPG_PlayerOrder();

            if (creature != null)
            {

                if (creature is ARPG_Enemy) order = new ARPG_PlayerAttackOrder(creature);               

            }
            else
            {

                _cursorIndicator.transform.position = newPos;

                //issue order
                 order = new ARPG_PlayerMoveOrder(newPos);

               
            }

            _player.ReceiveOrder(order);

        }
    }

    private void ZoomCamera()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            _distanceFromTarget = Vector3.Distance(_playerObject.position, transform.position);

            Vector3 directionToTarget = _playerObject.position - transform.position;


            _offset *= (1 - (scrollInput * _zoomSpeed) / _offset.magnitude); //shortens or makes the offset vector bigger


            if (_distanceFromTarget >= 5 && _distanceFromTarget <= 30) transform.position = _playerObject.position + _offset;
            if (_distanceFromTarget > 30)
            {
                _offset = _offset.normalized * 30;
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
