using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickHandler : MonoBehaviour
{
        
    ///movestate에서 player1을 클릭하면 이동 effect가 나오고, (공격범위도 나오나) 이동가능
    /// 
    /// tagstate에서 player2를 클릭하면 이동 effect가 나오고, 이동가능 
    private Piece selectedPiece = null; // 지금 선택된 Piece

    private (int,int) BoardPos; //이동할 보드 세계의 좌표(targetpos)
    private Vector3 dragOffset;
    private Vector3 originalPosition;

    // 마우스의 위치를 (int, int) 좌표로 보정해주는 함수
    private (int, int) GetBoardPosition(Vector3 WorldPosition)
    {
        float x = WorldPosition.x + (Utils.TileSize * Utils.FieldWidth) / 2f;
        float y = WorldPosition.y + (Utils.TileSize * Utils.FieldHeight) / 2f;
        
        int boardX = Mathf.FloorToInt(x / Utils.TileSize);
        int boardY = Mathf.FloorToInt(y / Utils.TileSize);
        
        return (boardX, boardY);
    }

    //마우스의 위치 실시간 반환
    public void OnLook(InputAction.CallbackContext context) { 

        Vector3 ScreenInput = context.ReadValue<Vector2>();
        Vector3 WorldPos = Camera.main.ScreenToWorldPoint(new Vector3(ScreenInput.x, ScreenInput.y, 10f));
        BoardPos = GetBoardPosition(WorldPos);
        //Debug.Log($"마우스 위치: {ScreenInput} -> 월드 위치: {WorldPos} -> 보드 좌표: {BoardPos}");
    }

    //이동할 보드를 클릭하는 입력을 받는 함수
    //보드 세계 좌표(boardpos)를 클릭 후 이동, selectedPiece 선택된 피스
    public void OnMove(InputAction.CallbackContext context) {
        if(GameManager.Instance.CurrentTurnState == Enums.TurnState.PlayerMove) {

            selectedPiece = GameManager.Instance.GetActivatePlayer();
            if (context.performed) {
                Debug.Log($"BoardPos = {BoardPos}");
                if(GameManager.Instance.IsValidMove(selectedPiece,BoardPos)) { 
                    GameManager.Instance.MovePlayer(selectedPiece,BoardPos);
                }
                GameManager.Instance.ClearEffects();
            }
        }
    }    
}

