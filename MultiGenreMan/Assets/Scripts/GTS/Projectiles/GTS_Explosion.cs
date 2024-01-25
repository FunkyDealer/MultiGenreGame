using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTS_Explosion : MonoBehaviour
{
    [HideInInspector]
    public int Damage;


    [SerializeField]
    float _radius;
    [SerializeField]
    float _lifeTime;

    // Start is called before the first frame update
    void Start()
    {
        Explode();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);

        for (int i = 0; i < colliders.Length; i++)
        {
            GTS_Entity hitEntity = colliders[i].gameObject.GetComponent<GTS_Entity>();

            if (hitEntity != null)
            {
                hitEntity.ReceiveDamage(Damage, transform.position);
            }
        }

        Destroy(gameObject, _lifeTime);
    }
}
