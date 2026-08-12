namespace LogicDetective;

internal sealed class Item
{
    public int CategoryIndex { get; }
    public int ItemIndex { get; }
    public string Name { get; }

    public Item(int categoryIndex, int itemIndex, string name)
    {
        CategoryIndex = categoryIndex;
        ItemIndex = itemIndex;
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }

    public bool Equal(Item item)
    {
        if (CategoryIndex != item.CategoryIndex) return false;
        if (ItemIndex != item.ItemIndex) return false;
        if (Name != item.Name) return false;
        return true;
    }

    public bool IsSame(Item item)
    {
        return CategoryIndex == item.CategoryIndex && ItemIndex == item.ItemIndex;
    }

    /// <summary>
    /// カテゴリインデックスとアイテムインデックスを比較して、順序を決定する。
    /// カテゴリインデックスが同じならアイテムインデックスで比較する。
    /// </summary>
    /// <param name="item">比較対象</param>
    /// <returns></returns>
    public int Compare(Item item)
    {
        var categoryCompare = CategoryIndex.CompareTo(item.CategoryIndex);
        if (categoryCompare != 0) return categoryCompare;

        return ItemIndex.CompareTo(item.ItemIndex);
    }
}