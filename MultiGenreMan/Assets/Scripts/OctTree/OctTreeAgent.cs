using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctTreeAgent : MonoBehaviour
{
    OctTreeNode _currentNode = null;

    OctTreeNode _nextNode = null;

    bool _moving = false;
    [SerializeField]
    float _stoppingDistance = 0.3f;
    [SerializeField]
    float _speed = 5;

    [SerializeField]
    OctTreeManager _navigationManager;

    // Start is called before the first frame update
    void Start()
    {
        GetCurrentNode();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_moving && Input.GetButtonDown("Fire1"))
        {
            GetCurrentNode();
        }
        if (!_moving && Input.GetButtonDown("Jump"))
        {
            SelectDestinationAtRandom();
        }

    }

    private void FixedUpdate()
    {

       if (_moving) MoveToNextNode();


    }

    void GetCurrentNode()
    {
       _currentNode = _navigationManager.OctTree.FindPosition(transform.position);

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
            _nextNode = _navigationManager.OctTree.FindClosestNodeTo(transform.position);
          //  Debug.Log($"Agent was not inside the tree, Moving to node {_nextNode.ID}");

        }
        else
        {
            _nextNode = _navigationManager.OctTree.GetRandomNode();
            //Debug.Log($"Agent Inside tree, Moving to node nr {_nextNode.ID}");
        }

        _moving = true;
    }

    void MoveToNextNode()
    {
        if (_nextNode != null)
        {
            if ((_currentNode == null || _nextNode.ID != _currentNode.ID) && _nextNode.myBounds.Contains(transform.position)) _currentNode = _nextNode;

            float distance = Vector3.Distance(transform.position, _nextNode.Pos);

            if (distance <= _stoppingDistance)
            {
                _currentNode = _nextNode;
                _nextNode = null;
                //arrived
                _moving = false;
            }
            else
            {
                //keep moving towards it
                //Vector3 direction = _nextNode.Pos - transform.position;
                //direction.Normalize();

                transform.position = Vector3.Lerp(transform.position, _nextNode.Pos, _speed * Time.deltaTime);

            }



        }


    }
}
