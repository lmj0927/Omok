using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : IBaseGameTypeController
{
    private MCTS _mctsAI;
    
    
    public void Initailize()
    {
        Debug.Log("AIController::Initailize");
        _mctsAI = new MCTS();
    }

    async public void Operate(int row, int col)
    {
        var bestMove = _mctsAI.RunSearch(row, col);
        GameManager.Instance.matchController.SetTurn(bestMove.Item1, bestMove.Item2);
    }
    
    public void Operate()
    {

    }

    public void Dispose()
    {

    }
    
}