using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3DNavigationManager : NavigationManager
{
    [SerializeField]
    List<Transform> nodeLocations;

    enum DrawState
    {
        DrawGraph,
        NoDraw
    }

    [SerializeField] DrawState _drawState = DrawState.DrawGraph;


    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        List<Node> nodes = new List<Node>();

        for (int i = 0; i < nodeLocations.Count; i++)
        {
            Node n = new Node(nodeLocations[i].position, i);
            nodes.Add(n);
        }


        _navGraph = new Graph(nodes);
        _navGraph.CreateConnections();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            switch (_drawState)
            {
                case DrawState.DrawGraph:
                    _navGraph.Draw();
                    break;
                case DrawState.NoDraw:
                    //nothing

                    break;
                default:
                    break;
            }
        }
    }
}
