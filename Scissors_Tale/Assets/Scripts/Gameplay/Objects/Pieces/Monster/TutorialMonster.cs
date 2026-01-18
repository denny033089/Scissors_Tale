using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//01.18 정수민
public class TutorialMonster : Monster
{
    
    [Header("Tutorial Path")]
    // 유니티 인스펙터에서 정해줄 좌표 리스트 (예: (1,2), (2,2), (2,3)...)
    public List<Vector2Int> fixedPath = new List<Vector2Int>();
    private int _currentPathIndex = 0;
    
    public override void InitializePath() {  //맨처음 placepiece 때 호출 되는 함수
        
        moveQueue.Clear();

        _currentPathIndex = 0;

        for (int i = 0; i < PlanLength && i < fixedPath.Count; i++) //moveQueue의 맨앞 두번째만 루트를 보여주도록
        {
            moveQueue.Add((fixedPath[i].x, fixedPath[i].y));
        }

        Debug.Log($"Monster Initialized. Planned {moveQueue.Count} steps.");
        DrawPath();
    }

    public override void PerformTurn() {  //매 턴마다 호출되는 함수
        if (fixedPath.Count == 0 || _currentPathIndex >= fixedPath.Count)
        {
            Debug.Log("Tutorial Monster has reached the end of its path.");
            return;
        }

        // 현재 가야 할 목표 지점
        Vector2Int targetPos = fixedPath[_currentPathIndex];

        // 장애물 체크 (부모 로직 응용)
        Piece obstacle = GameManager.Instance.Pieces[targetPos.x, targetPos.y];
        if (obstacle != null)
        {
            Debug.Log($"Tutorial Monster blocked by {obstacle.name}. Waiting...");
            return; // 튜토리얼이므로 경로를 재계산하지 않고 기다리게 설정
        }

        // 이동 실행
        this.MoveTo((targetPos.x, targetPos.y));

        // 다음 목적지로 인덱스 이동
        _currentPathIndex++;

        // moveQueue 업데이트 (화살표 시각화를 위해 맨 앞 데이터 삭제)
        if (moveQueue.Count > 0)
        {
            moveQueue.RemoveAt(0);
        }

        int nextVisualIndex = _currentPathIndex + moveQueue.Count;

        //다음 경로 moveQueue에 추가
        if (nextVisualIndex < fixedPath.Count) {
            Vector2Int nextPos = fixedPath[nextVisualIndex];
            moveQueue.Add((nextPos.x, nextPos.y));
        }

        DrawPath();
    }
}
