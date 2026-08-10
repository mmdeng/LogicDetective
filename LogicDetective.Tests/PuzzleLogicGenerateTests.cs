namespace LogicDetective.Tests;

public class PuzzleLogicGenerateTests
{
    [Fact]
    public void Generate_ReturnsPlayablePuzzle_WithConsistentClues_AndUniqueSolution()
    {
        var categories = CreateCategories();
        var puzzle = PuzzleLogic.Generate(categories);

        Assert.NotNull(puzzle);
        Assert.NotNull(puzzle.Categories);
        Assert.NotNull(puzzle.Solution);
        Assert.NotNull(puzzle.Clues);

        Assert.Equal(categories.Count, puzzle.Categories.Count);
        Assert.True(puzzle.Categories.Count >= 3);
        Assert.True(puzzle.Categories.All(category => category.Items.Count >= 2));
        Assert.NotEmpty(puzzle.Clues);

        foreach (var clue in puzzle.Clues)
        {
            var sameGroup = puzzle.Solution.AreSameGroup(
                clue.FirstItem.CategoryIndex,
                clue.FirstItem.Index,
                clue.SecondItem.CategoryIndex,
                clue.SecondItem.Index);

            if (clue is SameClue)
            {
                Assert.True(sameGroup);
                continue;
            }
            if (clue is DifferentClue)
            {
                Assert.False(sameGroup);
                continue;
            }
            throw new Xunit.Sdk.XunitException($"Unknown clue type: {clue.GetType().Name}");
        }
        var solutionCount = PuzzleSolver.CountSolutions(puzzle.Categories, puzzle.Clues);
        Assert.Equal(1, solutionCount);
    }

    [Fact]
    public void Generate_MultipleTimes_AllPuzzlesHaveUniqueSolution()
    {
        var categories = CreateCategories();
        for (var iteration = 0; iteration < 10; iteration++)
        {
            var puzzle = PuzzleLogic.Generate(categories);
            Assert.NotEmpty(puzzle.Clues);

            var solutionCount = PuzzleSolver.CountSolutions(puzzle.Categories, puzzle.Clues);
            Assert.Equal(1, solutionCount);
        }
    }
    [Fact]
    public void Generate_ReturnsPuzzleWithMinimalClues()
    {
        var categories = CreateCategories();
        var puzzle = PuzzleLogic.Generate(categories);
        Assert.Equal(1, PuzzleSolver.CountSolutions(puzzle.Categories, puzzle.Clues));

        for (var index = 0; index < puzzle.Clues.Count; index++)
        {
            var cluesWithoutOne = puzzle.Clues
                .Where((_, clueIndex) => clueIndex != index)
                .ToList();

            var solutionCount = PuzzleSolver.CountSolutions(puzzle.Categories, cluesWithoutOne);
            Assert.NotEqual(1, solutionCount);
        }
    }

    [Fact]
    public void Generate_Minimize_PreservesUniqueSolution()
    {
        var categories = CreateCategories();
        var puzzle = PuzzleLogic.Generate(categories);
        var minimizedPuzzle = PuzzleMinimizer.Minimize(puzzle);
        var solutionCount = PuzzleSolver.CountSolutions(minimizedPuzzle.Categories, minimizedPuzzle.Clues);
        Assert.Equal(1, solutionCount);
    }

    [Fact]
    public void Generate_Minimize_DoesNotIncreaseClueCount()
    {
        var categories = CreateCategories();
        var puzzle = PuzzleLogic.Generate(categories);
        var minimizedPuzzle = PuzzleMinimizer.Minimize(puzzle);
        Assert.True(minimizedPuzzle.Clues.Count <= puzzle.Clues.Count);
    }

    [Fact]
    public void Generate_Minimize_PreservesSolution()
    {
        var categories = CreateCategories();
        var puzzle = PuzzleLogic.Generate(categories);
        var minimizedPuzzle = PuzzleMinimizer.Minimize(puzzle);
        AssertSolutionsEqual(puzzle.Solution, minimizedPuzzle.Solution);
    }

    [Fact]
    public void Generate_Minimize_DoesNotModifyOriginalPuzzle()
    {
        var categories = CreateCategories();

        var puzzle = PuzzleLogic.Generate(categories);
        var originalClues = puzzle.Clues.ToList();

        _ = PuzzleMinimizer.Minimize(puzzle);

        Assert.Equal(originalClues.Count, puzzle.Clues.Count);
        Assert.Equal(originalClues, puzzle.Clues);
    }

    private static void AssertSolutionsEqual(Solution expected, Solution actual)
    {
        Assert.Equal(expected.GroupCount, actual.GroupCount);
        Assert.Equal(expected.CategoryCount, actual.CategoryCount);

        for (var group = 0; group < expected.GroupCount; group++)
        {
            for (var category = 0; category < expected.CategoryCount; category++)
            {
                Assert.Equal(expected.GetItemIndex(group, category), actual.GetItemIndex(group, category));
            }
        }
    }

    private static IReadOnlyList<Category> CreateCategories()
    {
        return new[]
        {
            new Category(0, "People", new[] { "Alice", "Bob", "Carol" }),
            new Category(1, "Pets", new[] { "Cat", "Dog", "Bird" }),
            new Category(2, "Drinks", new[] { "Tea", "Coffee", "Juice" })
        };
    }
}
