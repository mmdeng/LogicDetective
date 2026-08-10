namespace LogicDetective;

internal enum PlayerPairState
{
    Unknown,
    Yes,
    No
}

internal sealed class PlayerState
{
    private readonly Dictionary<PairKey, PlayerPairState> _states = new();

    public PlayerPairState GetState(Item firstItem, Item secondItem)
    {
        var key = CreateKey(firstItem, secondItem);
        return _states.TryGetValue(key, out var state)
            ? state
            : PlayerPairState.Unknown;
    }

    public void SetState(Item firstItem, Item secondItem, PlayerPairState state)
    {
        var key = CreateKey(firstItem, secondItem);

        if (state == PlayerPairState.Unknown)
        {
            _states.Remove(key);
            return;
        }

        _states[key] = state;
    }

    public IReadOnlyList<Clue> ToClues(IReadOnlyList<Category> categories)
    {
        var clues = new List<Clue>();
        foreach (var pair in _states)
        {
            var firstCategory = categories[pair.Key.CategoryA];
            var secondCategory = categories[pair.Key.CategoryB];

            var firstItem = new Item(
                pair.Key.CategoryA,
                pair.Key.IndexA,
                firstCategory.Items[pair.Key.IndexA]);

            var secondItem = new Item(
                pair.Key.CategoryB,
                pair.Key.IndexB,
                secondCategory.Items[pair.Key.IndexB]);

            if (pair.Value == PlayerPairState.Yes)
            {
                clues.Add(new SameClue(firstItem, secondItem));
            }
            else if (pair.Value == PlayerPairState.No)
            {
                clues.Add(new DifferentClue(firstItem, secondItem));
            }
        }
        return clues;
    }

    private static PairKey CreateKey(Item firstItem, Item secondItem)
    {
        if (IsSameItem(firstItem, secondItem))
        {
            throw new ArgumentException("同一項目の組み合わせは指定できません。");
        }

        if (Compare(firstItem, secondItem) <= 0)
        {
            return new PairKey(
                firstItem.CategoryIndex,
                firstItem.Index,
                secondItem.CategoryIndex,
                secondItem.Index);
        }

        return new PairKey(
            secondItem.CategoryIndex,
            secondItem.Index,
            firstItem.CategoryIndex,
            firstItem.Index);
    }

    private static bool IsSameItem(Item firstItem, Item secondItem)
    {
        return firstItem.CategoryIndex == secondItem.CategoryIndex && firstItem.Index == secondItem.Index;
    }

    private static int Compare(Item firstItem, Item secondItem)
    {
        var categoryCompare = firstItem.CategoryIndex.CompareTo(secondItem.CategoryIndex);
        if (categoryCompare != 0) return categoryCompare;

        return firstItem.Index.CompareTo(secondItem.Index);
    }

    private readonly record struct PairKey(
        int CategoryA,
        int IndexA,
        int CategoryB,
        int IndexB);
}