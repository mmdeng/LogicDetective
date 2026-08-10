namespace LogicDetective;

internal readonly record struct PuzzleMinimizationMeasurement(
    int OriginalClueCount,
    int MinimizedClueCount,
    int ReducedClueCount,
    int MinimizedSolutionCount);

internal sealed class PuzzleMinimizationReport
{
    public required IReadOnlyList<PuzzleMinimizationMeasurement> Measurements { get; init; }
    public required int SampleCount { get; init; }
    public required double AverageBefore { get; init; }
    public required double AverageAfter { get; init; }
    public required double AverageReduced { get; init; }
    public required double MedianBefore { get; init; }
    public required double MedianAfter { get; init; }
    public required double MedianReduced { get; init; }
    public required int MinAfter { get; init; }
    public required int MaxAfter { get; init; }
    public required bool AllUniqueAfter { get; init; }
}

internal static class PuzzleMinimizerDiagnostics
{
    public static PuzzleMinimizationMeasurement Measure(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        var minimized = PuzzleMinimizer.Minimize(puzzle);
        var originalClueCount = puzzle.Clues.Count;
        var minimizedClueCount = minimized.Clues.Count;
        var reducedClueCount = originalClueCount - minimizedClueCount;
        var minimizedSolutionCount = PuzzleSolver.CountSolutions(minimized.Categories, minimized.Clues);

        return new PuzzleMinimizationMeasurement(
            originalClueCount,
            minimizedClueCount,
            reducedClueCount,
            minimizedSolutionCount);
    }

    public static PuzzleMinimizationReport Run(IReadOnlyList<Category> categories, int sampleCount)
    {
        if (sampleCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sampleCount), "サンプル数は1以上にしてください。");
        }
        var measurements = new List<PuzzleMinimizationMeasurement>(sampleCount);

        for (var index = 0; index < sampleCount; index++)
        {
            var puzzle = PuzzleLogic.Generate(categories);
            measurements.Add(Measure(puzzle));
        }
        return Summarize(measurements);
    }

    public static PuzzleMinimizationReport Summarize(IReadOnlyList<PuzzleMinimizationMeasurement> measurements)
    {
        ArgumentNullException.ThrowIfNull(measurements);

        if (measurements.Count == 0)
        {
            throw new ArgumentException("測定結果が空です。", nameof(measurements));
        }
        var before = measurements.Select(measurement => measurement.OriginalClueCount).ToArray();
        var after = measurements.Select(measurement => measurement.MinimizedClueCount).ToArray();
        var reduced = measurements.Select(measurement => measurement.ReducedClueCount).ToArray();

        return new PuzzleMinimizationReport
        {
            Measurements = measurements.ToList(),
            SampleCount = measurements.Count,
            AverageBefore = before.Average(),
            AverageAfter = after.Average(),
            AverageReduced = reduced.Average(),
            MedianBefore = CalculateMedian(before),
            MedianAfter = CalculateMedian(after),
            MedianReduced = CalculateMedian(reduced),
            MinAfter = after.Min(),
            MaxAfter = after.Max(),
            AllUniqueAfter = measurements.All(measurement => measurement.MinimizedSolutionCount == 1)
        };
    }

    private static double CalculateMedian(IEnumerable<int> values)
    {
        var sorted = values.OrderBy(value => value).ToArray();
        var middle = sorted.Length / 2;

        if (sorted.Length % 2 == 1)
        {
            return sorted[middle];
        }
        return (sorted[middle - 1] + sorted[middle]) / 2.0;
    }
}
