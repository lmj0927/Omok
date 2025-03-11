using System;

public class ReplayController : IBaseGameTypeController
{  
    MatchInfo _matchInfo;

    public void Initailize(int replayIndex)
    {
        //선택된 기보데이터를 읽어와서 게임을 시작함.
    }

    public MatchInfo GetMatchInfo()
    {
        return _matchInfo;
    }

    public void Operate()
    {
    }

    public void Dispose()
    {
    }

}