using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    //참고용
    public const float TileSize = 1.33f;  // 타일의 실제 크기
    // 체스판의 크기
    public const int FieldWidth = 5;
    public const int FieldHeight = 5;

    // (int, int)의 좌표를 실제 좌표(Vector3)로 변환
    public static Vector3 ToRealPos((int, int) targetPos)
    {
        return (TileSize * (new Vector3(
            targetPos.Item1 - (FieldWidth - 1) / 2f, 
            targetPos.Item2 - (FieldHeight - 1) / 2f,
            0f
        ))+new Vector3(0,0.5f,0));
    }

    // 좌표를 받아 Board 안에 있는지를 리턴
    public static bool IsInBoard((int, int) targetPos)
    {
        (int x, int y) = targetPos;
        return x >= 0 && x < FieldWidth && y >= 0 && y < FieldHeight;
    }

    public static (int x, int y) ToTuple(this Vector2Int v)
    {
        return (v.x, v.y);
    }
}