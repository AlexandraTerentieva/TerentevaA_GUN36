public interface ISharedData
{
    GameEvent Event { get; set; }
    Cell Target { get; set; }
}

public enum GameEvent
{
    None,
    NewTurn,
    PerformMove,
    PerformAttack
}
