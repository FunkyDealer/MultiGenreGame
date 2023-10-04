using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_LaserRicochetProjectile : FPS_Projectile
{

    private LineRenderer _myLineRenderer;


    [HideInInspector]
    public FPS_Creature HitEntity;
    [HideInInspector]
    public List<Vector3> HitPoints;

    private Color _hitEnemyColor = Color.red;
    private Color _hitEnvironmentColor = Color.yellow;
    private Color _hitNothingColor = Color.blue;

    Vector3 _start;


    protected override void Awake()
    {
        base.Awake();
        _myLineRenderer = GetComponentInChildren<LineRenderer>();

    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();


        int count = HitPoints.Count;

        _myLineRenderer.positionCount = count + 1;

        _start = Vector3.zero;

        _myLineRenderer.SetPosition(0, _start);

        

        int currentPos = 1;
        foreach (var h in HitPoints)
        {
            Vector3 e = transform.InverseTransformPoint(h);

            _myLineRenderer.SetPosition(currentPos, e);
            currentPos++;
        }
        if (currentPos < count + 1)
        {
            Debug.Log("enter");
            for (int i = currentPos; i < count + 1; i++)
            {
                Vector3 e = transform.InverseTransformPoint(HitPoints[count - 1]);
                _myLineRenderer.SetPosition(i, e);
            }
        }





        //Debug.Break();

        StartCoroutine(CountDownToDeath(_lifeTime));
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();


    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }
}
