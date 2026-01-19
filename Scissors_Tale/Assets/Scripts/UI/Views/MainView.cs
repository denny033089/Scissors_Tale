using UnityEngine;
using UnityEngine.SceneManagement;

public class MainView : Singleton<MainView>
{
    private void Start()
    {
        SoundManager.Instance.PlayBGM("Menu_BGM");
    }
    public void OnGameStartClicked() {
        SoundManager.Instance.PlaySFX("Click");
        SceneManager.LoadScene("StageSelection");
    }

    public void OnEndGameClicked() {
        SoundManager.Instance.PlaySFX("Click");
        UnityEditor.EditorApplication.isPlaying=false;  
        
        Application.Quit();

    }
    
}