using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPG_PlayerMoveOrder : ARPG_PlayerOrder
{



    public Vector3 Position { get; private set; }


    public ARPG_PlayerMoveOrder(Vector3 position)
    {
        this.Position = position;
    }
}
