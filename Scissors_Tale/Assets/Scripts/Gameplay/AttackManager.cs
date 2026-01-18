using UnityEngine;
using System.Collections.Generic;

//캐릭터 주변 공격 범위 계산
//중첩 범위 구역 계산(두 캐릭터의 공격범위가 겹치는 곳)
public class AttackManager : Singleton<AttackManager> 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private const int FIELD_SIZE = 5;

    public void Attack() {
       GameManager gm = GameManager.Instance;

       Vector2Int p1Pos = gm.GetPlayer1Pos();   // GameManager.cs에서 위치 받아옴
       Vector2Int p2Pos = gm.GetPlayer2Pos();
       Vector2Int monsterPos = gm.GetMonsterPos(); 

       HashSet<Vector2Int> area1 = GetAttackArea(p1Pos);
       HashSet<Vector2Int> area2 = GetAttackArea(p2Pos);

       bool inArea1 = area1.Contains(monsterPos);
       bool inArea2 = area2.Contains(monsterPos);

       int damage = 0;

       if (inArea1 && inArea2)  // 장판 중첩 영역에 존재하면 데미지 3
            damage = 3;
       else if (inArea1 || inArea2) // 한쪽 장판에만 존재하면 데미지 1
            damage = 1;

       gm.ApplyMonsterDamage(damage);
    }

    private HashSet<Vector2Int> GetAttackArea(Vector2Int center)    // 장판 안에 존재하는지
    {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int x = center.x + dx;
                int y = center.y + dy;

                if (x >= 0 && x < FIELD_SIZE && y >= 0 && y < FIELD_SIZE)
                {
                    result.Add(new Vector2Int(x, y));
                }
            }
        }

        return result;
    }
}
