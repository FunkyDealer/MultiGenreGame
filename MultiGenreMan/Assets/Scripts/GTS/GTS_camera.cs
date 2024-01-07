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
    private Vector3 _targetLookAt;

    [SerializeField]
    private bool _active;

    Vector3 _smoothPosition;

    bool _on = true;

    [SerializeField] float _smoothTranslationSpeed = 7;
    [SerializeField] float _smoothLookSpeed = 5;
    [SerializeField] float _smoothRotationSpeed = 5;

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
        if (_active)
        {
            _currentLookAt = Vector3.Lerp(_currentLookAt, _targetLookAt, Time.deltaTime * _smoothLookSpeed);
            _smoothPosition = Vector3.Lerp(transform.position, _targetPosition, _smoothTranslationSpeed * Time.deltaTime);
            _currentUp = Vector3.Lerp(_currentUp, _targetUp, Time.deltaTime * _smoothRotationSpeed);

            //_smoothPosition = Vector3.SmoothDamp(transform.position, myPoint, ref velocity, 0.25f);
            transform.position = _smoothPosition;
            transform.LookAt(_currentLookAt, _currentUp);

        }
    }

    private void LateUpdate()
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

    [SerializeField] float _maxYOffset = 10;
    [SerializeField] float _minYOffset = 2.5f;
    [SerializeField] float _LowestYoffSet = 0.5f;
    [SerializeField] float _maxLookPosOffSet = 10;


    public void UpdateCameraPosition(Vector2 mouse, float sensitivity, float speed, Transform target, Transform lookAtTarget)
    {
        if (_lookPosOffset < 0.01) _yOffset -= mouse.y * sensitivity * speed * Time.deltaTime;

        if (_yOffset >= _maxYOffset) _yOffset = _maxYOffset;
        if (_yOffset <= _minYOffset)
        {
            //_yOffset = _minYOffset;


            

            //raise the look target
            _lookPosOffset -= -mouse.y * sensitivity * speed * Time.deltaTime;
            //_yOffset -= _lookPosOffset;

            if (_yOffset <= _LowestYoffSet) _yOffset = _LowestYoffSet;

            if (_lookPosOffset > _maxLookPosOffSet)
            {
                _lookPosOffset = _maxLookPosOffSet;
            }

            if (_lookPosOffset < 0)
            {
                _lookPosOffset = 0;

            }
        }

        Vector3 myPoint = target.position - target.forward.normalized * _zOffset;
        myPoint += target.up.normalized * _yOffset;
        _targetPosition = myPoint;

        Vector3 lookAtPoint = lookAtTarget.position;
        lookAtPoint += lookAtTarget.up.normalized * _lookPosOffset;
        _targetLookAt = lookAtPoint;        

        _targetUp = target.up;

        GTS_DebugUi.inst.DebugLine("CurrentLookAt", $"CurrentLookat: {_currentLookAt}");

        GTS_DebugUi.inst.DebugLine("TargetLookAt", $"TargetLookAt: {lookAtPoint}");        





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
