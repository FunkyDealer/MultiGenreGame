using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondOrderAgent : MonoBehaviour
{
    [SerializeField]
    List<Transform> _points;

    Vector3 _nextPos;

    [SerializeField, Range(0f, 5f)]
    float minDistance = 1.5f;
    int _currentPoint = 0;

    private SecondOrderDynamics XDynamics;
    private SecondOrderDynamics YDynamics;
    private SecondOrderDynamics ZDynamics;

    [SerializeField, Range(-5, 10)] private float Frequency = 1f; // the frequency of the vertical dynamics //f
    [SerializeField, Range(-5, 10)] private float DampingRatio = 0.1f; // the damping ratio of the vertical dynamics //zeta
    [SerializeField, Range(-5, 10)] private float PositionGain = 5f; // the gain applied to the vertical position input //r


    // Start is called before the first frame update
    void Start()
    {
        _nextPos = _points[_currentPoint].position;

        InitiateDynamics();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateDynamics();

        // update the dynamics with the current position and velocity inputs
        float currentXVelocity = (_nextPos.x - transform.position.x);
        float currentXPosition = XDynamics.Update(Time.deltaTime, transform.position.x, currentXVelocity);

        float currentYVelocity = (_nextPos.y - transform.position.y);
        float currentYPosition = YDynamics.Update(Time.deltaTime, transform.position.y, currentYVelocity);

        float currentZVelocity = (_nextPos.z - transform.position.z);
        float currentZPosition = ZDynamics.Update(Time.deltaTime, transform.position.z, currentZVelocity);

        // update the position of the game object based on the dynamics output
        transform.position = new Vector3(currentXPosition, currentYPosition, currentZPosition);

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

    void InitiateDynamics()
    {
        // initialize the x dynamics with the current parameter values and an initial position of 0
        XDynamics = new SecondOrderDynamics(Frequency, DampingRatio, PositionGain, transform.position.x);

        // initialize the y dynamics with the current parameter values and an initial position of 0
        YDynamics = new SecondOrderDynamics(Frequency, DampingRatio, PositionGain, transform.position.y);

        // initialize the z dynamics with the current parameter values and an initial position of 0
        ZDynamics = new SecondOrderDynamics(Frequency, DampingRatio, PositionGain, transform.position.z);
    }

    void UpdateDynamics()
    {
        XDynamics.RecomputeConstants(Frequency, DampingRatio, PositionGain);
        YDynamics.RecomputeConstants(Frequency, DampingRatio, PositionGain);
        ZDynamics.RecomputeConstants(Frequency, DampingRatio, PositionGain);
    }


}

public class SecondOrderDynamics
{
    private float xp; // previous input 
    private float y, yd; // state variables 
    private float k1, k2, k3; // dynamics constants, scalars

    private float PI = Mathf.PI;

    public SecondOrderDynamics(float f, float z, float r, float x0)
    {
        // compute constants
        k1 = z / (PI * f);
        k2 = 1 / ((2 * PI * f) * (2 * PI * f));
        k3 = r * z / (2 * PI * f);

        // initialize variables
        xp = x0;
        y = x0;
        yd = 0f;
    }

    public float Update(float time, float currentPos, float? xd = null)
    {
        if (xd == null)
        { // estimate velocity
            xd = (currentPos - xp) / time;
            xp = currentPos;
        }

        y = y + time * yd; // integrate position by velocity
        yd = (float)(yd + time * (currentPos + k3 * xd - y - k1 * yd) / k2); // integrate velocity by acceleration

        return y;
    }

    public void RecomputeConstants(float f, float z, float r)
    {
        // compute constants
        k1 = z / (PI * f);
        k2 = 1 / ((2 * PI * f) * (2 * PI * f));
        k3 = r * z / (2 * PI * f);
    }
}