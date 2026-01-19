using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//01.18 정수민
public class TutorialManager : Singleton<TutorialManager>
{
    
    private GameObject effectPrefab;
    private Transform effectParent;
    private List<GameObject> currentEffects = new List<GameObject>();   // 현재 effect들을 저장할 리스트
    //리스트에 Effect라는 GameObject들을 담을 거임. Effect는 기물들의 예상 이동경로를 보여주는 역할.
    
    
    [Header("Tutorial Path")]
    // 유니티 인스펙터에서 정해줄 좌표 리스트 (예: (1,2), (2,2), (2,3)...)
    public List<Vector2Int> playerFixedPath = new List<Vector2Int>();

    public int currentStep = 0; //currentStep = 0 이면 turn은 1임
    private (int x,int y) requiredPos;
    


    // 현재 튜토리얼 단계에서 가야 할 목표 좌표
    

    void Update() 
    {
        
    }
    
    public void Initialize(GameObject effectPrefab, Transform effectParent)
    {
        this.effectPrefab = effectPrefab;
        this.effectParent = effectParent;
    }
    
    public bool IsValidTutorialMove(Piece piece,(int x, int y) clickedPos) {

        if (!Utils.IsInBoard(clickedPos) || clickedPos == piece.MyPos) return false;

        requiredPos = playerFixedPath[currentStep].ToTuple();
        
        if (clickedPos.x == requiredPos.x && clickedPos.y == requiredPos.y) {
            
            return true; // 정해진 방향으로 클릭함
        }
        
        Debug.Log("이쪽으로 가면 안 돼요! 화살표가 가리키는 곳을 클릭하세요.");
        return false;
    }

    public void ShowTutorialMoves(Piece piece) {
        ClearEffects();

        //리스트 최신화
        if (currentStep < playerFixedPath.Count) {
        requiredPos = playerFixedPath[currentStep].ToTuple();
        }

        //필요한 좌표에 effect 추가  
        GameObject effectObj = Instantiate(effectPrefab,Utils.ToRealPos(requiredPos),Quaternion.identity,effectParent);
        currentEffects.Add(effectObj);
    }

    public void ClearEffects()  //이전 예상 경로 지우기
    {
        Debug.Log("이펙트삭제");
        foreach (var effect in currentEffects)
        {
            if (effect != null) Destroy(effect);
        }
        currentEffects.Clear();
    }


    public void NextStep() {
        currentStep++;
        
        // 튜토리얼 경로가 끝났는지 체크
        if (currentStep >= playerFixedPath.Count)
        {
            Debug.Log("모든 튜토리얼 이동을 완료했습니다!");
            // 여기서 튜토리얼 종료 처리나 다음 씬 전환 등을 호출할 수 있습니다.
            return;
        }

        // 다음 목표 지점이 바뀌었으니 이펙트(화살표 등)를 다시 그려줍니다.

    }

    public void Dialogue() {
        TutorialDialogue.Instance.AdvanceDialogue();
    }

}
