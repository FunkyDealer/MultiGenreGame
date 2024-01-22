using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyNavMeshSpace : MonoBehaviour
{
    public Vector3Int gridCoords { get; private set; }

    public float _size { get; private set; }
    public Vector3 _worldSpace { get; private set; }

    public FlyNavMeshSpace Up { get; private set; }
    public FlyNavMeshSpace Down { get; private set; }
    public FlyNavMeshSpace Left { get; private set; }
    public FlyNavMeshSpace Right { get; private set; }
    public FlyNavMeshSpace Forward { get; private set; }
    public FlyNavMeshSpace Back { get; private set; }

    public List<FlyNavMeshSpace> Neighbours { get; private set; } = new List<FlyNavMeshSpace>();

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpaceInfo(SpaceInfo info)
    {
        this.gridCoords = new Vector3Int(info._x, info._y, info._z);

        this._size = info._size;
        this._worldSpace = info._worldSpace;


       

        //transform.localScale = new Vector3(_size, transform.localScale.y, _size);
       // transform.localScale = new Vector3(_size, _size, _size);
    }

    public void PostGridCreation()
    {
        ComputeSides();
    }

    public SpaceInfo GetSpaceInfo()
    {
        return new SpaceInfo(_worldSpace, gridCoords.x, gridCoords.y, gridCoords.z, _size);
    }

    private void ComputeSides()
    {
        Vector3Int up = new Vector3Int(gridCoords.x, gridCoords.y + 1, gridCoords.z);
        Vector3Int down = new Vector3Int(gridCoords.x, gridCoords.y - 1, gridCoords.z);
        Vector3Int right = new Vector3Int(gridCoords.x + 1, gridCoords.y, gridCoords.z);
        Vector3Int left = new Vector3Int(gridCoords.x - 1, gridCoords.y, gridCoords.z);
        Vector3Int forward = new Vector3Int(gridCoords.x, gridCoords.y, gridCoords.z + 1);
        Vector3Int back = new Vector3Int(gridCoords.x, gridCoords.y, gridCoords.z - 1);

        //if (FlyNavMeshGrid.ValidateSpace3D(up)) Up = FlyNavMeshGrid.GetSpace(up);
        //if (FlyNavMeshGrid.ValidateSpace3D(down)) Down = FlyNavMeshGrid.GetSpace(down);
        //if (FlyNavMeshGrid.ValidateSpace3D(right)) Right = FlyNavMeshGrid.GetSpace(right);
        //if (FlyNavMeshGrid.ValidateSpace3D(left)) Left = FlyNavMeshGrid.GetSpace(left);
        //if (FlyNavMeshGrid.ValidateSpace3D(forward)) Forward = FlyNavMeshGrid.GetSpace(forward);
        //if (FlyNavMeshGrid.ValidateSpace3D(back)) Back = FlyNavMeshGrid.GetSpace(back);

        if (Up != null) Neighbours.Add(Up);
        if (Down != null) Neighbours.Add(Down);
        if (Right != null) Neighbours.Add(Right);
        if (Left != null) Neighbours.Add(Left);
        if (Forward != null) Neighbours.Add(Forward);
        if (Back != null) Neighbours.Add(Back);
        
    }




}
