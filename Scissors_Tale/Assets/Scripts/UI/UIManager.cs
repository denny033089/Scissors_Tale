using UnityEngine;
using TMPro;

/// <summary>
/// Views의 UI 출력 / 숨김 등 관리 담당
/// 턴이 바뀌었음을 알림
/// <para>게임 전체 전역 싱글톤</para>
/// </summary>
public class UIManager : Singleton<UIManager>
{
    //01.17 정수민
    public GameObject moveButton;
    public GameObject turnEndButton;
    
    [SerializeField]
    private TextMeshProUGUI turnText;
    
    
    public void OnMoveButtonClicked()
    {
        if(GameManager.Instance.CurrentTurnState is Enums.TurnState.Ready) {        
            Debug.Log("무브 버튼 클릭");
            GameManager.Instance.HandleMove();
            
            //01.17 정수민 무브버튼 클릭 시 무브버튼 비활성화
            moveButton.SetActive(false);
            turnEndButton.SetActive(true);
        }

        
    }
    public void OnTagButtonClicked()
    {

        if(GameManager.Instance.CurrentTurnState is Enums.TurnState.PlayerMove) {

            Debug.Log("태그 버튼 클릭!");
            // MovementManager에게 태그 로직 실행 요청
            GameManager.Instance.HandleTag();
        }
    }

    public void OnEndTurnButtonClicked()
    {
        if(GameManager.Instance.CurrentTurnState is Enums.TurnState.PlayerMove or Enums.TurnState.PlayerTag) {
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
        turnText.text = $"TURN : {remainTurn} / {totalTurn}";
    }

    public void ShowRetryPanel() {

    }
}

