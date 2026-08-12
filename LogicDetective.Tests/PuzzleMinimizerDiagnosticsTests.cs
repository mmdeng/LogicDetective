namespace LogicDetective.Tests;

public class PuzzleMinimizerDiagnosticsTests
{
    [Fact]
    public void Measure_ComputesExpectedValues_ForSinglePuzzle()
    {
        var puzzle = CreatePuzzleWithRedundantClues();
        var measurement = PuzzleMinimizerDiagnostics.Measure(puzzle);
        Assert.Equal(3, measurement.OriginalClueCount);
        Assert.Equal(1, measurement.MinimizedClueCount);
        Assert.Equal(2, measurement.ReducedClueCount);
        Assert.Equal(1, measurement.MinimizedAnswerCount);
    }

    [Fact]
    public void Summarize_ComputesStatistics()
    {
        var measurements = new[]
        {
            new PuzzleMinimizationMeasurement(10, 7, 3, 1),
            new PuzzleMinimizationMeasurement(8, 6, 2, 1),
            new PuzzleMinimizationMeasurement(6, 5, 1, 2)
        };
        var report = PuzzleMinimizerDiagnostics.Summarize(measurements);
        Assert.Equal(3, report.SampleCount);
        Assert.Equal(8.0, report.AverageBefore);
        Assert.Equal(6.0, report.AverageAfter);
        Assert.Equal(2.0, report.AverageReduced);
        Assert.Equal(8.0, report.MedianBefore);
        Assert.Equal(6.0, report.MedianAfter);
        Assert.Equal(2.0, report.MedianReduced);
        Assert.Equal(5, report.MinAfter);
        Assert.Equal(7, report.MaxAfter);
        Assert.False(report.AllUniqueAfter);
    }

    private static Puzzle CreatePuzzleWithRedundantClues()
    {
        var categories = new CategoryList
        {
            new Category(0, "People", ["A", "B"]),
            new Category(1, "Pets", ["X", "Y"])
        };
        var answer = new Answer(categories);
        answer.SetItemIndex(0, 0, 0);
        answer.SetItemIndex(0, 1, 0);
        answer.SetItemIndex(1, 0, 1);
        answer.SetItemIndex(1, 1, 1);

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
        return new Puzzle(categories, answer, clues);
    }
}
