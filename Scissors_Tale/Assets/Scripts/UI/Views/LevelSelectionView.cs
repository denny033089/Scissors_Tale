using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionView : Singleton<LevelSelectionView>
{
    public void OnStageZeroButtonClicked() {
        SceneManager.LoadScene("Synopsis");
    }

    public void OnStageOneButtonClicked() {
        SceneManager.LoadScene("Testscene");
    }
    
}
