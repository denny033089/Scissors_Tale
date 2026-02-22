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
        //Vector2Int p1Pos = gm.GetPlayer1Pos();
        //Vector2Int p2Pos = gm.GetPlayer2Pos();

        //02.23 정수민
        Player1 p1Script = GameObject.FindAnyObjectByType<Player1>();
        Player2 p2Script = GameObject.FindAnyObjectByType<Player2>();

        bool isP1Alive = p1Script != null; 
        bool isP2Alive = p2Script != null;

        Vector2Int p1Pos = isP1Alive ? gm.GetPlayer1Pos() : new Vector2Int(-999, -999);
        Vector2Int p2Pos = isP2Alive ? gm.GetPlayer2Pos() : new Vector2Int(-999, -999);



        HashSet<Vector2Int> allMonsterPos = gm.GetAllMonsterPositions();
        
        // 02.23 정수민 살아있는 플레이어만 범위를 계산합니다.
        HashSet<Vector2Int> area1 = isP1Alive ? GetAttackArea(p1Pos, SkillManager.Instance.GetAOE(0)) : new HashSet<Vector2Int>();
        HashSet<Vector2Int> area2 = isP2Alive ? GetAttackArea(p2Pos, SkillManager.Instance.GetAOE(1)) : new HashSet<Vector2Int>();
        //2/20 구본환
        int radius1 = SkillManager.Instance != null ? SkillManager.Instance.GetAOE(0) : 1;
        int radius2 = SkillManager.Instance != null ? SkillManager.Instance.GetAOE(1) : 1;
        //HashSet<Vector2Int> area1 = GetAttackArea(p1Pos, radius1);
        //HashSet<Vector2Int> area2 = GetAttackArea(p2Pos, radius2);


        // 3. 플레이어 공격 애니메이션 실행
        // 적이 있을 때만 PerformAnimation을 호출하여 모션을 취합니다.
        if (p1Script != null) p1Script.PerformAnimation();
        if (p2Script != null) p2Script.PerformAnimation();
        yield return new WaitForSeconds(0.4f);
        CameraController.Instance.ShakeCamera();



        foreach (Vector2Int mPos in allMonsterPos)
        {
            //02.23 정수민
            bool inArea1 = isP1Alive && area1.Contains(mPos);
            bool inArea2 = isP2Alive && area2.Contains(mPos);

            int activePlayerIndex = gm.CurrentPlayer;

            // 스프라이트 확인
            Sprite p1Sprite = Player1AttackSprite;
            Sprite p2Sprite = Player2AttackSprite;

            //2/22 구본환
            int damageP1 = SkillManager.Instance != null ? SkillManager.Instance.GetDamageForPlayer(0) : 1;
            int damageP2 = SkillManager.Instance != null ? SkillManager.Instance.GetDamageForPlayer(1) : 1;

            if (inArea1 && inArea2)  // 장판 중첩 영역에 존재하면 데미지 3
            {

                Sprite firstSprite = (activePlayerIndex == 0) ? p2Sprite : p1Sprite; 
                Sprite secondSprite = (activePlayerIndex == 0) ? p1Sprite : p2Sprite; 
                Sprite bonusSprite = secondSprite; //일단 두번째 스프라이트로 설정
                //2/22 구본환
                int firstDamage = (activePlayerIndex == 0) ? damageP2 : damageP1;
                int secondDamage = (activePlayerIndex == 0) ? damageP1 : damageP2;

                // 태그받은 플레이어 히트
                SoundManager.Instance.PlaySFX("Attack");
                ApplyDamageWithVisual(firstDamage, firstSprite,mPos);
                yield return new WaitForSeconds(0.1f);

                // 태그하는 플레이어 히트
                SoundManager.Instance.PlaySFX("Attack");
                ApplyDamageWithVisual(secondDamage, secondSprite,mPos);
                yield return new WaitForSeconds(0.1f);

                // 태그 보너스 히트
                if (gm.IsTagTurn)
                {   

                    yield return new WaitForSeconds(0.1f);
                    SoundManager.Instance.PlaySFX("Attack");
                    ApplyDamageWithVisual(secondDamage, bonusSprite,mPos);
                }
            }
            // 영역 안겹칠때
            else if (inArea1)
            {
                SoundManager.Instance.PlaySFX("Attack");
                ApplyDamageWithVisual(damageP1, p1Sprite,mPos);
            }
            else if (inArea2)
            {
                SoundManager.Instance.PlaySFX("Attack");
                ApplyDamageWithVisual(damageP2, p2Sprite,mPos);
            }
            
        }

        if (SkillManager.Instance != null)
            SkillManager.Instance.ClearAOEAfterAttack();

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
            monster.SpawnDamageEffect(sprite, damage);
        }
        
        gm.ApplyMonsterDamage(damage,mPos);
    }

    //2/20 구본환
    //범위 구하기
    private HashSet<Vector2Int> GetAttackArea(Vector2Int center, int radius = 1)
    {
        HashSet<Vector2Int> result = new HashSet<Vector2Int>();

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
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
