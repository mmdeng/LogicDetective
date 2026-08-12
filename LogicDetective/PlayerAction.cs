namespace LogicDetective;

internal sealed class PlayerAction
{
    public ReasoningState Type { get; }
    public ItemPair Pair { get; }

    public PlayerAction(ReasoningState type, Item item1, Item item2) : this(type, new ItemPair(item1, item2)) { }
    public PlayerAction(ReasoningState type, ItemPair itemPair)
    {
        Type = type;
        Pair = itemPair;
    }
}
