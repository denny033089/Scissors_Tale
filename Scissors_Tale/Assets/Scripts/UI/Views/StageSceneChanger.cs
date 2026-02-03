using System;
using UnityEngine;

public class StageSceneChanger : BaseSceneChanger
{

    //0: 레벨 선택 화면
    //1: 다음 레벨

    //레벨 나가기
    public void OnClickedQuit()
    {
        //정지 풀기가 ChangeScene 안에 있음
        ChangeScene(nextScenes[0], 0.5f);
        GameSystemManager.Instance.ChangeGameState(Enums.GameState.StageSelection);
    }


    // 인게임 결과창의 '다음 스테이지' 버튼에서 사용할 때
    public void OnNextStageClick()
    {
        MapData nextData = StageDataManager.Instance.GetNextStageData();

        if (nextData != null)
        {
            GameManager.SelectedMapData = nextData;

            //GameManager.Instance.StageRestart();
            ChangeScene(nextScenes[1], 0.5f); //01.25 정수민

        }
        else
        {
            Debug.Log("모든 스테이지 클리어! 메인으로 이동합니다.");
            ChangeScene(nextScenes[0], 0.5f);
            GameSystemManager.Instance.ChangeGameState(Enums.GameState.StageSelection);
        }
    }

    //다음 레벨로 //안쓸 것 같음
    // public void OnClickedNext()
    // {
    //     ChangeScene(nextScenes[1],0.5f);
    // }
}

