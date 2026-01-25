using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageButton : MonoBehaviour
{
    // 1. 스테이지 선택 씬에서 사용할 때: 버튼마다 인덱스 번호를 지정 (0, 1, 2...)
    public void OnStageClick(int index)
    {
        StageDataManager.Instance.currentStageIndex = index;

        // 선택한 번호의 데이터를 정적 변수에 저장
        GameManager.SelectedMapData = StageDataManager.Instance.allStages[index];
    }

    // 2. 인게임 결과창의 '다음 스테이지' 버튼에서 사용할 때
    public void OnNextStageClick()
    {
        MapData nextData = StageDataManager.Instance.GetNextStageData();

        if (nextData != null)
        {
            GameManager.SelectedMapData = nextData;
            GameManager.Instance.StageRestart();
        }
        else
        {
            Debug.Log("모든 스테이지 클리어! 메인으로 이동합니다.");
            UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelectionScene");
        }
    }

    // 공통 로드 로직
    // private void LoadBattleScene(MapData data)
    // {
    //     // GameManager의 정적 변수에 데이터 전달 (base.Awake 안 쓰는 방식)
    //     GameManager.SelectedMapData = data;
        
    //     // 씬 로드
    //     UnityEngine.SceneManagement.SceneManager.LoadScene("BattleScene");
    // }
}


// public class StageButton : MonoBehaviour
// {

//     public List<MapData> allStageDatas;
    
//     public void OnStageClick(int stageIndex)
//     {
//         if (stageIndex < 0 || stageIndex >= allStageDatas.Count) return;

//         // 선택한 번호의 데이터를 정적 변수에 저장
//         GameManager.SelectedMapData = allStageDatas[stageIndex];
//     }
// }
