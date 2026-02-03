using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectionSceneChanger : BaseSceneChanger
{
    
    //01.25 정수민 수정
    public void OnBackToMainClicked() {
        SoundManager.Instance.PlaySFX("Click");
        GameSystemManager.Instance.ChangeGameState(Enums.GameState.Main);
        ChangeScene(nextScenes[3]); //메인메뉴로

    }
    
    public void OnStageClicked(int Stageindex) {
        SoundManager.Instance.PlaySFX("Click");

        if(Stageindex == 1) {
            GameSystemManager.Instance.ChangeGameState(Enums.GameState.Tutorial);
            ChangeScene(nextScenes[Stageindex]);

        } else {
            GameSystemManager.Instance.ChangeGameState(Enums.GameState.InGame);
            ChangeScene(nextScenes[Stageindex]);
        }

    }

    // 1. 스테이지 선택 씬에서 사용할 때: 버튼마다 인덱스 번호를 지정 (0, 1, 2...)
    public void OnStageNumClick(int Stagenum)
    {
        StageDataManager.Instance.currentStageIndex = Stagenum;

        // 선택한 번호의 데이터를 정적 변수에 저장
        GameManager.SelectedMapData = StageDataManager.Instance.allStages[Stagenum];
    }
    
}
