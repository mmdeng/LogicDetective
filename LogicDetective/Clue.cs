namespace LogicDetective;

internal abstract class Clue
{
    public ItemPair Pair { get; }

    protected Clue(Item item1, Item item2)
    {
        Pair = new ItemPair(item1, item2);
    }

    protected Clue(ItemPair itemPair)
    {
        Pair = itemPair;
    }
}

internal sealed class SameClue : Clue
{
    public SameClue(Item item1, Item item2) : base(item1, item2)
    {
    }

    public SameClue(ItemPair itemPair) : base(itemPair)
    {
    }

    public override string ToString()
    {
        return $"{Pair.Item1.Name} {Pair.Item2.Name}は同じ組です。";
    }
}

internal sealed class DifferentClue : Clue
{
    public DifferentClue(Item item1, Item item2) : base(item1, item2)
    {
    }

    public DifferentClue(ItemPair itemPair) : base(itemPair)
    {
    }

    public override string ToString()
    {
        return $"{Pair.Item1.Name} {Pair.Item2.Name}は同じ組ではありません。";
    }
}

internal enum ConditionOperator
{
    Equal,
    NotEqual
}

internal sealed class ConditionalClue : Clue
{
    public Item ConditionItem2 { get; }
    public ConditionOperator ConditionOperator { get; }
    public Item ResultItem { get; }
    public ConditionOperator ResultOperator { get; }

    public ConditionalClue(
        Item conditionItem1,
        Item conditionItem2,
        ConditionOperator conditionOperator,
        Item resultItem1,
        Item resultItem2,
        ConditionOperator resultOperator)
        : base(conditionItem1, resultItem1)
    {
        ConditionItem2 = conditionItem2;
        ConditionOperator = conditionOperator;
        ResultItem = resultItem2;
        ResultOperator = resultOperator;
    }

    public override string ToString()
    {
        var condOp = ConditionOperator == ConditionOperator.Equal ? "=" : "≠";
        var resultOp = ResultOperator == ConditionOperator.Equal ? "=" : "≠";
        return $"もし{Pair.Item1.Name}{condOp}{ConditionItem2.Name}なら、{Pair.Item1.Name}{resultOp}{ResultItem.Name}です。";
    }

    public bool SatisfiesConditionalClue(Answer answer)
    {
        var conditionSatisfied = answer.AreSameGroup(Pair.Item1, ConditionItem2);

        // ConditionOperator が ≠ の場合は逆転
        if (ConditionOperator == ConditionOperator.NotEqual)
            conditionSatisfied = !conditionSatisfied;

        if (!conditionSatisfied)
        {
            // 条件が満たされない場合は、この手掛かりは常に true
            // （条件が false なら、結果がどうであってもこの手掛かりは矛盾しない）
            return true;
        }

        // 条件が満たされた場合、結果をチェック
        var resultSatisfied = answer.AreSameGroup(Pair.Item1, ResultItem);

        // ResultOperator が ≠ の場合は逆転
        if (ResultOperator == ConditionOperator.NotEqual)
            resultSatisfied = !resultSatisfied;

        return resultSatisfied;
    }
}