namespace LogicDetective;

internal static class PuzzleLogic
{
    private static readonly Random Random = new();

    public static Puzzle GeneratePuzzle(CategoryList categories, bool isRandom = true)
    {
        categories.Validate();
        var answer = GenerateAnswer(categories, isRandom);
        var comparisonClues = CreateComparisonClues(categories);
        var clues = GetSolvableClues(categories, answer);
        var puzzle = new Puzzle(categories, answer, clues);
        //AddComparisonClues(puzzle);
        return PuzzleMinimizer.Minimize(puzzle);
    }

    static List<ClueComparison> CreateComparisonClues(CategoryList categories)
    {
        var ageCategory = categories.SingleOrDefault(m => m.Name == "年齢");
        if (ageCategory is null) return [];

        var otherCategory = categories.First(m => m.Index != ageCategory.Index);
        var clue = new ClueComparison(otherCategory.Items[0], otherCategory.Items[1], ageCategory, 1, ClueComparisonOperator.LessThanOrEqual);
        return [clue];
    }

    public static Answer GenerateAnswer(CategoryList categories, bool isRandom = true)
    {
        var itemCount = categories[0].Items.Count;
        var categoryCount = categories.Count;
        var answer = new Answer(itemCount, categoryCount);

        var solver = new PuzzleSolver(categories);
        var permutations = solver.CreatePermutations(isRandom);
        answer.SetPermutations(permutations);
        return answer;
    }
    /// <summary>
    /// 一意解を導くことが可能な手掛かりのリストを取得
    /// </summary>
    /// <param name="categories"></param>
    /// <param name="answer"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static List<Clue> GetSolvableClues(CategoryList categories, Answer answer)
    {
        var clues = EnumerateAllClues(categories, answer).ToList();
        Mathmatics.Shuffle(clues);

        var selectedClues = new List<Clue>();
        var solver = new PuzzleSolver(categories);
        foreach (var clue in clues)
        {
            selectedClues.Add(clue);
            if (solver.CountAnswers(selectedClues) == 1)
            {
                return selectedClues;
            }
        }
        throw new InvalidOperationException("一意解となるヒントを生成できませんでした。");
    }

    /// <summary>
    /// 正解から全手掛かりを列挙する。
    /// </summary>
    /// <param name="categories"></param>
    /// <param name="answer">正解</param>
    /// <returns>全手掛かり</returns>
    private static IEnumerable<Clue> EnumerateAllClues(CategoryList categories, Answer answer)
    {
        foreach (var itemPair in categories.EnumerateAllItemPairs())
        {
            if (answer.AreSameGroup(itemPair))
            {
                yield return new SameClue(itemPair);
            }
            else
            {
                yield return new DifferentClue(itemPair);
            }
        }
    }
}