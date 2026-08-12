namespace LogicDetective;

internal readonly record struct PairKey
{
    public readonly int CategoryIndex1 { get; }
    public readonly int ItemIndex1 { get; }
    public readonly int CategoryIndex2 { get; }
    public readonly int ItemIndex2 { get; }

    public PairKey(Item item1, Item item2)
    {
        CategoryIndex1 = item1.CategoryIndex;
        ItemIndex1 = item1.ItemIndex;
        CategoryIndex2 = item2.CategoryIndex;
        ItemIndex2 = item2.ItemIndex;
    }
}