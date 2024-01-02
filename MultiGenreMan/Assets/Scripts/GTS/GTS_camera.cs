using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_camera : MonoBehaviour
{
    private static GTS_camera _instance;
    public static GTS_camera inst { get { return _instance; } }


    private Transform _FollowTarget;

    [SerializeField, Range(0, 20)]
    private float _xOffset = 5;
    [SerializeField, Range(0, 20)]
    private float _yOffset = 5;
    [SerializeField, Range(0, 20)]
    private float _zOffset = 5;

    private float pitch = 0;

    private Vector3 _offset = Vector3.zero;

    private Camera _myCamera;

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


        _FollowTarget = null;

        _myCamera = gameObject.GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _FollowTarget = GTS_GameManager.inst.Player.transform;

        SetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        //if (_FollowTarget != null)
        //{

        //    UpdateCameraPositon();


        //    UpdateCameraRotation();


        //}


    }

    private void FixedUpdate()
    {
        
    }

    private void SetPosition()
    {
        transform.position = _FollowTarget.position;

        _offset = new Vector3(_xOffset, _yOffset, -_zOffset);

        transform.position += GTS_GameManager.inst.Player.OffSet;

        //transform.LookAt(_lookAtTarget.position);
    }

    private void UpdateCameraPositon()
    {
        _offset = new Vector3(_xOffset, _yOffset, -_zOffset);

        Vector3 target = _FollowTarget.TransformPoint(GTS_GameManager.inst.Player.OffSet);

        //transform.position = _FollowTarget.position + _offset;
        transform.position = target;     



    }

    private void UpdateCameraRotation()
    {
       // transform.rotation = _FollowTarget.rotation;

       // transform.LookAt(_lookAtTarget.position, _lookAtTarget.up);
    }

    private float _lookPosOffset = 0;

    public void UpdateCameraPosition(Vector2 mouse, float sensitivity, float speed, Transform target, Transform lookAtTarget)
    {
        if (_lookPosOffset < 0.01) _yOffset -= mouse.y * sensitivity * speed * Time.deltaTime;

        if (_yOffset >= 10) _yOffset = 10;
        if (_yOffset <= 3)
        {
            _yOffset = 3;

            //raise the look target

            _lookPosOffset -= -mouse.y * sensitivity * speed * Time.deltaTime;

            if (_lookPosOffset > 5)
            {
                _lookPosOffset = 5;
            }

            if (_lookPosOffset < 0)
            {
                _lookPosOffset = 0;

            }

        }

       // GTS_DebugUi.inst.ChangeDebugLine1($"yOffset: {_yOffset}");
       // GTS_DebugUi.inst.ChangeDebugLine2($"LookPosOffset: {_lookPosOffset}");

        Vector3 myPoint = target.position - target.forward.normalized * _zOffset;
        myPoint += target.up.normalized * _yOffset;


        Vector3 lookAtPoint = lookAtTarget.position;

        lookAtPoint += lookAtTarget.up.normalized * _lookPosOffset;

        //pitch += mouse.y * sensivity * speed * Time.deltaTime;

        //rotate the camera arround the target's right vector
        transform.position = myPoint;

        transform.RotateAround(target.position, target.right, pitch);

        


        transform.LookAt(lookAtPoint, target.up);

        //transform.rotation = target.rotation;
    }

}
