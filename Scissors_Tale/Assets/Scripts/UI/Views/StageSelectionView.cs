using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectionView : MonoBehaviour
{
    public void OnStageZeroButtonClicked() {
        SoundManager.Instance.PlaySFX("Click");
        GameSystemManager.Instance.ChangeGameState(Enums.GameState.Tutorial);
        
    }

    public void OnStageOneButtonClicked() {
        SoundManager.Instance.PlaySFX("Click");
        GameSystemManager.Instance.ChangeGameState(Enums.GameState.InGame);
        
    }

    public void OnBackToMainClicked() {
        SoundManager.Instance.PlaySFX("Click");
        GameSystemManager.Instance.ChangeGameState(Enums.GameState.Main);

    }
    
}
