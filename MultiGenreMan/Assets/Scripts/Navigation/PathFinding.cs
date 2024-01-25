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

    public static bool CheckIfCanGoStraightToNextNode(Vector3 agentPos ,Node nextNode)
    {
        float distance = Vector3.Distance(agentPos, nextNode.Pos);
        Vector3 direction = nextNode.Pos - agentPos;
        Ray ray = new Ray(agentPos, direction);
        if (Physics.Raycast(ray, distance))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static void TryToReduceCurrentPath(Vector3 agentPos, ref List<Node> pathList)
    {
        List<Node> reversePath = new List<Node>(pathList);
        reversePath.Reverse(); //reverse the list so that we can interate throught it in reverse order

        Node nextNode = null; //next potentioal node

        for (int i = 0; i < reversePath.Count; i++) //find a node that is not blocked by an obstacle
        {
            Vector3 direction = reversePath[i].Pos - agentPos;
            float distance = Vector3.Distance(agentPos, reversePath[i].Pos);
            Ray ray = new Ray(agentPos, direction);
            if (Physics.Raycast(ray, distance))
            {
                continue; 
            }
            else
            {
                nextNode = reversePath[i]; //this node is the first node that isn't blocked, so we can remove all that come after (the agent can just do a straight line to this one)
                break;
            }
        }

        if (nextNode != null) //lets find out which position the node is in the original path, so that we can remove everything that come before it
        {
            int nodeNr = 0;

            for (int i = 0; i < pathList.Count; i++)
            {
                if (pathList[i].ID == nextNode.ID)
                {
                    nodeNr = i;
                    break;
                }
            }

            //Debug.Log($"removing till {nodeNr}");
            //Debug.Log($"current path has {_currentPath.Count} nodes");
            for (int i = 0; i < nodeNr; i++) //remove every node that comes before the node that the agent can just fly straight to
            {
                //Debug.Log($"removing at position {i}");
                pathList.RemoveAt(0); 
            }
        }
        else
        {
            Debug.Log("nextNode was null");
        }


    }

}
