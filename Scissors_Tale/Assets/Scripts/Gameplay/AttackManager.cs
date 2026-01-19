using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//캐릭터 주변 공격 범위 계산
//중첩 범위 구역 계산(두 캐릭터의 공격범위가 겹치는 곳)
public class AttackManager : Singleton<AttackManager> 
{
    
    public Sprite Player1AttackSprite; 
    public Sprite Player2AttackSprite; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private const int FIELD_SIZE = 5;

    public void Attack()
    {
        //Edited By 구본환 1/19
        //Coroutine으로 변경
        StartCoroutine(ProcessAttackSequence());
    }

    private IEnumerator ProcessAttackSequence()
    {
        // GameManager.cs에서 위치 받아옴
        GameManager gm = GameManager.Instance;
        Vector2Int p1Pos = gm.GetPlayer1Pos();
        Vector2Int p2Pos = gm.GetPlayer2Pos();
        Vector2Int monsterPos = gm.GetMonsterPos();

        HashSet<Vector2Int> area1 = GetAttackArea(p1Pos);
        HashSet<Vector2Int> area2 = GetAttackArea(p2Pos);

        bool inArea1 = area1.Contains(monsterPos);
        bool inArea2 = area2.Contains(monsterPos);

        // 플레이어 확인

        int activePlayerIndex = gm.CurrentPlayer;

        // 스프라이트 확인
        Sprite p1Sprite = Player1AttackSprite;
        Sprite p2Sprite = Player2AttackSprite;

        
        if (inArea1 && inArea2)  // 장판 중첩 영역에 존재하면 데미지 3
        {

            Sprite firstSprite = (activePlayerIndex == 0) ? p2Sprite : p1Sprite; 
            Sprite secondSprite = (activePlayerIndex == 0) ? p1Sprite : p2Sprite; 
            Sprite bonusSprite = secondSprite; //일단 두번째 스프라이트로 설정

            // 태그받은 플레이어 히트
            ApplyDamageWithVisual(1, firstSprite);
            yield return new WaitForSeconds(0.1f); 

            // 태그하는 플레이어 히트
            ApplyDamageWithVisual(1, secondSprite);
            yield return new WaitForSeconds(0.1f);

            // 태그 보너스 히트
            if (gm.IsTagTurn)
            {
                yield return new WaitForSeconds(0.1f);
                ApplyDamageWithVisual(1, bonusSprite);
            }
        }
        // 영역 안겹칠때
        else if (inArea1)
        {
            ApplyDamageWithVisual(1, p1Sprite);
        }
        else if (inArea2)
        {
            ApplyDamageWithVisual(1, p2Sprite);
        }
    }
    private void ApplyDamageWithVisual(int damage, Sprite sprite)
    {
        GameManager gm = GameManager.Instance;

        // 데미지 부여
        

        // 몬스터에 반영
        Vector2Int monsterPos = gm.GetMonsterPos();
        Piece piece = gm.Pieces[monsterPos.x, monsterPos.y];

        if (piece is Monster monster)
        {
            monster.SpawnDamageEffect(sprite);
        }
        
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
