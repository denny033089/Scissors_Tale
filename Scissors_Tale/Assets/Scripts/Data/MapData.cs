using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct MonsterSpawnInfo
{
    public string monsterName; // Dictionary의 키로 사용할 이름(몬스터이므로 index는 3이상)
    public Vector2Int spawnPos;
}

[CreateAssetMenu(fileName = "NewMapData", menuName = "Stage/MapData")]
public class MapData : ScriptableObject
{
    public int stageId;
    public int width;
    public int height;
    public string stageName;
    public int totalTurnLimit; // 스테이지별 제한 턴

    public Vector2Int startpos1;
    public Vector2Int startpos2;
    public List<MonsterSpawnInfo> monsterSpawns; // 이 맵에 나올 몬스터 목록
    public GameObject tilePrefab; // 맵마다 타일 모양이 다를 경우

}