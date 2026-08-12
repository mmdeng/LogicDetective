namespace LogicDetective;

internal sealed class ClueMultiple : Clue
{
    public ItemPair Pair2 { get; }

    public ClueMultiple(Item item1, Item item2, Item item3, Item item4)
        : base(item1, item2)
    {
        Pair2 = new ItemPair(item3, item4);
    }
    public override string ToString()
    {
        return $"{Pair.Item1.Name}と{Pair2.Item2.Name}はどちらかが{Pair.Item2.Name}でどちらかが{Pair2.Item2.Name}です。";
    }
    public bool Satisfies(Answer answer)
    {
        // var r11 = answer.AreSameGroup(Pair.Item1, Pair.Item2);
        // var r12 = answer.AreSameGroup(Pair.Item1, Pair2.Item2);
        // var r21 = answer.AreSameGroup(Pair2.Item1, Pair.Item2);
        // var r22 = answer.AreSameGroup(Pair2.Item1, Pair2.Item2);
        return false;
    }
}
