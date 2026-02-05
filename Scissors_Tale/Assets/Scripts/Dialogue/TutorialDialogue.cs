using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;



public class TutorialDialogue : MonoBehaviour, IPointerDownHandler
{
    public static TutorialDialogue Instance { get; private set; }

    [Header("UI")]
    public GameObject DialoguePanel;
    public Text ScriptText_dialogue;

    [Header("Dialogue Data")]
    public string[] dialogue = {


    "엘리 : 검은 조각이 달라붙은 두더디야!",
    "소피 : ...우선 마법 가위의 공격 범위가 두더디에 닿도록 한 칸씩 이동해 보자.",
    "엘리 : 오케이. 이동 버튼을 누르고, 오른쪽 칸을 클릭!",
    // 이동(stepcount == 1)
    // 이동 완료 stepcount ==2
    "엘리 : 잘했어!",
    "소피 : ...한 턴에는 한 칸씩만 이동할 수 있으니까.",
    "엘리 : 이제 턴 종료 버튼을 눌러서, 한 번씩 마법 가위로 공격해 보자!",
    // 턴 종료(stepcount == 3)
    "엘리 : 아직 두더디와 너무 멀어. 턴 종료 시점에 우리 주변의 색칠된 마법 가위의 공격 범위 안에 두더디가 들어오도록 해야 해.",
    "소피 : ...저기 몬스터에서 나오는 화살표가 다음 두 턴 간의 이동 경로니까. 그걸 고려해서 이동해 보자.",
    "엘리 : 일단은, 이동 버튼을 눌러서 오른쪽으로 한 칸만 더 이동해 볼까?",
    // 이동(stepcount == 4)
    // 이동 완료 stepcount ==5
    "엘리 : 좋아. 이제 턴 종료 버튼을 눌러서 공격을...",
    "소피 : ...잠깐, 엘리, 두더지의 이동 경로를 보면 앞으로 내 쪽에 가까워지니까, 다음 턴에는 내가 이동하는 게 좋겠어.",
    "엘리 : 앗, 정말이네! 네가 이동하려면 어떻게 해야 하지?",
    "소피 : ...태그 버튼을 누르면, 다음 턴에 이동할 캐릭터를 바꿀 수 있잖아.",
    "엘리 : 아하, 그랬었지!",
    "소피 : ...그럼 이제 태그 버튼을 누르고, 턴 종료 버튼까지 눌러 보자.",
    // 태그(stepcount == 6) + 턴 종료(stepcount == 7)
    "엘리 : 확실히 태그 버튼을 누르는 건, 다음 턴의 이동을 고려해 전략적으로 선택해야겠네.",
    "소피 : ...맞아. 자, 그러면 나도 이동 버튼을 눌러서 오른쪽으로 한 칸 이동해 볼까.",
    // 이동(stepcount == 8)
    // 이동 완료 stepcount==9
    "엘리 : 좋아, 이제 다음 턴이면 소피의 마법 가위의 공격 범위에 두더디가 들어오겠어!",
    "소피 : ...맞아, 이제 턴 종료 버튼을 눌러서 다음 턴으로 넘어가 보자.",
    // 턴 종료(stepcount == 10)
    "소피 : ...이번 턴에는 이동 버튼을 누르고, 아래로 한 칸 이동해 보자.",
    "엘리 : 맞아. 그러면 턴 종료 시점에 두더디의 검은 조각을 하나 잘라낼 수 있을 거야!",
    // 이동(stepcount == 11)
    // 이동완료 stepcount ==12
    "소피 : 이제 턴 종료 버튼을 누르고 공격을...",
    "엘리 : 잠깐! 몬스터의 이동 경로를 보면, 다음 턴에는 내가 이동하는 게 나을 것 같아.",
    "소피 : ...좋아, 그러면 아까 했던 것처럼, 태그 버튼을 누르고 턴 종료 버튼을 누르면 되겠지?",
    // 태그(stepcount == 13) + 턴 종료(stepcount == 14)
    "엘리 : 야호! 공격 성공!",
    "소피 : ...아직 잘라내야 할 검은 조각이 여섯 개나 남았으니까. 턴 제한도 있고. 방심해서는 안 돼.",
    "엘리 : 알겠어! 그러면 최대한 빠르게 검은 조각을 잘라내기 위해, 너와 내 마법 가위의 공격 범위가 겹치는 곳에 몬스터가 들어오도록 이동해야겠다.",
    "소피 : ...좋은 생각이야.",
    "엘리 : 이동 버튼을 눌러서, 아래쪽으로 한 칸 이동해 볼까?",
    // 이동(stepcount == 15)
    // 이동완료 stepcount ==16
    "엘리 : 이렇게 나와 소피의 마법 가위의 공격 범위가 겹치는 곳에 몬스터가 들어와 있는 경우에 턴을 종료하면, 내 공격 한 번, 소피의 공격 한 번. 검은 조각 두 개를 자를 수 있는 거 맞지?",
    "소피 : ...그런데, 검은 조각을 한 개 더 잘라낼 수 있는 방법이 있어.",
    "엘리 : 정말? 그게 뭐야?",
    "소피 : ...태그 버튼을 눌러서 ‘태그 보너스’를 받으면, 검은 조각 한 개를 더 잘라낼 수 있어.",
    "엘리 : 그러면 총 세 개의 검은 조각을 한 턴만에 잘라낼 수 있는 거네? 짱이다!",
    "소피 : ...턴 수에 제한이 있으니까, 태그 보너스를 활용해 한 턴에 최대한 검은 조각을 많이 잘라내는 게 좋겠지.",
    "엘리 : 자, 그러면 태그 버튼을 누르고 턴 종료 버튼을 눌러서, 태그 보너스를 받아 볼까?",
    // 태그(stepcount == 17) + 턴 종료(stepcount == 18)
    "엘리 : 야호! 한 번에 검은 조각을 세 개나 잘라냈다!",
    "소피 : ...다만 태그 보너스를 받기 위해서는 어쨌든 태그 버튼을 눌러야 하잖아?",
    "엘리 : 앗, 그러면 다음 턴에는 원하지 않았더라도 다른 캐릭터를 이동시키게 된다는 리스크가 있겠네!",
    "소피 : ...맞아. 그것까지도 전략적으로 고려해 선택하길 바라.",
    "엘리 : 알겠어!",
    "소피 : ...자, 이제 거의 다 왔어. 이동 버튼을 눌러서 오른쪽으로 한 칸 이동해 보자.",
    // 이동(stepcount == 19)
    // 이동완료 stepcount ==20
    "소피 : ...이번 턴에도 태그 보너스를 활용해 공격하면 남은 검은 조각 3개를 완전히 잘라낼 수 있을 거야.",
    "엘리 : 오케이! 그러면 태그 버튼을 누르고 턴 종료 버튼을 눌러 보자.",
    // 태그(stepcount == 21) + 턴 종료(stepcount == 22)
    // 이 부분 수정 필요함.
    "엘리 : 야호! 정화 완료!",
    "소피 : ...훌륭한 전략이었어.",
    "엘리 : 앞으로도 이동과 태그를 적절히 활용해서, 검은 조각들을 잘라내 보자!",
    "소피 : ...화이팅.",
     // 여기서 스테이지 클리어가 나오도록 한다.
};


