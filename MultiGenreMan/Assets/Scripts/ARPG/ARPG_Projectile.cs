using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPG_Projectile : MonoBehaviour
{
    public ARPG_Creature _shooter;
    public ARPG_Creature _target;

    [SerializeField]
    private int _damage = 50;
    [SerializeField]
    private float _speed;

    private Vector3 _direction;

    [SerializeField]
    private float _lifeTime;

    // Start is called before the first frame update
    void Start()
    {

        if (_target != null) _direction = _target.gameObject.transform.position - transform.position;
        else _direction = _shooter.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {


        transform.position += _direction * _speed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (_target != null)
        _direction = _target.gameObject.transform.position - transform.position;
        _direction.Normalize();

        

    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            ARPG_Creature creature = other.GetComponent<ARPG_Creature>();

            if (creature == null)
            {
                Destroy(gameObject);
                return;
            }

            if (creature != _shooter)
            {
                creature.ReceiveDamage(_damage);
                Destroy(gameObject);
            }
        }
        

    }

    public void GetInfo(ARPG_Creature shooter, ARPG_Creature target)
    {
        this._shooter = shooter;
        this._target = target;
    }

    private IEnumerator TimeOut(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }

}
