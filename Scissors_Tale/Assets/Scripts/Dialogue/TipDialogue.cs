using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;



public class TipDialogue : MonoBehaviour, IPointerDownHandler
{
    public static TipDialogue Instance { get; private set; }

    [Header("UI")]
    public GameObject DialoguePanel;
    public Text ScriptText_dialogue;

    [Header("Dialogue Data")]
    public string[] dialogue = {


    "몬스터가 가만히 공격을 당해주는 것은 아닙니다.",
    "타일에 빨간색 이펙트가 보인다면 그건 몬스터가 공격을 준비하고 있다는 뜻입니다",
    "몬스터의 공격을 피해서 몬스터를 정화해보세요!",
    "몬스터를 클릭하면 해당 몬스터의 공격범위를 알 수 있으니 잘해봐라",
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