using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPG_Player : ARPG_Creature
{
    private ARPG_MainCamera _camera;



    protected override void Awake()
    {
        base.Awake();


    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();



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


    protected override void ReceiveDamage(int damage)
    {
        base.ReceiveDamage(damage);


    }

    public void GetCamera(ARPG_MainCamera mainCamera)
    {
        this._camera = mainCamera;


    }
}
