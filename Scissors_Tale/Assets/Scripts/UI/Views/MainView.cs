using UnityEngine;
using UnityEngine.SceneManagement;

public class MainView : Singleton<MainView>
{
    public void OnGameStartClicked() {
        SceneManager.LoadScene("StageSelection");
    }

    public void OnEndGameClicked() {
        UnityEditor.EditorApplication.isPlaying=false;  
        
        Application.Quit();

    }
    
}