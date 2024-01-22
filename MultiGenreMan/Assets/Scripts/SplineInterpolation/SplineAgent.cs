using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineAgent : MonoBehaviour
{
    [SerializeField]
    List<Transform> _points;

    Vector3 _nextPos;

    [SerializeField, Range(0f, 1f)]
    float minDistance = 1.5f;
    int _currentPoint = 0;

    


    // Start is called before the first frame update
    void Start()
    {
        _nextPos = _points[_currentPoint].position;


    }

    // Update is called once per frame
    void Update()
    {

        transform.position = _nextPos;

        float distance = Vector3.Distance(transform.position, _nextPos);
        if (distance < minDistance)
        {
            _currentPoint++;
            if (_currentPoint >= _points.Count) _currentPoint = 0;
            _nextPos = _points[_currentPoint].position;

        }
    }


    private void FixedUpdate()
    {





    }

   
}

