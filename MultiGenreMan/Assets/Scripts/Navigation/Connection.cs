using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection 
{
    public Node StartNode;
    public Node EndNode;
    public float Weight { get; private set; }
    public int ID { get; private set; }
    private Vector3 center;
    private Vector3 direction;

    public Connection(Node from, Node to, ref int id)
    {
        StartNode = from;
        EndNode = to;
        ID = id;
        id++;

        Weight = Vector3.Distance(from.Pos, to.Pos);
        
        center = new Vector3((from.Pos.x + to.Pos.x) / 2, (from.Pos.y + to.Pos.y) / 2, (from.Pos.z + to.Pos.z) / 2);
        direction = EndNode.Pos - StartNode.Pos;
        direction.Normalize();
    }

    public void Draw()
    {
        Gizmos.color = new Color(1, 0, 0, 1);
        //Gizmos.DrawLine(StartNode.Pos, StartNode.Pos + (direction * (Weight / 2)) - direction);
        Gizmos.DrawLine(StartNode.Pos, EndNode.Pos);

       // Gizmos.color = Color.black;
       // UnityEditor.Handles.Label(StartNode.Pos + (direction * (Weight / 2) - direction), $"c{ID}");
    }


}
