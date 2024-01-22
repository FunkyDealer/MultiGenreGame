using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyNavMeshGrid : MonoBehaviour
{
    public struct Space3D {
        public Vector3Int gridCoords;
        public float _size;
        public Vector3 _worldSpace;

        public Vector3Int Up;
        public Vector3Int Down;
        public Vector3Int Left;
        public Vector3Int Right;
        public Vector3Int Forward;
        public Vector3Int Back;


    }


    private static FlyNavMeshGrid _instance;
    public static FlyNavMeshGrid inst { get { return _instance; } }

    [SerializeField] int _TotalXsize;
    [SerializeField] int _TotalYsize;
    [SerializeField] int _TotalZsize;

    [SerializeField] float _spaceSize;

    [SerializeField] GameObject _space3DPrefab;

    //private FlyNavMeshSpace[,,] _grid;
    private Space3D[,,] _grid;

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

        _grid = new Space3D[_TotalXsize, _TotalYsize, _TotalZsize];
    }

    // Start is called before the first frame update
    void Start()
    {

        for (int x = 0; x < _TotalXsize; x++)
        {
            for (int y = 0; y < _TotalYsize; y++)
            {
                for (int z = 0; z < _TotalZsize; z++)
                {
                    //_grid[x, y, z] = CreatePhysicalSpace(x, y, z);
                    _grid[x, y, z] = CreateSpaces(x, y, z);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private FlyNavMeshSpace CreatePhysicalSpace(int x, int y, int z)
    {
        Vector3 pos = new Vector3(transform.position.x + (x * _spaceSize) - (_TotalXsize / 2 * _spaceSize), transform.position.y + (y * _spaceSize) - (_TotalYsize / 2 * _spaceSize), transform.position.z + (z * _spaceSize) - (_TotalZsize / 2 * _spaceSize));
        //Vector3 pos = new Vector3(transform.position.x + (x * _spaceSize), transform.position.y + (y * _spaceSize), transform.position.z + (z * _spaceSize));

        FlyNavMeshSpace space = Instantiate(_space3DPrefab, pos, Quaternion.identity, transform).GetComponent<FlyNavMeshSpace>();

        SpaceInfo info = new SpaceInfo(pos, x, y, z, _spaceSize);

        space.SetSpaceInfo(info);

        return space;
    }

    private Space3D CreateSpaces(int x, int y, int z)
    {
        Space3D s = new Space3D();

        Vector3 pos = new Vector3(transform.position.x + (x * _spaceSize) - (_TotalXsize / 2 * _spaceSize), transform.position.y + (y * _spaceSize), transform.position.z + (z * _spaceSize) - (_TotalZsize / 2 * _spaceSize));

        s.gridCoords = new Vector3Int(x, y, z);
        s._size = _spaceSize;
        s._worldSpace = pos;

        s.Up = new Vector3Int(s.gridCoords.x, s.gridCoords.y + 1, s.gridCoords.z);
        s.Down = new Vector3Int(s.gridCoords.x, s.gridCoords.y - 1, s.gridCoords.z);
        s.Right = new Vector3Int(s.gridCoords.x + 1, s.gridCoords.y, s.gridCoords.z);
        s.Left = new Vector3Int(s.gridCoords.x - 1, s.gridCoords.y, s.gridCoords.z);
        s.Forward = new Vector3Int(s.gridCoords.x, s.gridCoords.y, s.gridCoords.z + 1);
        s.Back = new Vector3Int(s.gridCoords.x, s.gridCoords.y, s.gridCoords.z - 1);

        return s;
    }

    public static Space3D GetSpace(Vector3Int space)
    {
        return inst._grid[space.x, space.y, space.z];
    }

    public static float GetSpaceSize() => inst._spaceSize;

    public static Vector3 GetSpace3D(Vector3Int s) => inst._grid[s.x, s.y, s.z]._worldSpace;

    public static bool ValidateSpace3D(Vector3Int s)
    {
        return (s.x < inst._TotalXsize && s.x >= 0 && s.y < inst._TotalYsize && s.y >= 0 && s.z < inst._TotalZsize && s.z >= 0);

    }
}
