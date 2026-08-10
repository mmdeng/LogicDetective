namespace LogicDetective;

internal abstract class Clue
{
    public Item FirstItem { get; }
    public Item SecondItem { get; }

    protected Clue(Item firstItem, Item secondItem)
    {
        FirstItem = firstItem;
        SecondItem = secondItem;
    }
}

internal sealed class SameClue : Clue
{
    public SameClue(Item firstItem, Item secondItem) : base(firstItem, secondItem)
    {
    }

    public override string ToString()
    {
        return $"{FirstItem.Name}と{SecondItem.Name}は同じ組です。";
    }
}

internal sealed class DifferentClue : Clue
{
    public DifferentClue(Item firstItem, Item secondItem) : base(firstItem, secondItem)
    {
    }

    public override string ToString()
    {
        return $"{FirstItem.Name}と{SecondItem.Name}は同じ組ではありません。";
    }
}