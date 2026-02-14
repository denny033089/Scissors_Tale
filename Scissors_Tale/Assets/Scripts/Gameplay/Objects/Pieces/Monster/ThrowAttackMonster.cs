using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThrowAttackMonster : Monster
{    
    public override void InitializePath() {
        //움직이지 않음
    }
    
    public override AttackInfo[] GetAttacks()
    {
        List<Vector2Int> cross = new List<Vector2Int>()
        {

            new Vector2Int(0, 0),
            new Vector2Int(-1, 0),  
            new Vector2Int(1, 0), 
            new Vector2Int(0, 1),
            new Vector2Int(0,-1)
        };

        return new AttackInfo[]
        {
            new AttackInfo(AttackType.Splash, 3, 5, cross)
        };
    }
}
