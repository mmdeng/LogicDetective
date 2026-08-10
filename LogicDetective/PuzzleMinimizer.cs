namespace LogicDetective;

internal static class PuzzleMinimizer
{
    public static Puzzle Minimize(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        var minimizedClues = puzzle.Clues.ToList();
        var index = 0;

        while (index < minimizedClues.Count)
        {
            var candidateClues = minimizedClues.Where((_, clueIndex) => clueIndex != index).ToList();

            if (PuzzleSolver.CountSolutions(puzzle.Categories, candidateClues) == 1)
            {
                minimizedClues = candidateClues;
                continue;
            }
            index++;
        }
        return new Puzzle(puzzle.Categories, puzzle.Solution, minimizedClues);
    }
}
