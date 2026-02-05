using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterAttackManager : Singleton<MonsterAttackManager>
{
    public Sprite AttackSprite; 
    
    Vector2Int p1pastposition;
    Vector2Int p2pastposition;
    private List<GameObject> currentAttackEffects = new List<GameObject>();
    

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MonsterAttack(Monster monster) {
        StartCoroutine(ProcessAttackSequence(monster));
    }

    private IEnumerator ProcessAttackSequence(Monster monster)
    {        
        monster.PerformAttack();    
        yield return new WaitForSeconds(0.1f);
        // 플레이어 확인
    }
    
    //가까운 플레이어의 위치를 얻어옴->몬스터의 공격방향 결정
    public Vector2Int FindClosePlayerPosition(Monster monster) {
        float distance1 = -1;
        float distance2 = -1;
        Vector2Int PlayerPos = monster.MyPos.ToVector2Int();
        if(GameManager.Instance.p1Instance != null) {
            distance1 = Vector2.Distance(p1pastposition, monster.MyPos.ToVector2Int());
            PlayerPos = p1pastposition;
        }
        if(GameManager.Instance.p2Instance != null) {
            distance2 = Vector2.Distance(p2pastposition, monster.MyPos.ToVector2Int());
            if(distance2<distance1) {
                PlayerPos = p2pastposition;
            }
        }

        return PlayerPos;

    }

    //사거리 안에 있는지 확인
    public bool isInRange(Monster monster) {
        // 1. 공격 정보 가져오기 (리스트의 첫 번째 패턴 기준)
        if (monster.attackPatterns == null || monster.attackPatterns.Count == 0) return false;
        AttackInfo info = monster.attackPatterns[0]; 

        // 2. 플레이어와 몬스터의 위치 가져오기 (Vector2Int 변환)
        Vector2Int playerPos = FindClosePlayerPosition(monster);
        Vector2Int monsterPos = monster.MyPos.ToVector2Int();

        // 3. 거리 계산 (직선 거리)
        float distance = Vector2Int.Distance(playerPos, monsterPos);

        // 4. 사거리(range) 안에 들어왔는지 확인
        if (distance <= info.range)
        {
            Debug.Log($"{monster.name}: 사거리 내 플레이어 발견!");
            return true;
        }
        else
        {
            Debug.Log($"{monster.name}: 플레이어가 너무 멉니다. (거리: {distance})");
            return false;
        }

    }
    
    //플레이어가 이동하기 전의 위치정보를 얻어옴
    public void GetPastPosition(Vector2Int pos1,Vector2Int pos2) {
        p1pastposition = pos1;
        p2pastposition = pos2;
    }

    public void ApplyDamage(int x, int y, int damage) {
        if(MapManager.Instance.Pieces[x,y] == GameManager.Instance.p1Instance) {
            DamageCalculator.Instance.CalculateDamage(GameManager.Instance.p1Instance,damage);
        } else if(MapManager.Instance.Pieces[x,y] == GameManager.Instance.p2Instance) {
            DamageCalculator.Instance.CalculateDamage(GameManager.Instance.p2Instance,damage);
        } else return;
    }

    public Vector2Int GetDirectionToPlayer(Monster monster) {
            // 1. 목표 플레이어 위치 가져오기 (가장 가까운 플레이어)
        // 앞서 만든 MonsterAttackManager의 FindClosePlayerPosition을 활용하거나 직접 참조
        Vector2Int playerPos = FindClosePlayerPosition(monster);
        Vector2Int monsterPos = monster.MyPos.ToVector2Int();

        // 2. 두 지점 간의 거리 차이 계산
        Vector2Int diff = playerPos - monsterPos;

        // 3. 더 멀리 떨어진 축을 기준으로 방향 결정 (상하좌우 4방향)
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            // X축 차이가 더 크면 좌(+) 또는 우(-)
            return new Vector2Int(diff.x > 0 ? 1 : -1, 0);
        }
        else
        {
            // Y축 차이가 더 크거나 같으면 위(+) 또는 아래(-)
            return new Vector2Int(0, diff.y > 0 ? 1 : -1);
        }
    }

    public Vector2Int RotateOffset(Vector2Int offset, Vector2Int dir) {
        // 정면(위) 방향: (0, 1) -> 변환 없음
        if (dir.x == 0 && dir.y == 1) return offset;

        // 아래 방향: (0, -1) -> 180도 회전 (x, y -> -x, -y)
        if (dir.x == 0 && dir.y == -1) return new Vector2Int(-offset.x, -offset.y);

        // 오른쪽 방향: (1, 0) -> 90도 회전 (x, y -> y, -x)
        if (dir.x == 1 && dir.y == 0) return new Vector2Int(offset.y, -offset.x);

        // 왼쪽 방향: (-1, 0) -> 270도 회전 (x, y -> -y, x)
        if (dir.x == -1 && dir.y == 0) return new Vector2Int(-offset.y, offset.x);

        return offset; // 예외 케이스 처리

    }

    public void ShowPossibleMonsterAttack(Monster monster) {

        if (monster.attackPatterns.Count > 0)
        {
            AttackInfo info = monster.attackPatterns[monster.currentPatternIndex];
            List<Vector2Int> tiles = monster.GetAttackTiles(info);
            Debug.Log($"계산된 타일 수: {tiles.Count}"); // 타일이 0개는 아닌지 확인

            foreach (Vector2Int tile in tiles)
            {
                Vector3 realPos = Utils.ToRealPos(tile.ToTuple());
                Debug.Log($"이펙트 생성 위치: {realPos}");
                GameObject effect = Instantiate(GameManager.Instance.AttackEffectPrefab, realPos, Quaternion.identity,GameManager.Instance.EffectParent);
                    
                // 나중에 지우기 위해 리스트에 보관
                monster.currentAttackEffects.Add(effect);
            }
        }
    }

    public void ClearAttackEffects(Monster monster) {
        foreach (GameObject obj in monster.currentAttackEffects)
        {
            if (obj != null) Destroy(obj);
        }
        monster.currentAttackEffects.Clear();
    }

    public virtual void ShowDamageEffect(int damage,Piece piece)
    {
        StartCoroutine(ProcessEffectSequence(damage,piece));
    }

    private IEnumerator ProcessEffectSequence(int damage,Piece piece) {
        for(int i = 0;i<damage;i++) {
            if (piece is Player1 player1)
            {
                player1.SpawnDamageEffect(AttackSprite);
            }
            if (piece is Player2 player2)
            {
                player2.SpawnDamageEffect(AttackSprite);
            }
            yield return new WaitForSeconds(0.1f);
        }

    }

}
