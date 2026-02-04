using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


//01.19 정수민 로직 전부 하위 monster에서 받을 수 있도록 virtual로 수정
public class Monster : Piece
{
    [Header("UI")]
    public TMP_Text HPText;

    public GameObject DamagePopupPrefab;
    public Transform PopupSpawnPoint;

    [Header("스탯")]
    public int CurrentHP = 10;
    public int MaxHP = 10;

    [Header("경로")]
    public int PlanLength = 2; //미래 경로의 수
    public GameObject ArrowPrefab; 
    private List<GameObject> _spawnedArrows = new List<GameObject>();

    // FIFO ť: [Step1, Step2]
    public List<(int, int)> moveQueue = new List<(int, int)>();


    //02.04 정수민
    public List<AttackInfo> attackPatterns;
    protected int currentPatternIndex = 0;

    public int turnCounter = 0;

    // 스폰시 호출
    //01.19 정수민: InitializeStats 인수 삭제
    public virtual void InitializeStats()
    {

        UpdateHPText();

        if (HPText != null)
        {
            
            Canvas canvas = HPText.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                
                canvas.overrideSorting = true;
                canvas.sortingOrder = 30; 
            }
            else
            {
                Renderer textRenderer = HPText.GetComponent<Renderer>();
                if (textRenderer != null)
                {
                    textRenderer.sortingOrder = 30;
                }
            }
        }

