using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : Player //02.04 상속 추가
{
    public GameObject SkillEffect;
    public override MoveInfo[] GetMoves()
    {
        // --- TODO ---

        return new MoveInfo[]
        {
            new MoveInfo(1, 0, 1),   // 오른쪽
            new MoveInfo(0, 1, 1),  // 위쪽
            new MoveInfo(-1, 0, 1), // 왼쪽
            new MoveInfo(0, -1, 1)   // 아래
        };
        // ------
    }

    public override void MoveTo((int, int) targetPos)
    {

        //플레이어2의 스프라이트가 왼쪽을 보고있으므로
        if(MyPos.Item1<targetPos.Item1) { //오른쪽으로 이동   
            MySpriteRenderer.flipX = true;
        } else if(MyPos.Item1>targetPos.Item1) { //왼쪽으로 이동
            MySpriteRenderer.flipX = false;
        }
        base.MoveTo(targetPos);
    }

    public override IEnumerator SpawnEffect() {
        yield return new WaitForSeconds(0.5f);
        
            // 일반 공격 이펙트 생성
        Instantiate(AttackEffect, Utils.ToRealPos(MyPos), Quaternion.identity);

    }
}
