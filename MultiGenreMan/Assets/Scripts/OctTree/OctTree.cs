using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctTree
{
    public int TotalNodes { get; private set; }
    private OctTreeNode rootNode;
    public OctTreeNode RootNode => rootNode;
    public Dictionary<int, OctTreeNode> NodesByID { get; private set; }
    public List<OctTreeNode> Emptyleafs { get; private set; }

    public OctTree(GameObject[] worldObjects, float minNodeSize)
    {
        TotalNodes = 0;
        NodesByID = new Dictionary<int, OctTreeNode>();
        Bounds bounds = new Bounds();

        foreach (var g in worldObjects)
        {
            bounds.Encapsulate(g.GetComponent<Collider>().bounds);
        }

        float maxSize = Mathf.Max(new float[] {bounds.size.x, bounds.size.y, bounds.size.z});
        Vector3 sizeVector = new Vector3(maxSize, maxSize, maxSize);
        sizeVector *= 0.5f; //half it because it measures size from center of objects to edge
        bounds.SetMinMax(bounds.center - sizeVector, bounds.center + sizeVector);

        int totalNodes = 0;


        rootNode = new OctTreeNode(bounds, minNodeSize, ref totalNodes, this);
        

        AddObjects(worldObjects,ref totalNodes);

        TotalNodes = totalNodes;

        Emptyleafs = new List<OctTreeNode>();
        foreach (var n in NodesByID)
        {
            if (n.Value.children == null && !n.Value.occupied) Emptyleafs.Add(n.Value);
        }


    }

    public void AddObjects(GameObject[] worldObjects, ref int nodes)
    {
        foreach (var g in worldObjects)
        {
            rootNode.AddObject(g, ref nodes, this);
        }
    }

    public void AddNodeToDictionary(int ID, OctTreeNode node)
    {
        if (!NodesByID.ContainsKey(ID))
        {
            NodesByID.Add(ID, node);
        }
        else
        {
            Debug.Log($"Dictionary already contained ID {ID}");
        }
    }

    public Graph CreateGraph()
    {
        Graph graph = new Graph(Emptyleafs);

        List<Vector3> rays = new List<Vector3>()
        {
            new Vector3(1, 0, 0), 
            new Vector3(-1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, -1, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
        };

        int count = Emptyleafs.Count;
        int cCount = 0;
        for (int i = 0; i < count; i++)
        {
            List<Node> neighbours = new List<Node>();
            for (int j = 0; j < count; j++)
            {
                if (i == j) continue;

                for (int r = 0; r < 6; r++) //iterate through each ray direction to find neighbours
                {
                    Ray ray = new Ray(Emptyleafs[i].Pos, rays[r]);
                    float maxLengh = Emptyleafs[i].myBounds.size.y / 2.0f + 0.01f;
                    float hitlengh;
                    if (Emptyleafs[j].myBounds.IntersectRay(ray, out hitlengh))
                    {
                        if (hitlengh < maxLengh) neighbours.Add(graph.GetNode(j));
                    }
                }
            }

            foreach (Node o in neighbours)
            {
                Connection c1 = new Connection(graph.GetNode(i), o, ref cCount);
                graph.AddConnection(c1);
                Connection c2 = new Connection(o, graph.GetNode(i), ref cCount);
                graph.AddConnection(c2);
            }
        }

        return graph;
    }

    public OctTreeNode FindClosestNodeTo(Vector3 position)
    {
        OctTreeNode node = null;

        OctTreeNode rootNode = RootNode;

        if (rootNode.myBounds.Contains(position)) //we are already inside the root, thus inside the octTree
        {
            node = FindPosition(position);
        }
        else //we are not inside, lets find the closest node in the octTree
        {
            OctTreeNode closestChild = rootNode.children[0];
            float closestDist = Vector3.Distance(position, closestChild.myBounds.center);
            for (int i = 1; i < rootNode.children.Length; i++)
            {
                float newDistance = Vector3.Distance(position, rootNode.children[i].myBounds.center);

                if (newDistance < closestDist)
                {
                    closestDist = newDistance;
                    closestChild = rootNode.children[i];
                }
            }
            //we now have the closest child of the root
            node = closestChild;

            if (node.children != null)
            {
                node.FindClosestTo(position, ref closestChild, ref closestDist);
            }
            node = closestChild;
        }
        return node;
    }

    public OctTreeNode FindPosition(Vector3 position)
    {
        OctTreeNode node = null;

        OctTreeNode rootNode = RootNode;

        if (rootNode.myBounds.Contains(position))
        {
            node = rootNode;
            if (rootNode.children != null)
            {
                rootNode.FindPosition(position, ref node);
            }
        }
        return node;
    }

    public OctTreeNode GetRandomNode()
    {
        OctTreeNode node = null;

        int nodeNumber = 0;

        int totalNodes = Emptyleafs.Count;
        nodeNumber = UnityEngine.Random.Range(0, totalNodes);

        node = Emptyleafs[nodeNumber];

        return node;
    }
    public void Draw()
    {
        rootNode.Draw();
    }

}
