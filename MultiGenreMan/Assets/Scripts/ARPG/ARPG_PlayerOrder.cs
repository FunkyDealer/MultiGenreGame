using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPG_PlayerOrder
{
    
    enum OrderType
    {
        NONE,
        MOVE
    }

    public Vector3 _position { get; private set; }


    public ARPG_PlayerOrder(Vector3 position)
    {
        this._position = position;
    }

}
