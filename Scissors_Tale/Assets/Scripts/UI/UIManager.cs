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

    public void ShowClearPanel() {
        ClearUI.SetActive(true);
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

