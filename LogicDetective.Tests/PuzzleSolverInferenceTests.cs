namespace LogicDetective.Tests;

public class PuzzleSolverInferenceTests
{
    [Fact]
    public void FindCertainRelations_WithSameClue_FindsSameAndDifferentRelations()
    {
        var categories = CreateCategories();
        var itemPair0010 = categories.GetItemPair("a", "00", "b", "10");
        var itemPair0011 = categories.GetItemPair("a", "00", "b", "11");
        var clues = new List<Clue> { new SameClue(itemPair0010) };
        var solver = new PuzzleSolver(categories);
        var answers = solver.Solve(clues);

        var hintGenerator = new HintGenrator(categories);
        var relations = hintGenerator.FindCertainRelations(answers);
        Assert.Contains(relations, m => m is SameClue && m.Pair.Equal(itemPair0010));
        Assert.Contains(relations, m => m is DifferentClue && m.Pair.Equal(itemPair0011));
    }

    [Fact]
    public void FindCertainRelations_WithInsufficientInformation_DoesNotInventRelations()
    {
        var categories = CreateCategories();
        var solver = new PuzzleSolver(categories);
        var answers = solver.Solve([]);

        var hintGenerator = new HintGenrator(categories);
        var relations = hintGenerator.FindCertainRelations(answers);
        Assert.Empty(relations);
    }

    [Fact]
    public void FindCertainRelations_WithContradictoryClues_ReturnsNoRelations()
    {
        var categories = CreateCategories();
        var itemPair = categories.GetItemPair("a", "00", "b", "10");
        var clues = new List<Clue> { new SameClue(itemPair), new DifferentClue(itemPair) };
        var solver = new PuzzleSolver(categories);
        var answers = solver.Solve(clues);

        var hintGenerator = new HintGenrator(categories);
        var relations = hintGenerator.FindCertainRelations(answers);
        Assert.Empty(relations);
    }

    [Fact]
    public void FindCertainRelations_DoesNotReturnRelationsWithinSameCategory()
    {
        var categories = CreateCategories();
        var solver = new PuzzleSolver(categories);
        var answers = solver.Solve([]);

        var hintGenerator = new HintGenrator(categories);
        var relations = hintGenerator.FindCertainRelations(answers);
        Assert.All(relations, m => { Assert.NotEqual(m.Pair.Item1.CategoryIndex, m.Pair.Item2.CategoryIndex); });
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
