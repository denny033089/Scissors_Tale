using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TwoTurnMonster : Monster
{    
    public override AttackInfo[] GetAttacks()
    {
        // 2x3 범위를 위한 상대 좌표 리스트 생성
        // (x, y) 형태: x는 좌우(-1, 0, 1), y는 정면(1, 2, 3)
        List<Vector2Int> area3x2 = new List<Vector2Int>()
        {
            new Vector2Int(-1, 1), new Vector2Int(-1, 2), // 정면 1열 (세로 2칸)
            new Vector2Int(0, 1), new Vector2Int(0, 2), // 정면 2열 (세로 2칸)
            new Vector2Int(1, 1), new Vector2Int(1, 2)  // 정면 3열 (세로 2칸)
        };

        return new AttackInfo[]
        {
            // AttackType.Directional을 사용하여 플레이어 방향으로 이 범위를 출력합니다.
            new AttackInfo(AttackType.Directional, 2, 2, area3x2)
        };
    }
}
