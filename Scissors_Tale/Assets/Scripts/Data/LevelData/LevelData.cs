using UnityEngine;
using System.Collections.Generic;
//Edited by 구본환 1/18
//레벨 데이터 추가(현재는 튜토리얼만)
public class LevelData : MonoBehaviour
{
    [Header("Player Settings")]
    public Vector2Int Player1Start = new Vector2Int(1, 1);
    public Vector2Int Player2Start = new Vector2Int(3, 1);

    [Header("Monster Settings")]
    public int MonsterHP = 5;
    public List<Vector2Int> MonsterSpawnPositions; 
}