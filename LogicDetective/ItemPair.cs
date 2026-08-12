namespace LogicDetective;

internal sealed class ItemPair
{
    public Item Item1 { get; set; }
    public Item Item2 { get; set; }

    public ItemPair(Item item1, Item item2)
    {
        Item1 = item1 ?? throw new ArgumentNullException(nameof(item1));
        Item2 = item2 ?? throw new ArgumentNullException(nameof(item2));
    }

    public bool Equal(ItemPair itemPair)
    {
        if (!Item1.Equal(itemPair.Item1)) return false;
        if (!Item2.Equal(itemPair.Item2)) return false;
        return true;
    }
    public override string ToString()
    {
        return $"{Item1} X {Item2}";
    }
    public Item Get1(bool first)
    {
        if (first) return Item1;
        else return Item2;
    }
    public Item Get2(bool first)
    {
        if (first) return Item2;
        else return Item1;
    }
}
