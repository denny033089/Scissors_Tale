using System.Collections.Generic;
using UnityEngine;

public class StageDataManager : Singleton<StageDataManager>
{
    public List<MapData> allStages; // 모든 MapData를 순서대로 넣어두세요.
    public int currentStageIndex = 0; // 현재 몇 번째 스테이지인지 저장

    public MapData GetNextStageData()
    {
        currentStageIndex++;
        if (currentStageIndex < allStages.Count)
        {
            return allStages[currentStageIndex];
        }
        return null; // 다음 스테이지가 없음 (올클리어)
    }
}
