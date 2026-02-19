using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FrontAttackMonster : Monster
{    
    public override AttackInfo[] GetAttacks()
    {
        // 2x3 범위를 위한 상대 좌표 리스트 생성
        // (x, y) 형태: x는 좌우(-1, 0, 1), y는 정면(1, 2, 3)
        List<Vector2Int> area3x2 = new List<Vector2Int>()
        {
            // new Vector2Int(-1, 1), new Vector2Int(-1, 2), // 정면 1열 (세로 2칸)
            // new Vector2Int(0, 1), new Vector2Int(0, 2), // 정면 2열 (세로 2칸)
            // new Vector2Int(1, 1), new Vector2Int(1, 2)  // 정면 3열 (세로 2칸)

            new Vector2Int(-1, 1),  // 정면 1열 (세로 2칸)
            new Vector2Int(0, 1),  // 정면 2열 (세로 2칸)
            new Vector2Int(1, 1)  // 정면 3열 (세로 2칸)
        };

        return new AttackInfo[]
        {
            // AttackType.Directional을 사용하여 플레이어 방향으로 이 범위를 출력합니다.
            new AttackInfo(AttackType.Directional, 2, 3, area3x2)
        };

        public override void PerformAttack() {
        
        if(anim != null) {
            PerformAnimation();
        }
            
        //데미지 적용을 collider 충돌 시로 이전
        
        turnCounter = 0; //턴카운터 초기화
        MonsterAttackManager.Instance.ClearAttackEffects(this); //이펙트 삭제
    }
    public override void PerformAnimation()
    {
        anim.SetTrigger("isAttacking");
    }

    public void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("충돌");
        if (other.CompareTag("Player")) {
        // 현재 공격 중(애니메이션 재생 중)일 때만 대미지 적용
            Debug.Log("플레이어충돌");
            if (anim.GetBool("isAttacking")) {

                if (other.TryGetComponent<Player>(out Player p)) 
                {
                    FindAttackTile(p.MyPos);
                }
                Debug.Log("캡슐 콜라이더 충돌! 플레이어 피격");
                // 플레이어 대미지 로직 실행
            }
        }
    }

    public void FindAttackTile((int,int) BoardPos) {
        AttackInfo info = attackPatterns[currentPatternIndex];
            
        List<Vector2Int> targetTiles = GetAttackTiles(info);
            // 계산된 타일들에 데미지 적용
        if(targetTiles.Contains(BoardPos.ToVector2Int())) {
            MonsterAttackManager.Instance.ApplyDamage(BoardPos.Item1, BoardPos.Item2, info.damage);
        }
    }
    }
}
