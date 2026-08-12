namespace LogicDetective;

internal sealed class HintTracker
{
    private readonly HashSet<HintKey> _shownHints = [];

    public Clue? GetNext(IReadOnlyList<Clue> relations)
    {
        foreach (var relation in relations)
        {
            var key = CreateKey(relation);

            // 既に表示済みのヒントはスキップ
            if (_shownHints.Add(key))
            {
                return relation;
            }
        }

        return null;
    }

    private static HintKey CreateKey(Clue clue)
    {
        var type = clue switch
        {
            SameClue => true,
            DifferentClue => false,
            _ => throw new ArgumentException("未知のClue型です。", nameof(clue))
        };

        return new HintKey(
            type,
            clue.Pair.Item1.CategoryIndex,
            clue.Pair.Item1.ItemIndex,
            clue.Pair.Item2.CategoryIndex,
            clue.Pair.Item2.ItemIndex);
    }

    private readonly record struct HintKey(
        bool IsSame,
        int CategoryIndex1,
        int ItemIndex1,
        int CategoryIndex2,
        int ItemIndex2);
}