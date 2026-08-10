namespace LogicDetective;

internal static class PlayerAnswerChecker
{
    public static AnswerResult Check(Puzzle puzzle, PlayerState playerState)
    {
        var hasUnknown = false;
        var categories = puzzle.Categories;
        var solution = puzzle.Solution;
        var groupCount = solution.GroupCount;

        for (var categoryA = 0; categoryA < categories.Count; categoryA++)
        {
            for (var categoryB = categoryA + 1; categoryB < categories.Count; categoryB++)
            {
                for (var itemA = 0; itemA < groupCount; itemA++)
                {
                    for (var itemB = 0; itemB < groupCount; itemB++)
                    {
                        var firstItem = new Item(categoryA, itemA, categories[categoryA].Items[itemA]);
                        var secondItem = new Item(categoryB, itemB, categories[categoryB].Items[itemB]);
                        var state = playerState.GetState(firstItem, secondItem);

                        if (state == PlayerPairState.Unknown)
                        {
                            hasUnknown = true;
                            continue;
                        }
                        var sameGroup = solution.AreSameGroup(categoryA, itemA, categoryB, itemB);

                        if (state == PlayerPairState.Yes && !sameGroup)
                        {
                            return AnswerResult.Incorrect;
                        }
                        if (state == PlayerPairState.No && sameGroup)
                        {
                            return AnswerResult.Incorrect;
                        }
                    }
                }
            }
        }
        return hasUnknown ? AnswerResult.Incomplete : AnswerResult.Correct;
    }

    public static IReadOnlyList<Clue> FindIncorrectAnswers(Puzzle puzzle, PlayerState playerState)
    {
        var incorrectAnswers = new List<Clue>();
        var categories = puzzle.Categories;
        var solution = puzzle.Solution;
        var groupCount = solution.GroupCount;

        for (var categoryA = 0; categoryA < categories.Count; categoryA++)
        {
            for (var categoryB = categoryA + 1; categoryB < categories.Count; categoryB++)
            {
                for (var itemA = 0; itemA < groupCount; itemA++)
                {
                    for (var itemB = 0; itemB < groupCount; itemB++)
                    {
                        var firstItem = new Item(categoryA, itemA, categories[categoryA].Items[itemA]);
                        var secondItem = new Item(categoryB, itemB, categories[categoryB].Items[itemB]);

                        var state = playerState.GetState(firstItem, secondItem);

                        if (state == PlayerPairState.Unknown)
                        {
                            continue;
                        }
                        var sameGroup = solution.AreSameGroup(categoryA, itemA, categoryB, itemB);

                        if (state == PlayerPairState.Yes && !sameGroup)
                        {
                            incorrectAnswers.Add(new SameClue(firstItem, secondItem));
                        }
                        else if (state == PlayerPairState.No && sameGroup)
                        {
                            incorrectAnswers.Add(new DifferentClue(firstItem, secondItem));
                        }
                    }
                }
            }
        }
        return incorrectAnswers;
    }
}
