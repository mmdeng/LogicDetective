namespace LogicDetective;

internal sealed class Answer
{
    private readonly int[][] _groups;

    public int ItemCount { get; }
    public int CategoryCount { get; }

    // デバッグ用途
    public int[][] Permutations { get; }

    public Answer(int itemCount, int categoryCount)
    {
        ItemCount = itemCount;
        CategoryCount = categoryCount;
        _groups = new int[itemCount][];

        for (var itemIndex = 0; itemIndex < itemCount; itemIndex++)
        {
            _groups[itemIndex] = new int[categoryCount];
        }
        Permutations = new int[categoryCount][];
    }
    public Answer(CategoryList categories) : this(categories.GetItemCount(), categories.Count) { }

    public void SetPermutations(int[][] permutations)
    {
        for (var i = 0; i < permutations.Length; i++)
        {
            Permutations[i] = new int[permutations[i].Length];
            Array.Copy(permutations[i], Permutations[i], permutations[i].Length);
        }
        for (var itemIndex = 0; itemIndex < ItemCount; itemIndex++)
        {
            for (var categoryIndex = 0; categoryIndex < CategoryCount; categoryIndex++)
            {
                SetItemIndex(itemIndex, categoryIndex, Permutations[categoryIndex][itemIndex]);
            }
        }
    }
    public void SetPermutations(int[] permutation)
    {
        Permutations[0] = permutation;
    }

    public IEnumerable<List<Item>> EnumerateGroups(CategoryList categories)
    {
        for (var itemIndex = 0; itemIndex < ItemCount; itemIndex++)
        {
            var group = new List<Item>();
            for (var categoryIndex = 0; categoryIndex < CategoryCount; categoryIndex++)
            {
                var actualItemIndex = _groups[itemIndex][categoryIndex];
                var item = categories[categoryIndex].Items[actualItemIndex];
                group.Add(item);
            }
            yield return group;
        }
    }

    public bool AreSame(Answer answer)
    {
        if (ItemCount != answer.ItemCount) return false;
        if (CategoryCount != answer.CategoryCount) return false;

        for (var group = 0; group < ItemCount; group++)
        {
            for (var category = 0; category < CategoryCount; category++)
            {
                if (GetItemIndex(group, category) != answer.GetItemIndex(group, category))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public int GetItemIndex(int itemIndex, int categoryIndex)
    {
        return _groups[itemIndex][categoryIndex];
    }

    public void SetItemIndex(int itemIndex, int categoryIndex, int permutation)
    {
        _groups[itemIndex][categoryIndex] = permutation;
    }

    public bool AreSameGroup(Item item1, Item item2)
    {
        for (var groupIndex = 0; groupIndex < ItemCount; groupIndex++)
        {
            if (_groups[groupIndex][item1.CategoryIndex] == item1.ItemIndex)
            {
                return _groups[groupIndex][item2.CategoryIndex] == item2.ItemIndex;
            }
        }
        throw new InvalidOperationException("指定された項目が解に存在しません。");
    }

    public bool AreSameGroup(ItemPair itemPair)
    {
        return AreSameGroup(itemPair.Item1, itemPair.Item2);
    }
    public bool SatisfiesAllClues(IReadOnlyList<Clue> clues)
    {
        foreach (var clue in clues)
        {
            if (clue is SameClue)
            {
                var sameGroup = AreSameGroup(clue.Pair);
                if (!sameGroup) return false;
            }
            else if (clue is DifferentClue)
            {
                var sameGroup = AreSameGroup(clue.Pair);
                if (sameGroup) return false;
            }
            else if (clue is ClueComparison comparisonClue)
            {
                if (!comparisonClue.SatisfiesComparisonClue(this)) return false;
            }
            else if (clue is ClueMultiple multipleClue)
            {
                if (!multipleClue.Satisfies(this)) return false;
            }
            else if (clue is ConditionalClue conditionalClue)
            {
                if (!conditionalClue.SatisfiesConditionalClue(this)) return false;
            }
        }
        return true;
    }
}


// internal sealed class AnswerRecord : List<Item>
// {

// }
// internal sealed class Answer2 : List<AnswerRecord>
// {
//     public bool AreSameGroup(Item item1, Item item2)
//     {
//         foreach (var record in this)
//         {
//             var found1 = record.Any(m => m.CategoryIndex == item1.CategoryIndex && m.ItemIndex == item1.ItemIndex);
//             var found2 = record.Any(m => m.CategoryIndex == item2.CategoryIndex && m.ItemIndex == item2.ItemIndex);
//             if (found1 && found2) return true;
//         }
//         return false;
//     }

//     public bool AreSameGroup(ItemPair itemPair)
//     {
//         return AreSameGroup(itemPair.Item1, itemPair.Item2);
//     }

//     public bool AreSame(Answer2 answer)
//     {
//         if (Count != answer.Count) return false;
//         if (this[0].Count != answer[0].Count) return false;

//         for (var group = 0; group < Count; group++)
//         {
//             for (var category = 0; category < this[group].Count; category++)
//             {
//                 if (this[group][category] != answer[group][category])
//                 {
//                     return false;
//                 }
//             }
//         }
//         return true;
//     }

// }