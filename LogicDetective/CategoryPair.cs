namespace LogicDetective;

internal sealed class CategoryPair
{
    public Category Category1 { get; set; }
    public Category Category2 { get; set; }

    public CategoryPair(Category category1, Category category2)
    {
        Category1 = category1 ?? throw new ArgumentNullException(nameof(category1));
        Category2 = category2 ?? throw new ArgumentNullException(nameof(category2));
    }
    public override string ToString()
    {
        return $"{Category1} x {Category2}";
    }
    public Category Get1(bool first)
    {
        if (first) return Category1;
        else return Category2;
    }
    public Category Get2(bool first)
    {
        if (first) return Category2;
        else return Category1;
    }
}
