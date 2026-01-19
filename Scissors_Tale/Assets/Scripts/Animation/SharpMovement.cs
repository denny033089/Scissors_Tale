using UnityEngine;
using System.Collections;

public class SharpMovement : MonoBehaviour
{
    public float interval = 0.5f;   // 각도가 바뀌는 시간 간격
    public float angleAmount = 15f; // 한 번에 꺾이는 각도 양

    void Start()
    {
        StartCoroutine(SwingStepByStep());
    }

    IEnumerator SwingStepByStep()
    {
        bool isLeft = true;
        while (true)
        {
            float targetZ = isLeft ? 15f : -15f; // 좌우 15도씩 번갈아
            transform.localRotation = Quaternion.Euler(0, 0, targetZ);
            
            isLeft = !isLeft;
            yield return new WaitForSeconds(interval);
        }
    }
}
