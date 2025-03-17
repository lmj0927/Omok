using System;

public interface IBaseGameTypeController
{
    public void Operate(OperateCommand command);
    void Dispose();
}