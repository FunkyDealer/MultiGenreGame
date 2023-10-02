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

    private void Awake()
    {
        _myLineRenderer = GetComponentInChildren<LineRenderer>();

    }


    // Start is called before the first frame update
    void Start()
    {       
        Vector3 start = Vector3.zero;
        Vector3 end = transform.InverseTransformPoint(HitPoint);  //transform the hit point from world space to local space

        _myLineRenderer.SetPosition(0, start);
        _myLineRenderer.SetPosition(1, end);


         //Debug.Break();

        StartCoroutine(CountDownToDeath(_lifeTime));
    }

    // Update is called once per frame
    void Update()
    {
       
        

    }

    private void FixedUpdate()
    {
        
    }

    private IEnumerator CountDownToDeath(float time)
    {
        yield return new WaitForSeconds(time);

        //this.gameObject.SetActive(false);
        Destroy(gameObject);
    }



}
