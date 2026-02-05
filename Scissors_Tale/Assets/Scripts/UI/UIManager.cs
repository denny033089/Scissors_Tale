using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Views의 UI 출력 / 숨김 등 관리 담당
/// 턴이 바뀌었음을 알림
/// <para>게임 전체 전역 싱글톤</para>
/// </summary>
public class UIManager : Singleton<UIManager>
{
    
    //01.24 정수민 씬 전환 시 삭제
    protected override bool DontDestroy => false;
    
    //01.27 정수민
    [Header("Stage Number UI")]
    public Image stageNumberImage; // 화면 상단의 'STAGE.0' 이미지가 들어갈 곳
    public Sprite[] stageNumberSprites; // 0번부터 순서대로 숫자 스프라이트들을 넣어주세요.
    
    //01.17 정수민
    public GameObject moveButton;
    public GameObject turnEndButton;

    public GameObject ClearUI;

    public GameObject PauseUI;
    public GameObject FailUI;

    //2/5 구본환
    [Header("Objective UI")]
    [SerializeField] private TextMeshProUGUI[] objectiveTexts;
    [SerializeField] private Image[] objectiveStars;

    [SerializeField] private GameObject[] objectiveCompleteIcons;

    [SerializeField] private Sprite completedStarSprite;   // 성공별
    [SerializeField] private Sprite incompleteStarSprite;  // 실패별
    // 검정 텍스트
    [SerializeField] private Color completedTextColor = new Color(0.12f, 0.10f, 0.08f, 1f);
    [SerializeField] private Color incompleteTextColor = new Color(0.45f, 0.43f, 0.40f, 1f);
    [SerializeField] private Color completedStarTint = Color.white;
    [SerializeField] private Color incompleteStarTint = Color.white;

    [Header("Turn UI")]
    [SerializeField]
    private TextMeshProUGUI turnText;
    [SerializeField]
    private TextMeshProUGUI RemainMoveText;
    
    
    public void OnMoveButtonClicked()
    {
        //01.19 정수민
        if(GameManager.Instance.CurrentStageState is Enums.StageState.Victory or Enums.StageState.Gameover) {
            return;
        }
        
        //01.27 조건 변경, movable 추가
        if(GameManager.Instance.CurrentTurnState is Enums.TurnState.Ready or Enums.TurnState.PlayerMovable) { 
            SoundManager.Instance.PlaySFX("Click");
            Debug.Log("무브 버튼 클릭");
            GameManager.Instance.HandleMove();
            
            //01.17 정수민 무브버튼 클릭 시 무브버튼 비활성화
            moveButton.SetActive(false);
            turnEndButton.SetActive(true);
        }

        
    }
    public void OnTagButtonClicked()
    {

        //01.19 정수민
        if(GameManager.Instance.CurrentStageState is Enums.StageState.Victory or Enums.StageState.Gameover) {
            return;
        }

        //01.27 조건 변경, playermovable 추가
        if(GameManager.Instance.CurrentTurnState is Enums.TurnState.PlayerMove or Enums.TurnState.PlayerMovable) {

            if(GameManager.Instance.IsTagTurn==false) { //01.27 정수민 tag 조건 추가(playerremainmove용)
                SoundManager.Instance.PlaySFX("Click");
                Debug.Log("태그 버튼 클릭!");
                // MovementManager에게 태그 로직 실행 요청
                GameManager.Instance.HandleTag();
                moveButton.SetActive(false); //01.27 정수민 tag 눌렀을 시에 move다시 못하도록 수정
                turnEndButton.SetActive(true);
            } else {
                Debug.Log("태그 이미 했음");
            }
        }
    }

