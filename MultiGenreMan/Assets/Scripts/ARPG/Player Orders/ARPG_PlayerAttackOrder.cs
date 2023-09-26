using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPG_PlayerAttackOrder : ARPG_PlayerOrder
{

    public ARPG_Creature Enemy { get; private set; }

    public ARPG_PlayerAttackOrder(ARPG_Creature enemy)
    {
        this.Enemy = enemy;
    }
}
