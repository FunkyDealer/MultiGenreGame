using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctTreeNode 
{
    public int ID { get; private set; }

    public Bounds myBounds { get; private set; }
    public Vector3 Pos => myBounds.center;
    private float minSize;
    private Bounds[] childBounds;
    public OctTreeNode[] children { get; private set; } = null;

    public bool occupied { get; private set; }

    public OctTreeNode(Bounds b, float minNodeSize, ref int totalNodes, OctTree myTree)
    {
        ID = totalNodes;
        totalNodes++;
        this.myBounds = b;
        this.minSize = minNodeSize;

        float quarter = myBounds.size.y / 4.0f;
        float childLenght = myBounds.size.y / 2;
        Vector3 childSize = new Vector3(childLenght, childLenght, childLenght);

        childBounds = new Bounds[8];

        childBounds[0] = new Bounds(myBounds.center + new Vector3(-quarter, quarter, -quarter), childSize);
        childBounds[1] = new Bounds(myBounds.center + new Vector3(quarter, quarter, -quarter), childSize);
        childBounds[2] = new Bounds(myBounds.center + new Vector3(-quarter, quarter, quarter), childSize);
        childBounds[3] = new Bounds(myBounds.center + new Vector3(quarter, quarter, quarter), childSize);
        childBounds[4] = new Bounds(myBounds.center + new Vector3(-quarter, -quarter, -quarter), childSize);
        childBounds[5] = new Bounds(myBounds.center + new Vector3(quarter, -quarter, -quarter), childSize);
        childBounds[6] = new Bounds(myBounds.center + new Vector3(-quarter, -quarter, quarter), childSize);
        childBounds[7] = new Bounds(myBounds.center + new Vector3(quarter, -quarter, quarter), childSize);

        myTree.AddNodeToDictionary(ID, this);
    }

    public void AddObject(GameObject g, ref int nodes, OctTree myTree)
    {
        DivideAndAdd(g,ref nodes, myTree);
    }

    private void DivideAndAdd(GameObject g, ref int nodes, OctTree myTree)
    {       

        if (myBounds.size.y <= minSize)
        {
            Collider b = g.GetComponent<Collider>();
            if (myBounds.Intersects(b.bounds)) occupied = true;

            return;
        }

        if (children == null) children = new OctTreeNode[8];

        bool dividing = false;
        for (int i = 0; i < 8; i++)
        {
            if (children[i] == null)
            {
                children[i] = new OctTreeNode(childBounds[i], minSize, ref nodes, myTree);
            }
            if (childBounds[i].Intersects(g.GetComponent<Collider>().bounds))
            {
                dividing = true;
                children[i].DivideAndAdd(g, ref nodes, myTree);
            }
        }

        if (!dividing)
        {
            children = null;

            Collider b = g.GetComponent<Collider>();
            if (myBounds.Intersects(b.bounds)) occupied = true;

        }

    }

    //find which child contains this position
    public void FindPosition(Vector3 position, ref OctTreeNode currentNode)
    {
        if (this.children != null)
        {
            foreach (var n in children)
            {
                if (n != null && n.myBounds.Contains(position))
                {
                    currentNode = n;
                    n.FindPosition(position, ref currentNode);
                    return;
                }
            }
        }
    }

    //find which child is closest to this position
    public void FindClosestTo(Vector3 position, ref OctTreeNode closest, ref float closestDist)
    {     
        if (this.children != null) //this node isn't final, keep going
        {
            foreach (var c in this.children)
            {
                c.FindClosestTo(position, ref closest, ref closestDist);
            }
        }
        else //this node is final, check distance
        {
            float newDistance = Vector3.Distance(position, this.myBounds.center);
            if (newDistance < closestDist)
            {
                closest = this;
                closestDist = newDistance;
            }
        }
    }


    public void Draw()
    {
        Color c = Color.green;
        if (occupied) c = Color.red;

        Gizmos.color = c;
        Gizmos.DrawWireCube(myBounds.center, myBounds.size);

        if (children != null)
        {
            for (int i = 0; i < 8; i++)
            {
                if (children[i] != null)
                {
                    children[i].Draw();
                }
            }
        }
        else
        {
            Gizmos.color = Color.red;
            UnityEditor.Handles.Label(myBounds.center, $"{ID}");
        }
    }



}


