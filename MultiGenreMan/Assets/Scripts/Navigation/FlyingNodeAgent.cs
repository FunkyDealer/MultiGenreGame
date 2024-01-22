using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlyingNodeAgent : MonoBehaviour
{
    Node _currentNode = null;

    bool _moving = false;
    [SerializeField]
    float _stoppingDistance = 0.3f;
    [SerializeField]
    float _speed = 5;

    Node _closestNode = null;
    Node _nextNode = null;
    List<Node> _currentPath = new List<Node>();

    [SerializeField]
    Color _pathColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {

        if (OctTreeManager.inst.NavGraph != null) GetCurrentNode();

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
        _currentNode = OctTreeManager.inst.NavGraph.FindPositionInside(transform.position);

        if (_currentNode != null)
        {
            //Debug.Log($"Agent is in node {_currentNode.ID}");
        }
        else
        {
           // Debug.Log("agent is not in node");
        }
    }

    void SelectDestinationAtRandom()
    {
        GetCurrentNode();

        if (_currentNode == null)
        {
            _closestNode = OctTreeManager.inst.NavGraph.FindClosestNodeTo(transform.position);
            //Debug.Log($"Agent was not inside the tree, Moving to node {_closestNode.ID}");
        }
        else
        {
            _closestNode = null;
            Node nextNode = OctTreeManager.inst.NavGraph.GetRandomNode();
            //Debug.Log($"Agent Inside tree, Moving to node nr {nextNode.ID}");

            PathFinding.AStarPathFind(_currentNode, nextNode, ref _currentPath);
            //Debug.Log($"Current path has {_currentPath.Count} nodes");
            _nextNode = _currentPath[0];
        }

        _moving = true;
    }

    void MoveToNextNode()
    {
        if (_closestNode != null)
        {
            _nextNode = _closestNode;
        }

        if (_nextNode != null)
        {
            if ((_currentNode == null || _nextNode.ID != _currentNode.ID) && _nextNode.myBounds.Contains(transform.position)) _currentNode = _nextNode;

            float distance = Vector3.Distance(transform.position, _nextNode.Pos);

            if (distance <= _stoppingDistance)
            {
                _currentNode = _nextNode;

                if (_closestNode != null)
                {
                    _closestNode = null;
                    _nextNode = null;
                    //arrived
                    _moving = false;
                    SelectDestinationAtRandom();
                } 
                else
                {
                    _currentPath.RemoveAt(0);
                    if (_currentPath.Count > 0)
                    {
                        _nextNode = _currentPath[0];
                    }
                    else
                    {
                        _moving = false;
                        SelectDestinationAtRandom();
                    }
                }                
            }
            else
            {
                //keep moving towards next node
                //Vector3 direction = _nextNode.Pos - transform.position;
                //direction.Normalize();

                transform.position = Vector3.Lerp(transform.position, _nextNode.Pos, _speed * Time.deltaTime);

            }



        }
        else
        {
            
            _moving = false;
        }


    }


    private void OnDrawGizmos()
    {
        //if (_currentPath.Count > 0)
        //{
        //    for (int i = 0; i < _currentPath.Count; i++)
        //    {
        //        if (i + 1 < _currentPath.Count) 
        //        { 
        //            Gizmos.color = _pathColor;
        //            Gizmos.DrawLine(_currentPath[i].Pos, _currentPath[i + 1].Pos);
        //        }

        //    }
        //}
    }
}
