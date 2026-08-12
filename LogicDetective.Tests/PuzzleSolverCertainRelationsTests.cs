namespace LogicDetective.Tests;

public class PuzzleSolverCertainRelationsTests
{
    // [Fact]
    // public void FindCertainRelations_ReturnsEmptyWhenThereAreMultipleUnconstrainedAnswers()
    // {
    //     var categories = CreateCategories();
    //     var relations = categories.FindCertainRelations([]);
    //     Assert.Empty(relations);
    // }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Clue              Answer
    ///    | 10 | 11 |         | 10 | 11 |
    /// 00 | 1  |    |   →  00 | 1  | 0  |
    /// 01 |    |    |      01 | 0  | 1  |
    /// </remarks>
    [Fact]
    public void FindCertainRelations_ReturnsAllCertainRelationsForTwoByTwoPuzzle()
    {
        var categories = CreateCategories();
        var itemPair0010 = categories.GetItemPair("a", "00", "b", "10");
        var itemPair0111 = categories.GetItemPair("a", "01", "b", "11");
        var itemPair0011 = categories.GetItemPair("a", "00", "b", "11");
        var itemPair0110 = categories.GetItemPair("a", "01", "b", "10");
        var clues = new Clue[] { new SameClue(itemPair0010) };
        var solver = new PuzzleSolver(categories);
        var answers = solver.Solve(clues);

        var hintGenerator = new HintGenrator(categories);
        var relations = hintGenerator.FindCertainRelations(answers);
        Assert.Equal(4, relations.Count());
        Assert.Contains(relations, m => m is SameClue && m.Pair.Equal(itemPair0010));
        Assert.Contains(relations, m => m is SameClue && m.Pair.Equal(itemPair0111));
        Assert.Contains(relations, m => m is DifferentClue && m.Pair.Equal(itemPair0011));
        Assert.Contains(relations, m => m is DifferentClue && m.Pair.Equal(itemPair0110));
    }

    [Fact]
    public void FindCertainRelations_ReturnsEmptyWhenCluesAreContradictory()
    {
        var categories = CreateCategories();
        var itemPair = categories.GetItemPair("a", "00", "b", "10");
        var clues = new Clue[] { new SameClue(itemPair), new DifferentClue(itemPair) };
        var solver = new PuzzleSolver(categories);
        var answers = solver.Solve(clues);

        var hintGenerator = new HintGenrator(categories);
        var relations = hintGenerator.FindCertainRelations(answers);
        Assert.Empty(relations);
    }

    private static CategoryList CreateCategories()
    {
        return new CategoryList
        {
            { "a", ["00", "01"] },
            { "b", ["10", "11"] }
        };
    }
}