    // stepcount == 23에서 끝나도록 바꿔야 할 것 같음. 




    private int dialogue_count = 0;
    private int stepcount = -1;
    private bool isOpen = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DialoguePanel.SetActive(false);
    }

    private void Start()
    {

    }

    public void OpenDialogue(int step)
    {
        stepcount = step;
        isOpen = true;

        DialoguePanel.SetActive(true);

        PrintCurrentLine();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isOpen) return;

        AdvanceDialogue();
    }

    private void AdvanceDialogue()
    {
        dialogue_count++;

        if (stepcount == 0)
        {
            if (dialogue_count <= 2) PrintCurrentLine();
            else CloseDialogue();
        }
        else if (stepcount == 2)
        {
            if (dialogue_count <= 5) PrintCurrentLine();
            else CloseDialogue();
        }
        else if (stepcount == 3)
        {
            if (dialogue_count <= 8) PrintCurrentLine();
            else CloseDialogue();
        }
        else if (stepcount == 5)
        {
            if (dialogue_count <= 14) PrintCurrentLine();
            else CloseDialogue();
        }
        else if (stepcount == 7)
        {
            if (dialogue_count <= 16) PrintCurrentLine();
            else CloseDialogue();
        }
        else if (stepcount == 9)
        {
            if (dialogue_count <= 18) PrintCurrentLine();
            else CloseDialogue();
        }
        else if (stepcount == 10)
        {
            if (dialogue_count <= 20) PrintCurrentLine();
            else CloseDialogue();
        }
        else if (stepcount == 12)
        {
            if (dialogue_count <= 23) PrintCurrentLine();
            else CloseDialogue();
        }
        else if (stepcount == 14)
        {
            if (dialogue_count <= 28) PrintCurrentLine();
            else CloseDialogue();
        }
        else if (stepcount == 16)
        {
            if (dialogue_count <= 35) PrintCurrentLine();
            else CloseDialogue();
        }
        else if (stepcount == 18)
        {
            if (dialogue_count <= 41) PrintCurrentLine();
            else CloseDialogue();
        }
        else if (stepcount == 20)
        {
            if (dialogue_count <= 43) PrintCurrentLine();
            else CloseDialogue();
        }
        else if (stepcount == 22)
        {
            if (dialogue_count <= 47) PrintCurrentLine();
            else CloseDialogue();
        }
    }

    private void PrintCurrentLine()
    {
        if (dialogue_count < 0 || dialogue_count >= dialogue.Length)
        {
            CloseDialogue();
            return;
        }

        ScriptText_dialogue.text = dialogue[dialogue_count];
    }

    private void CloseDialogue()
    {
        isOpen = false;
        DialoguePanel.SetActive(false);

        TutorialManager.Instance.ReturnToGame();
    }

}