using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Piece
{
    [Header("AI Settings")]
    public int PlanLength = 2; //�̷� ����� ��

    [Header("Visualization")]
    public GameObject ArrowPrefab; 
    private List<GameObject> _spawnedArrows = new List<GameObject>();

    // FIFO ť: [Step1, Step2]
    private List<(int, int)> moveQueue = new List<(int, int)>();

    // ���Ͱ� (GameManager����)������ ���Ŀ� ���� 
    public void InitializePath()
    {
        // ���� N���� ��� ���� (���)
        moveQueue = MonsterDirection.InitPathRecursive(MyPos, PlanLength);

        // ���� ���·� �����ɶ�, ���� ���������� ť ä���
        while (moveQueue.Count < PlanLength)
        {
            (int x, int y) lastPos = (moveQueue.Count > 0) ? moveQueue[moveQueue.Count - 1] : MyPos;
            moveQueue.Add(lastPos);
        }

        Debug.Log($"Monster Initialized. Planned {moveQueue.Count} steps.");
        DrawPath();
    }

    // MonsterMove �ܰ迡�� ȣ��
    public void PerformTurn()
    {
        if (moveQueue.Count == 0) return;

        // ���� ��� Ȯ��
        (int targetX, int targetY) = moveQueue[0];

        // ����������:
        Piece obstacle = GameManager.Instance.Pieces[targetX, targetY];

        if (obstacle != null)
        {
            Debug.Log($"Monster blocked at ({targetX},{targetY}) by {obstacle.name}. Recalculating path...");

            // ���� ���� ��� ����
            moveQueue.Clear();

            // ���� pos���� ���ο� ��� ����
            moveQueue = MonsterDirection.InitPathRecursive(MyPos, PlanLength);

            // ���̻� �������϶�
            if (moveQueue.Count == 0)
            {
                Debug.Log("Monster is surrounded and cannot move.");
                // ���� ������ ��ȯ
                for (int i = 0; i < PlanLength; i++) moveQueue.Add(MyPos);
                DrawPath();
                return; // ��ŵ�ϱ�
            }
            // �� ��η� Ÿ�� ����
            (targetX, targetY) = moveQueue[0];
        }

        // ť���� ��� ����
        moveQueue.RemoveAt(0);

        // �̵�
        //GameManager.Instance.MovePlayer(this, (targetX, targetY));
        this.MoveTo((targetX,targetY));


        // ť�� �� ��� �߰�
        (int lastPlannedX, int lastPlannedY) = (moveQueue.Count > 0) ? moveQueue[moveQueue.Count - 1] : (targetX, targetY);
        (int newX, int newY) = MonsterDirection.GetOneFutureStep((lastPlannedX, lastPlannedY));
        moveQueue.Add((newX, newY));

        DrawPath();
    }

    private void DrawPath()
    {
        // ���� ȭ��ǥ ����
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

            // ���� ����
            Vector3 dir = endPos - startPos;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrowObj.transform.rotation = Quaternion.Euler(0, 0, angle);

            // �����Ͽ� ���� ȭ��ǥ ũ�� ����
            // ���̳��̰� ��õ������ �ʿ� ������ �����ϴ�
            float dist = Vector3.Distance(startPos, endPos);
            arrowObj.transform.localScale = new Vector3(dist, 1, 1);

            _spawnedArrows.Add(arrowObj);

            (currentX, currentY) = (nextX, nextY);
        }
    }
}