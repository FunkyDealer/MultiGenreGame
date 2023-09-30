using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private bool _dragSelect = false;
    private Vector3 _clickPosition;
    private Vector3 _clickUpPosition;

    private List<GameObject> _selectedObjects;
    private int selectionCount = 0;

    private void Awake()
    {
        _player = _playerObject.gameObject.GetComponent<RTS_Player>();
        _myCamera = gameObject.GetComponent<Camera>();

        _selectedObjects = new List<GameObject>();
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
            _clickPosition = Input.mousePosition; //save positon where the click was made  
           if (_selectedObjects.Count > 0)
            {
                _selectedObjects.Clear();
                selectionCount = 0;
                Debug.Log("clearing selection list");
            } 
        }       

        if (Input.GetButton("Fire1")) if ((_clickPosition - Input.mousePosition).magnitude > 20) _dragSelect = true;

        if (Input.GetButtonUp("Fire1")) SelectionInput();

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

    private void SelectionInput()
    {

        if (!_dragSelect)
        {
            SingleSelect();
        }
        else
        {
            BoxSelect();
        }

        _dragSelect = false;
    }

    //create a 3d box that will get all object inside it
    private void BoxSelect()
    {
        Vector3 currentgameObjectPos = gameObject.transform.position;
       // transform.position = Vector3.zero;

        _clickUpPosition = Input.mousePosition;

        //Debug.Log("starting Box select");
        //drag select box
        //Collider variables
        MeshCollider selectionBox;
        Mesh selectionMesh;

        //the corners of the 2d selection box
        Vector2[] corners = GetBoundingBox(_clickPosition, _clickUpPosition);

        //the vertices of the meshcollider
        Vector3[] verts = new Vector3[4];
        Vector3[] vecs = new Vector3[4];

        int i = 0;
        foreach (Vector2 corner in corners)
        {
            Ray ray = _myCamera.ScreenPointToRay(corner);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                verts[i] = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                vecs[i] = ray.origin - hit.point;
                Debug.DrawLine(_myCamera.ScreenToWorldPoint(corner), hit.point, Color.red, 5.0f);
            }
            i++;
        }

        //generate the mesh
        selectionMesh = GenerateSelectionMesh(verts, vecs);

        selectionBox = gameObject.AddComponent<MeshCollider>();
        selectionBox.sharedMesh = selectionMesh;
        selectionBox.convex = true;
        selectionBox.isTrigger = true;

        //Debug.Log("finished selection box");

       // transform.position = currentgameObjectPos;

        StartCoroutine(SendSelectedObjects(false, selectionBox));
    }

    private IEnumerator SendSelectedObjects(bool adding, MeshCollider selectionBox)
    {
        //yield return new WaitUntil( () => selectionCount > 0);
        yield return 0;

        //Debug.Log("finished waiting for selection count to increase");

        _player.TryToSelectEntities(_selectedObjects, adding);
        _selectedObjects.Clear();
        selectionCount = 0;
        Destroy(selectionBox);
    }

    //create a bounding box (4 corners in order) from the start and end mouse position
    private Vector2[] GetBoundingBox(Vector3 p1, Vector3 p2)
    {        
        // Min and Max to get 2 corners of rectangle regardless of drag direction.
        Vector3 bottomLeft = Vector3.Min(p1, p2);
        Vector3 topRight = Vector3.Max(p1, p2);

        // 0 = top left; 1 = top right; 2 = bottom left; 3 = bottom right;
        Vector2[] corners =
        {
            new Vector2(bottomLeft.x, topRight.y),
            new Vector2(topRight.x, topRight.y),
            new Vector2(bottomLeft.x, bottomLeft.y),
            new Vector2(topRight.x, bottomLeft.y)
        };
        //Debug.Log("Finished Bounding Box");
        return corners;
    }

    private Mesh GenerateSelectionMesh(Vector3[] corners, Vector3[] vecs)
    {
        Vector3[] verts = new Vector3[8];
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 }; //map the tris of the cube

        for (int i = 0; i < 4; i++)
        {
            verts[i] = corners[i] - transform.position;

            verts[i] =  Quaternion.Euler(-transform.rotation.eulerAngles) * verts[i];
        }

        for (int j = 4; j < 8; j++)
        {
            verts[j] = corners[j - 4] + vecs[j - 4] - transform.position;

            verts[j] = Quaternion.Euler(-transform.rotation.eulerAngles) * verts[j];
        }

        Mesh selectionMesh = new Mesh();      
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        //Debug.Log(selectionMesh.vertices.Distinct().Count());

       // Debug.Log("Finished Selection mesh");
       // Debug.Break();

        return selectionMesh;
    }

    private void SingleSelect()
    {
        Vector3 newPos = _playerObject.position;

        //float distance;
        Ray ray = _myCamera.ScreenPointToRay(_clickPosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            newPos = hit.point;

            GameObject g = hit.collider.gameObject;

            //Debug.DrawLine(ray.origin, newPos, Color.red, 1);

            _selectedObjects.Add(g);
            selectionCount++;
        }
        else
        {
            Debug.Log("Nothing was clicked on");
        }

        if (!_player.TryToSelectEntities(_selectedObjects, false))
        {
            _cursorIndicator.transform.position = newPos;
        }

        _selectedObjects.Clear();
        selectionCount = 0;
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

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("added object");
        _selectedObjects.Add(other.gameObject);
        selectionCount++;   

    }

    private void OnGUI()
    {
        if (_dragSelect == true)
        {
            var rect = Utils.GetScreenRect(_clickPosition, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(1, 0, 0, .2f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(1f, 0.2f, 0f, 0.8f));
        }
    }




}
