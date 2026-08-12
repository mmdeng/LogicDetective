namespace LogicDetective;

internal enum ClueComparisonOperator
{
    LessThan,      // <
    LessThanOrEqual,  // <=
    GreaterThan,   // >
    GreaterThanOrEqual  // >=
}

internal sealed class ClueComparison : Clue
{
    public Category CategoryToCompare { get; }
    public int ItemIndexToCompare { get; }
    public ClueComparisonOperator Operator { get; }

    public ClueComparison(Item item1, Item item2, Category category, int itemIndexToCompare, ClueComparisonOperator op)
        : base(item1, item2)
    {
        CategoryToCompare = category;
        ItemIndexToCompare = itemIndexToCompare;
        Operator = op;
    }

    public override string ToString()
    {
        var op = Operator switch
        {
            ClueComparisonOperator.LessThan => "より小さい",
            ClueComparisonOperator.LessThanOrEqual => "以下",
            ClueComparisonOperator.GreaterThan => "より大きい",
            ClueComparisonOperator.GreaterThanOrEqual => "以上",
            _ => "?"
        };
        return $"{Pair.Item1.Name}の{CategoryToCompare.Name}は{Pair.Item2.Name}の{CategoryToCompare.Name}{op}です。";
    }
    public bool SatisfiesComparisonClue(Answer answer)
    {
        // どのグループに属しているかを確認
        // 同じ人物カテゴリのアイテム同士の場合は処理できない
        if (Pair.Item1.CategoryIndex == Pair.Item2.CategoryIndex)
        {
            return true; // 同一カテゴリの比較は無意味なので常に true
        }

        var itemGroup1 = FindGroupForItem(answer, Pair.Item1.CategoryIndex, Pair.Item1.ItemIndex);
        if (itemGroup1 < 0) return true;

        var itemGroup2 = FindGroupForItem(answer, Pair.Item2.CategoryIndex, Pair.Item2.ItemIndex);
        if (itemGroup2 < 0) return true;

        // 比較するカテゴリのアイテムインデックスを取得
        var compareItemIndex1 = answer.GetItemIndex(itemGroup1, ItemIndexToCompare);
        var compareItemIndex2 = answer.GetItemIndex(itemGroup2, ItemIndexToCompare);

        // 比較演算子を適用
        return Operator switch
        {
            ClueComparisonOperator.LessThan => compareItemIndex1 < compareItemIndex2,
            ClueComparisonOperator.LessThanOrEqual => compareItemIndex1 <= compareItemIndex2,
            ClueComparisonOperator.GreaterThan => compareItemIndex1 > compareItemIndex2,
            ClueComparisonOperator.GreaterThanOrEqual => compareItemIndex1 >= compareItemIndex2,
            _ => true
        };
    }
    private static int FindGroupForItem(Answer answer, int categoryIndex, int itemIndex)
    {
        for (var group = 0; group < answer.ItemCount; group++)
        {
            if (answer.GetItemIndex(group, categoryIndex) == itemIndex)
                return group;
        }
        return -1;
    }
}
