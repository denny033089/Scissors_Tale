using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionView : Singleton<LevelSelectionView>
{
    public void OnStageZeroButtonClicked() {
        SoundManager.Instance.PlaySFX("Click");
        SceneManager.LoadScene("Synopsis");
    }

    public void OnStageOneButtonClicked() {
        SoundManager.Instance.PlaySFX("Click");
        SceneManager.LoadScene("Testscene");
    }
    
}
