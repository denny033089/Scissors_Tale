using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Monster : Piece
{
    [Header("UI Settings")]
    public TMP_Text HPText;

    [Header("Stats")]
    public int CurrentHP = 10;
    public int MaxHP = 10;

    [Header("AI Settings")]
    public int PlanLength = 2; //미래 경로의 수

    [Header("Visualization")]
    public GameObject ArrowPrefab; 
    private List<GameObject> _spawnedArrows = new List<GameObject>();

    // FIFO ť: [Step1, Step2]
    public List<(int, int)> moveQueue = new List<(int, int)>();

    // 스폰시 호출
    public void InitializeStats(int hp)
    {
        MaxHP = hp;
        CurrentHP = hp;
        UpdateHPText();
        InitializePath();
    }

    // 데미지 받을때
    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;
        if (CurrentHP < 0) CurrentHP = 0;

        UpdateHPText();

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    private void UpdateHPText()
    {
        if (HPText != null)
        {
            HPText.text = $"{CurrentHP}/{MaxHP}";
        }
    }

    private void Die()
    {
        Debug.Log("몬스터 사망");
        // 보드에서 지우기
        GameManager.Instance.Pieces[MyPos.Item1, MyPos.Item2] = null;

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
        Piece obstacle = GameManager.Instance.Pieces[targetX, targetY];

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

    public void DrawPath()
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
}