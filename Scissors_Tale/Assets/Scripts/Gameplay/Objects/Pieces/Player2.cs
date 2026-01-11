using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : Piece
{
    public override MoveInfo[] GetMoves()
    {
        // --- TODO ---

        return new MoveInfo[]
        {
            new MoveInfo(1, 0, 1),   // 오른쪽
            new MoveInfo(0, 1, 1),  // 위쪽
            new MoveInfo(-1, 0, 1), // 왼쪽
            new MoveInfo(0, -1, 1)   // 아래
        };
        // ------
    }
}
