namespace LogicDetective;

internal sealed class Category
{
    public int Index { get; }
    public string Name { get; }
    public IReadOnlyList<string> Items { get; }

    public Category(int index, string name, IReadOnlyList<string> items)
    {
        Index = index;
        Name = name;
        Items = items;
    }
}