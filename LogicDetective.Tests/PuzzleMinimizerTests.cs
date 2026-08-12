namespace LogicDetective.Tests;

public class PuzzleMinimizerTests
{
    private static Puzzle CreatePuzzleWithRedundantClues()
    {
        var categories = new CategoryList
        {
            { "a", ["00", "01"] },
            { "b", ["10", "11"] }
        };
        var item00 = categories.GetItem("a", "00");
        var item01 = categories.GetItem("a", "01");
        var item10 = categories.GetItem("b", "10");
        var item11 = categories.GetItem("b", "11");

        var clues = new Clue[]
        {
            new SameClue(item00, item10),
            new DifferentClue(item00, item11),
            new SameClue(item01, item11)
        };
        var answer = new Answer(categories);
        answer.SetItemIndex(0, 0, 0);
        answer.SetItemIndex(0, 1, 0);
        answer.SetItemIndex(1, 0, 1);
        answer.SetItemIndex(1, 1, 1);
        return new Puzzle(categories, answer, clues);
    }
    [Fact]
    public void Minimize_KeepsUniqueAnswer()
    {
        var puzzle = CreatePuzzleWithRedundantClues();
        var minimized = PuzzleMinimizer.Minimize(puzzle);
        var solver = new PuzzleSolver(puzzle.Categories);
        Assert.Equal(1, solver.CountAnswers(minimized.Clues.ToList()));
    }

    [Fact]
    public void Minimize_ReducesClues_WhenRemovableCluesExist()
    {
        var puzzle = CreatePuzzleWithRedundantClues();
        var minimized = PuzzleMinimizer.Minimize(puzzle);
        Assert.True(minimized.Clues.Count < puzzle.Clues.Count);
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

        var puzzle1 = PuzzleMinimizer.Minimize(puzzle);
        var puzzle2 = PuzzleMinimizer.Minimize(puzzle1);
        var puzzle3 = PuzzleMinimizer.Minimize(puzzle2);

        var solver = new PuzzleSolver(puzzle.Categories);
        Assert.Equal(1, solver.CountAnswers(puzzle1.Clues.ToList()));
        Assert.Equal(1, solver.CountAnswers(puzzle2.Clues.ToList()));
        Assert.Equal(1, solver.CountAnswers(puzzle3.Clues.ToList()));
    }

    [Fact]
    public void Minimize_PreservesOriginalAnswerReference()
    {
        var puzzle = CreatePuzzleWithRedundantClues();
        var minimized = PuzzleMinimizer.Minimize(puzzle);
        Assert.Same(puzzle.Answer, minimized.Answer);
    }

    [Fact]
    public void Minimize_KeepsRequiredClues()
    {
        var categories = new CategoryList
        {
            { "a", ["00", "01"] },
            { "b", ["10", "11"] }
        };
        var answer = new Answer(categories);
        answer.SetItemIndex(0, 0, 0);
        answer.SetItemIndex(0, 1, 0);
        answer.SetItemIndex(1, 0, 1);
        answer.SetItemIndex(1, 1, 1);

        var itemPair = categories.GetItemPair("a", "00", "b", "10");
        var clues = new Clue[] { new SameClue(itemPair) };
        var puzzle = new Puzzle(categories, answer, clues);

        var minimized = PuzzleMinimizer.Minimize(puzzle);
        Assert.Single(minimized.Clues);
        var solver = new PuzzleSolver(puzzle.Categories);
        Assert.Equal(1, solver.CountAnswers(minimized.Clues.ToList()));
    }
}
