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

    //01.20 정수민: FIELD_SIZE 삭제

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


        HashSet<Vector2Int> allMonsterPos = gm.GetAllMonsterPositions();
        

        HashSet<Vector2Int> area1 = GetAttackArea(p1Pos);
        HashSet<Vector2Int> area2 = GetAttackArea(p2Pos);

        foreach (Vector2Int mPos in allMonsterPos)
        {
            bool inArea1 = area1.Contains(mPos);
            bool inArea2 = area2.Contains(mPos);

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
                SoundManager.Instance.PlaySFX("Attack");
                ApplyDamageWithVisual(1, firstSprite,mPos);
                yield return new WaitForSeconds(0.1f);

                // 태그하는 플레이어 히트
                SoundManager.Instance.PlaySFX("Attack");
                ApplyDamageWithVisual(1, secondSprite,mPos);
                yield return new WaitForSeconds(0.1f);

                // 태그 보너스 히트
                if (gm.IsTagTurn)
                {   

                    yield return new WaitForSeconds(0.1f);
                    SoundManager.Instance.PlaySFX("Attack");
                    ApplyDamageWithVisual(1, bonusSprite,mPos);
                }
            }
            // 영역 안겹칠때
            else if (inArea1)
            {
                SoundManager.Instance.PlaySFX("Attack");
                ApplyDamageWithVisual(1, p1Sprite,mPos);
            }
            else if (inArea2)
            {
                SoundManager.Instance.PlaySFX("Attack");
                ApplyDamageWithVisual(1, p2Sprite,mPos);
            }
            
        }



        // 플레이어 확인

        



    }
    //01.20 정수민: mPos 인자 추가
    private void ApplyDamageWithVisual(int damage, Sprite sprite,Vector2Int mPos)
    {
        GameManager gm = GameManager.Instance;

        // 데미지 부여
        

        // 몬스터에 반영
        Piece piece = MapManager.Instance.Pieces[mPos.x, mPos.y];  //01.20 정수민 수정

        if (piece is Monster monster)
        {
            monster.SpawnDamageEffect(sprite);
        }
        
        gm.ApplyMonsterDamage(damage,mPos);
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

                if (x >= 0 && x < Utils.FieldWidth && y >= 0 && y < Utils.FieldHeight) //01.20 정수민 Utils로 수정
                {
                    result.Add(new Vector2Int(x, y));
                }
            }
        }

        return result;
    }
}
