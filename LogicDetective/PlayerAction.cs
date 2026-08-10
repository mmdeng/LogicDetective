namespace LogicDetective;

internal sealed class PlayerAction
{
    public PlayerActionType Type { get; }
    public Item FirstItem { get; }
    public Item SecondItem { get; }

    public PlayerAction(PlayerActionType type, Item firstItem, Item secondItem)
    {
        Type = type;
        FirstItem = firstItem ?? throw new ArgumentNullException(nameof(firstItem));
        SecondItem = secondItem ?? throw new ArgumentNullException(nameof(secondItem));
    }
}
