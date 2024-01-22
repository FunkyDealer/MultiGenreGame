using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//for the 3D nav mesh grid
public class SpaceInfo
{

    public Vector3 _worldSpace { get; private set; }

    public int _x { get; private set; }
    public int _y { get; private set; }
    public int _z { get; private set; }

    public float _size { get; private set; }

    public SpaceInfo(Vector3 worldSpace, int x, int y,int z, float size)
    {

        _worldSpace = worldSpace;
        _x = x;
        _y = y;
        _z = z;
    
        _size = size;






    }





}
