namespace LogicDetective;

internal sealed class Category
{
    public int Index { get; }
    public string Name { get; }
    public IReadOnlyList<Item> Items { get; }

    public Category(int index, string name, IReadOnlyList<Item> items)
    {
        Index = index;
        Name = name;
        Items = items;
    }
    public Category(int index, string name, IReadOnlyList<string> items)
    {
        Index = index;
        Name = name;
        Items = [.. items.Select((item, i) => new Item(index, i, item))];
    }
    public Item GetItem(string name)
    {
        return Items.Single(m => m.Name == name);
    }
    public override string ToString()
    {
        return Name;
    }
}