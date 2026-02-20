using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BatMonster : Monster
{    
    [SerializeField]
    private GameObject NeedlePrefab;
    [SerializeField]
    private Transform EffectSpawner;
    
    public override void InitializePath() {
        //움직이지 않음
    }
    
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
            new AttackInfo(AttackType.Splash, 3, 5, cross)
        };
    }

    public override void PerformAttack() {
        
        if(anim != null) {
            PerformAnimation();
        }
    }
    
    public override void PerformAnimation()
    {
        anim.SetTrigger("isAttacking");
    }

    public void MakeNeedleEffect() {

        StartCoroutine(ProcessNeedleEffect());

    }

    public IEnumerator ProcessNeedleEffect() {
        yield return new WaitForSeconds(0.3f);
        AttackInfo info = attackPatterns[currentPatternIndex];
   
        List<Vector2Int> targetTiles = GetAttackTiles(info);
        Vector3 TargetPos = Utils.ToRealPos(targetTiles[0].ToTuple());
    
        GameObject Needle = Instantiate(NeedlePrefab, EffectSpawner.position, Quaternion.identity);
        Needle.GetComponent<Needle_Animation>().Setup(this,EffectSpawner.position,TargetPos, 7f); // 속도 7로 발사

    }

    public void ApplyAttack() {
        StartCoroutine(ProcessApplyAttack());
    }

    public IEnumerator ProcessApplyAttack() {  //플레이어 피격 타이밍...
        yield return new WaitForSeconds(0.3f);

        AttackInfo info = attackPatterns[currentPatternIndex];
        List<Vector2Int> targetTiles = GetAttackTiles(info);
            // 계산된 타일들에 데미지 적용
        foreach (var tile in targetTiles) {
            MonsterAttackManager.Instance.ApplyDamage(tile.x, tile.y, info.damage);
        }
     
        turnCounter = 0; //턴카운터 초기화
        MonsterAttackManager.Instance.ClearAttackEffects(this); //이펙트 삭제
    }
}
