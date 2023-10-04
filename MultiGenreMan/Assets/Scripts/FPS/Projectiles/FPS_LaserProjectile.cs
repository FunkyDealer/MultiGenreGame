using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_LaserProjectile : FPS_Projectile
{

    private LineRenderer _myLineRenderer;


    [HideInInspector]
    public FPS_Creature HitEntity;
    [HideInInspector]
    public Vector3 HitPoint;

    private Color _hitEnemyColor = Color.red;
    private Color _hitEnvironmentColor = Color.yellow;
    private Color _hitNothingColor = Color.blue;

    Vector3 _direction;
    Vector3 _start;
    Vector3 _end;


    protected override void Awake()
    {
        base.Awake();
        _myLineRenderer = GetComponentInChildren<LineRenderer>();

    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _start = Vector3.zero;
        _end = transform.InverseTransformPoint(HitPoint);  //transform the hit point from world space to local space

        _direction = _end - _start;

        _myLineRenderer.SetPosition(0, _start);
        _myLineRenderer.SetPosition(1, _end);


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
        _start += _direction * 2.5f * Time.deltaTime;

        _myLineRenderer?.SetPosition(0, _start);
    }




}
