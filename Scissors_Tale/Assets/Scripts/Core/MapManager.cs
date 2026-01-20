using UnityEngine;

public class MapManager : Singleton<MapManager>
{

    public MapData currentMapData; //01.21 정수민  currentMapData를 인스펙터에서 할당

    public Tile[,] Tiles = new Tile[Utils.FieldWidth, Utils.FieldHeight];   // Tile.cs 담는 2차원 배열
    public Piece[,] Pieces = new Piece[Utils.FieldWidth, Utils.FieldHeight];    // Piece.cs들
    public GameObject TilePrefab;  //인스펙터 창에 tileprefab 삽입
    public GameObject[] PiecePrefabs;
    private Transform TileParent;
    private Transform PieceParent;


    private Piece p1Instance;  //Instantiate해서 만들어진 실제 gameobject의 piece.cs를 받아줄 변수
    private Piece p2Instance;


    protected override void Awake() {
        TileParent = GameObject.Find("TileParent").transform;
        PieceParent = GameObject.Find("PieceParent").transform;
    }
    
    public void InitializeBoard()
    {
        //01.20 정수민
        Utils.FieldWidth = currentMapData.width;
        Utils.FieldHeight = currentMapData.height;

        
        // 변경된 크기를 바탕으로 Tiles 배열과 Pieces 배열 새로 생성
        // 여기서 새로 할당함
        Tiles = new Tile[Utils.FieldWidth, Utils.FieldHeight];
        Pieces = new Piece[Utils.FieldWidth, Utils.FieldHeight];
        
        
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
        // 1/19 구본환
        SoundManager.Instance.PlayBGM("Stage_BGM");

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
        

        //01.20정수민
        foreach (MonsterSpawnInfo info in currentMapData.monsterSpawns)
        {
            // 몬스터 이름(Key)을 통해 프리팹을 찾거나 타입을 결정 (Dictionary 활용 가능)
            int typeIndex = GetMonsterTypeByName(info.monsterName); 
            
            Piece p = PlacePiece(typeIndex, (info.spawnPos.x, info.spawnPos.y));

            if (p is Monster monster)
            {
                monster.InitializeStats();
                monster.InitializePath();
            }
        }
        
        //01.20 정수민 player 위치도 추가
        p1Instance = PlacePiece(0,currentMapData.startpos1.ToTuple());
        p2Instance = PlacePiece(1,currentMapData.startpos2.ToTuple());

        GameManager.Instance.p1Instance = p1Instance;  //플레이어 정보 gamemanager에 전달
        GameManager.Instance.p2Instance = p2Instance;
        Debug.Log("플레이어 게임매니저에 할당");
        //monster 배치
        //Edited By 구본환, 1/18
       

    }


    int GetMonsterTypeByName(string name)
    {
        if (name == "monster") return 2;
        if (name == "TutorialMonster") return 3;
        return 2; // 기본값  나중에 dictionary로 대체
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
}
