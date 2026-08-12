namespace LogicDetective.Tests;

public class PuzzleLogicGenerateTests
{
    [Fact]
    public void Generate_MultipleTimes_AllPuzzlesHaveUniqueAnswer()
    {
        var categories = CreateCategories();
        var solver = new PuzzleSolver(categories);
        for (var iteration = 0; iteration < 10; iteration++)
        {
            var puzzle = PuzzleLogic.GeneratePuzzle(categories);
            Assert.NotEmpty(puzzle.Clues);
            Assert.Equal(1, solver.CountAnswers(puzzle.Clues.ToList()));
        }
    }

    /// <summary>
    /// 手掛かりが一つでも欠けると一意解でなくなることを確認するテスト
    /// </summary>
    [Fact]
    public void Generate_ReturnsPuzzleWithMinimalClues()
    {
        var categories = CreateCategories();
        var puzzle = PuzzleLogic.GeneratePuzzle(categories);
        var solver = new PuzzleSolver(categories);
        for (var i = 0; i < puzzle.Clues.Count; i++)
        {
            var cluesWithoutOne = puzzle.Clues.Where((_, clueIndex) => clueIndex != i).ToList();
            Assert.NotEqual(1, solver.CountAnswers(cluesWithoutOne));
        }
    }

    /// <summary>
    /// もう一回Minimizeしても何も変わらないことを確認するテスト
    /// </summary>
    [Fact]
    public void Generate_Minimize_PreservesUniqueAnswer()
    {
        var categories = CreateCategories();
        var puzzle = PuzzleLogic.GeneratePuzzle(categories);
        var solver = new PuzzleSolver(categories);
        var originalClues = puzzle.Clues.ToList();

        var minimizedPuzzle = PuzzleMinimizer.Minimize(puzzle);

        Assert.Equal(1, solver.CountAnswers(minimizedPuzzle.Clues.ToList()));
        Assert.Equal(originalClues, minimizedPuzzle.Clues);
        Assert.Equal(originalClues.Count, minimizedPuzzle.Clues.Count);
        Assert.True(puzzle.Answer.AreSame(minimizedPuzzle.Answer));
    }

    private static CategoryList CreateCategories()
    {
        return
        [
            new Category(0, "People", ["Alice", "Bob", "Carol"]),
            new Category(1, "Pets", ["Cat", "Dog", "Bird"]),
            new Category(2, "Drinks", ["Tea", "Coffee", "Juice"])
        ];
    }
}
