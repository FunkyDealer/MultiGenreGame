using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTS_PlayerMoveOrder : RTS_PlayerOrder
{
    public Vector3 Position { get; private set; }

    public RTS_PlayerMoveOrder(Vector3 position)
    {
        this.Position = position;
    }




}
