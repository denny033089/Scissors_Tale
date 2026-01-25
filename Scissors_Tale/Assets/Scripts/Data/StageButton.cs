using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageButton : MonoBehaviour
{

    public List<MapData> allStageDatas;
    
    public void OnStageClick(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= allStageDatas.Count) return;

        // 선택한 번호의 데이터를 정적 변수에 저장
        GameManager.SelectedMapData = allStageDatas[stageIndex];
    }
}
