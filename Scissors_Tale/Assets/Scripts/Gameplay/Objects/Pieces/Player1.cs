using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1 : Player // 02.04 상속 추가
{
    

    public GameObject SkillEffect;
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

    public override IEnumerator SpawnEffect() {

        int myIndex = (this == GameManager.Instance.p1Instance) ? 0 : 1; 
        int currentAOE = (SkillManager.Instance != null) ? SkillManager.Instance.GetAOE(myIndex) : 1;

        yield return new WaitForSeconds(0.5f);

        Debug.Log($"GetAOE의 값: {SkillManager.Instance.GetAOE(myIndex)}");
        // 현재 공격 범위가 확장된 상태(스킬 사용 중)인지 확인
        if (currentAOE == 2) 
        {
            // 스킬 전용 이펙트 생성 (일반 공격 이펙트 대신 생성하거나 둘 다 생성 가능)
            Instantiate(SkillEffect, Utils.ToRealPos(MyPos), Quaternion.identity);
            Instantiate(AttackEffect, Utils.ToRealPos(MyPos), Quaternion.identity);
            Debug.Log("강화 공격 이펙트 출력!");
        }
        else 
        {
            // 일반 공격 이펙트 생성
            Instantiate(AttackEffect, Utils.ToRealPos(MyPos), Quaternion.identity);
        }
    }
}

