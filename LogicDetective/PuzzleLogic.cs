namespace LogicDetective;

internal static class PuzzleLogic
{
    private static readonly Random Random = new();

    public static Puzzle Generate(IReadOnlyList<Category> categories)
    {
        ValidateCategories(categories);

        var solution = CreateRandomSolution(categories);
        var clues = CreateUniqueClues(categories, solution);
        var puzzle = new Puzzle(categories, solution, clues);
        return PuzzleMinimizer.Minimize(puzzle);
    }

    private static Solution CreateRandomSolution(IReadOnlyList<Category> categories)
    {
        var groupCount = categories[0].Items.Count;
        var categoryCount = categories.Count;
        var solution = new Solution(groupCount, categoryCount);

        for (var group = 0; group < groupCount; group++)
        {
            solution.SetItemIndex(group, 0, group);
        }
        for (var category = 1; category < categoryCount; category++)
        {
            var permutation = CreateRandomPermutation(groupCount);
            for (var group = 0; group < groupCount; group++)
            {
                solution.SetItemIndex(group, category, permutation[group]);
            }
        }

        return solution;
    }

    private static List<Clue> CreateUniqueClues(IReadOnlyList<Category> categories, Solution solution)
    {
        var candidateClues = CreateCandidateClues(categories, solution);
        Shuffle(candidateClues);

        var selectedClues = new List<Clue>();

        foreach (var clue in candidateClues)
        {
            selectedClues.Add(clue);

            if (PuzzleSolver.CountSolutions(categories, selectedClues) == 1)
            {
                return selectedClues;
            }
        }
        throw new InvalidOperationException("一意解となるヒントを生成できませんでした。");
    }

    private static List<Clue> CreateCandidateClues(IReadOnlyList<Category> categories, Solution solution)
    {
        var clues = new List<Clue>();

        for (var categoryA = 0; categoryA < categories.Count; categoryA++)
        {
            for (var categoryB = categoryA + 1; categoryB < categories.Count; categoryB++)
            {
                for (var itemA = 0; itemA < solution.GroupCount; itemA++)
                {
                    for (var itemB = 0; itemB < solution.GroupCount; itemB++)
                    {
                        var firstItem = new Item(categoryA, itemA, categories[categoryA].Items[itemA]);
                        var secondItem = new Item(categoryB, itemB, categories[categoryB].Items[itemB]);
                        if (solution.AreSameGroup(categoryA, itemA, categoryB, itemB))
                        {
                            clues.Add(new SameClue(firstItem, secondItem));
                        }
                        else
                        {
                            clues.Add(new DifferentClue(firstItem, secondItem));
                        }
                    }
                }
            }
        }
        return clues;
    }

    private static int[] CreateRandomPermutation(int count)
    {
        var values = Enumerable.Range(0, count).ToArray();
        Shuffle(values);
        return values;
    }

    private static void ValidateCategories(IReadOnlyList<Category> categories)
    {
        if (categories.Count < 3)
        {
            throw new ArgumentException("カテゴリは3個以上必要です。");
        }

        var itemCount = categories[0].Items.Count;

        if (itemCount < 2)
        {
            throw new ArgumentException("各カテゴリには2個以上の項目が必要です。");
        }

        foreach (var category in categories)
        {
            if (category.Items.Count != itemCount)
            {
                throw new ArgumentException("全カテゴリの項目数を同じにしてください。");
            }
        }
    }

    private static void Shuffle<T>(IList<T> items)
    {
        for (var i = items.Count - 1; i > 0; i--)
        {
            var randomIndex = Random.Next(i + 1);
            (items[i], items[randomIndex]) = (items[randomIndex], items[i]);
        }
    }
}