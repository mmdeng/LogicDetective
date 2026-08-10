namespace LogicDetective;

internal static class PuzzleSolver
{
    public static IReadOnlyList<Solution> Solve(IReadOnlyList<Category> categories, IReadOnlyList<Clue> clues)
    {
        var solutions = new List<Solution>();

        foreach (var solution in GetAllSolutions(categories))
        {
            if (SatisfiesAllClues(solution, clues))
            {
                solutions.Add(solution);
            }
        }
        return solutions;
    }

    public static int CountSolutions(IReadOnlyList<Category> categories, IReadOnlyList<Clue> clues)
    {
        var solutionCount = 0;

        foreach (var solution in GetAllSolutions(categories))
        {
            if (!SatisfiesAllClues(solution, clues))
            {
                continue;
            }
            solutionCount++;

            if (solutionCount >= 2)
            {
                return solutionCount;
            }
        }

        return solutionCount;
    }

    public static IReadOnlyList<Clue> FindCertainRelations(IReadOnlyList<Category> categories, IReadOnlyList<Clue> clues)
    {
        var solutions = Solve(categories, clues);

        if (solutions.Count == 0)
        {
            return Array.Empty<Clue>();
        }
        var certainRelations = new List<Clue>();

        for (var categoryA = 0; categoryA < categories.Count; categoryA++)
        {
            for (var categoryB = categoryA + 1; categoryB < categories.Count; categoryB++)
            {
                for (var itemA = 0; itemA < categories[categoryA].Items.Count; itemA++)
                {
                    for (var itemB = 0; itemB < categories[categoryB].Items.Count; itemB++)
                    {
                        var firstItem = new Item(categoryA, itemA, categories[categoryA].Items[itemA]);

                        var secondItem = new Item(categoryB, itemB, categories[categoryB].Items[itemB]);

                        var firstRelation = solutions[0].AreSameGroup(
                            categoryA,
                            itemA,
                            categoryB,
                            itemB);

                        var isCertain = true;

                        for (var solutionIndex = 1; solutionIndex < solutions.Count; solutionIndex++)
                        {
                            var relation = solutions[solutionIndex].AreSameGroup(
                                categoryA,
                                itemA,
                                categoryB,
                                itemB);

                            if (relation != firstRelation)
                            {
                                isCertain = false;
                                break;
                            }
                        }
                        if (!isCertain) continue;
                        if (firstRelation)
                        {
                            certainRelations.Add(new SameClue(firstItem, secondItem));
                        }
                        else
                        {
                            certainRelations.Add(new DifferentClue(firstItem, secondItem));
                        }
                    }
                }
            }
        }
        return certainRelations;
    }

    private static bool SatisfiesAllClues(Solution solution, IReadOnlyList<Clue> clues)
    {
        foreach (var clue in clues)
        {
            var sameGroup = solution.AreSameGroup(
                clue.FirstItem.CategoryIndex,
                clue.FirstItem.Index,
                clue.SecondItem.CategoryIndex,
                clue.SecondItem.Index);

            if (clue is SameClue && !sameGroup) return false;
            if (clue is DifferentClue && sameGroup) return false;
        }
        return true;
    }

    private static IEnumerable<Solution> GetAllSolutions(IReadOnlyList<Category> categories)
    {
        var groupCount = categories[0].Items.Count;
        var permutations = new int[categories.Count][];

        permutations[0] = Enumerable.Range(0, groupCount).ToArray();

        foreach (var solution in GenerateSolutions(categories, permutations, 1))
        {
            yield return solution;
        }
    }

    private static IEnumerable<Solution> GenerateSolutions(IReadOnlyList<Category> categories, int[][] permutations, int category)
    {
        var groupCount = categories[0].Items.Count;

        if (category == categories.Count)
        {
            var solution = new Solution(groupCount, categories.Count);

            for (var group = 0; group < groupCount; group++)
            {
                for (var categoryIndex = 0; categoryIndex < categories.Count; categoryIndex++)
                {
                    solution.SetItemIndex(group, categoryIndex, permutations[categoryIndex][group]);
                }
            }
            yield return solution;
            yield break;
        }
        var permutation = Enumerable.Range(0, groupCount).ToArray();
        do
        {
            permutations[category] = permutation.ToArray();
            foreach (var solution in GenerateSolutions(categories, permutations, category + 1))
            {
                yield return solution;
            }
        }
        while (NextPermutation(permutation));
    }

    private static bool NextPermutation(int[] values)
    {
        var pivotIndex = values.Length - 2;

        while (pivotIndex >= 0 && values[pivotIndex] >= values[pivotIndex + 1])
        {
            pivotIndex--;
        }
        if (pivotIndex < 0)
        {
            return false;
        }
        var swapIndex = values.Length - 1;
        while (values[swapIndex] <= values[pivotIndex])
        {
            swapIndex--;
        }
        (values[pivotIndex], values[swapIndex]) = (values[swapIndex], values[pivotIndex]);
        Array.Reverse(values, pivotIndex + 1, values.Length - pivotIndex - 1);
        return true;
    }
}