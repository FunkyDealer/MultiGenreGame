using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph 
{
    Dictionary<int, Node> Nodes = new Dictionary<int, Node>();
    List<Connection> Connections = new List<Connection>();

    Bounds myBounds;

    public Graph()
    {
        Nodes = new Dictionary<int, Node>();
        Connections = new List<Connection>();

        GrowBounds();
    }

    public Graph(Dictionary<int ,Node> nodes, List<Connection> connections)
    {
        Nodes = nodes;
        Connections = connections;


    }

    public Graph(List<Node> nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            AddNode(nodes[i], i);
        }
    }

    public Graph(List<OctTreeNode> leafs)
    {   

        for (int i = 0; i < leafs.Count; i++)
        {
            Node n = new(leafs[i], i);
            AddNode(n);
        }

        GrowBounds();
    }

    private void GrowBounds()
    {
        Bounds[] b = new Bounds[Nodes.Count];

        for (int i = 0; i < Nodes.Count; i++)
        {
            myBounds.Encapsulate(Nodes[i].myBounds);
        }
    }

    public void AddNode(Node node)
    {
        if (Nodes == null) Nodes = new Dictionary<int, Node>();
        Nodes.Add(node.ID ,node);
    }

    public void AddNode(Node node, int ID)
    {
        if (Nodes == null) Nodes = new Dictionary<int, Node>();
        node.SetNewID(ID);
        Nodes.Add(ID, node);
    }

    public Node GetNode(int id)
    {
        if (id > Nodes.Count) return null;
        return Nodes[id];
    }

    public void AddConnection(Connection c)
    {
        if (Connections == null) Connections = new List<Connection>();
        Connections.Add(c);

        c.StartNode.AddConnection(c);
    }

    public int TotalNodes()
    {
        return Nodes.Count;
    }   


    public void CreateConnections()
    {
        int connectionID = 0;

        for (int i = 0; i < Nodes.Count; i++)
        {            

            for (int j = 0; j < Nodes.Count; j++)
            {
                if (i == j) continue;

                float distance = Vector3.Distance(Nodes[i].Pos, Nodes[j].Pos);
                Vector3 direction = Nodes[j].Pos - Nodes[i].Pos;
                direction.Normalize();
                Ray r = new Ray(Nodes[i].Pos, direction);    

                if (Physics.Raycast(r, distance))
                {
                    //do nothing
                }
                else
                {
                    Connection c = new Connection(Nodes[i], Nodes[j], ref connectionID);
                    Nodes[i].AddConnection(c);
                    Connections.Add(c);
                }
            }
        }
    }

    //Find closest node in case the agent is outside the graph's bounds
    //public Node FindClosestNodeTo(Vector3 position)
    //{
    //    if (myBounds.Contains(position)) //you are already inside
    //    {
    //        return FindPositionInside(position);
    //    }

    //    Node closest = Nodes[0];
    //        float closestDist = Vector3.Distance(position, Nodes[0].Pos);
    //        for (int i = 1; i < Nodes.Count; i++)
    //        {
    //            float newDistance = Vector3.Distance(position, Nodes[i].Pos);

    //            if (newDistance < closestDist)
    //            {
    //                closestDist = newDistance;
    //                closest = Nodes[i];
    //            }
    //        }
        
    //    return closest;
    //}

    //find the node that the agent is closest to in case they are inside the graph's bounds
    //public Node FindPositionInside(Vector3 position)
    //{
    //    if (!myBounds.Contains(position)) //you are outside
    //    {
    //        return null;
    //    }

    //    Node node = null;

    //    for (int i = 0; i < Nodes.Count; i++)
    //    {
    //        if (Nodes[i].myBounds.Contains(position))
    //        {
    //            node = Nodes[i];
    //            break;
    //        }
    //    }
    //    return node;
    //}

    public Node FindClosestNode(Vector3 position)
    {
        float distance = Vector3.Distance(position, Nodes[0].Pos);
        Node closest = Nodes[0];

        for (int i = 1; i < Nodes.Count; i++)
        {
            float newDistace = Vector3.Distance(position, Nodes[i].Pos);
            if (newDistace < distance)
            {
                distance = newDistace;
                closest = Nodes[i];
            }
        }


        return closest;

    }

    public Node GetRandomNode()
    {
        Node node = null;
        int nodeNumber = 0;
        int totalNodes = Nodes.Count;
        nodeNumber = UnityEngine.Random.Range(0, totalNodes);

        node = Nodes.ElementAt(nodeNumber).Value;

        return node;
    }

    public void Draw()
    {
        for (int i = 0; i < Connections.Count; i++)
        {
           Connections[i].Draw();
        }

        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].Draw();
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(myBounds.center, myBounds.size);
    }

   

}
