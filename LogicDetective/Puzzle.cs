namespace LogicDetective;

internal sealed class Puzzle
{
    public IReadOnlyList<Category> Categories { get; }
    public Solution Solution { get; }
    public IReadOnlyList<Clue> Clues { get; }

    public Puzzle(IReadOnlyList<Category> categories, Solution solution, IReadOnlyList<Clue> clues)
    {
        Categories = categories;
        Solution = solution;
        Clues = clues;
    }
}