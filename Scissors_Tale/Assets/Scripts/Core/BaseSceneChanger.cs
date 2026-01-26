using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//<summary>
//scene들을 바꾸는 메소드와 scene들을 바꿀 때 나올 효과를 위한 기본 클래스
//<summary>
public class BaseSceneChanger : MonoBehaviour
{
    //public scene 바꾸기
    //private 씬 로드

    //이동할 씬들 목록
    /// 0: stageselection
    /// 1: tutorialscene(synopsis)
    /// 2: stagescene
    /// 3: Mainmenu
    /// 4: 그밖의 것들
    public string[] nextScenes;

    public CanvasGroup transitionUI;                //transitionUI
    public GraphicRaycaster graphicRaycaster;       //transition UI의 graphicRaycaster

    protected virtual void Awake()
    {
        //fadein 실행 안하고 싶으면 Awake 함수 override
        StartCoroutine(SceneEnter(0.5f));
    }

    //외부에서 호출하는 씬 바꾸기
    protected void ChangeScene(string _sceneName, float fadeTime = 0.5f)
    {
        StartCoroutine(SceneExit(_sceneName, fadeTime));
    }

    //씬 나가면
    IEnumerator SceneExit(string _sceneName, float fadeTime)
    {
        //01.25 fadeout 구문
        //graphicRaycaster.enabled = true;
        //StartCoroutine(FadeOutCo(fadeTime));

        //fade 시간 기다려주고 씬바꾸기
        yield return new WaitForSecondsRealtime(fadeTime + 0.1f);
        //게임 정지 풀기
        Time.timeScale = 1f;
        SceneManager.LoadScene(_sceneName);
    }

    //씬 들어오면
    IEnumerator SceneEnter(float fadeTime)
    {
        //01.25 정수민
        //graphicRaycaster.enabled = true;
        //transitionUI.alpha = 1f;
        //StartCoroutine(FadeInCo(fadeTime));

        //fade 시간 기다려주기
        yield return new WaitForSecondsRealtime(fadeTime + 0.1f);
        //graphicRaycaster.enabled = false;
    }
    
    //fadeTime(초) 동안 투명 -> 불투명
    IEnumerator FadeOutCo(float fadeTime)
    {
        float elaspedTime = 0f;

        while(elaspedTime <= fadeTime)
        {
            elaspedTime += Time.deltaTime;
            transitionUI.alpha = Mathf.Lerp(0, 1, elaspedTime / fadeTime);
            yield return null;
        }

        transitionUI.alpha = 1f;
    }


    //fadeTime(초) 동안 붍투명 -> 투명
    IEnumerator FadeInCo(float fadeTime)
    {
        float elaspedTime = 0f;

        while(elaspedTime <= fadeTime)
        {
            elaspedTime += Time.deltaTime;
            transitionUI.alpha = Mathf.Lerp(1, 0, elaspedTime / fadeTime);
            yield return null;
        }

        transitionUI.alpha = 0f;
    }
    

    //LoadSceneAsync 위한 코루틴
    /*
    private IEnumerator SceneLoading()
    {
        
    }
    */
}

