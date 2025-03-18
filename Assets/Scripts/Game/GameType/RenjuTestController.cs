using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RenjuTestController : IBaseGameTypeController
{
    List<string> _keysB;
    List<string> _keysW;
    int _index = -1;

    public void Initailize()
    {
        RenjuRuleLogic.InitializeTestPatterns();
        var testPatterns = RenjuRuleLogic.testPatterns.Keys;
        var testPatternsWhite = RenjuRuleLogic.testWhitePatterns.Keys;
        
        _keysB = new List<string>(testPatterns);
        _keysW = new List<string>(testPatternsWhite);
    }

    public void Operate(OperateCommand command)
    {
        _index++;
        if (_index >= _keysB.Count)
        {
            _index = 0;
        }
        var turnDataList = RenjuRuleLogic.testPatterns[_keysB[_index]];
        var turnDataWhiteList = RenjuRuleLogic.testWhitePatterns[_keysW[_index]];
        
        for(int i = 0; i < 14; i++)
        {
            for(int j = 0; j < 14; j++)
            {
                GameManager.Instance.matchController.OnDrawCell(new TurnData(){row=i, col=j}, Constants.CELL_TYPE.None);
            }
        }

        foreach (var turnData in turnDataList)
        {
            GameManager.Instance.matchController.OnDrawCell(turnData, Constants.CELL_TYPE.Black);
        }
        
        foreach (var turnData in turnDataWhiteList)
        {
            GameManager.Instance.matchController.OnDrawCell(turnData, Constants.CELL_TYPE.White);
        }

        GameManager.Instance.matchController.onNextTest();
    }

    public void Dispose()
    {
    }
}