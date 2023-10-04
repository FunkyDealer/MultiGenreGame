using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_LaserProjectile : MonoBehaviour
{
    [SerializeField]
    private float _lifeTime = 0.3f;

    private LineRenderer _myLineRenderer;

    [HideInInspector]
    public FPS_Creature Shooter;
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


    private void Awake()
    {
        _myLineRenderer = GetComponentInChildren<LineRenderer>();

    }


    // Start is called before the first frame update
    void Start()
    {       
        _start = Vector3.zero;
        _end = transform.InverseTransformPoint(HitPoint);  //transform the hit point from world space to local space

        _direction = _end - _start;

        _myLineRenderer.SetPosition(0, _start);
        _myLineRenderer.SetPosition(1, _end);


         //Debug.Break();

        StartCoroutine(CountDownToDeath(_lifeTime));
    }

    // Update is called once per frame
    void Update()
    {
       
        

    }

    private void FixedUpdate()
    {
        _start += _direction * 2.5f * Time.deltaTime;

        _myLineRenderer?.SetPosition(0, _start);
    }

    private IEnumerator CountDownToDeath(float time)
    {
        yield return new WaitForSeconds(time);

        //this.gameObject.SetActive(false);
        Destroy(gameObject);
    }



}
