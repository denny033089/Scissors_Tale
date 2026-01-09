using UnityEngine;

public class Piece : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        // abstract를 지웠다면 virtual을 추가해야 자식이 override 할 수 있습니다.
    public virtual MoveInfo[] GetMoves() 
    {
    // 기본적으로는 빈 배열을 반환하거나 기본 로직을 넣습니다.
        return new MoveInfo[0]; 
    }
    //public MoveInfo[] GetMoves();
}
