using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_HealthDispenser : FPS_Interactible
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact(FPSPlayer p)
    {
        base.Interact(p);

        Tuple<int, int> healthStatus = p.GetHealthStatus();
        int current = healthStatus.Item2;
        int max = healthStatus.Item1;

        if (current < max) p.ReceiveHealth(5);

    }
}
