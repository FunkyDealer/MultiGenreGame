using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlyingNodeAgent : MonoBehaviour
{
    bool _moving = false;
    [SerializeField]
    float _stoppingDistance = 0.3f;
    [SerializeField]
    float _speed = 5;

    Node _currentNode = null;
    Node _nextNode = null;
    List<Node> _currentPath = new List<Node>();

    [SerializeField]
    Color _pathColor = Color.white;

    [SerializeField]
    NavigationManager _navigationManager;
  

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartAreaPatrol());
    }

    IEnumerator StartAreaPatrol()
    {
        yield return new WaitForSeconds(1f);

        SelectDestinationAtRandom();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!_moving && Input.GetButtonDown("Fire1"))
        //{
        //    GetCurrentNode();
        //}
        //if (!_moving && Input.GetButtonDown("Jump"))
        //{
        //    SelectDestinationAtRandom();
        //}
    }

    private void FixedUpdate()
    {
        if (_moving) MoveToNextNode();
    }   

    void GetCurrentNode()
    {
        _currentNode = _navigationManager.Graph.FindClosestNode(transform.position);
    }

    void SelectDestinationAtRandom()
    {
        if (_currentNode == null)
        {
           GetCurrentNode();
           //Debug.Log($"Agent was not inside the tree, Moving to node {_closestNode.ID}");
        }
        else
        {
            Node nextNode = _navigationManager.Graph.GetRandomNode();
            //Debug.Log($"Agent Inside tree, Moving to node nr {nextNode.ID}");

            GetNextPathToNode(nextNode);
        }

        _moving = true;
    }

    void GetNextPathToNode(Node nextNode)
    {
        bool canGoStraight = PathFinding.CheckIfCanGoStraightToNextNode(transform.position,nextNode); //optimization measure, if the agent can just go straight into the next node, skip Astarpathfind

        if (!canGoStraight)
        {
            PathFinding.AStarPathFind(_currentNode, nextNode, ref _currentPath);
            //Debug.Log($"Current path has {_currentPath.Count} nodes");

            //Reduce Path size by raycasting the nodes to see if they can traversed in a straight line
            PathFinding.TryToReduceCurrentPath(transform.position, ref _currentPath);
        }
        else
        {
            _currentPath.Clear();
            _currentPath.Add(nextNode);
        }

        _nextNode = _currentPath[0];
    }   

    void MoveToNextNode()
    {
        if (_nextNode != null)
        {
            if ((_currentNode == null)) _currentNode = _nextNode;

            float distance = Vector3.Distance(transform.position, _nextNode.Pos);

            if (distance <= _stoppingDistance)
            {
                _currentNode = _nextNode;

                    _currentPath.RemoveAt(0);
                    if (_currentPath.Count > 0)
                    {
                        PathFinding.TryToReduceCurrentPath(transform.position, ref _currentPath);
                        _nextNode = _currentPath[0];
                    }
                    else
                    {
                        _moving = false;
                        SelectDestinationAtRandom();
                    }                              
            }
            else
            {
                Locomotion();
            }
        }
        else
        {
            Debug.Log("finished Moving");
            _moving = false;
            SelectDestinationAtRandom();
        }
    }

    private void Locomotion()
    {
        //keep moving towards next node
        //Vector3 direction = _nextNode.Pos - transform.position;
        //direction.Normalize();

        transform.position = Vector3.Lerp(transform.position, _nextNode.Pos, _speed * Time.deltaTime);
    }


    private void OnDrawGizmos()
    {
        if (_currentPath.Count > 0)
        {
            for (int i = 0; i < _currentPath.Count; i++)
            {
                if (i + 1 < _currentPath.Count)
                {
                    Gizmos.color = _pathColor;
                    Gizmos.DrawLine(_currentPath[i].Pos, _currentPath[i + 1].Pos);
                }

            }
        }
    }
}
