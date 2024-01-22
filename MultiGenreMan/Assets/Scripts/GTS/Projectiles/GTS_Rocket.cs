using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_Rocket : GTS_Projectile
{
    private LineRenderer _myLineRenderer;
    private Color _hitEnemyColor = Color.red;
    private Color _hitEnvironmentColor = Color.yellow;
    private Color _hitNothingColor = Color.blue;

    [HideInInspector]
    public Vector3 Direction;
    Vector3 _start;
    Vector3 _end;

    [SerializeField] private float _speed;
    [HideInInspector]
    public int Damage;

    [SerializeField]
    GameObject _explosionPrefab;

    SphereCollider _myCollider;

    protected override void Awake()
    {
        base.Awake();
        _myLineRenderer = GetComponentInChildren<LineRenderer>();

        _myCollider = GetComponent<SphereCollider>();
       // Debug.Break();
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _start = Vector3.zero;
        _end = _start - Direction.normalized * 3;

        Direction = _end - _start;

        _myLineRenderer.SetPosition(0, _end);
        _myLineRenderer.SetPosition(1, _start);

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


        transform.position -= _speed * Time.deltaTime * Direction;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        GTS_Entity hit = other.GetComponent<GTS_Entity>();
        if (hit != null)
        {
            if (hit == Shooter) return;
        }

        Explode();
    }

    private void Explode()
    {
        GameObject explosion = Instantiate(_explosionPrefab, transform.TransformPoint(_myCollider.center), Quaternion.identity);
        GTS_Explosion e = explosion.GetComponent<GTS_Explosion>();
        e.Damage = Damage;


        Destroy(gameObject);
    }
}
