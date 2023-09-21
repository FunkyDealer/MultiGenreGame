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
    public Vector3 EyePoint;
    [HideInInspector]
    public Vector3 ShootingDirection;

    private Vector3 _ContactPoint;

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
        Vector3 end = EyePoint + ShootingDirection * 100;

        RaycastHit hit;

        if (Physics.Raycast(EyePoint, ShootingDirection, out hit, 100))
        {

            Debug.Log("Something was hit");

            _myLineRenderer.startColor = _hitEnvironmentColor;
            _myLineRenderer.endColor = _hitEnvironmentColor;

            end = transform.InverseTransformPoint(hit.point); //transform the hit point from world space to local space

            FPS_Creature target = hit.collider.gameObject.GetComponent<FPS_Creature>();
            if (target != null)
            {
                _myLineRenderer.startColor = _hitEnemyColor;
                _myLineRenderer.endColor = _hitEnemyColor;
                target.ReceiveDamage(50);
            }
            

        }
        else
        {
            _myLineRenderer.startColor = _hitNothingColor;
            _myLineRenderer.endColor = _hitNothingColor;

            Debug.Log("Nothing was hit");
        }

        _myLineRenderer.SetPosition(0, start);
        _myLineRenderer.SetPosition(1, end);

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
