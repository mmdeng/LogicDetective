namespace LogicDetective.Tests;

public class PuzzleSolverTests
{
    [Fact]
    public void DifferentClue_AcceptsDifferentGroupAndRejectsSameGroup()
    {
        var fixture = CreateTwoByTwoFixture();
        var clues = new Clue[]
        {
            new DifferentClue(fixture.A, fixture.X)
        };

        var solutions = PuzzleSolver.Solve(fixture.Categories, clues);

        Assert.Single(solutions);
        Assert.False(solutions[0].AreSameGroup(0, 0, 1, 0));
    }

    [Fact]
    public void SameClue_AcceptsSameGroupAndRejectsDifferentGroup()
    {
        var fixture = CreateTwoByTwoFixture();
        var clues = new Clue[]
        {
            new SameClue(fixture.A, fixture.X)
        };

        var solutions = PuzzleSolver.Solve(fixture.Categories, clues);

        Assert.Single(solutions);
        Assert.True(solutions[0].AreSameGroup(0, 0, 1, 0));
    }

    [Fact]
    public void MultipleClues_AreAppliedTogether()
    {
        var categories = new[]
        {
            new Category(0, "People", new[] { "A", "B" }),
            new Category(1, "Pets", new[] { "X", "Y" }),
            new Category(2, "Colors", new[] { "P", "Q" })
        };

        var a = new Item(0, 0, "A");
        var x = new Item(1, 0, "X");
        var p = new Item(2, 0, "P");

        var clues = new Clue[]
        {
            new SameClue(a, x),
            new DifferentClue(a, p)
        };

        var solutions = PuzzleSolver.Solve(categories, clues);

        Assert.Single(solutions);
        Assert.True(solutions[0].AreSameGroup(0, 0, 1, 0));
        Assert.False(solutions[0].AreSameGroup(0, 0, 2, 0));
    }

    [Fact]
    public void CountSolutions_ReturnsOne_WhenUniqueSolutionExists()
    {
        var fixture = CreateTwoByTwoFixture();
        var clues = new Clue[]
        {
            new SameClue(fixture.A, fixture.X)
        };

        var count = PuzzleSolver.CountSolutions(fixture.Categories, clues);

        Assert.Equal(1, count);
    }

    [Fact]
    public void CountSolutions_ReturnsTwoOrMore_WhenMultipleSolutionsExist()
    {
        var fixture = CreateTwoByTwoFixture();

        var count = PuzzleSolver.CountSolutions(fixture.Categories, Array.Empty<Clue>());

        Assert.True(count >= 2);
    }

    [Fact]
    public void CountSolutions_ReturnsZero_WhenNoSolutionExists()
    {
        var fixture = CreateTwoByTwoFixture();
        var clues = new Clue[]
        {
            new SameClue(fixture.A, fixture.X),
            new DifferentClue(fixture.A, fixture.X)
        };

        var count = PuzzleSolver.CountSolutions(fixture.Categories, clues);

        Assert.Equal(0, count);
    }

    private static (IReadOnlyList<Category> Categories, Item A, Item X) CreateTwoByTwoFixture()
    {
        var categories = new[]
        {
            new Category(0, "People", new[] { "A", "B" }),
            new Category(1, "Pets", new[] { "X", "Y" })
        };

        var a = new Item(0, 0, "A");
        var x = new Item(1, 0, "X");

        return (categories, a, x);
    }
}
