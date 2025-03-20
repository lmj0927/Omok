using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AIController : IBaseGameTypeController
{
    private MCTS _mctsAI;
    
    private readonly Dictionary<string, int> iterations = new Dictionary<string, int>
    {
        { "ai1", 1000 },
        { "ai2", 10000 },
        { "ai3", 50000 }
    };
    
    public void Initailize()
    {
        Debug.Log("AIController::Initailize");
        int tier = GameManager.Instance.GetUserInfo().tier;
        _mctsAI = new MCTS();
        _mctsAI.onSearchComplete += (turnData) => {
            GameManager.Instance.matchController.SetTurn(turnData.row, turnData.col);
        };
    }

    public void Operate(OperateCommand command)
    {
        _ = _mctsAI.RunSearch(command.turnData.row, command.turnData.col);        
    }

    public void SetAILevel(string modelName)
    {
        _mctsAI.SetIterations(iterations[modelName]);
    }

    public void Dispose()
    {
        _mctsAI = null;
    }
    
}