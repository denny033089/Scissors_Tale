using UnityEngine;
using UnityEngine.UI;

public class PlayerUIStatus : MonoBehaviour
{

    [Header("UI Elements")]
    public Image portraitImage; // 바뀔 이미지 컴포넌트
    
    [Header("Resources")]
    public Sprite player1Sprite; // 플레이어 1용 그림
    public Sprite player2Sprite; // 플레이어 2용 그림
    
    // Update is called once per frame

    void Awake() {
        UpdatePlayerPortrait();
    }
    
    public void UpdatePlayerPortrait() {
        int currentplayer = GameManager.Instance.CurrentPlayer;

        if(currentplayer == 0) {
            portraitImage.sprite = player1Sprite;
        } else {
            portraitImage.sprite = player2Sprite;
        }
    }

}
