using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AttackType 
{ 
    Directional, // 플레이어 방향으로 특정 범위 (2x3 등)
    XAxisLine,    // 가로줄 전체 공격
    YAxisLine,    // 세로줄 전체 공격
    Splash       // 원거리 특정 지점 폭발 (포격)
}


[System.Serializable]
public struct AttackInfo
{
    public AttackType type;      // 공격 형태
    public int damage;           // 데미지
    public int range;            // 사거리 (Splash의 경우 던지는 거리)
    public List<Vector2Int> areaOffsets; // 타격 범위 (중심점 기준 상대 좌표들)

    public AttackInfo(AttackType type, int damage, int range, List<Vector2Int> offsets = null)
    {
        this.type = type;
        this.damage = damage;
        this.range = range;
        this.areaOffsets = offsets ?? new List<Vector2Int>();
    }
}
