using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;


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
    public MapData currentMapData; //01.20 정수민  currentMapData를 인스펙터에서 할당, totalturn을 받아옴
    
    // 01.25 정수민: 씬이 바뀌어도 유일하게 메모리에 남는 정적 변수
    public static MapData SelectedMapData;
    


    public GameObject EffectPrefab;
    public Piece p1Instance;  //Instantiate해서 만들어진 실제 gameobject의 piece.cs를 받아줄 변수
    public Piece p2Instance;
    

    // 오브젝트의 parent들


    private Transform EffectParent;
    private UIManager uiManager;

    // Piece.cs들
  


    public int NextPlayer = 0; //0은 플레이어 1, 1은 플레이어2 맨처음엔 0
    public int CurrentPlayer = 0; //01.19 턴 종료 하기전에 조종하고 있는 플레이어

    // 1/14 서진현
    //01.17 정수민
    public int totalTurn; //스테이지 마다 정해진 총 턴 수
    public int currentTurn = 0; //현재 턴수 


    //01.20 정수민: startpos 정보 삭제
    //01.20 정수민: pieces 삭제, tiles 삭제

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
            UIManager.Instance.ShowClearPanel();

            break;
            case Enums.StageState.Gameover:
            UIManager.Instance.ShowFailPanel();
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

            if(!IsRemainMonster()) {
                ChangeStageState(Enums.StageState.Victory);
            }

            break;
        }
    }

    
    //01.25 정수민 reset 버튼 구현을 위한 함수(싱글톤)
    // protected override void OnSceneLoaded(string sceneName)
    // {
        
    //     bool isGameScene = sceneName.Contains("Stage") || sceneName.Contains("Tutorial") || sceneName.Contains("Test"); //게임 씬 바로 실행 가능
    //     if(GameSystemManager.Instance.CurrentGameState is not (Enums.GameState.InGame or Enums.GameState.Tutorial) && !isGameScene) {
    //         Debug.Log("게임 씬이 아닙니다");
    //         return;
    //     }
        
    //     playeruistatus = FindFirstObjectByType<PlayerUIStatus>();
        
        
    //     EffectParent = GameObject.Find("EffectParent").transform;

    //     MovementManager.Instance.Initialize(EffectPrefab, EffectParent);

    //     //01.18 정수민 tutorialmanager 추가
    //     if(isTutorialMode) {
    //         TutorialManager.Instance.Initialize(EffectPrefab,EffectParent);
    //     }

    //     //01.20 정수민 totalturn 초기화
    //     if (currentMapData != null) 
    //     {
    //         totalTurn = currentMapData.totalTurnLimit;
    //     }
        
        
    //     //01.25 정수민
    //     MapManager sceneMapMgr = FindFirstObjectByType<MapManager>();
    
    //     if (sceneMapMgr != null)
    //     {
    //         // 핵심: 씬에 있는 MapManager가 들고 있는 데이터를 GameManager로 복사합니다.
    //         this.currentMapData = sceneMapMgr.currentMapData; 
            
    //         if (currentMapData != null) 
    //         {
    //             totalTurn = currentMapData.totalTurnLimit;
    //             Debug.Log($"[GameManager] {sceneName}의 데이터를 성공적으로 로드했습니다.");
                
    //             // 데이터 할당 후 보드 생성 호출
    //             sceneMapMgr.InitializeBoard(); 
    //         }
    //     }
        
        
        
    //     // if (MapManager.Instance != null)
    //     // {
    //     //     Debug.Log($"[GameManager] {sceneName} 씬에서 보드 초기화를 시작합니다.");
    //     //     MapManager.Instance.InitializeBoard();
    //     // }
    // }
    protected override void Awake()
    {
    
        //01.25 정수민
        // 1. 만약 정적 변수에 선택된 데이터가 있다면 덮어씌움
        if (SelectedMapData != null)
        {
            currentMapData = SelectedMapData;
        }


        
        
        
        //01.25 싱글톤 로직 실행
        // base.Awake();     
        // if (Instance != this) return; // 내가 진짜 인스턴스일 때만 아래 실행
        
        
        //01.19 정수민

        playeruistatus = FindFirstObjectByType<PlayerUIStatus>();
        
        
        EffectParent = GameObject.Find("EffectParent").transform;

        MovementManager.Instance.Initialize(EffectPrefab, EffectParent);

        //01.18 정수민 tutorialmanager 추가
        if(isTutorialMode) {
            TutorialManager.Instance.Initialize(EffectPrefab,EffectParent);
        }

        //01.20 정수민 totalturn 초기화
        if (currentMapData != null) 
        {
            totalTurn = currentMapData.totalTurnLimit;
        }
                
        
    }
    //1/19 구본환
    private void Start()
    {
        //SoundManager 문제로 InitializeBoard를 Start로 옮김
        MapManager.Instance.InitializeBoard();

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

    

    ///piece의 종류는 player1 player2 monster1, monster2, monster3,.....
    /// startpos를 정해두고 그곳에 배치할 수 있도록...
    /// 맵마다 startpos가 다르다고 생각됨 <---map1.cs map2.cs ....에 따라 다를듯


    // 플레이어 및 몬스터 위치
    //Edit By 구본환 1/18
    public Vector2Int GetPlayer1Pos()
    {
        if (p1Instance != null)
            return new Vector2Int(p1Instance.MyPos.Item1, p1Instance.MyPos.Item2);
        return currentMapData.startpos1;
    }
    public Vector2Int GetPlayer2Pos()
    {
        if (p2Instance != null)
            return new Vector2Int(p2Instance.MyPos.Item1, p2Instance.MyPos.Item2);
        return currentMapData.startpos2;
    }

    //01.20 정수민
    public HashSet<Vector2Int> GetAllMonsterPositions()
    {
        HashSet<Vector2Int> monsterPositions = new HashSet<Vector2Int>();

        for (int x = 0; x < Utils.FieldWidth; x++)
        {
            for (int y = 0; y < Utils.FieldHeight; y++)
            {
                if (MapManager.Instance.Pieces[x, y] != null && MapManager.Instance.Pieces[x, y] is Monster)  //01.20 정수민 수정
                {
                    monsterPositions.Add(new Vector2Int(x, y));
                }
            }
        }
        return monsterPositions;
    }

    // 몬스터 HP 계산(pos.x, pos.y)
    //01.20 정수민: mPos 인자 추가
    public void ApplyMonsterDamage(int damage,Vector2Int mPos)
    {
        if (damage <= 0) return;

        Piece targetPiece = MapManager.Instance.Pieces[mPos.x, mPos.y];

        if (targetPiece != null && targetPiece is Monster monster)
        {
            monster.TakeDamage(damage);
            Debug.Log($"Applied {damage} damage to Monster at {mPos}. Remaining HP: {monster.CurrentHP}");
            if (monster.CurrentHP < 0) //01.20 정수민
            {
                Debug.Log("승리");
                
                return;
            }
            
        }
        else
        {
            Debug.LogWarning("ApplyMonsterDamage 호출되었지만, 몬스터 못찾음");
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

        SoundManager.Instance.PlaySFX("Walk");

        // Piece를 이동시킴
        // 배열에서 원래 자리 비우기
        (int oldX, int oldY) = piece.MyPos;
        MapManager.Instance.Pieces[oldX, oldY] = null;

        // 오브젝트 이동 
        piece.MoveTo(targetPos);
        piece.hasMoved = true; //이동했음

        // 배열에 새 자리 채우기
        MapManager.Instance.Pieces[targetPos.Item1, targetPos.Item2] = piece;

        if(isTutorialMode) TutorialManager.Instance.IncrementStep(); //01.24 정수민


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
                MapManager.Instance.Tiles[x, y].ResetColor();  //01.20 정수민: Tiles를 MapManager의 것으로 쓰도록
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

                if (inP1 && inP2) MapManager.Instance.Tiles[x, y].SetColor(overlapColor);  //01.20 정수민 수정
                else if (inP1) MapManager.Instance.Tiles[x, y].SetColor(player1Color);
                else if (inP2) MapManager.Instance.Tiles[x, y].SetColor(player2Color); 
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
                Piece p = MapManager.Instance.Pieces[x, y];

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
        if(isTutorialMode) TutorialManager.Instance.IncrementStep(); //01.24 정수민
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
        if(isTutorialMode) TutorialManager.Instance.IncrementStep(); //01.24 정수민
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
            if(TutorialManager.Instance.currentStep is 1 or 3 or 4 or 5) {
                if(CurrentTurnState is Enums.TurnState.PlayerMove) {
                    Debug.Log("tag버튼 누르라니깐");
                    return;
                }
            }

        }

        ClearEffects(); //01.19 정수민 코드 안움직이더라도 이펙트 사라지도록
        
        StartCoroutine(ProcessTurnSequence());
        if(isTutorialMode) TutorialManager.Instance.IncrementStep(); //01.24 정수민
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

    //01.20 정수민
    public bool IsRemainMonster() {
        for (int x = 0; x < Utils.FieldWidth; x++)
        {
            for (int y = 0; y < Utils.FieldHeight; y++)
            {
                //  해당 칸에 기물이 있고, 그 타입이 Monster인지 확인
                if (MapManager.Instance.Pieces[x, y] != null && MapManager.Instance.Pieces[x, y] is Monster)
                {
                    // 몬스터를 하나라도 찾으면 즉시 true 반환 (알고리즘 효율성)
                    return true;
                }
            }
        }

        //  모든 칸을 다 돌았는데 없으면 false 반환
        return false;

    }

    public void RequestPause()
    {
        // 일시정지 처리
        CurrentStageState = Enums.StageState.Pause;
        // 일시정지 UI 표시
        UIManager.Instance.ShowPausePanel();
        
        Time.timeScale = 0f;    //게임 정지
    }

    public void CancelPause()
    {
        Time.timeScale = 1f;    //게임 재개

        // 일시정지 UI 숨김
        UIManager.Instance.HidePausePanel();
        // play 처리
        CurrentStageState = Enums.StageState.Playing;
    }

    public void StageRestart()
    {
        Time.timeScale = 1f;

        
        //씬 초기화
        string currentSceneName = SceneManager.GetActiveScene().name; //현재 씬 가져와서 로드
        SceneManager.LoadScene(currentSceneName);
    }

    // 추가적인 게임 관리 기능들
}
