using UnityEngine;
using UnityEngine.SceneManagement;

public class MainView : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayBGM("Menu_BGM");
    }
    public void OnGameStartClicked() {
        SoundManager.Instance.PlaySFX("Click");
        GameSystemManager.Instance.ChangeGameState(Enums.GameState.StageSelection);
    }

    public void OnEndGameClicked() {
        SoundManager.Instance.PlaySFX("Click");
        UnityEditor.EditorApplication.isPlaying=false;  
        
        GameSystemManager.Instance.ChangeGameState(Enums.GameState.Quit);
        

    }
    
}