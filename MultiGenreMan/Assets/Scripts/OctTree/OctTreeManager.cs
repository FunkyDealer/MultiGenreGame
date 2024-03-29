using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctTreeManager : NavigationManager
{
    [SerializeField]
    private GameObject[] _worldObjects;
    [SerializeField]
    private int _nodeMinSize = 5;
    public OctTree OctTree { get; private set; }
    int totalNodes = 0;

    [SerializeField]
    GameObject _floor;
    public Transform Floor => _floor.transform;
    [SerializeField]
    private float _floorExtraHeight = 3;
    public float FloorExtraHeight => _floorExtraHeight;

    [SerializeField]
    private float _myBoundsSizeX;
    [SerializeField]
    private float _myBoundsSizeY;
    [SerializeField]
    private float _myBoundsSizeZ;
    public Vector3 MyBoundsSize => new Vector3(_myBoundsSizeX, _myBoundsSizeY, _myBoundsSizeZ);

    [SerializeField]
    bool _drawBounds = true;

    enum DrawState
    {
        DrawOctTree,
        DrawGraph,
        NoDraw
    }

    [SerializeField] DrawState _drawState = DrawState.DrawGraph;
    

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        OctTree = new OctTree(_worldObjects, _nodeMinSize, this);
        totalNodes = OctTree.TotalNodes;
        Debug.Log($"Total OctTree Nodes: {totalNodes}");

        _navGraph = OctTree.CreateGraph();
        Debug.Log($"Total graph nodes: {_navGraph.TotalNodes()}");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
       
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            switch (_drawState)
            {
                case DrawState.DrawOctTree:
                    OctTree.Draw();
                    break;                    
                case DrawState.DrawGraph:
                    _navGraph.Draw();
                    break;
                case DrawState.NoDraw:
                    //nothing

                    break;
                default:
                    break;
            }
        }


#if UNITY_EDITOR
        if (_drawBounds)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, MyBoundsSize);
        }
#endif
    }




}
