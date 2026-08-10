namespace LogicDetective.Tests;

public class PuzzleMinimizerTests
{
    [Fact]
    public void Minimize_KeepsUniqueSolution()
    {
        var puzzle = CreatePuzzleWithRedundantClues();
        var minimized = PuzzleMinimizer.Minimize(puzzle);
        Assert.Equal(1, PuzzleSolver.CountSolutions(minimized.Categories, minimized.Clues));
    }

    [Fact]
    public void Minimize_ReducesClues_WhenRemovableCluesExist()
    {
        var puzzle = CreatePuzzleWithRedundantClues();
        var minimized = PuzzleMinimizer.Minimize(puzzle);
        Assert.True(minimized.Clues.Count < puzzle.Clues.Count);
    }

    [Fact]
    public void Minimize_KeepsRequiredClues()
    {
        var puzzle = CreatePuzzleWithRequiredClue();
        var minimized = PuzzleMinimizer.Minimize(puzzle);
        Assert.Single(minimized.Clues);
        Assert.Equal(1, PuzzleSolver.CountSolutions(minimized.Categories, minimized.Clues));
    }

    [Fact]
    public void Minimize_DoesNotMutateOriginalPuzzle()
    {
        var puzzle = CreatePuzzleWithRedundantClues();
        var originalClues = puzzle.Clues.ToList();

        _ = PuzzleMinimizer.Minimize(puzzle);

        Assert.Equal(originalClues.Count, puzzle.Clues.Count);
        for (var i = 0; i < originalClues.Count; i++)
        {
            Assert.Same(originalClues[i], puzzle.Clues[i]);
        }
    }

    [Fact]
    public void Minimize_CanRunMultipleTimes_AndKeepsUniqueness()
    {
        var puzzle = CreatePuzzleWithRedundantClues();

        var first = PuzzleMinimizer.Minimize(puzzle);
        var second = PuzzleMinimizer.Minimize(first);
        var third = PuzzleMinimizer.Minimize(second);

        Assert.Equal(1, PuzzleSolver.CountSolutions(first.Categories, first.Clues));
        Assert.Equal(1, PuzzleSolver.CountSolutions(second.Categories, second.Clues));
        Assert.Equal(1, PuzzleSolver.CountSolutions(third.Categories, third.Clues));
    }

    [Fact]
    public void Minimize_PreservesOriginalSolutionReference()
    {
        var puzzle = CreatePuzzleWithRedundantClues();
        var minimized = PuzzleMinimizer.Minimize(puzzle);
        Assert.Same(puzzle.Solution, minimized.Solution);
    }

    private static Puzzle CreatePuzzleWithRedundantClues()
    {
        var categories = new[]
        {
            new Category(0, "People", new[] { "A", "B" }),
            new Category(1, "Pets", new[] { "X", "Y" })
        };
        var solution = new Solution(groupCount: 2, categoryCount: 2);
        solution.SetItemIndex(0, 0, 0);
        solution.SetItemIndex(0, 1, 0);
        solution.SetItemIndex(1, 0, 1);
        solution.SetItemIndex(1, 1, 1);

        var a = new Item(0, 0, "A");
        var b = new Item(0, 1, "B");
        var x = new Item(1, 0, "X");
        var y = new Item(1, 1, "Y");

        var clues = new Clue[]
        {
            new SameClue(a, x),
            new DifferentClue(a, y),
            new SameClue(b, y)
        };
        return new Puzzle(categories, solution, clues);
    }

    private static Puzzle CreatePuzzleWithRequiredClue()
    {
        var categories = new[]
        {
            new Category(0, "People", new[] { "A", "B" }),
            new Category(1, "Pets", new[] { "X", "Y" })
        };
        var solution = new Solution(groupCount: 2, categoryCount: 2);
        solution.SetItemIndex(0, 0, 0);
        solution.SetItemIndex(0, 1, 0);
        solution.SetItemIndex(1, 0, 1);
        solution.SetItemIndex(1, 1, 1);

        var a = new Item(0, 0, "A");
        var x = new Item(1, 0, "X");
        var clues = new Clue[] { new SameClue(a, x) };
        return new Puzzle(categories, solution, clues);
    }
}
