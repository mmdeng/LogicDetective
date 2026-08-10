namespace LogicDetective;

internal sealed class HintTracker
{
    private readonly HashSet<HintKey> _shownHints = new();

    public Clue? GetNext(IReadOnlyList<Clue> relations)
    {
        foreach (var relation in relations)
        {
            var key = CreateKey(relation);

            if (_shownHints.Add(key))
            {
                return relation;
            }
        }

        return null;
    }

    private static HintKey CreateKey(Clue clue)
    {
        var relationType = clue switch
        {
            SameClue => true,
            DifferentClue => false,
            _ => throw new ArgumentException("未知のClue型です。", nameof(clue))
        };

        return new HintKey(
            relationType,
            clue.FirstItem.CategoryIndex,
            clue.FirstItem.Index,
            clue.SecondItem.CategoryIndex,
            clue.SecondItem.Index);
    }

    private readonly record struct HintKey(
        bool IsSame,
        int FirstCategoryIndex,
        int FirstItemIndex,
        int SecondCategoryIndex,
        int SecondItemIndex);
}