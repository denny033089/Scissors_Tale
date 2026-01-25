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
    }

    //다음 레벨로 //안쓸 것 같음
    public void OnClickedNext()
    {
        ChangeScene(nextScenes[1],0.5f);
    }
}

