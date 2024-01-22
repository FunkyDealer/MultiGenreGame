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

    //Find closest node in case the agent is outside the graph's bounds
    public Node FindClosestNodeTo(Vector3 position)
    {
        if (myBounds.Contains(position)) //you are already inside
        {
            return FindPositionInside(position);
        }

        Node closest = Nodes[0];
            float closestDist = Vector3.Distance(position, Nodes[0].Pos);
            for (int i = 1; i < Nodes.Count; i++)
            {
                float newDistance = Vector3.Distance(position, Nodes[i].Pos);

                if (newDistance < closestDist)
                {
                    closestDist = newDistance;
                    closest = Nodes[i];
                }
            }
        
        return closest;
    }

    //find the node that the agent is closest to in case they are inside the graph's bounds
    public Node FindPositionInside(Vector3 position)
    {
        if (!myBounds.Contains(position)) //you are outside
        {
            return null;
        }

        Node node = null;

        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].myBounds.Contains(position))
            {
                node = Nodes[i];
                break;
            }
        }
        return node;
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

    //A* finds a path from start to goal.
    public bool AStarPathFind(Node from, Node to, ref List<Node> path)
    {
        //clear the path
        path.Clear();

        if (from == null || to == null) return false;
        if (from.ID == to.ID) return false;

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();

        float tentative_G_Score = 0;
        bool tentativeIsBetter = false;

        from.g = 0;
        from.h = Vector3.SqrMagnitude(from.Pos - to.Pos);
        from.f = from.h + from.g;

        open.Add(from);

        while(open.Count > 0)
        {
            int lowestF = LowestF(open);
            Node curNode = open[lowestF];

            if (curNode.ID == to.ID)
            {
                ReconstructPath(from, to, ref path);
                return true;
            }

            open.RemoveAt(lowestF);
            closed.Add(curNode);

            Node Neighbour;
            foreach (Connection c in curNode.Connections)
            {
                Neighbour = c.EndNode;

                bool isOpenListHave = open.Contains(Neighbour);

                if (!isOpenListHave)
                {
                    Neighbour.g = curNode.g + Vector3.SqrMagnitude(curNode.Pos - Neighbour.Pos);
                }

                if (closed.Contains(Neighbour)) continue;

                tentative_G_Score = curNode.g + Vector3.SqrMagnitude(curNode.Pos - Neighbour.Pos);

                if (!isOpenListHave)
                {
                    open.Add(Neighbour);
                    tentativeIsBetter = true;
                }
                else if (tentative_G_Score < Neighbour.g)
                {
                    tentativeIsBetter = true;
                }
                else
                {
                    tentativeIsBetter = false;
                }

                if (tentativeIsBetter)
                {
                    Neighbour.ComeFrom = curNode;
                    Neighbour.g = tentative_G_Score;
                    Neighbour.h = Vector3.SqrMagnitude(curNode.Pos - to.Pos);
                    Neighbour.f = Neighbour.g + Neighbour.h;
                }
            }
        }
        return false;
    }

    public void ReconstructPath(Node start, Node end, ref List<Node> path)
    {
        path.Clear();
        path.Add(end);

        Node p = end.ComeFrom;
        while (p != start && p != null)
        {
            path.Insert(0, p);
            p = p.ComeFrom;
        }

        path.Insert(0, start);
    }
    private int LowestF(List<Node> list)
    {
        float lowestF = 0;
        int count = 0;
        int iteratorCount = 0;

        for (int i = 0; i < list.Count; i++)
        {
            if (i == 0)
            {
                lowestF = list[i].f;
                iteratorCount = 0;
            }
            else if (list[i].f <= lowestF)
            {
                lowestF = list[i].f;
                iteratorCount = count;
            }
            count++;
        }
        return iteratorCount;
    }

}
