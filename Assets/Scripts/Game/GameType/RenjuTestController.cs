using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RenjuTestController : IBaseGameTypeController
{
    List<string> _keys;
    int _index = -1;

    public void Initailize()
    {
        RenjuRuleLogic.InitializeTestPatterns();
        var testPatterns = RenjuRuleLogic.testPatterns.Keys;
        _keys = new List<string>(testPatterns);
    }

    public void Operate(OperateCommand command)
    {
        _index++;
        if (_index >= _keys.Count)
        {
            _index = 0;
        }
        var turnDataList = RenjuRuleLogic.testPatterns[_keys[_index]];
        
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

        GameManager.Instance.matchController.onNextTest();
    }

    public void Dispose()
    {
    }
}