using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance; // 어디서든 접근 가능하도록 싱글톤 설정

    void Awake() {
        Instance = this;
    }

    // 화면을 흔드는 메인 함수
    public void ShakeCamera(float duration = 0.2f, float strength = 0.1f) {
        // DOShakePosition(지속시간, 강도, 진동횟수, 무작위성)
        // 기존에 진행 중인 흔들림이 있다면 완료하고 새로 시작하기 위해 정지 후 실행 권장
        Camera.main.transform.DOComplete(); 
        Camera.main.transform.DOShakePosition(duration, strength, 20, 90, false, true);
    }
}
