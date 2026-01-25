using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 사운드 효과 및 배경 음악 관리
/// <para>게임 전체 전역 싱글톤</para>
/// </summary>
public class GameSystemManager : Singleton<GameSystemManager>
{
    public  Enums.GameState CurrentGameState { get; private set; } = Enums.GameState.Main;
    public void ChangeGameState(Enums.GameState newGameState)
    {
        CurrentGameState = newGameState;

        Debug.Log($"Game State changed to: {CurrentGameState}");
        switch (CurrentGameState)
        {
            case Enums.GameState.Main:
            
            break;
            case Enums.GameState.StageSelection:
            
            break;
            case Enums.GameState.Tutorial:
            GameManager.Instance.isTutorialMode = true; //01.25 정수민

            break;

            case Enums.GameState.InGame:
            
            break;
            case Enums.GameState.Story:
            break;
            case Enums.GameState.Quit:
            
            break;
        }


        // 상태 변경에 따른 로직

        Debug.Log($"Game State changed to: {CurrentGameState}");
    }


}

