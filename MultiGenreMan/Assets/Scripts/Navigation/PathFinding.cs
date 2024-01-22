using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathFinding 
{

    //A* finds a path from start to goal.
    public static bool AStarPathFind(Node from, Node to, ref List<Node> path)
    {
        Debug.Log("Doing a*");
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

        while (open.Count > 0)
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

    private static void ReconstructPath(Node start, Node end, ref List<Node> path)
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
    private static int LowestF(List<Node> list)
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
