using UnityEngine;

/// <summary>
/// 열거형(Enums) 모음 클래스
/// </summary>
public static class Enums
{
    
        public enum GameState
    {
        Main, 
        WorldSelection, 
        LevelSelection, 
        Level, 
        Story
    }

    public enum TurnState
    {
        Ready,
        PlayerMove,
        PlayerTag,
        PlayerAttack,
        MonsterMove,
        End
    }
}

