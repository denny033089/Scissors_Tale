using UnityEngine;

/// <summary>
/// 열거형(Enums) 모음 클래스
/// </summary>
public static class Enums
{
    
        public enum GameState
    {
        Main, 
        StageSelection, 

        Tutorial,
        InGame, 
        Story,
        Quit
    }

    public enum TurnState
    {
        Ready,
        PlayerMovable,
        PlayerMove,
        PlayerTag,
        PlayerAttack,
        MonsterAttack,
        MonsterMove,
        End
    }

    public enum StageState
    {
        Playing,
        Pause,
        Victory,
        Gameover
    }
}

