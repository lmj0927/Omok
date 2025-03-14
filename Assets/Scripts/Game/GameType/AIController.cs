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

    async public void Operate(OperateCommand command)
    {
        var bestMove = await _mctsAI.RunSearch(command.turnData.row, command.turnData.col);
        GameManager.Instance.matchController.SetTurn(bestMove.Item1, bestMove.Item2);
    }

    public void Dispose()
    {

    }
    
}