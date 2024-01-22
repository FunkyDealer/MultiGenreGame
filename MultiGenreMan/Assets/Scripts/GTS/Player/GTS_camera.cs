using System;
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

    private Vector3 _currenTPStLookAt;
    private Vector3 _currentTPSPosition;
    private Vector3 _targetTPSPosition;
    private Quaternion _targetRotation;
    private Vector3 _targetTPSLookAt;

    [SerializeField]
    private bool _active;

    Vector3 _smoothPosition;

    bool _on = true;

    [SerializeField] float _smoothTranslationSpeed = 7;
    [SerializeField] float _smoothLookSpeed = 5;
    [SerializeField] float _smoothRotationSpeed = 5;

    private float _lookPosOffset = 0;

    [SerializeField] float _maxYOffset = 10;
    [SerializeField] float _minYOffset = 2.5f;
    [SerializeField] float _LowestYoffSet = 0.5f;
    [SerializeField] float _maxLookPosOffSet = 10;

    private float _pitch = 0;

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

    //public bool CheckForBlocked()
    //{

    //}

    private void FixedUpdate()
    {
       
    }

    private void LateUpdate()
    {

    }

    public bool CheckIfBlocked(Transform target)
    {
        //check if there is something blocking the camera
        if (Physics.CheckSphere(_targetTPSPosition, 0.5f))
        {
            return true;
        }

        Vector3 direction = target.position - _targetTPSPosition;

        float distance = Vector3.Distance(target.position, _targetTPSPosition);
        RaycastHit hit;
        if (Physics.Raycast(_targetTPSPosition, direction, out hit, distance)) //There is something blocking us
        {
            

            if (hit.collider.gameObject.CompareTag("Player"))
            {
                Debug.DrawLine(_targetTPSPosition, transform.position + direction * distance, Color.red);
                return false;
            }
            Debug.DrawLine(_targetTPSPosition, _targetTPSPosition + direction * distance, Color.green);
            return true;
        }
        else
        {
            Debug.DrawLine(_targetTPSPosition, _targetTPSPosition + direction * distance, Color.red);
        }

        return false;
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
        _currenTPStLookAt = lookAtTarget.position;
    }

    public void UpdateCameraValues(Vector2 mouse, float sensitivity, float speed, Transform target, Transform lookAtTarget)
    {
        _pitch += mouse.y * -sensitivity * 20 * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, -50, 50);

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
        _targetTPSPosition = myPoint;

        Vector3 lookAtPoint = lookAtTarget.position;
        lookAtPoint += lookAtTarget.up.normalized * _lookPosOffset;
        _targetTPSLookAt = lookAtPoint;

        _targetUp = target.up;

    }

    public void UpdateTPSCamera(bool inTPS)
    {     
        if (_active)
        {
            _currenTPStLookAt = Vector3.Lerp(_currenTPStLookAt, _targetTPSLookAt, Time.deltaTime * _smoothLookSpeed);
            _smoothPosition = Vector3.Lerp(transform.position, _targetTPSPosition, _smoothTranslationSpeed * Time.deltaTime);
            _currentUp = Vector3.Lerp(_currentUp, _targetUp, Time.deltaTime * _smoothRotationSpeed);

            //_smoothPosition = Vector3.SmoothDamp(transform.position, myPoint, ref velocity, 0.25f);
            if (inTPS)
            {
                transform.position = _smoothPosition;
                transform.LookAt(_currenTPStLookAt, _currentUp);
            }
        }
;
    }

    public void UpdateFPSCamera(Transform objectTrans, Transform targetTransform)
    {

        Vector3 myForward = objectTrans.forward;
        Vector3 myRight = objectTrans.right;

        myForward = Quaternion.AngleAxis(_pitch, myRight) * myForward;

        Vector3 myUp = Vector3.Cross(myForward, myRight);

        Quaternion rotation = Quaternion.LookRotation(myForward, myUp);


        transform.position = targetTransform.position;
        transform.rotation = rotation;
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

    public void ResetFPSCamera()
    {
        pitch = 0;
    }

    public Vector3 GetLookPoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward,out hit, 1000))
        {

            return hit.point;
        }



        return transform.position + transform.forward * 1000;
    }


}
