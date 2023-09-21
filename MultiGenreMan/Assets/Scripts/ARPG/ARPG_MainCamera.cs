using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPG_MainCamera : MonoBehaviour
{
    [SerializeField, Range(5, 20)]
    private int _xOffset = 5;
    [SerializeField, Range(5, 20)]
    private int _yOffset = 5;
    [SerializeField, Range(5, 20)]
    private int _zOffset = 5;

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

        transform.LookAt(_playerObject.position);

        _player.GetCamera(this);

    }

    // Update is called once per frame
    void Update()
    {

        UpdateCameraPositon();


       




        if (Input.GetButtonDown("Fire2"))
        {
            Vector3 newPos = _playerObject.position;

           // Vector3 mouseScreenPos = Input.mousePosition;
            //mouseScreenPos.z = _myCamera.nearClipPlane;
            //Vector3 mouseWorldPos = _myCamera.ScreenToWorldPoint(mouseScreenPos);

            //float distance;
            Ray ray = _myCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                newPos = hit.point;
            }

            _cursorIndicator.transform.position = newPos;



        }


    }

    private void UpdateCameraPositon()
    {

        _offset = new Vector3(-_xOffset, _yOffset, -_zOffset);

        transform.position = _playerObject.position + _offset;
        transform.LookAt(_playerObject.position);
    }





}
