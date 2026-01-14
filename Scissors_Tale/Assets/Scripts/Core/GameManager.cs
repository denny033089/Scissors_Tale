using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 게임의 전체 상태(로비, 인게임, 일시정지) 관리
/// 
/// 턴의 상태 관리(movestate, tag, attack....)
/// <para>게임 전체 전역 싱글톤</para>
/// </summary>
public class GameManager : Singleton<GameManager>
{    
    
    public  Enums.GameState CurrentState { get; private set; } = Enums.GameState.Main;
    public Enums.TurnState CurrentTurnState { get; private set; } = Enums.TurnState.Ready;

    

    public GameObject TilePrefab;  //인스펙터 창에 tileprefab 삽입
    public GameObject[] PiecePrefabs;
    public GameObject EffectPrefab;
    private Piece p1Instance;  //Instantiate해서 만들어진 실제 gameobject의 piece.cs를 받아줄 변수
    private Piece p2Instance;
    

    // 오브젝트의 parent들
    private Transform TileParent;
    private Transform PieceParent;
    private Transform EffectParent;
    private UIManager uiManager;
    public Tile[,] Tiles = new Tile[Utils.FieldWidth, Utils.FieldHeight];   // Tile.cs 담는 2차원 배열
    public Piece[,] Pieces = new Piece[Utils.FieldWidth, Utils.FieldHeight];    // Piece.cs들
    public int monster_num = 1; // 초기 몬스터는 1명
    public int MonsterHP = 10; // 초기 몬스터 HP는 10

    public int NextPlayer = 0; //0은 플레이어 1, 1은 플레이어2 맨처음엔 0

    private Vector2Int startpos1; // 플레이어 및 몬스터 위치
    private Vector2Int startpos2;
    private Vector2Int monster_pos;

    public void ChangeState(Enums.GameState newState)
    {
        CurrentState = newState;

        // 상태 변경에 따른 로직

        Debug.Log($"Game State changed to: {CurrentState}");
    }

    public void ChangeTurnState(Enums.TurnState newTurnState)
    {
        CurrentTurnState = newTurnState;
        // 상태 변경에 따른 로직
        Debug.Log($"Game State changed to: {CurrentTurnState}");
        switch (CurrentTurnState)
        {
            case Enums.TurnState.PlayerMove:
            
            Piece piece = GetActivatePlayer();        
            ShowPossibleMoves(piece);                            
            break;

            case Enums.TurnState.PlayerTag:
            GetActivatePlayer();
            break;

            case Enums.TurnState.PlayerAttack:
            Attack();
                // 공격 범위 계산 및 표시
            break;
            case Enums.TurnState.MonsterMove:
            MonsterMove();
                // 몬스터 AI 시작 (코루틴 등 호출)
            break;

            case Enums.TurnState.End:
            p1Instance.hasMoved = false; //턴이 끝나면 이동여부 초기화
            p2Instance.hasMoved = false;

            break;
        }
    }

    

    protected override void Awake()
    {
        TileParent = GameObject.Find("TileParent").transform;
        PieceParent = GameObject.Find("PieceParent").transform;
        EffectParent = GameObject.Find("EffectParent").transform;

        MovementManager.Instance.Initialize(EffectPrefab, EffectParent);        
        InitializeBoard();
    }
    /// ---Ready---
    /// [Move] 버튼을 누르면 HandleMove, PlayerMove 상태로 바뀜
    /// ---PlayerMove---
    /// GetActivatePlayer->ShowPossibleMoves
    /// 이동할 수 있는 보드 판을 클릭
    /// IsValidMove->MovePlayer->ClearEffects
    /// [Tag] 버튼을 누르면 HandleTag, PlayerTag 상태로 바뀜
    /// ---PlayerTag---
    /// GetActivatePlayer
    /// [End] 버튼을 누르면 HandleEnd, PlayerAttack 상태로 바뀜
    /// ---PlayerAttack---
    /// ...플레이어 공격
    /// MonsterMove상태로 바뀜
    /// ...몬스터 이동
    /// ---MonsterMove---
    /// End상태로 바뀜
    /// ... 주어진 턴 감소, 상황 정리? 등
    /// Ready상태로 바뀜
    /// ---Ready---
    /// ...반복

    /// 버튼은 uimanager에서 받고 입력은 clickhandler에서 받기

    void InitializeBoard()
    {
        // 타일 배치
        // TilePrefab을 TileParent의 자식으로 생성하고, 배치함
        // Tiles를 채움
        for(int x=0; x<Utils.FieldWidth; x++) {

            for(int y=0; y<Utils.FieldHeight; y++) {

                GameObject tileObj = Instantiate(TilePrefab, Utils.ToRealPos((x,y)),Quaternion.identity,TileParent);
                //TilePrefab을 가져온뒤, tileObj 안에 할당
                //TilePrefab을 배치한 뒤, 부모는 TileParent로 상속되기
                Tiles[x,y] = tileObj.GetComponent<Tile>(); //Tile.cs를 2차원 배열에 담아두기
            }
        }

        PlacePieces();
    }

