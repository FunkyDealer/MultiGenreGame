using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Hazard : MonoBehaviour
{
    Dictionary<int, FPS_Creature> _creatureList;

    [SerializeField]
    private int _damage;

    [SerializeField]
    private float _startDelay;
    [SerializeField]
    private float _damageDelay;

    private void Awake()
    {
        _creatureList = new Dictionary<int, FPS_Creature>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        FPS_Creature c = other.gameObject.GetComponent<FPS_Creature>();

        if (c != null && !_creatureList.ContainsKey(other.GetInstanceID())) _creatureList.Add(other.GetInstanceID(), c);

        if (c != null && c.HazardReady) c.ReceiveDamageFromHazard(0, _startDelay);

    }

    private void OnTriggerStay(Collider other)
    {
        if (_creatureList.ContainsKey(other.GetInstanceID()))
        {
            FPS_Creature c = _creatureList[other.GetInstanceID()];

            if (c.HazardReady) c.ReceiveDamageFromHazard(_damage, _damageDelay);


        }
    }



}
