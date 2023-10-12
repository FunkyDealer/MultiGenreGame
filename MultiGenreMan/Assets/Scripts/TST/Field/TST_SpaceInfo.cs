using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TST_SpaceInfo 
{

    public Vector3 _topLeft { get; private set; }
    public Vector3 _topRight { get; private set; }
    public Vector3 _bottomLeft { get; private set; }
    public Vector3 _bottomRight { get; private set; }
    public Vector3 _pos { get; private set; }

    public int _width { get; private set; }
    public int _length { get; private set; }

    public float _size { get; private set; }

    public TST_SpaceInfo(Vector3 pos, int width, int length, float size)
    {

        _pos = pos;
        _width = width;
        _length = length;
        _size = size;
            
          
        _topLeft = new Vector3(_pos.x - size, _pos.y, pos.z - size);
        _topRight = new Vector3 (_pos.x + size, _pos.y, _pos.z - size);
        _bottomLeft = new Vector3(_pos.x - size, _pos.y, pos.z + size);
        _bottomRight = new Vector3(_pos.x + size, _pos.y, pos.z + size);



    }


}