        InitializePath();
    }

    public virtual void SpawnDamageEffect(Sprite sprite)
    {
        if (DamagePopupPrefab == null || sprite == null) return;

        // 몬스터 위에서 스폰
        Vector3 spawnPos = (PopupSpawnPoint != null) ? PopupSpawnPoint.position : transform.position + Vector3.up * 1.0f;

        GameObject popupObj = Instantiate(DamagePopupPrefab, spawnPos, Quaternion.identity);
        DamagePopup popupScript = popupObj.GetComponent<DamagePopup>();

        if (popupScript != null)
        {
            popupScript.Setup(sprite);
        }
    }

    // 데미지 받을때
    public virtual void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0) CurrentHP = 0;

        SoundManager.Instance.PlaySFX("MonsterHurt");

        UpdateHPText();

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public virtual void UpdateHPText()  //01.19 정수민: public virtual로 수정
    {
        if (HPText != null)
        {
            HPText.text = $"{CurrentHP}/{MaxHP}";
        }
    }

    public virtual void Die() //01.19 정수민: public virtual로 수정 + MapManager 추가
    {
        Debug.Log("몬스터 사망");
        // 보드에서 지우기
        MapManager.Instance.Pieces[MyPos.Item1, MyPos.Item2] = null;

        // 화살표 삭제
        foreach (var arrow in _spawnedArrows) if (arrow != null) Destroy(arrow);

        // 오브젝트 삭제
        Destroy(gameObject);

    }


    // 몬스터가 (GameManager에서) 생성된 이후에 스폰 
    public virtual void InitializePath()
    {
        // 기존 N개의 경로 생성 (재귀)
        moveQueue = MonsterDirection.InitPathRecursive(MyPos, PlanLength);

        // 갇힌 상태로 스폰될때, 기존 포지션으로 큐 채우기
        while (moveQueue.Count < PlanLength)
        {
            (int x, int y) lastPos = (moveQueue.Count > 0) ? moveQueue[moveQueue.Count - 1] : MyPos;
            moveQueue.Add(lastPos);
        }

        Debug.Log($"Monster Initialized. Planned {moveQueue.Count} steps.");
        DrawPath();
    }

    // MonsterMove 단계에서 호출
    public virtual void PerformTurn()
    {
        if (moveQueue.Count == 0) return;

        // 다음 경로 확인
        (int targetX, int targetY) = moveQueue[0];

        // 막혀있을때:
        Piece obstacle = MapManager.Instance.Pieces[targetX, targetY];

        if (obstacle != null)
        {
            Debug.Log($"Monster blocked at ({targetX},{targetY}) by {obstacle.name}. Recalculating path...");

            // 기존 막힌 경로 삭제
            moveQueue.Clear();

            // 현재 pos에서 새로운 경로 생성
            moveQueue = MonsterDirection.InitPathRecursive(MyPos, PlanLength);

            // 더이상 못움직일때
            if (moveQueue.Count == 0)
            {
                Debug.Log("Monster is surrounded and cannot move.");
                // 기존 포지션 반환
                for (int i = 0; i < PlanLength; i++) moveQueue.Add(MyPos);
                DrawPath();
                return; // 스킵하기
            }
            // 새 경로로 타겟 변경
            (targetX, targetY) = moveQueue[0];
        }

        // 큐에서 경로 삭제
        moveQueue.RemoveAt(0);

        // 이동
        //GameManager.Instance.MovePlayer(this, (targetX, targetY));
        this.MoveTo((targetX,targetY));


        // 큐에 새 경로 추가
        (int lastPlannedX, int lastPlannedY) = (moveQueue.Count > 0) ? moveQueue[moveQueue.Count - 1] : (targetX, targetY);
        (int newX, int newY) = MonsterDirection.GetOneFutureStep((lastPlannedX, lastPlannedY));
        moveQueue.Add((newX, newY));

        DrawPath();
    }

    public virtual void DrawPath()
    {
        // 기존 화살표 삭제
        foreach (var arrow in _spawnedArrows)
        {
            if (arrow != null) Destroy(arrow);
        }
        _spawnedArrows.Clear();

        if (ArrowPrefab == null) return;

        (int currentX, int currentY) = MyPos;

        foreach ((int nextX, int nextY) in moveQueue)
        {
            if (currentX == nextX && currentY == nextY) continue;

            Vector3 startPos = Utils.ToRealPos((currentX, currentY));
            Vector3 endPos = Utils.ToRealPos((nextX, nextY));

            
            Vector3 midPoint = (startPos + endPos) / 2f;

            
            Vector3 spawnPos = new Vector3(midPoint.x, midPoint.y, 0f);

            GameObject arrowObj = Instantiate(ArrowPrefab, spawnPos, Quaternion.identity);

            // 방향 변경
            Vector3 dir = endPos - startPos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrowObj.transform.rotation = Quaternion.Euler(0, 0, angle);

            // 스케일에 맞춰 화살표 크기 조정
            // 제미나이가 추천했지만 필요 없을것 같습니다
            float dist = Vector3.Distance(startPos, endPos);
            arrowObj.transform.localScale = new Vector3(dist, 1, 1);

            _spawnedArrows.Add(arrowObj);

            (currentX, currentY) = (nextX, nextY);
        }
    }



    //02.04 정수민
    //공격범위에 해당하는 타일 획득
    public List<Vector2Int> GetAttackTiles(AttackInfo info) {
        List<Vector2Int> targetTiles = new List<Vector2Int>();

        switch (info.type)
        {
            case AttackType.FullLine:
                // 몬스터가 있는 줄 전체 (X축 0부터 끝까지)
                for (int x = 0; x < Utils.FieldWidth; x++) 
                    targetTiles.Add(new Vector2Int(x, MyPos.Item2));
                break;

            case AttackType.Directional:
                // 플레이어 방향을 구하고, 그 방향에 맞춰 areaOffsets를 회전/적용
                Vector2Int dir = MonsterAttackManager.Instance.GetDirectionToPlayer(this);
                foreach (var offset in info.areaOffsets)
                    targetTiles.Add(MonsterAttackManager.Instance.RotateOffset(offset, dir));
                break;

            case AttackType.Splash:
                // 플레이어의 현재 위치를 중심점으로 잡고 범위 타격
                Vector2Int center = GameManager.Instance.p1Instance.MyPos.ToVector2Int();
                foreach (var offset in info.areaOffsets)
                    targetTiles.Add(center + offset);
                break;
        }

        return targetTiles;

    }


    //02.04 정수민 특별한 기믹이 있다면 override하여 사용할 것
    public virtual void PerformAttack()
    {
        
        if (attackPatterns.Count == 0) return;
        AttackInfo info = attackPatterns[currentPatternIndex];
        
        //공격범위 안에 있을때만 공격
        if(MonsterAttackManager.Instance.isInRange(this)) {

            turnCounter++; // 공격범위 안에 들어가면 일단 공격안함, 1턴 지나면 카운트가 증가하여 공격함
            
            // 2의 배수가 아닐 때는 공격하지 않고 종료 (1턴 쉬고 2턴째 공격)
            if (turnCounter == 0) {
                Debug.Log($"{this.name}: 기를 모으는 중... (다음 턴에 공격)");
                return; 
            }
            List<Vector2Int> targetTiles = GetAttackTiles(info);
            // 계산된 타일들에 데미지 적용
            foreach (var tile in targetTiles) {
                MonsterAttackManager.Instance.ApplyDamage(tile.x, tile.y, info.damage);
            }
        } else {
            turnCounter = 0;
        }
    }

}