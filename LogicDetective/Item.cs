namespace LogicDetective;

internal sealed class Item
{
    public int CategoryIndex { get; }
    public int Index { get; }
    public string Name { get; }

    public Item(int categoryIndex, int index, string name)
    {
        CategoryIndex = categoryIndex;
        Index = index;
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}