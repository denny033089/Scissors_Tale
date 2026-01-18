
using System.Collections.Generic;
using UnityEngine;

public static class MonsterDirection
{
    private static readonly (int, int)[] _directions = new (int, int)[]
    {
        (0, 1), (0, -1), (-1, 0), (1, 0)
    };

    // 1.시작
    // 몬스터가 스폰할때 초기 경로계산
    public static List<(int, int)> InitPathRecursive((int x, int y) startPos, int steps)
    {
        if (steps <= 0) return new List<(int, int)>();

        List<(int, int)> validNeighbors = GetValidNeighbors(startPos);
        if (validNeighbors.Count == 0) return new List<(int, int)>(); //막혀있을때

        // 랜덤으로 하나 선택
        (int, int) nextStep = validNeighbors[Random.Range(0, validNeighbors.Count)];

        List<(int, int)> path = new List<(int, int)>();
        path.Add(nextStep);

        // 나머지는 재귀로 추가
        path.AddRange(InitPathRecursive(nextStep, steps - 1));

        return path;
    }

    // 2. 보충
    // 매턴마다 추가
    public static (int, int) GetOneFutureStep((int x, int y) lastPlannedPos)
    {
        List<(int, int)> validNeighbors = GetValidNeighbors(lastPlannedPos);

        if (validNeighbors.Count == 0)
        {
            // 미래 경로가 데드 엔드일때 제자리에 있기
            return lastPlannedPos;
        }

        return validNeighbors[Random.Range(0, validNeighbors.Count)];
    }

    // 다음 경로 생성
    private static List<(int, int)> GetValidNeighbors((int x, int y) fromPos)
    {
        List<(int, int)> neighbors = new List<(int, int)>();

        foreach (var dir in _directions)
        {
            (int nextX, int nextY) = (fromPos.x + dir.Item1, fromPos.y + dir.Item2);

            // Bounds 확인
            if (!Utils.IsInBoard((nextX, nextY))) continue;

            if (GameManager.Instance.Pieces[nextX, nextY] != null) continue;

            neighbors.Add((nextX, nextY));
        }
        return neighbors;
    }
}

