
using System.Collections.Generic;
using UnityEngine;

public static class MonsterDirection
{
    private static readonly (int, int)[] _directions = new (int, int)[]
    {
        (0, 1), (0, -1), (-1, 0), (1, 0)
    };

    // 1.����
    // ���Ͱ� �����Ҷ� �ʱ� ��ΰ��
    public static List<(int, int)> InitPathRecursive((int x, int y) startPos, int steps)
    {
        if (steps <= 0) return new List<(int, int)>();

        List<(int, int)> validNeighbors = GetValidNeighbors(startPos);
        if (validNeighbors.Count == 0) return new List<(int, int)>(); //����������

        // �������� �ϳ� ����
        (int, int) nextStep = validNeighbors[Random.Range(0, validNeighbors.Count)];

        List<(int, int)> path = new List<(int, int)>();
        path.Add(nextStep);

        // �������� ��ͷ� �߰�
        path.AddRange(InitPathRecursive(nextStep, steps - 1));

        return path;
    }

    // 2. ����
    // ���ϸ��� �߰�
    public static (int, int) GetOneFutureStep((int x, int y) lastPlannedPos)
    {
        List<(int, int)> validNeighbors = GetValidNeighbors(lastPlannedPos);

        if (validNeighbors.Count == 0)
        {
            // �̷� ��ΰ� ���� �����϶� ���ڸ��� �ֱ�
            return lastPlannedPos;
        }

        return validNeighbors[Random.Range(0, validNeighbors.Count)];
    }

    // ���� ��� ����
    private static List<(int, int)> GetValidNeighbors((int x, int y) fromPos)
    {
        List<(int, int)> neighbors = new List<(int, int)>();

        foreach (var dir in _directions)
        {
            (int nextX, int nextY) = (fromPos.x + dir.Item1, fromPos.y + dir.Item2);

            // Bounds Ȯ��
            if (!Utils.IsInBoard((nextX, nextY))) continue;

            if (MapManager.Instance.Pieces[nextX, nextY] != null) continue; //01.20정수민

            neighbors.Add((nextX, nextY));
        }
        return neighbors;
    }
}

