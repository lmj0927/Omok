public enum OperateType
{
    None,
    Draw,
    Remove,
}

public class OperateCommand
{
    public OperateType operateType;
    public TurnData turnData;
}
