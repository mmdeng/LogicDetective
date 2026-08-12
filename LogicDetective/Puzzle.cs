namespace LogicDetective;

internal sealed class Puzzle
{
    public CategoryList Categories { get; }
    public Answer Answer { get; }
    public IList<Clue> Clues { get; }

    public Puzzle(CategoryList categories, Answer answer, IList<Clue> clues)
    {
        Categories = categories;
        Answer = answer;
        Clues = clues;
    }
}
