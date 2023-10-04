using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Projectile : MonoBehaviour
{
    [SerializeField]
    protected float _lifeTime = 0.3f;

    [HideInInspector]
    public FPS_Creature Shooter;


    protected virtual void Awake()
    {

    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual IEnumerator CountDownToDeath(float time)
    {
        yield return new WaitForSeconds(time);

        //this.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
