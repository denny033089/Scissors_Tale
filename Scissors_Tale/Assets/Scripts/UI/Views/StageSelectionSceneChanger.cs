using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectionSceneChanger : BaseSceneChanger
{
    
    //01.25 정수민 수정
    public void OnBackToMainClicked() {
        SoundManager.Instance.PlaySFX("Click");
        GameSystemManager.Instance.ChangeGameState(Enums.GameState.Main);
        ChangeScene(nextScenes[0]);

    }
    
    public void OnStageClicked(int Stagenum) {
        SoundManager.Instance.PlaySFX("Click");

        if(Stagenum == 1) {
            GameSystemManager.Instance.ChangeGameState(Enums.GameState.Tutorial);
            ChangeScene(nextScenes[Stagenum]);

        } else {
            GameSystemManager.Instance.ChangeGameState(Enums.GameState.InGame);
            ChangeScene(nextScenes[Stagenum]);
        }

    }
    
}
