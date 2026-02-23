using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BatMonster2 : BatMonster
{    
    
    public override AttackInfo[] GetAttacks()
    {
        List<Vector2Int> cross = new List<Vector2Int>()
        {

            new Vector2Int(0, 0),  //중앙부분
            new Vector2Int(-1, 0),  
            new Vector2Int(1, 0), 
            new Vector2Int(0, 1),
            new Vector2Int(0,-1)
        };

        return new AttackInfo[]
        {
            new AttackInfo(AttackType.Splash, 2, 5, cross) //데미지만 다름
        };
    }
}
