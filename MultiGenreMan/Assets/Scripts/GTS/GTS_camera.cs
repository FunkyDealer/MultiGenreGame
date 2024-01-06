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

    private Vector3 _currentUp;
    private Vector3 _targetUp;

    private Vector3 _currentLookAt;

    private Vector3 _targetPosition;

    private Quaternion _targetRotation;

    [SerializeField]
    private bool _active;

    Vector3 _smoothPosition;

    bool _on = true;

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


    }

    // Update is called once per frame
    void Update()
    {



    }

    private void FixedUpdate()
    {
        
    }

    public void SetPosition(Transform target, Transform lookAtTarget)
    {

        Vector3 myPoint = target.position - target.forward.normalized * _zOffset;
        myPoint += target.up.normalized * _yOffset;

        if (_active)
        {
            transform.position = myPoint;
            transform.LookAt(lookAtTarget.position, target.up);
        }

        _currentUp = transform.up;
        _currentLookAt = lookAtTarget.position;
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

        Vector3 myPoint = target.position - target.forward.normalized * _zOffset;
        myPoint += target.up.normalized * _yOffset;

        Vector3 lookAtPoint = lookAtTarget.position;

        lookAtPoint += lookAtTarget.up.normalized * _lookPosOffset;

        _currentLookAt = Vector3.Lerp(_currentLookAt, lookAtPoint, Time.deltaTime * 5);
        _currentUp = Vector3.Lerp(_currentUp, target.up, Time.deltaTime * 5);

        GTS_DebugUi.inst.DebugLine("CurrentLookAt", $"CurrentLookat: {_currentLookAt}");

        GTS_DebugUi.inst.DebugLine("TargetLookAt", $"TargetLookAt: {lookAtPoint}");        

        if (_active)
        {
            
            _smoothPosition = Vector3.Lerp(transform.position, myPoint, Time.deltaTime * 9  );
            //_smoothPosition = Vector3.SmoothDamp(transform.position, myPoint, ref velocity, 0.25f);
            transform.position = _smoothPosition;
            transform.LookAt(lookAtPoint, _currentUp);
            
        }



    }

    public void SwitchOff()
    {
        _on = false;

        _myCamera.enabled = _on;



    }

    public void SwitchOn()
    {
        _on = true;

        _myCamera.enabled = _on;
    }


    Vector3 velocity = Vector3.zero;

}
