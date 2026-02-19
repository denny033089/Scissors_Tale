using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class FullLineMonster : Monster
{        
    public override AttackInfo[] GetAttacks()
    {

        return new AttackInfo[]
        {
            new AttackInfo(AttackType.XAxisLine, 3, 200)
        };
    }

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
        StartCoroutine(ProcessMoveAndAttack());
    }

    public IEnumerator ProcessMoveAndAttack() {
        float moveDuration = 2f;
        Vector2 realPos = Utils.ToRealPos(MyPos);

        // 1. 애니메이션 시작
        anim.SetBool("isAttacking", true);

        // 2. DOTween Sequence 생성 (순차적 실행)
        Sequence moveSeq = DOTween.Sequence();

        moveSeq.Append(transform.DOMove(new Vector2(-15f, realPos.y), moveDuration).SetEase(Ease.OutQuad))
            .AppendCallback(() => {
                // 화면 밖으로 나간 즉시 반대편으로 순간이동
                transform.position = new Vector2(15f, realPos.y);
            })
            .Append(transform.DOMove(realPos, moveDuration).SetEase(Ease.OutQuad))
            .OnComplete(() => {
                // 모든 이동이 끝난 후 애니메이션 종료
                anim.SetBool("isAttacking", false);
            });

        // Sequence가 끝날 때까지 코루틴 대기
        yield return moveSeq.WaitForCompletion();
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
