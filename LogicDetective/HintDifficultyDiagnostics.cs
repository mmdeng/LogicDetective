namespace LogicDetective;

internal readonly record struct HintDifficultyMeasurement(
    int ClueCount,
    int MinimumClueCount,
    int RedundantClueCount);

internal sealed class HintDifficultyReport
{
    public required IReadOnlyList<HintDifficultyMeasurement> Measurements { get; init; }
    public required int SampleCount { get; init; }
    public required double AverageClueCount { get; init; }
    public required double AverageMinimumClueCount { get; init; }
    public required double AverageRedundantClueCount { get; init; }
    public required double MedianClueCount { get; init; }
    public required double MedianMinimumClueCount { get; init; }
    public required double MedianRedundantClueCount { get; init; }
    public required int MinMinimumClueCount { get; init; }
    public required int MaxMinimumClueCount { get; init; }
}

internal static class HintDifficultyDiagnostics
{
    public static HintDifficultyMeasurement Measure(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        var clues = puzzle.Clues.ToList();

        var minimumClueCount = FindMinimumClueCount(
            puzzle.Categories,
            clues);

        return new HintDifficultyMeasurement(
            clues.Count,
            minimumClueCount,
            clues.Count - minimumClueCount);
    }

    public static HintDifficultyReport Run(
        IReadOnlyList<Category> categories,
        int sampleCount)
    {
        ArgumentNullException.ThrowIfNull(categories);

        if (sampleCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sampleCount),
                "サンプル数は1以上で指定してください。");
        }

        var measurements = new List<HintDifficultyMeasurement>(sampleCount);

        for (var index = 0; index < sampleCount; index++)
        {
            var puzzle = PuzzleLogic.Generate(categories);
            measurements.Add(Measure(puzzle));
        }

        return Summarize(measurements);
    }

    public static HintDifficultyReport Summarize(
        IReadOnlyList<HintDifficultyMeasurement> measurements)
    {
        ArgumentNullException.ThrowIfNull(measurements);

        if (measurements.Count == 0)
        {
            throw new ArgumentException("測定結果が空です。", nameof(measurements));
        }

        var clueCounts = measurements.Select(x => x.ClueCount).ToArray();
        var minimumClueCounts = measurements.Select(x => x.MinimumClueCount).ToArray();
        var redundantClueCounts = measurements.Select(x => x.RedundantClueCount).ToArray();

        return new HintDifficultyReport
        {
            Measurements = measurements.ToList(),
            SampleCount = measurements.Count,

            AverageClueCount = clueCounts.Average(),
            AverageMinimumClueCount = minimumClueCounts.Average(),
            AverageRedundantClueCount = redundantClueCounts.Average(),

            MedianClueCount = CalculateMedian(clueCounts),
            MedianMinimumClueCount = CalculateMedian(minimumClueCounts),
            MedianRedundantClueCount = CalculateMedian(redundantClueCounts),

            MinMinimumClueCount = minimumClueCounts.Min(),
            MaxMinimumClueCount = minimumClueCounts.Max()
        };
    }

    private static int FindMinimumClueCount(IReadOnlyList<Category> categories, IReadOnlyList<Clue> clues)
    {
        for (var count = 0; count <= clues.Count; count++)
        {
            foreach (var subset in GetCombinations(clues, count))
            {
                if (PuzzleSolver.CountSolutions(categories, subset) == 1)
                {
                    return count;
                }
            }
        }
        throw new InvalidOperationException("与えられたClueから一意解を構成できません。");
    }

    private static IEnumerable<IReadOnlyList<Clue>> GetCombinations(IReadOnlyList<Clue> clues, int count)
    {
        var selected = new Clue[count];
        return GenerateCombinations(clues, count, 0, 0, selected);
    }

    private static IEnumerable<IReadOnlyList<Clue>> GenerateCombinations(
        IReadOnlyList<Clue> clues,
        int count,
        int startIndex,
        int depth,
        Clue[] selected)
    {
        if (depth == count)
        {
            yield return selected.ToArray();
            yield break;
        }

        var remaining = count - depth;

        for (var index = startIndex; index <= clues.Count - remaining; index++)
        {
            selected[depth] = clues[index];
            foreach (var combination in GenerateCombinations(
                         clues,
                         count,
                         index + 1,
                         depth + 1,
                         selected))
            {
                yield return combination;
            }
        }
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