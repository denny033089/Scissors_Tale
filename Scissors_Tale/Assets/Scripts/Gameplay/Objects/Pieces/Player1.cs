using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : Player // 02.04 상속 추가
{
    

    public override void Awake() {
        base.Awake();
        MySpriteRenderer.material.SetFloat("_Thickness",thickness);
    }
    
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
        //이동하기 전에 스프라이트방향 결정
        //플레이어1의 스프라이트가 오른쪽을 보고있으므로
        if(MyPos.Item1<targetPos.Item1) { //오른쪽으로 이동   
            MySpriteRenderer.flipX = false;
        } else if(MyPos.Item1>targetPos.Item1) { //왼쪽으로 이동
            MySpriteRenderer.flipX = true;
        }
        base.MoveTo(targetPos);
    }
}

