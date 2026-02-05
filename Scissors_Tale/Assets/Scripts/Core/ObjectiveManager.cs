using UnityEngine;

// 스테이지 목표 추적(3개)
// 설명 + 성공 플래그그
// 실제 목표는 나중에...
public class ObjectiveManager : Singleton<ObjectiveManager>
{
    protected override bool DontDestroy => false;

    public const int ObjectiveCount = 3;

    private readonly string[] _descriptions = new string[ObjectiveCount];
    private readonly bool[] _completed = new bool[ObjectiveCount];

    protected override void Awake()
    {
        base.Awake();
    }

    public void InitializeForStage(MapData mapData)
    {
        // 성공진도 리셋셋
        for (int i = 0; i < ObjectiveCount; i++)
        {
            _completed[i] = false;
        }

        // MapData 에서 설명 받기, 없으면 기본값으로 설정
        for (int i = 0; i < ObjectiveCount; i++)
        {
            string desc = null;
            if (mapData != null && mapData.objectives != null && i < mapData.objectives.Length)
            {
                desc = mapData.objectives[i].description;
            }

            _descriptions[i] = string.IsNullOrWhiteSpace(desc) ? $"목표 {i + 1}" : desc;
        }
    }

    // 스테이지 클리어시 호출(스켈레톤): 만약 목표 1번이 기본값이면, 적절한 기본값으로 이름 바꾸고, 성공 플래그 설정
    public void NotifyStageCleared()
    {
        if (_descriptions[0] == "목표 1")
        {
            _descriptions[0] = "스테이지 완료";
        }
        _completed[0] = true;
    }

    public void SetDescription(int index, string description)
    {
        if (!IsValidIndex(index)) return;
        _descriptions[index] = string.IsNullOrWhiteSpace(description) ? $"목표 {index + 1}" : description;
    }

    public void SetCompleted(int index, bool isCompleted = true)
    {
        if (!IsValidIndex(index)) return;
        _completed[index] = isCompleted;
    }

    public bool IsCompleted(int index)
    {
        if (!IsValidIndex(index)) return false;
        return _completed[index];
    }

    public string[] GetDescriptions()
    {
        return (string[])_descriptions.Clone();
    }

    public bool[] GetCompletionStatus()
    {
        return (bool[])_completed.Clone();
    }

    private bool IsValidIndex(int index) => index >= 0 && index < ObjectiveCount;
}


