using UnityEngine;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    public (int, int) MyPos;    // 자신의 좌표
    public bool hasMoved = false;

    //01.18 정수민
    [Header("Directional Sprites")]
    public Sprite spriteUp;
    public Sprite spriteDown;
    public Sprite spriteLeft;
    public Sprite spriteRight;

    private SpriteRenderer spriterenderer;

    void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo((int, int) targetPos) {

        //UpdateDirectionSprite(targetPos);        
        // MyPos를 업데이트하고, targetPos로 이동(보드세계 이동 + 실제 이동)
        // GameManager.Pieces를 업데이트
        GameManager.Instance.Pieces[MyPos.Item1,MyPos.Item2] = null; //기존의 보드세계 좌표에 null
        MyPos = targetPos;
        GameManager.Instance.Pieces[targetPos.Item1,targetPos.Item2] = this; //보드세계의 목표좌표에 현재의 piece넣기
        transform.DOMove((Utils.ToRealPos(targetPos)),0.3f).SetEase(Ease.OutQuad); //01.18 정수민: dotween으로 수정
    }


    //01.18 정수민
    public void UpdateDirectionSprite((int x, int y) targetPos) {

        int diffX = targetPos.Item1 - MyPos.Item1; //방향 찾기
        int diffY = targetPos.Item2 - MyPos.Item2;

        if (diffY > 0) {      // 위로 이동
            spriterenderer.sprite = spriteUp;
        } else if (diffY < 0) {     // 아래로 이동
            spriterenderer.sprite = spriteDown;
        } else if (diffX < 0) {      // 왼쪽으로 이동
            spriterenderer.sprite = spriteLeft;
        } else if (diffX > 0) {      // 오른쪽으로 이동
            spriterenderer.sprite = spriteRight;
        }
    }



        // abstract를 지웠다면 virtual을 추가해야 자식이 override 할 수 있습니다.
    public virtual MoveInfo[] GetMoves() 
    {
    // 기본적으로는 빈 배열을 반환하거나 기본 로직을 넣습니다.
        return new MoveInfo[0]; 
    }
    //public MoveInfo[] GetMoves();
}
