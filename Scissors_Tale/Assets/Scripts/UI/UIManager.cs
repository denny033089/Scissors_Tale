using UnityEngine;

/// <summary>
/// Views의 UI 출력 / 숨김 등 관리 담당
/// 턴이 바뀌었음을 알림
/// <para>게임 전체 전역 싱글톤</para>
/// </summary>
public class UIManager : Singleton<UIManager>
{
    public void OnMoveButtonClicked()
    {
        if(GameManager.Instance.CurrentTurnState is Enums.TurnState.Ready) {        
            Debug.Log("무브 버튼 클릭");
            GameManager.Instance.HandleMove();
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
}

