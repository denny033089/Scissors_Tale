using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Needle_Animation : MonoBehaviour
{
    float speed;
    Vector3 direction;
    Vector3 lastPosition;
    private BatMonster batmonster;
    private AttackInfo cachedInfo; // batmonster 참조 에러 방지를 위해 미리 저장
    public GameObject PoisonEffect;
    public void DestroySelf() {
        Destroy(gameObject);
    }

    public void Setup(BatMonster owner,Vector3 start, Vector3 target, float speed) {
        this.batmonster = owner;
        this.cachedInfo = owner.attackPatterns[owner.currentPatternIndex];
        // DOJump(목표위치, 점프높이, 점프횟수, 지속시간)
        float duration = Vector3.Distance(start, target) / speed;
        
        transform.DOJump(target, 1.5f, 1, duration)
                .SetEase(Ease.Linear) // 등속 이동
                .OnComplete(() => {
                    ApplyAttack();
                    SpawnPoisonEffect(target);
                    Destroy(gameObject);
                });
    }
    void Start() {
        lastPosition = transform.position;

    }
    
    void Update() {
        // 이전 프레임과 현재 프레임의 위치 차이로 방향 계산, 방향에 따라 투사체의 각도 변화
        Vector3 dir = transform.position - lastPosition;
        if (dir != Vector3.zero) {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
        lastPosition = transform.position;
    }

    public void ApplyAttack() {  //플레이어 피격 타이밍...

        List<Vector2Int> targetTiles = batmonster.GetAttackTiles(cachedInfo);
            // 계산된 타일들에 데미지 적용
        foreach (var tile in targetTiles) {
            MonsterAttackManager.Instance.ApplyDamage(tile.x, tile.y, cachedInfo.damage);
        }
     
        batmonster.turnCounter = 0; //턴카운터 초기화
        MonsterAttackManager.Instance.ClearAttackEffects(batmonster); //이펙트 삭제


    }
    
    
    public void SpawnPoisonEffect(Vector3 targetposition) {
        Instantiate(PoisonEffect, targetposition, Quaternion.identity);
    }
}
