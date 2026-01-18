//using System.Drawing;
using UnityEngine;

public class Tile : MonoBehaviour
{
    
    
    //참고용입니다
    public (int, int) MyPos;    // Tile의 좌표  tuple이라는 구조체 MyPos 는 (x,y)의 값을 가짐
    Color tileColor = new Color(255 / 255f, 193 / 255f, 204 / 255f);    // 색깔
    SpriteRenderer MySpriteRenderer;

    private void Awake()
    {
        MySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 타일을 처음에 배치하는 함수
    public void Set((int x, int y) targetPos)
    {
        // MyPos를 targetPos로 지정함
        // 위치를 targetPos 이동시키고, 배치에 따라 색깔을 지정
        //gamemanager의 위치는 게임세상 속 위치, 이 스크립트의 위치는 구조상 타일의 위치
        //타일의 몇번째 칸이 흰색인지 검은색인지 설정
        // --- TODO ---
        MyPos = targetPos;
        if((targetPos.x + targetPos.y)%2 == 0) {
            MySpriteRenderer.color = tileColor;
        } else {
            MySpriteRenderer.color = new Color(255 / 255f, 255 / 255f, 255 / 255f);
        }
        
        // ------
    }

    // 1/18 서진현
    // 타일 색깔 지정(UpdateAttackAreaTiles 단계에서 호출)
    public void SetColor(Color color)
    {
        MySpriteRenderer.color = color;
    }

    public void ResetColor()
    {
        MySpriteRenderer.color = Color.white;
    }
}
