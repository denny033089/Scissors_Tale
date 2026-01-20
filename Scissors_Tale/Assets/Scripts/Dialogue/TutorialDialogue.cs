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
    "A: 앗! 저기, 저길 봐! 검은 조각이 달라붙어 있어!",
    "B: ...5x5 영역에서 돌아다니는 걸로 알려진 두더지네.",
    "A: 어쩌지? 검은 조각을 잘라내기에는 아직 너무 멀어, 숲속 친구들에게 더 가까이 다가가야 해!",
    "B: ...우리의 가위질이 닿는 곳은, 각자를 둘러싼 3x3 영역뿐이니까.",
    "게임은 5x5 보드판에서 이뤄집니다. A, B의 위치 기준 3x3 가위질 영역 안에 숲속 친구들이 들어와야 턴 종료 시 가위질을 통해 검은 조각을 제거할 수 있습니다.",
    "A: 우리의 가위질 영역 안에 두더지가 들어오면, 검은 조각을 한 개씩 잘라낼 수 있어! 도와줄 거지?",
    "B: ...그렇지만 한 번에 무한정 잘라낼 수 있는 건 아냐. 규칙이 있어.",
    "A: 우리가 이동하고, 가위질하고, 두더지가 이동하는 것까지가 세트로 묶여서, 한 턴!",
    "B: ...너는 우리를 상하좌우 한 칸씩만 이동시켜 주면 돼. 그러면 우리가 알아서 각자의 영역에 가위질을 시작할 거고, 가위질이 끝나면 두더지는 정해진 화살표를 따라 한 칸씩 움직일 거야.",
    "A와 B 둘 중 조작이 가능한 캐릭터를 골라 상하좌우 중 한 칸으로 이동할 수 있습니다. 턴 종료 버튼을 누르면 A와 B가 각자의 3x3 영역을 가위질하고, 가위질 영역 내의 숲속 친구들에게 달라붙은 검은 조각을 하나씩 잘라냅니다.",
    "A: 다만, 검은 조각들을 빨리 제거하지 않으면 우리의 마법 가위로도 손쓸 수 없게 되어 버릴지도 모르니까.",
    "B: ...구체적으로는 이 안에, 끝내주길 바라.",
    "남은 턴 안에 숲속 친구들에게 달라붙은 모든 검은 조각을 잘라내고 정화에 성공해야 스테이지를 클리어할 수 있습니다.",
    "A: 자! 그러면, 먼저 우리의 가위질 영역 안에 두더지가 들어오도록 해야겠지?",
    "B: ...이동 버튼을 누르면, A를 상하좌우 중 한 칸으로 이동할 수 있어. 지금은 오른쪽에 두더지가 있으니까, 그쪽으로 움직여 보자.",
    // 이동(stepcount == 1)
    "A: 잘했어!",
    "B: ...이제 이번 턴의 이동은 끝났으니까, 턴 종료 버튼을 누르면 돼.",
    // 턴 종료(stepcount == 2)
    "A: 한 번 더 이동해 볼까?",
    // 이동(stepcount == 3)
    "A: 좋아. 이제 이동이 끝났으니 턴 종료 버튼을...",
    "B: ...잠깐, A, 이대로라면 두더지에게 닿기도 전에 턴을 전부 써 버릴 거야. 두더지의 이동 경로를 보면 앞으로 내 쪽에 가까우니까, 다음 턴에는 내가 움직일 수 있게 해 줘.",
    "A: 앗, 정말이네!",
    "B: ...태그 버튼을 누르면, 다음 턴부터는 이동 버튼을 눌러서 이동하는 캐릭터가 A에서 나로 바뀌게 돼. 방금 말했듯이 남은 턴 수와 두더지의 이동 경로를 확인해서, 어느 캐릭터로 이동하는 것이 유리한지 판단해서 태그를 전략적으로 활용하도록 해.",
    "A: 참고로 태그 버튼을 누르면, 자동 턴 종료가 되니까 명심해 줘~",
    // 태그(stepcount == 4) + 턴 종료(stepcount == 5)
    "B: ...자, 그러면 이번 턴에는 나를 두더지를 향해 이동시켜 줘.",
    // 이동(stepcount == 6)
    "A: 좋아, 이제 다음 턴이면 B의 가위질이 두더지에 닿을 수 있겠어!",
    "B: ...맞아, 네가 전략을 잘 짜서 우리를 이동시켜 준 덕분이야. 이제 턴 종료를 눌러서 다음 턴으로 넘어가 보자.",
    // 턴 종료(stepcount == 7)
    "B: ...이번 턴에는 따로 말하지 않아도, 내가 어디로 움직여야 두더지가 가위질 영역에 들어오게 되는지 알겠지?",
    "A: 아래쪽으로 한 칸만 이동하면 돼! 해 보자!",
    // 이동(stepcount == 8)
    "B: ...이제 턴 종료를...",
    "A: 잠깐! 다음 턴에는 내가 움직이게 해 줘.",
    "A: ...그렇다고 하니까, 태그를 눌러서 다음 턴에는 A가 이동할 수 있게 해 주자.",
    // 태그(stepcount == 9) + 턴 종료(stepcount == 10)
    "A: 좋아! 이제 나도 아래쪽으로 한 칸만 이동하면 두더지에게 달라붙은 검은 조각을 잘라낼 수 있어! 그리로 이동시켜 줘,",
    // 이동(stepcount == 11)
    "A: 봐봐, 이렇게 나와 B의 가위질 영역이 겹친 곳에 숲속 친구들이 들어와 있는 경우에는, 기본적으로 내 가위질 한 번, B의 가위질 한 번으로 검은 조각 두 개를 자르는 거잖아?",
    "B: ...그렇지.",
    "A: 그런데 이럴 때, 태그를 사용하면 ‘태그 보너스’를 받아서 검은 조각 한 개를 더 잘라낼 수 있어! 그러니까 총 세 개의 검은 조각을 한 턴만에 잘라낼 수 있는 거야. 대단하지?",
    "B: ...턴 수에 제한이 있으니까, 숲속 친구들이 우리 가위질 영역에 최대한 겹치도록 전략적으로 이동한 다음, 태그 보너스를 활용해 한 번에 최대한 많은 검은 조각을 잘라낸다.",
    "A: 바로 그거야! 자, 그러면 태그해 볼까?",
    // 태그(stepcount == 12) + 턴 종료(stepcount == 13)
    "A: 야호! 한 번에 검은 조각을 세 개나 잘라냈어. 이게 다 네가 우리를 전략적으로 이동시켜 준 덕분이야!",
    "B: ...다만 태그 보너스를 받기 위해서는 어쨌든 태그 버튼을 눌러야 하는 만큼, 다음 턴에는 반드시 다른 캐릭터로 이동하게 된다는 리스크가 있다는 점은 명심해 둬.",
    "A: 맞아! 그치만 너는 똑똑하니까, 문제없을 거야!",
    "B: ...그럼, 계속해 볼까. 이제 검은 조각이 얼마 남지 않았어. 조금만 더 힘내 줘.",
    // 이동(stepcount == 14)
    "B: ...두더지에게 달라붙어 있는 남은 검은 조각 개수가 3개니까, 이번 턴에도 태그 보너스를 활용하면 검은 조각을 완전히 제거할 수 있을 거야.",
    // 태그(stepcount == 15) + 턴 종료(stepcount == 16)
    "A: 야호! 두더지를 정화하는 데 성공했어. 이제 다시 숲속에서 자유롭게 놀 수 있을 거야.",
    "B: ...멋진 전략을 보여 줘서 고마워. 다 네 덕분이야.",
    "A: 앞으로도 지금처럼 우리를 이동하고, 태그시켜서, 남은 숲속 친구들도 모두 정화할 수 있도록 도와줄 거지?",
    "B: ...그럼, 이제 다음 구역으로 넘어가 볼까." };


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
            if (dialogue_count <= 14)
            {
                PrintCurrentLine();
            }
            else
            {
                CloseDialogue();
            }
        }
        else if (stepcount == 1)
        {
            if (dialogue_count <= 16)
            {
                PrintCurrentLine();
            }
            else
            {
                CloseDialogue();
            }
        }
        else if (stepcount == 2)
        {
            CloseDialogue();
        }
        else if (stepcount == 3)
        {
            if (dialogue_count <= 22)
            {
                PrintCurrentLine();
            }
            else
            {
                CloseDialogue();
            }
        }
        else if (stepcount == 5)
        {
            CloseDialogue();
        }
        else if (stepcount == 6)
        {
            if (dialogue_count <= 25)
            {
                PrintCurrentLine();
            }
            else
            {
                CloseDialogue();
            }
        }
        else if (stepcount == 7)
        {
            if (dialogue_count <= 27)
            {
                PrintCurrentLine();
            }
            else
            {
                CloseDialogue();
            }
        }
        else if (stepcount == 8)
        {
            if (dialogue_count <= 30)
            {
                PrintCurrentLine();
            }
            else
            {
                CloseDialogue();
            }
        }
        else if (stepcount == 10)
        {
           CloseDialogue();
        }
        else if (stepcount == 11)
        {
            if (dialogue_count <= 36)
            {
                PrintCurrentLine();
            }
            else
            {
                CloseDialogue();
            }
        }
        else if (stepcount == 13)
        {
            if (dialogue_count <= 40)
            {
                PrintCurrentLine();
            }
            else
            {
                CloseDialogue();
            }
        }
        else if (stepcount == 14)
        {
            CloseDialogue();
        }
        else if (stepcount == 16)
        {
            if (dialogue_count <= 46)
            {
                PrintCurrentLine();
            }
            else
            {
                CloseDialogue();
            }
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
