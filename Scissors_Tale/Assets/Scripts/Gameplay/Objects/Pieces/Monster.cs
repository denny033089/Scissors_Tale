using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Piece
{
    [Header("AI Settings")]
    public int PlanLength = 2; //미래 경로의 수

    // FIFO 큐: [Step1, Step2]
    private List<(int, int)> moveQueue = new List<(int, int)>();

    // 몬스터가 (GameManager에서)생성된 이후에 스폰 
    public void InitializePath()
    {
        // 1. 기존 N개의 경로 생성 (재귀)
        moveQueue = MonsterDirection.InitPathRecursive(MyPos, PlanLength);

        // 갇힌 상태로 스폰될때, 기존 포지션으로 큐 채우기
        while (moveQueue.Count < PlanLength)
        {
            (int x, int y) lastPos = (moveQueue.Count > 0) ? moveQueue[moveQueue.Count - 1] : MyPos;
            moveQueue.Add(lastPos);
        }

        Debug.Log($"Monster Initialized. Planned {moveQueue.Count} steps.");
    }
    
    // MonsterMove 단계에서 호출
    public void PerformTurn()
    {
        if (moveQueue.Count == 0) return;

        // 다음 경로 확인
        (int targetX, int targetY) = moveQueue[0];

        // 막혀있을때:
        Piece obstacle = GameManager.Instance.Pieces[targetX, targetY];

        if (obstacle != null)
        {
            //TODO
            //1.플레이어가 막을때
            //2.다른 기물이 막을때
        }


        // 큐에서 경로 삭제
        moveQueue.RemoveAt(0);

        // 이동
        GameManager.Instance.MovePlayer(this, (targetX, targetY));

        // 큐에 새 경로 추가
        (int lastPlannedX, int lastPlannedY) = (moveQueue.Count > 0) ? moveQueue[moveQueue.Count - 1] : (targetX, targetY);
        (int newX, int newY) = MonsterDirection.GetOneFutureStep((lastPlannedX, lastPlannedY));
        moveQueue.Add((newX, newY));
    }
}