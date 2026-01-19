using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;


/// <summary>
/// 게임의 전체 상태(로비, 인게임, 일시정지) 관리
/// 
/// 턴의 상태 관리(movestate, tag, attack....)
/// <para>게임 전체 전역 싱글톤</para>
/// </summary>
public class GameManager : Singleton<GameManager>
{    
    //01.18 정수민, 튜토리얼 체크
    public bool isTutorialMode=false;
    
    //01.17 정수민
    public Enums.StageState CurrentStageState { get; private set; } = Enums.StageState.Playing;
    public Enums.TurnState CurrentTurnState { get; private set; } = Enums.TurnState.Ready;

    //01.19 정수민
    private PlayerUIStatus playeruistatus;
    

    

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
    public int CurrentPlayer = 0; //01.19 턴 종료 하기전에 조종하고 있는 플레이어

    // 1/14 서진현
    //01.17 정수민
    public int totalTurn = 5; //스테이지 마다 정해진 총 턴 수
    public int currentTurn = 0; //현재 턴수 


    //01.18 정수민: serialize화
    [Header("StartPos")]
    [SerializeField] private Vector2Int startpos1= new Vector2Int(1,1); // 플레이어 및 몬스터 위치
    [SerializeField] private Vector2Int startpos2= new Vector2Int(3,1);
    [SerializeField] private Vector2Int monster_pos=new Vector2Int(2,3);

    // 장판 색깔
    public Color player1Color = Color.red;
    public Color player2Color = Color.blue;
    public Color overlapColor = Color.magenta;

    //1/19 구본환
    public bool IsTagTurn = false;
    

    //01.17 정수민 stagestate 변경
    public void ChangeStageState(Enums.StageState newStageState)
    {
        CurrentStageState = newStageState;
        Debug.Log($"Stage State changed to: {CurrentStageState}");
        switch (CurrentStageState)
        {
            case Enums.StageState.Playing:
            break;
            case Enums.StageState.Pause:
            break;
            case Enums.StageState.Victory:
            break;
            case Enums.StageState.Gameover:
            UIManager.Instance.ShowRetryPanel();
            break;
        }
    }
    
    public void ChangeTurnState(Enums.TurnState newTurnState)
    {
        CurrentTurnState = newTurnState;
        // 상태 변경에 따른 로직
        Debug.Log($"Turn State changed to: {CurrentTurnState}");
        switch (CurrentTurnState)
        {
            //01.17 정수민, ready일 때 turn 계산
            case Enums.TurnState.Ready:
            UIManager.Instance.ShowMoveButton();
            CalculateTurn();
            //01.19 정수민 초상화 업데이트
            GetActivatePlayer();
            playeruistatus.UpdatePlayerPortrait();


            break;
            
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

            //01.18 정수민 tutorialmanager
            if (isTutorialMode) {
                TutorialManager.Instance.NextStep();
            }

            break;
        }
    }

    

