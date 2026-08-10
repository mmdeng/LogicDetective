namespace LogicDetective.Tests;

public sealed class HintDifficultyDiagnosticsTests
{
    [Fact]
    public void Measure_WithGeneratedPuzzle_ReturnsClueCount()
    {
        var categories = CreateCategories();

        var report = HintDifficultyDiagnostics.Run(categories, 1);

        var measurement = Assert.Single(report.Measurements);

        Assert.True(measurement.ClueCount > 0);
        Assert.True(measurement.MinimumClueCount > 0);
        Assert.True(measurement.RedundantClueCount >= 0);
    }

    [Fact]
    public void Run_GeneratedPuzzleHasValidMinimumClueCount()
    {
        var categories = CreateCategories();

        var report = HintDifficultyDiagnostics.Run(categories, 10);

        Assert.Equal(10, report.SampleCount);

        Assert.All(
            report.Measurements,
            measurement =>
            {
                Assert.True(measurement.ClueCount > 0);
                Assert.True(measurement.MinimumClueCount > 0);
                Assert.True(
                    measurement.MinimumClueCount <= measurement.ClueCount);

                Assert.Equal(
                    measurement.ClueCount - measurement.MinimumClueCount,
                    measurement.RedundantClueCount);
            });
    }
    [Fact]
    public void Summarize_CalculatesAverages()
    {
        var measurements = new[]
        {
            new HintDifficultyMeasurement(5, 5, 0),
            new HintDifficultyMeasurement(7, 6, 1),
            new HintDifficultyMeasurement(9, 7, 2)
        };

        var report = HintDifficultyDiagnostics.Summarize(measurements);

        Assert.Equal(3, report.SampleCount);
        Assert.Equal(7.0, report.AverageClueCount);
        Assert.Equal(6.0, report.AverageMinimumClueCount);
        Assert.Equal(1.0, report.AverageRedundantClueCount);
    }

    [Fact]
    public void Summarize_CalculatesMedians()
    {
        var measurements = new[]
        {
            new HintDifficultyMeasurement(5, 4, 1),
            new HintDifficultyMeasurement(7, 6, 1),
            new HintDifficultyMeasurement(9, 8, 1)
        };

        var report = HintDifficultyDiagnostics.Summarize(measurements);

        Assert.Equal(7.0, report.MedianClueCount);
        Assert.Equal(6.0, report.MedianMinimumClueCount);
        Assert.Equal(1.0, report.MedianRedundantClueCount);
    }

    [Fact]
    public void Summarize_CalculatesEvenMedians()
    {
        var measurements = new[]
        {
            new HintDifficultyMeasurement(5, 4, 1),
            new HintDifficultyMeasurement(6, 5, 1),
            new HintDifficultyMeasurement(8, 6, 2),
            new HintDifficultyMeasurement(10, 8, 2)
        };

        var report = HintDifficultyDiagnostics.Summarize(measurements);

        Assert.Equal(7.0, report.MedianClueCount);
        Assert.Equal(5.5, report.MedianMinimumClueCount);
        Assert.Equal(1.5, report.MedianRedundantClueCount);
    }

    [Fact]
    public void Summarize_ReturnsMinimumAndMaximumMinimumClues()
    {
        var measurements = new[]
        {
            new HintDifficultyMeasurement(5, 4, 1),
            new HintDifficultyMeasurement(8, 6, 2),
            new HintDifficultyMeasurement(10, 8, 2),
            new HintDifficultyMeasurement(7, 5, 2)
        };

        var report = HintDifficultyDiagnostics.Summarize(measurements);

        Assert.Equal(4, report.MinMinimumClueCount);
        Assert.Equal(8, report.MaxMinimumClueCount);
    }

    [Fact]
    public void Summarize_EmptyMeasurements_Throws()
    {
        var measurements =
            Array.Empty<HintDifficultyMeasurement>();

        Assert.Throws<ArgumentException>(() =>
            HintDifficultyDiagnostics.Summarize(measurements));
    }

    [Fact]
    public void Run_WithInvalidSampleCount_Throws()
    {
        var categories = CreateCategories();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            HintDifficultyDiagnostics.Run(categories, 0));
    }

    [Fact]
    public void Run_ReturnsRequestedNumberOfMeasurements()
    {
        var categories = CreateCategories();

        var report =
            HintDifficultyDiagnostics.Run(categories, 5);

        Assert.Equal(5, report.SampleCount);
        Assert.Equal(5, report.Measurements.Count);
    }

    [Fact]
    public void Run_GeneratedPuzzlesHaveNoRedundantClues()
    {
        var categories = CreateCategories();

        var report =
            HintDifficultyDiagnostics.Run(categories, 10);

        Assert.All(
            report.Measurements,
            measurement =>
            {
                Assert.Equal(
                    measurement.ClueCount,
                    measurement.MinimumClueCount);

                Assert.Equal(
                    0,
                    measurement.RedundantClueCount);
            });
    }

    private static Category[] CreateCategories()
    {
        return
        [
            new Category(
                0,
                "Days",
                ["Monday", "Tuesday", "Wednesday"]),

            new Category(
                1,
                "People",
                ["A", "B", "C"]),

            new Category(
                2,
                "Food",
                ["Curry", "Ramen", "Sushi"])
        ];
    }

    private static Solution CreateSolution()
    {
        return new Solution(3, 3);
        // var assignments = new Dictionary<Item, int>
        // {
        //     [Item(0, 0)] = 0,
        //     [Item(0, 1)] = 1,
        //     [Item(0, 2)] = 2,

        //     [Item(1, 0)] = 0,
        //     [Item(1, 1)] = 1,
        //     [Item(1, 2)] = 2,

        //     [Item(2, 0)] = 0,
        //     [Item(2, 1)] = 1,
        //     [Item(2, 2)] = 2
        // };

        // return new Solution(assignments);
    }

    private static Item Item(int categoryIndex, int index)
    {
        var names = new[]
        {
            new[] { "Monday", "Tuesday", "Wednesday" },
            new[] { "A", "B", "C" },
            new[] { "Curry", "Ramen", "Sushi" }
        };

        return new Item(
            categoryIndex,
            index,
            names[categoryIndex][index]);
    }
}