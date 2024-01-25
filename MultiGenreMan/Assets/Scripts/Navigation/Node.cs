using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public List<Connection> Connections = new List<Connection>();
    public Node Path = null;
    public Vector3 Pos { get; private set; }
    public Bounds myBounds { get; private set; }
    public int ID { get; private set; }

    public float f, g, h;
    public Node ComeFrom;

    public Node(OctTreeNode n, int id)
    {
        Pos = n.Pos;
        myBounds = n.myBounds;
        this.ID = id;
        Path = null;
    }

    public Node(Vector3 pos, int ID)
    {
        this.Pos = pos;
        myBounds = new Bounds(pos, new Vector3(1, 1, 1));
        this.ID=ID;
        Path = null;
    }

    public void AddConnection(Connection to)
    {
        Connections.Add(to);
    }

    public void Draw()
    {
        Gizmos.color = new Color(1, 1, 0);
        Gizmos.DrawWireSphere(Pos, 0.05f);

        Gizmos.color = Color.white;
        UnityEditor.Handles.Label(myBounds.center, $"{ID}");
    }

    public void SetNewID(int newID)
    {
        ID = newID;
    }

}
