using UnityEngine;
using TMPro;
using UnityEngine.UI;


/// Views의 UI 출력 / 숨김 등 관리 담당
/// 턴이 바뀌었음을 알림
/// <para>게임 전체 전역 싱글톤</para>

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

    //2/20 구본환
    public GameObject skillButtonPlayer1; 
    public GameObject skillButtonPlayer2; 

    public GameObject ClearUI;

    public GameObject PauseUI;
    public GameObject FailUI;

    //2/5 구본환
    [Header("Clear UI Objectives")]
    [SerializeField] private TextMeshProUGUI[] clearObjectiveTexts;
    [SerializeField] private Image[] clearObjectiveStars;
    [SerializeField] private GameObject[] objectiveCompleteIcons;

    [Header("In-Game Objectives")]
    [SerializeField] private TextMeshProUGUI[] inGameObjectiveTexts;
    [SerializeField] private Image[] inGameObjectiveStars;

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



    //2/20 구본환
    public void OnSkillButtonPlayer1Clicked()
    {
        if(GameManager.Instance.CurrentStageState is Enums.StageState.Victory or Enums.StageState.Gameover) {
            return;
        }

        //  PlayerMove 또는 PlayerMovable states에만만 P1 이동가능
        if(GameManager.Instance.CurrentTurnState is Enums.TurnState.PlayerMove or Enums.TurnState.PlayerMovable)
        {
            if(GameManager.Instance.NextPlayer != 0) return;

            // 플레이어가 이동했는지 확인
            Piece currentPiece = GameManager.Instance.GetCurrentPlayer();
            if(currentPiece == null || !currentPiece.hasMoved)
            {
                Debug.Log("이동하고 스킬을 사용하세요");
                return;
            }

            // 스킬사용용
            if(SkillManager.Instance != null && SkillManager.Instance.UseExpandAOE())
            {
                SoundManager.Instance.PlaySFX("Click");
                Debug.Log("P1 스킬 사용");
                
                // 스킬 버튼 숨기기
                if(skillButtonPlayer1 != null)
                    skillButtonPlayer1.SetActive(false);
                
                // 공격 시퀀스 트리거(HandleEnd랑 똑같이)
                GameManager.Instance.ClearEffects();
                GameManager.Instance.StartCoroutine(GameManager.Instance.ProcessTurnSequence());
            }
            else
            {
                Debug.Log("플레이어1 스킬 쿨다운중!");
            }
        }
    }


    public void OnSkillButtonPlayer2Clicked()
    {
        if(GameManager.Instance.CurrentStageState is Enums.StageState.Victory or Enums.StageState.Gameover) {
            return;
        }

        if(GameManager.Instance.CurrentTurnState is Enums.TurnState.PlayerMove or Enums.TurnState.PlayerMovable)
        {
            if(GameManager.Instance.NextPlayer != 1) return; 

            // 플레이어가 이동했는지 확인
            Piece currentPiece = GameManager.Instance.GetCurrentPlayer();
            if(currentPiece == null || !currentPiece.hasMoved)
            {
                Debug.Log("이동하고 스킬을 사용하세요");
                return;
            }

            if(SkillManager.Instance != null && SkillManager.Instance.UseHeal())
            {
                SoundManager.Instance.PlaySFX("Click");
                Debug.Log("P2 스킬 사용");
                
                if(skillButtonPlayer2 != null)
                    skillButtonPlayer2.SetActive(false);
                
                GameManager.Instance.ClearEffects();
                GameManager.Instance.StartCoroutine(GameManager.Instance.ProcessTurnSequence());
            }
            else
            {
                Debug.Log("플레이어2 스킬 쿨다운중!");
            }
        }
    }

    //01.17 정수민: 이동버튼 복구
    public void ShowMoveButton()
    {
        moveButton.SetActive(true);
        turnEndButton.SetActive(false);
        UpdateSkillButtonVisibility();
    }


    //2/20 구본환
    //현재 플레이어에 따라 스킬 버튼 표시 여부 업데이트
    public void UpdateSkillButtonVisibility()
    {
        if (GameManager.Instance == null) return;

        int currentPlayer = GameManager.Instance.NextPlayer;
        
        // Player 1일때는 범위 증가 스킬 버튼 표시
        if (skillButtonPlayer1 != null)
        {
            bool shouldShow = currentPlayer == 0 && 
                             GameManager.Instance.CurrentTurnState is Enums.TurnState.PlayerMove or Enums.TurnState.PlayerMovable &&
                             SkillManager.Instance != null && 
                             SkillManager.Instance.IsExpandAOEAvailable();
            skillButtonPlayer1.SetActive(shouldShow);
        }

        // Player 2일때는 체력 회복 스킬 버튼 표시
        if (skillButtonPlayer2 != null)
        {
            bool shouldShow = currentPlayer == 1 && 
                             GameManager.Instance.CurrentTurnState is Enums.TurnState.PlayerMove or Enums.TurnState.PlayerMovable &&
                             SkillManager.Instance != null && 
                             SkillManager.Instance.IsHealAvailable();
            skillButtonPlayer2.SetActive(shouldShow);
        }
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
        UpdateClearObjectiveUI(objectiveDescriptions, objectiveCompletionStatus);
    }

    //2/13 구본환
    // 현재 스테이지의 목표 정보를 바로 UI에 표시할 때 사용 (플레이 중 상단 HUD용)
    public void RefreshObjectiveUI()
    {
        // ObjectiveManager가 준비되지 않았거나 싱글톤이 없으면 무시
        if (ObjectiveManager.Instance == null) return;

        var descriptions = ObjectiveManager.Instance.GetDescriptions();
        var completionStatus = ObjectiveManager.Instance.GetCompletionStatus();

        UpdateInGameObjectiveUI(descriptions, completionStatus);
    }

    // 클리어 패널에 표시되는 목표 UI 갱신
    private void UpdateClearObjectiveUI(string[] objectiveDescriptions, bool[] objectiveCompletionStatus)
    {
        UpdateObjectiveUIInternal(objectiveDescriptions, objectiveCompletionStatus, clearObjectiveTexts, clearObjectiveStars);
    }

    // 플레이 중 상단 HUD에 표시되는 목표 UI 갱신
    private void UpdateInGameObjectiveUI(string[] objectiveDescriptions, bool[] objectiveCompletionStatus)
    {
        UpdateObjectiveUIInternal(objectiveDescriptions, objectiveCompletionStatus, inGameObjectiveTexts, inGameObjectiveStars);
    }

    // 공통 목표 UI 갱신 로직
    private void UpdateObjectiveUIInternal(
        string[] objectiveDescriptions,
        bool[] objectiveCompletionStatus,
        TextMeshProUGUI[] targetTexts,
        Image[] targetStars)
    {
        if (objectiveDescriptions == null || objectiveCompletionStatus == null) return;

        int count = int.MaxValue;
        if (targetTexts != null) count = Mathf.Min(count, targetTexts.Length);
        if (objectiveDescriptions != null) count = Mathf.Min(count, objectiveDescriptions.Length);
        if (objectiveCompletionStatus != null) count = Mathf.Min(count, objectiveCompletionStatus.Length);
        count = Mathf.Clamp(count, 0, 3); // 목표 3개만

        for (int i = 0; i < count; i++)
        {
            bool isDone = objectiveCompletionStatus[i];

            // 실패시에 목표설명 회색처리
            if (targetTexts != null && i < targetTexts.Length && targetTexts[i] != null)
            {
                targetTexts[i].text = objectiveDescriptions[i];
                targetTexts[i].color = isDone ? completedTextColor : incompleteTextColor;
            }

            // 별 이미지 변경
            Image starImage = null;

            if (targetStars != null && i < targetStars.Length)
            {
                starImage = targetStars[i];
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

