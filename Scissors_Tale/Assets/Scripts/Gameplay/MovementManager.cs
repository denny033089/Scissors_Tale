using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//캐릭터의 이동과 태그 시스템 관리
public class MovementManager : Singleton<MovementManager>
{
    

    private GameObject effectPrefab;
    private Transform effectParent;
    private List<GameObject> currentEffects = new List<GameObject>();   // 현재 effect들을 저장할 리스트
    //리스트에 Effect라는 GameObject들을 담을 거임. Effect는 기물들의 예상 이동경로를 보여주는 역할. 





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void Initialize(GameObject effectPrefab, Transform effectParent)
    {
        this.effectPrefab = effectPrefab;
        this.effectParent = effectParent;
    }

    private bool TryMove(Piece piece, (int, int) targetPos, MoveInfo moveInfo)
    {
        // moveInfo의 distance만큼 direction을 이동시키며 이동이 가능한지를 체크
        // 보드에 있는지 체크, 다른 piece에 의해 막히는지는 아직 체크하지 않음, 따로 추가할 것

        //moveInfo 하나씩 받아오는중.. ex) moveInfo(1,1,8)
        // --- TODO ---
        for(int i=1; i<=moveInfo.distance; i++) {
            int NextX = piece.MyPos.Item1 + moveInfo.dirX * i;
            int NextY = piece.MyPos.Item2 + moveInfo.dirY * i;
            (int x, int y) currentPos = (NextX,NextY);

            if(!Utils.IsInBoard(currentPos)) break; //가려는 pos이 보드에 없으면 for구문 탈출, false 반환

            Piece PieceAtPos = MapManager.Instance.Pieces[NextX,NextY]; //(NextX,NextY)에 있는 piece를 pieceAtpos에 넣고
            //01.20 정수민 수정
           
            if(currentPos == targetPos) { //targetPos에 도착하는 경우의 조건
                return (PieceAtPos == null); //경로에 아무것도 없으면 true                
            }
        }

            // if(PieceAtPos != null ) {  //targetPos까지 가는 도중에 뭔가가 있다면
            //     break;
            // }
        return false;
        // ------
    }


    //가능한 움직임인지를 검증
    public bool IsValidMove(Piece piece, (int, int) targetPos)
    {
        if (!Utils.IsInBoard(targetPos) || targetPos == piece.MyPos) return false;

        foreach (var moveInfo in piece.GetMoves()) //var 는 암시적 타입, 우변의 값을 보고 알아서 판단
        {
            if (TryMove(piece, targetPos, moveInfo)) //가능한 움직임이 하나라도 있으면 true 반환
                return true;
        }
        
        return false;
        
    }

    public void ShowPossibleMoves(Piece piece)
    {
        ClearEffects();

        // 가능한 움직임을 표시
        // IsValidMove를 사용
        // effectPrefab을 effectParent의 자식으로 생성하고 위치를 적절히 설정
        // currentEffects에 effectPrefab을 추가

        // --- TODO ---
        for(int x = 0; x<Utils.FieldWidth; x++) {
            for(int y = 0; y <Utils.FieldHeight; y++) {
                if(IsValidMove(piece,(x,y))) { //그 칸으로 이동할 수 있는지 최종확인 후, 된다면 effect하나 생성
                    Debug.Log($"Effect x = {x} y = {y} ");
                    GameObject effectObj = Instantiate(effectPrefab,Utils.ToRealPos((x,y)),Quaternion.identity,effectParent);
                    currentEffects.Add(effectObj);
                }
            }
        }
        // ------
    }

    // 효과 비우기
    public void ClearEffects()  //이전 예상 경로 지우기
    {
        foreach (var effect in currentEffects)
        {
            if (effect != null) Destroy(effect);
        }
        currentEffects.Clear();
    }
}
