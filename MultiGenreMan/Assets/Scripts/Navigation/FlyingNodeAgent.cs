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


    int _manualGoToNode = 0;

    [SerializeField]
    private TMP_InputField mainInputField;

    // Start is called before the first frame update
    void Start()
    {
        mainInputField.onEndEdit.AddListener(delegate { ChangeManualNode(mainInputField.text); });

        if (OctTreeManager.inst.NavGraph != null) GetCurrentNode();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!_moving && Input.GetButtonDown("Fire1"))
        //{
        //    GetCurrentNode();
        //}
        if (!_moving && Input.GetButtonDown("Jump"))
        {
            SelectDestinationAtRandom();
        }

    }

    private void FixedUpdate()
    {
        if (_moving) MoveToNextNode();
    }

    public void ChangeManualNode(string s)
    {
        int n = int.Parse(s);
        _manualGoToNode = n;
    }

    public void ManualGoTo()
    {
        if (!_moving)
        {
            if (_currentNode == null)
            {
                _closestNode = OctTreeManager.inst.NavGraph.FindClosestNodeTo(transform.position);
                Debug.Log($"Agent was not inside the tree, Moving to node {_closestNode.ID}");
            }
            else
            {
                _closestNode = null;
                Node nextNode = OctTreeManager.inst.NavGraph.GetNode(_manualGoToNode);
                Debug.Log($"Agent Inside tree, Moving to node nr {nextNode.ID}");

                OctTreeManager.inst.NavGraph.AStarPathFind(_currentNode, nextNode, ref _currentPath);
                Debug.Log($"Current path has {_currentPath.Count} nodes");
                if (_currentPath.Count > 0) _nextNode = _currentPath[0];
                else _nextNode = null;
            }

            _moving = true;
        }
    }

    void GetCurrentNode()
    {
        _currentNode = OctTreeManager.inst.NavGraph.FindPositionInside(transform.position);

        if (_currentNode != null)
        {
            Debug.Log($"Agent is in node {_currentNode.ID}");
        }
        else
        {
            Debug.Log("agent is not in node");
        }
    }

    void SelectDestinationAtRandom()
    {
        GetCurrentNode();

        if (_currentNode == null)
        {
            _closestNode = OctTreeManager.inst.NavGraph.FindClosestNodeTo(transform.position);
            Debug.Log($"Agent was not inside the tree, Moving to node {_closestNode.ID}");
        }
        else
        {
            _closestNode = null;
            Node nextNode = OctTreeManager.inst.NavGraph.GetRandomNode();
            Debug.Log($"Agent Inside tree, Moving to node nr {nextNode.ID}");

            OctTreeManager.inst.NavGraph.AStarPathFind(_currentNode, nextNode, ref _currentPath);
            Debug.Log($"Current path has {_currentPath.Count} nodes");
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
}
