using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneChanger : BaseSceneChanger
{
    private void Start()
    {
        SoundManager.Instance.PlayBGM("Menu_BGM");
    }
    public void OnGameStartClicked() {
        SoundManager.Instance.PlaySFX("Click");
        GameSystemManager.Instance.ChangeGameState(Enums.GameState.StageSelection);
        ChangeScene(nextScenes[0]);
    }

    public void OnEndGameClicked() {
        SoundManager.Instance.PlaySFX("Click");

        #if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때만 작동  01.21 정수민
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // 실제 빌드된 게임(PC, 모바일 등)에서 실행될 때 작동
        Application.Quit();
        #endif
        
    }
    
}