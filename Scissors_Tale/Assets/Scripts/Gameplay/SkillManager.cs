using UnityEngine;


public class SkillManager : Singleton<SkillManager>
{
    public const int SkillCooldown = 1000;

    //다음 범위 증가 플레이어 설정(아무도 아닐때는 -1)
    private int _expandAOE = -1;

    private int _lastTurnUsedExpandArea = -SkillCooldown - 1;
    private int _lastTurnUsedHeal = -SkillCooldown - 1;

    public int HealAmount = 5;


    //범위 증가 스킬 사용

    //사용시 true, 쿨다운때는 false
    public bool UseExpandAOE()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return false;

        int currentTurn = gm.currentTurn;
        if (currentTurn - _lastTurnUsedExpandArea < SkillCooldown)
            return false;

        _lastTurnUsedExpandArea = currentTurn;
        _expandAOE = gm.NextPlayer;
        return true;
    }

    //사용시 true, 쿨다운때는 false
    public bool UseHeal()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null) return false;

        int currentTurn = gm.currentTurn;
        if (currentTurn - _lastTurnUsedHeal < SkillCooldown)
            return false;

        int otherIndex = 1 - gm.NextPlayer;
        Piece otherPiece = otherIndex == 0 ? gm.p1Instance : gm.p2Instance;
        if (otherPiece == null || !(otherPiece is Player otherPlayer))
            return false;

        _lastTurnUsedHeal = currentTurn;
        otherPlayer.Heal(HealAmount);
        return true;
    }

    //플레이어의 범위 반환(1 = 3x3, 2 = 5x5)
    //AttackManager에서 공격 영역 생성에 사용
    public int GetAOE(int playerIndex)
    {
        return _expandAOE == playerIndex ? 2 : 1;
    }

    //공격 후 범위 증가 초기화
    public void ClearAOEAfterAttack()
    {
        _expandAOE = -1;
    }

    public bool IsExpandAOEAvailable()
    {
        GameManager gm = GameManager.Instance;
        return gm != null && gm.currentTurn - _lastTurnUsedExpandArea >= SkillCooldown;
    }

    public bool IsHealAvailable()
    {
        GameManager gm = GameManager.Instance;
        return gm != null && gm.currentTurn - _lastTurnUsedHeal >= SkillCooldown;
    }
}