    protected override void Awake()
    {
        
        //01.19 정수민
        playeruistatus = FindFirstObjectByType<PlayerUIStatus>();
        
        TileParent = GameObject.Find("TileParent").transform;
        PieceParent = GameObject.Find("PieceParent").transform;
        EffectParent = GameObject.Find("EffectParent").transform;

        MovementManager.Instance.Initialize(EffectPrefab, EffectParent);

        //01.18 정수민 tutorialmanager 추가
        if(isTutorialMode) {
            TutorialManager.Instance.Initialize(EffectPrefab,EffectParent);
        }        
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
        //01.18 정수민: 벡터2형식의 변수들 튜플 형식의 변수와 구분(startpos1_tuple로 변환)
        
        (int,int) startpos1_tuple = (startpos1.x,startpos1.y);  //벡터의 튜플화
        (int,int) startpos2_tuple = (startpos2.x,startpos2.y);
        (int,int) monster_pos_tuple = (monster_pos.x,monster_pos.y);

        int [] setup = {}; //piece의 종류를 담는 배열
        

        p1Instance = PlacePiece(0,startpos1_tuple);
        p2Instance = PlacePiece(1,startpos2_tuple);
        //monster 배치
        //Edited By 구본환, 1/18
        List<Vector2Int> monsterSpawns = new List<Vector2Int>
    {
        new Vector2Int(4, 2)
        // 이후에 몬스터 추가할 때 이 리스트 사용
    };

        foreach (Vector2Int pos in monsterSpawns)
        {
            Piece p = PlacePiece(2, (pos.x, pos.y));

            if (p is Monster monster)
            {
                monster.InitializeStats(10);
                monster.InitializePath();
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
    //Edit By 구본환 1/18
    public Vector2Int GetPlayer1Pos()
    {
        if (p1Instance != null)
            return new Vector2Int(p1Instance.MyPos.Item1, p1Instance.MyPos.Item2);
        return startpos1;
    }
    public Vector2Int GetPlayer2Pos()
    {
        if (p2Instance != null)
            return new Vector2Int(p2Instance.MyPos.Item1, p2Instance.MyPos.Item2);
        return startpos2;
    }

    public Vector2Int GetMonsterPos()
    {
        for (int x = 0; x < Utils.FieldWidth; x++)
        {
            for (int y = 0; y < Utils.FieldHeight; y++)
            {
                if (Pieces[x, y] is Monster)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return monster_pos;
    }

    // 몬스터 HP 계산(pos.x, pos.y)
    public void ApplyMonsterDamage(int damage)
    {
        if (damage <= 0) return;

        Vector2Int targetPos = GetMonsterPos();
        Piece targetPiece = Pieces[targetPos.x, targetPos.y];

        if (targetPiece != null && targetPiece is Monster monster)
        {
            monster.TakeDamage(damage);
            Debug.Log($"Applied {damage} damage to Monster at {targetPos}. Remaining HP: {monster.CurrentHP}");
        }
        else
        {
            Debug.LogWarning("ApplyMonsterDamage 호출되었지만, 몬스터 못찾음");
        }

        //로컬 변수 업데이트
        MonsterHP -= damage;
        Debug.Log("HP: " + MonsterHP);
        if (MonsterHP < 0) MonsterHP = 0;

        if (MonsterHP == 0)
        {
            Debug.Log("승리");
            return;
        }
    }

    public Piece GetActivatePlayer() {
        //플레이어 바꾸기
        if(CurrentTurnState == Enums.TurnState.PlayerTag) { //tag를 누른 상태라면
            if(NextPlayer == 0) {
                CurrentPlayer = 0; //01.19 정수민
                NextPlayer = 1; //next는 player2(tag상태일 때만 바꾸기 가능)
            } else {
                CurrentPlayer = 1; //01.19 정수민
                NextPlayer = 0; //next는 player1(tag상태일 때만 바꾸기 가능)
            }
        } else { //01.19 정수민 : tag를 누르지 않은 상태라면 그대로감
            CurrentPlayer = NextPlayer;
        }

        return (NextPlayer == 0) ? p1Instance : p2Instance;
    }

    //01.19 정수민: 튜토리얼에서 쓸거임
    public Piece GetCurrentPlayer() {
        return (CurrentPlayer == 0) ? p1Instance : p2Instance;
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


        UpdateAttackAreaTiles();
    }

    // 플레이어 장판
    HashSet<Vector2Int> Get3x3Area(Vector2Int center)
    {
        HashSet<Vector2Int> area = new HashSet<Vector2Int>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int x = center.x + dx;
                int y = center.y + dy;

                if(x >= 0 && x < Utils.FieldWidth && y >= 0 && y < Utils.FieldHeight)
                {
                    area.Add(new Vector2Int(x, y));
                }
            }
        }

        return area;
    }

    public void UpdateAttackAreaTiles()
    {
        for (int x = 0; x < Utils.FieldWidth; x++)
        {
            for (int y = 0; y < Utils.FieldHeight; y++)
            {                
                Tiles[x, y].ResetColor();
            }
        } 

        Vector2Int p1Pos = new Vector2Int(p1Instance.MyPos.Item1, p1Instance.MyPos.Item2);
        Vector2Int p2Pos = new Vector2Int(p2Instance.MyPos.Item1, p2Instance.MyPos.Item2);
        
        var area1 = Get3x3Area(p1Pos);
        var area2 = Get3x3Area(p2Pos);

        for (int x = 0; x < Utils.FieldWidth; x++)
        {
            for (int y = 0; y < Utils.FieldHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                bool inP1 = area1.Contains(pos);
                bool inP2 = area2.Contains(pos);

                if (inP1 && inP2) Tiles[x, y].SetColor(overlapColor);
                else if (inP1) Tiles[x, y].SetColor(player1Color);
                else if (inP2) Tiles[x, y].SetColor(player2Color); 
            }
        }
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

    //01.17 정수민
    public void CalculateTurn() {
        //1/19 구본환
        IsTagTurn = false;
        currentTurn = currentTurn + 1;
        int remainTurn = totalTurn - currentTurn;
        UIManager.Instance.ShowRemainTurn(remainTurn, totalTurn);
        if(remainTurn == 0) {
            ChangeStageState(Enums.StageState.Gameover);
        }
        
    }
    //현재 턴 1 증가
    //uimanager에 남은 턴수 표시, 남은 턴수가 0이면 게임오버


    ///UIManager에서 받아오는 함수들
    public void HandleMove() {

        ChangeTurnState(Enums.TurnState.PlayerMove);
        TutorialManager.Instance.stepcount ++;  //01.19 정수민
    }

    public void HandleTag() {

        
        //01.19 정수민
        if(isTutorialMode) {
        Piece piece = GetCurrentPlayer();

            if(!piece.hasMoved) {
                Debug.Log("이동하고 눌러야지");
                return; 
            }
            
            // 2. 특정 단계(0, 2번 스텝)에서는 태그 금지
            if(TutorialManager.Instance.currentStep is 0 or 2) {
                Debug.Log("지금은 태그할 단계가 아닙니다.");
                return; 
            }
        }
        //1/19 구본환
        IsTagTurn = true;

        ChangeTurnState(Enums.TurnState.PlayerTag);
        TutorialManager.Instance.stepcount ++;  //01.19 정수민
    }
    //Edited By 구본환 1/13
    public void HandleEnd()
    {
        //01.18정수민 tutorialmanager tutorial일 때 이동을 하고 나서야 턴 종료가능
        if(isTutorialMode) {
            Piece piece = GetCurrentPlayer();
            if(!piece.hasMoved) {
                Debug.Log("이동하고 종료하쇼");
                return;
            }
            if(TutorialManager.Instance.currentStep is 1 or 3 or 4) {
                if(CurrentTurnState is Enums.TurnState.PlayerMove) {
                    Debug.Log("tag버튼 누르라니깐");
                    return;
                }
            }

        }
        
        StartCoroutine(ProcessTurnSequence());
        TutorialManager.Instance.stepcount ++; //01.19 정수민
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
        //01.18 정수민 tutorialmanager 추가
        if(isTutorialMode) {
            return TutorialManager.Instance.IsValidTutorialMove(piece,targetPos);
        } else
        return MovementManager.Instance.IsValidMove(piece, targetPos);
    }

    public void ShowPossibleMoves(Piece piece)
    {
        //01.18 정수민 tutorialmanager 추가
        if(isTutorialMode) {
            TutorialManager.Instance.ShowTutorialMoves(piece);
        } else
        MovementManager.Instance.ShowPossibleMoves(piece);
    }

    public void ClearEffects()
    {
        //01.18 정수민 tutorialmanager 추가
        TutorialManager.Instance.ClearEffects();
        MovementManager.Instance.ClearEffects();
    }

    // 추가적인 게임 관리 기능들
}
