using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;



public class SkillDialogue : MonoBehaviour, IPointerDownHandler
{
    public static SkillDialogue Instance { get; private set; }

    [Header("UI")]
    public GameObject DialoguePanel;
    public Text ScriptText_dialogue;

    [Header("Dialogue Data")]
    public string[] dialogue = {


    "스킬버튼에 대해서 설명해드릴게요",
    "이동버튼을 누르면 해당 플레이 중인 플레이어의 스킬을 쓸 수 있게 됩니다",
    "스킬에 마우스 커서를 올려보면 스킬정보를 알 수 있어요",
    "스킬버튼을 누르고 공격 버튼을 누르면 캐릭터는 스킬을 사용합니다",
    "태그버튼을 누르면 사용할 수 있는 스킬도 다른 플레이어의 스킬로 바뀝니다",
    "그럼 스킬->태그->스킬 순으로 버튼을 누르면 한 턴에 두 명의 캐릭터 모두 스킬을 쓸수 있게 되는 것이죠",
    "스킬은 스테이지 당 한 번밖에 쓸 수 없으니 주의하세요!",
};






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

    public void OpenDialogue()
    {
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
        PrintCurrentLine();
        
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