    ///piece의 종류는 player1 player2 monster1, monster2, monster3,.....
    /// startpos를 정해두고 그곳에 배치할 수 있도록...
    /// 맵마다 startpos가 다르다고 생각됨 <---map1.cs map2.cs ....에 따라 다를듯
    void PlacePieces()
    {
        // PlacePiece를 사용하여 Piece들을 적절한 모양으로 배치
        /// 4
        /// 3     *
        /// 2
        /// 1   *   *
        /// 0
        ///   0 1 2 3 4
        /// 
        // --- TODO ---
        
        (int,int) startpos1 = (1,1);
        (int,int) startpos2 = (3,1);
        (int,int) monster_pos = (2,3);

        int [] setup = {}; //piece의 종류를 담는 배열
        

        p1Instance = PlacePiece(0,startpos1);
        p2Instance = PlacePiece(1,startpos2);
        //monster 배치
        //Edited By 구본환, 1/13
        for (int x = 0; x < monster_num; x++)
        {
            // 기물 생성
            Piece p = PlacePiece(2, monster_pos);

            // 몬스터인지 확인
            if (p is Monster)
            {
                // N개의 경로 생성
                ((Monster)p).InitializePath();
            }
        }

    }

    Piece PlacePiece(int pieceType, (int x, int y) pos)
    {
        /// Piece를 생성한 후, initialize(moveto를 이용한 배치)
        ///setup[0] = player1, setup[1] = player2, setup[>1] = monster
        /// 
        /// 배치한 Piece를 리턴
        GameObject pieceObj = Instantiate(PiecePrefabs[pieceType],new Vector3(0,0,0),Quaternion.identity,PieceParent);
        Pieces[pos.x,pos.y] = pieceObj.GetComponent<Piece>();
        Pieces[pos.x,pos.y].MoveTo(pos);
        return pieceObj.GetComponent<Piece>();
    }

    // 플레이어 및 몬스터 위치
    public Vector2Int GetPlayer1Pos() => startpos1;
    public Vector2Int GetPlayer2Pos() => startpos2;

    public Vector2Int GetMonsterPos() => monster_pos;

    // 몬스터 HP 계산
    public void ApplyMonsterDamage(int damage)
    {
        if (damage <= 0) return;

        MonsterHP -= damage;
        if (MonsterHP < 0) MonsterHP = 0;

        if (MonsterHP == 0)
        {
            // 승리 표시
        }
    }

    public Piece GetActivatePlayer() {
        //플레이어 바꾸기
        if(CurrentTurnState == Enums.TurnState.PlayerTag) { //tag를 누른 상태라면
            if(NextPlayer == 0) {
                NextPlayer = 1; //next는 player2(tag상태일 때만 바꾸기 가능)
            } else {
                NextPlayer = 0; //next는 player1(tag상태일 때만 바꾸기 가능)
            }
        }

        return (NextPlayer == 0) ? p1Instance : p2Instance;
    }

    

    public void MovePlayer(Piece piece, (int,int) targetPos) {

        if(piece.hasMoved == true) { //이미 이동했는지 여부 확인
            Debug.Log("이미 이동했음");
            return;
        } 
        //boardpos을 받아서 isinboard인지 확인, moveinfo확인, 플레이어 이동
        //이동 가능한 구역인지 확인
        //Edited By 구본환, 1/13
        if (!(piece is Monster))
        {
            if (!IsValidMove(piece, targetPos)) return;
        }
        // Piece를 이동시킴
        // 배열에서 원래 자리 비우기
        (int oldX, int oldY) = piece.MyPos;
        Pieces[oldX, oldY] = null;

        // 오브젝트 이동 
        piece.MoveTo(targetPos);
        piece.hasMoved = true; //이동했음

        // 배열에 새 자리 채우기
        Pieces[targetPos.Item1, targetPos.Item2] = piece;
    }

    public void Attack() {
        AttackManager.Instance.Attack();
    }

    public void MonsterMove() {
        //몬스터 배열 새로 생성
        List<Monster> allMonsters = new List<Monster>();

        for (int x = 0; x < Utils.FieldWidth; x++)
        {
            for (int y = 0; y < Utils.FieldHeight; y++)
            {
                Piece p = Pieces[x, y];

                //p가 몬스터인지 확인
                if (p != null && p is Monster)
                {
                    allMonsters.Add((Monster)p);
                }
            }
        }


        // 몬스터 이동
        foreach (Monster m in allMonsters)
        {
            // 몬스터 살아있는지 확인
            if (m != null)
            {
                m.PerformTurn();
            }
        }
    }



    ///UIManager에서 받아오는 함수들
    public void HandleMove() {
        ChangeTurnState(Enums.TurnState.PlayerMove);
    }

    public void HandleTag() {
        ChangeTurnState(Enums.TurnState.PlayerTag);
    }
    //Edited By 구본환 1/13
    public void HandleEnd()
    {
        StartCoroutine(ProcessTurnSequence());
    }
    IEnumerator ProcessTurnSequence()
    {
        // 1. 플레이어 공격 페이즈
        ChangeTurnState(Enums.TurnState.PlayerAttack);
        yield return new WaitForSeconds(0.5f); // 공격 모션 대기

        // 2. 몬스터 이동 페이즈
        ChangeTurnState(Enums.TurnState.MonsterMove);
        yield return new WaitForSeconds(1.0f);

        // 3. 턴 종료
        ChangeTurnState(Enums.TurnState.End);
        yield return new WaitForSeconds(0.5f);

        // 4. 다시 플레이어 대기
        ChangeTurnState(Enums.TurnState.Ready);
    }


    ///MovementManager 호출하는 함수들
    public bool IsValidMove(Piece piece, (int, int) targetPos)
    {
        return MovementManager.Instance.IsValidMove(piece, targetPos);
    }

    public void ShowPossibleMoves(Piece piece)
    {
        MovementManager.Instance.ShowPossibleMoves(piece);
    }

    public void ClearEffects()
    {
        MovementManager.Instance.ClearEffects();
    }

    // 추가적인 게임 관리 기능들
}
