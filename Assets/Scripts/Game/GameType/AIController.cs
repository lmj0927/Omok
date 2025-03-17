using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AIController : IBaseGameTypeController
{
    private MCTS _mctsAI;
    
    public void Initailize()
    {
        Debug.Log("AIController::Initailize");
        _mctsAI = new MCTS();
        _mctsAI.onSearchComplete += (turnData) => {
            GameManager.Instance.matchController.SetTurn(turnData.row, turnData.col);
        };
    }

    public void Operate(OperateCommand command)
    {
        _ = _mctsAI.RunSearch(command.turnData.row, command.turnData.col);        
    }

    public void Dispose()
    {
        _mctsAI = null;
    }
    
}