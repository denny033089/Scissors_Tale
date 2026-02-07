using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FullLineMonster : Monster
{        
    public override AttackInfo[] GetAttacks()
    {

        return new AttackInfo[]
        {
            new AttackInfo(AttackType.XAxisLine, 3, 200)
        };
    }
}
