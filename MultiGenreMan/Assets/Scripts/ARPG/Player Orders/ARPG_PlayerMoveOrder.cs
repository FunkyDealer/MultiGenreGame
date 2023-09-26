using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPG_PlayerMoveOrder : ARPG_PlayerOrder
{



    public Vector3 _position { get; private set; }


    public ARPG_PlayerMoveOrder(Vector3 position)
    {
        this._position = position;
    }
}
