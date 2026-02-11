using UnityEngine;

// 스테이지 목표 추적(3개)
// 슬롯 0: 항상 "스테이지 클리어" (클리어 시 무조건 완료)
// 슬롯 1~2: 맵별로 선택 (턴/태그/체력% 등)
public class ObjectiveManager : Singleton<ObjectiveManager>
{
    protected override bool DontDestroy => false;

    public const int ObjectiveCount = 3;
    /// 슬롯 0은 항상 "스테이지 클리어"; 슬롯 1과 2는 맵별로 선택 
    public const int AlwaysClearObjectiveIndex = 0;

    private readonly string[] _descriptions = new string[ObjectiveCount];
    private readonly bool[] _completed = new bool[ObjectiveCount];
    private readonly Enums.ObjectiveType[] _types = new Enums.ObjectiveType[ObjectiveCount];
    private readonly int[] _maxTurns = new int[ObjectiveCount];
    private readonly int[] _maxTags = new int[ObjectiveCount];
    private readonly int[] _minHealthPercent = new int[ObjectiveCount];

    protected override void Awake()
    {
        base.Awake();
    }

    public void InitializeForStage(MapData mapData)
    {

        for (int i = 0; i < ObjectiveCount; i++)
        {
            _completed[i] = false;
            _types[i] = Enums.ObjectiveType.None;
            _maxTurns[i] = 0;
            _maxTags[i] = 0;
            _minHealthPercent[i] = 0;
        }

        // 슬롯 0: 항상 스테이지 클리어
        _descriptions[AlwaysClearObjectiveIndex] = "스테이지 클리어";
        _types[AlwaysClearObjectiveIndex] = Enums.ObjectiveType.None;

        // 슬롯 1과 2: 맵별로 선택 (인덱스 1과 2)
        if (mapData != null && mapData.objectives != null)
        {
            for (int i = 1; i < ObjectiveCount; i++)
            {
                int dataIndex = i;
                if (dataIndex >= mapData.objectives.Length)
                {
                    _descriptions[i] = $"목표 {i + 1}";
                    continue;
                }

                StageObjective obj = mapData.objectives[dataIndex];
                _types[i] = obj.type;
                _maxTurns[i] = obj.maxTurns;
                _maxTags[i] = obj.maxTags;
                _minHealthPercent[i] = Mathf.Clamp(obj.minHealthPercent, 0, 100);

                if (!string.IsNullOrWhiteSpace(obj.description))
                    _descriptions[i] = obj.description;
                else
                    _descriptions[i] = BuildDescription(_types[i], _maxTurns[i], _maxTags[i], _minHealthPercent[i]);
            }
        }
        else
        {
            for (int i = 1; i < ObjectiveCount; i++)
                _descriptions[i] = $"목표 {i + 1}";
        }
    }

    private static string BuildDescription(Enums.ObjectiveType type, int maxTurns, int maxTags, int minHealthPercent)
    {
        switch (type)
        {
            case Enums.ObjectiveType.FinishWithinTurns:
                return $"남은 턴 수 {maxTurns} 이상";
            case Enums.ObjectiveType.FinishWithinTags:
                return $"태그 사용 {maxTags}회 이하";
            case Enums.ObjectiveType.FinishWithHealthPercent:
                return $"체력 {minHealthPercent}% 이상 유지";
            default:
                return "스테이지 완료";
        }
    }

    /// 스테이지 클리어 시 호출. 슬롯 0은 항상 완료, 나머지는 수치로 평가.
    public void NotifyStageCleared()
    {
        _completed[AlwaysClearObjectiveIndex] = true;

        GameManager gm = GameManager.Instance;
        if (gm == null) return;

        int currentTurn = gm.currentTurn;
        int tagCount = gm.TagCountThisStage;
        float healthPercentP1 = GetHealthPercent(gm.Player1CurrentHP, gm.Player1MaxHP);
        float healthPercentP2 = GetHealthPercent(gm.Player2CurrentHP, gm.Player2MaxHP);

        for (int i = 1; i < ObjectiveCount; i++)
        {
            switch (_types[i])
            {
                case Enums.ObjectiveType.FinishWithinTurns:
                    _completed[i] = currentTurn <= _maxTurns[i];
                    break;
                case Enums.ObjectiveType.FinishWithinTags:
                    _completed[i] = tagCount <= _maxTags[i];
                    break;
                case Enums.ObjectiveType.FinishWithHealthPercent:
                    _completed[i] = healthPercentP1 >= _minHealthPercent[i] && healthPercentP2 >= _minHealthPercent[i];
                    break;
                default:
                    _completed[i] = true;
                    break;
            }
        }
    }

    private static float GetHealthPercent(int currentHP, int maxHP)
    {
        if (maxHP <= 0) return 100f;
        return Mathf.Clamp01((float)currentHP / maxHP) * 100f;
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
