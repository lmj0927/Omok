public enum OperateType
{
    None,
    Draw,
    Remove,
    DrawAll,
    RemoveAll,
}

public class OperateCommand
{
    public OperateType operateType;
    public TurnData turnData;
}
