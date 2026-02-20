using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class SeagullMonster : Monster
{    
    [SerializeField]
    private GameObject CutterPrefab;
    [SerializeField]
    private Transform EffectSpawner;
    
    public override AttackInfo[] GetAttacks()
    {
        // 2x3 범위를 위한 상대 좌표 리스트 생성
        // (x, y) 형태: x는 좌우(-1, 0, 1), y는 정면(1, 2, 3)
        List<Vector2Int> areaH = new List<Vector2Int>()
        {
            // new Vector2Int(-1, 1), new Vector2Int(-1, 2), // 정면 1열 (세로 2칸)
            // new Vector2Int(0, 1), new Vector2Int(0, 2), // 정면 2열 (세로 2칸)
            // new Vector2Int(1, 1), new Vector2Int(1, 2)  // 정면 3열 (세로 2칸)

            new Vector2Int(-1, 1),
            new Vector2Int(1, 1),  
            new Vector2Int(-1, 2),
            new Vector2Int(0, 2),  //H의 중앙
            new Vector2Int(1, 2),
            new Vector2Int(-1, 3),
            new Vector2Int(1, 3)
        };

        return new AttackInfo[]
        {
            // AttackType.Directional을 사용하여 플레이어 방향으로 이 범위를 출력합니다.
            new AttackInfo(AttackType.Directional, 1, 5, areaH)
        };

        
    }

    public override void PerformAttack() {
        
        if(anim != null) {
            PerformAnimation();
        }
            

        ApplyAttack();
    }
    
    public override void PerformAnimation()
    {
        anim.SetTrigger("isAttacking");
        transform.DOShakePosition(0.5f, 0.2f, 20, 90, false, true); //기모으는 동안 떨림

    }

    public void MakeWingEffect() {

        StartCoroutine(ProcessWingEffect());

    }

    public IEnumerator ProcessWingEffect() {
        AttackInfo info = attackPatterns[currentPatternIndex];
        Vector2Int dir = MonsterAttackManager.Instance.GetDirectionToPlayer(this);
   
        List<Vector2Int> targetTiles = GetAttackTiles(info);
        //Vector3 LeftPos = Utils.ToRealPos(targetTiles[0].ToTuple());  //H의 좌측부분에 위치설정
        //Vector3 RightPos = Utils.ToRealPos(targetTiles[1].ToTuple()); //H의 우측부분
        //Vector3 midPos = Utils.ToRealPos(targetTiles[3].ToTuple()); //H의 중앙

        Vector2Int rotatedLeftOrigin = MonsterAttackManager.Instance.RotateOffset(new Vector2Int(-1, 1), dir);
        Vector2Int rotatedRightOrigin = MonsterAttackManager.Instance.RotateOffset(new Vector2Int(1, 1), dir);
        Vector2Int rotatedMidOrigin = MonsterAttackManager.Instance.RotateOffset(new Vector2Int(0, 2), dir);

        // 3. 실제 월드 좌표로 변환 (몬스터 위치 + 회전된 오프셋)
        Vector3 LeftPos = Utils.ToRealPos((MyPos.ToVector2Int() + rotatedLeftOrigin).ToTuple());
        Vector3 RightPos = Utils.ToRealPos((MyPos.ToVector2Int() + rotatedRightOrigin).ToTuple());
        Vector3 midPos = Utils.ToRealPos((MyPos.ToVector2Int() + rotatedMidOrigin).ToTuple());



        SpawnCutter(LeftPos, (midPos - LeftPos).normalized);
        SpawnCutter(RightPos, (midPos - RightPos).normalized);
        yield return new WaitForSeconds(0.1f);
        LeftPos -= new Vector3(0f,0.2f,0f);
        RightPos -= new Vector3(0f,0.2f,0f);
        SpawnCutter(LeftPos, (midPos - LeftPos).normalized);
        SpawnCutter(RightPos, (midPos - RightPos).normalized);

    }

    public void SpawnCutter(Vector3 pos,Vector3 direction) {
        GameObject Cutter = Instantiate(CutterPrefab, pos, Quaternion.identity);
        // 칼날 스크립트에 방향과 속도 전달
        if(Cutter.TryGetComponent<Cutter_Animation>(out Cutter_Animation c)) {
            c.Setup(direction, 5f); // 방향과 이동 속도 5 설정
        }
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