    public void OnEndTurnButtonClicked()
    {
        //01.19 정수민
        if(GameManager.Instance.CurrentStageState is Enums.StageState.Victory or Enums.StageState.Gameover) {
            return;
        }
            
        
        if(GameManager.Instance.CurrentTurnState is Enums.TurnState.PlayerMove or Enums.TurnState.PlayerTag) {
            SoundManager.Instance.PlaySFX("Click");
            // GameManager에게 턴 종료 요청
            GameManager.Instance.HandleEnd();
        }
    }

    //01.17 정수민: 이동버튼 복구
    public void ShowMoveButton()
    {
        moveButton.SetActive(true);
        turnEndButton.SetActive(false);
    }

    //01.17 정수민: 남은 턴 수 보여주기
    public void ShowRemainTurn(int remainTurn, int totalTurn) {
        Debug.Log($"TURN : {remainTurn} / {totalTurn}");
        turnText.text = $"{remainTurn} / {totalTurn}";
    }

    public void ShowPlayerRemainMove(int PlayerRemainMove) {
        RemainMoveText.text = $"턴 당 최대 이동 횟수 : {PlayerRemainMove}";
    }

    public void ShowFailPanel() {
        FailUI.SetActive(true);
    }
    //2/5 구본환
    public void ShowClearPanel(string[] objectiveDescriptions, bool[] objectiveCompletionStatus) {
        ClearUI.SetActive(true);
        UpdateObjectiveUI(objectiveDescriptions, objectiveCompletionStatus);
    }

    private void UpdateObjectiveUI(string[] objectiveDescriptions, bool[] objectiveCompletionStatus)
    {
        if (objectiveDescriptions == null || objectiveCompletionStatus == null) return;

        int count = int.MaxValue;
        if (objectiveTexts != null) count = Mathf.Min(count, objectiveTexts.Length);
        if (objectiveDescriptions != null) count = Mathf.Min(count, objectiveDescriptions.Length);
        if (objectiveCompletionStatus != null) count = Mathf.Min(count, objectiveCompletionStatus.Length);
        count = Mathf.Clamp(count, 0, 3); // 목표 3개만

        for (int i = 0; i < count; i++)
        {
            bool isDone = objectiveCompletionStatus[i];

            // 실패시에 목표설명 회색처리
            if (objectiveTexts != null && i < objectiveTexts.Length && objectiveTexts[i] != null)
            {
                objectiveTexts[i].text = objectiveDescriptions[i];
                objectiveTexts[i].color = isDone ? completedTextColor : incompleteTextColor;
            }

            // 별 이미지 변경
            Image starImage = null;

            if (objectiveStars != null && i < objectiveStars.Length)
            {
                starImage = objectiveStars[i];
            }
            else if (objectiveCompleteIcons != null && i < objectiveCompleteIcons.Length && objectiveCompleteIcons[i] != null)
            {
                // 이전 코드 호환성을 위해 별 이미지 사용
                starImage = objectiveCompleteIcons[i].GetComponent<Image>();
                objectiveCompleteIcons[i].SetActive(true);
            }

            if (starImage != null)
            {
                Sprite desiredSprite = isDone ? completedStarSprite : incompleteStarSprite;
                if (desiredSprite != null)
                {
                    starImage.sprite = desiredSprite;
                }

                starImage.color = isDone ? completedStarTint : incompleteStarTint;
            }
        }
    }

    public void ShowPausePanel() {
        PauseUI.SetActive(true);
    }

    public void HidePausePanel() {
        PauseUI.SetActive(false);
    }


    //01.27 정수민
    public void UpdateStageNumberUI(int stageIndex)
    {
        if (stageIndex >= 0 && stageIndex < stageNumberSprites.Length)
        {
            if (stageNumberImage != null && stageNumberSprites[stageIndex] != null)
            {
                // 인덱스에 맞는 스프라이트로 교체
                stageNumberImage.sprite = stageNumberSprites[stageIndex];
                // 이미지 크기가 다를 경우를 대비해 원본 크기로 맞춰줌
                //stageNumberImage.SetNativeSize(); 
            }
        }
    }
}

