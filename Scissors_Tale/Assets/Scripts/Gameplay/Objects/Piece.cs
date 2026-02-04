using UnityEngine;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    public (int, int) MyPos;    // 자신의 좌표
    public bool hasMoved = false;

    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo((int, int) targetPos) {

        //UpdateDirectionSprite(targetPos);        
        // MyPos를 업데이트하고, targetPos로 이동(보드세계 이동 + 실제 이동)
        // GameManager.Pieces를 업데이트
        MapManager.Instance.Pieces[MyPos.Item1,MyPos.Item2] = null; //기존의 보드세계 좌표에 null
        MyPos = targetPos;
        MapManager.Instance.Pieces[targetPos.Item1,targetPos.Item2] = this; //보드세계의 목표좌표에 현재의 piece넣기
        transform.DOMove((Utils.ToRealPos(targetPos)),0.3f).SetEase(Ease.OutQuad); //01.18 정수민: dotween으로 수정
    }




        // abstract를 지웠다면 virtual을 추가해야 자식이 override 할 수 있습니다.
    public virtual MoveInfo[] GetMoves() 
    {
    // 기본적으로는 빈 배열을 반환하거나 기본 로직을 넣습니다.
        return new MoveInfo[0]; 
    }
    //public MoveInfo[] GetMoves();
    
    //02.04 정수민
    public virtual AttackInfo[] GetAttacks()
    {
        return new AttackInfo[0];
    }

}
