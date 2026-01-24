using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 싱글톤 패턴을 구현한 제네릭 클래스
/// <para>게임 전체 전역 싱글톤</para>
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual bool DontDestroy => true;

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (DontDestroy)
            {
                DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음

                // 01.24 정수민 [추가] 씬 로드 이벤트 구독
                SceneManager.sceneLoaded += OnSceneLoadedInternal;
            }
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    //01.24 정수민 씬 변경 시 초기화시킬 수 있도록
    // [추가] 씬이 로드될 때 유니티가 호출해주는 내부 함수
    private void OnSceneLoadedInternal(Scene scene, LoadSceneMode mode)
    {
        OnSceneLoaded(scene.name);
    }

    // [추가] 자식 클래스(GameManager 등)에서 이 함수를 override해서 초기화 로직을 짭니다.
    protected virtual void OnSceneLoaded(string sceneName)
    {
        // 자식 클래스에서 구현
    }

    // [추가] 오브젝트 파괴 시 이벤트 구독 해제 (메모리 누수 방지)
    protected virtual void OnDestroy()
    {
        if (DontDestroy)
        {
            SceneManager.sceneLoaded -= OnSceneLoadedInternal;
        }
    }
}