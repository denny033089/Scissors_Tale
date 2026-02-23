using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class synopsis : MonoBehaviour, IPointerDownHandler
{
    public GameObject Player1;
    public GameObject Player2;

    public Text ScriptText_dialogue;
    public string[] dialogue = { "검은 조각.",
"달라붙은 생명체를 분노에 휩싸이게 만든다는 무시무시한 저주.",
"이 검은 조각이, 엘리와 소피가 사는 숲에도 퍼져 나가기 시작하는데...",
"(숲 속)",
"엘리 : 얍! 이걸로, 하나 더 정화 완료!",
"소피 : ...우리가 움직이는 만큼, 몬스터들도 턴에 맞춰 움직이니 쉽지 않네.",
"엘리 : 그래도 마법 가위가 있어서 다행이야.",
"소피 : ...우리 가까이에 있는 검은 조각만 잘라낼 수 있다는 점이 문제지만.",
"엘리 : 앗, 저기 또 있다! 저기!", };


    public int dialogue_count = 0;

    void Start()
    {
        Player1.SetActive(false);
        Player2.SetActive(false);
    }

    public void OnPointerDown(PointerEventData data)
    {
        Player1.SetActive(false);
        Player2.SetActive(false);

        dialogue_count++;
        Debug.Log(dialogue_count);

        if (dialogue_count == 9)
        {
            Debug.Log("대화 종료");
            SceneManager.LoadScene("Tutorial");
            GameSystemManager.Instance.ChangeGameState(Enums.GameState.InGame); //01.25 정수민 추가
            dialogue_count = 0;
        }

        ScriptText_dialogue.text = dialogue[dialogue_count];



        if (dialogue_count > 3 && dialogue_count % 2 == 0) //02.05. 김동균 수정
        {
            Player1.SetActive(true);
            Player2.SetActive(false);
        }

        else if (dialogue_count > 3 && dialogue_count % 2 == 1) //02.05. 김동균 수정
        {
            Player1.SetActive(false);
            Player2.SetActive(true);
        }

    }
}

// public class meet_the_characters : MonoBehaviour, IPointerDownHandler
// {
//     public Text ScriptText_dialogue;
//     public string[] dialogue = { "A: 앗, 안녕! 사람을 보는 건 처음인데! 나는 A라고 해. 반가워!",
//     "B: ...안녕, 나는 A의 친구 B,",
//     "A: 긴 어떻게 찾아온 거야? 다친 데는 없어? 다닐 만해?",
//     "B: ...최근 나타나기 시작한 검은 조각들 때문에 함부로 숲속을 돌아다니면 위험해.",
//     "A: 그래도 괜찮아! 이 마법 가위만 있으면, 얼마든지 검은 조각들을 잘라낼 수 있으니까. 만일 검은 조각이 달라붙는다고 해도, 내가 바로 잘라내 줄게! 걱정하지 마!",
//     "B: ...아무렇게나 잘라낼 수 있는 건 아니지만 말이야.",
//     "A: 검은 조각이 붙은 채로 가만히 있으면 큰일 나니까, 꼭 말해줘야 해?",
//     "B: ...저기."
//     };
//     public int dialogue_count = 0;

//     public void OnPointerDown(PointerEventData data)
//     {
//         dialogue_count++;
//         Debug.Log(dialogue_count);

//         if (dialogue_count == 8)
//         {
//             Debug.Log("대화 종료");
//             dialogue_count = 0;
//         }

//         ScriptText_dialogue.text = dialogue[dialogue_count];
//     }
// }