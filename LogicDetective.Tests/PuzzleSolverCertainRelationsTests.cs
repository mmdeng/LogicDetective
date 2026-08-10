namespace LogicDetective.Tests;

public class PuzzleSolverCertainRelationsTests
{
    [Fact]
    public void FindCertainRelations_ReturnsEmptyWhenThereAreMultipleUnconstrainedSolutions()
    {
        var categories = CreateCategories();

        var relations = PuzzleSolver.FindCertainRelations(categories, Array.Empty<Clue>());

        Assert.Empty(relations);
    }

    [Fact]
    public void FindCertainRelations_ReturnsAllCertainRelationsForTwoByTwoPuzzle()
    {
        var categories = CreateCategories();
        var firstItem = new Item(0, 0, "A");
        var secondItem = new Item(1, 0, "X");
        var clues = new Clue[]
        {
            new SameClue(firstItem, secondItem)
        };

        var relations = PuzzleSolver.FindCertainRelations(categories, clues);

        Assert.Equal(4, relations.Count);

        Assert.Contains(relations, clue => IsSame(clue, 0, 0, 1, 0));
        Assert.Contains(relations, clue => IsSame(clue, 0, 1, 1, 1));
        Assert.Contains(relations, clue => IsDifferent(clue, 0, 0, 1, 1));
        Assert.Contains(relations, clue => IsDifferent(clue, 0, 1, 1, 0));
    }

    [Fact]
    public void FindCertainRelations_ReturnsEmptyWhenCluesAreContradictory()
    {
        var categories = CreateCategories();
        var firstItem = new Item(0, 0, "A");
        var secondItem = new Item(1, 0, "X");
        var clues = new Clue[]
        {
            new SameClue(firstItem, secondItem),
            new DifferentClue(firstItem, secondItem)
        };

        var relations = PuzzleSolver.FindCertainRelations(categories, clues);

        Assert.Empty(relations);
    }

    private static IReadOnlyList<Category> CreateCategories()
    {
        return new[]
        {
            new Category(0, "Category A", new[] { "A", "B" }),
            new Category(1, "Category B", new[] { "X", "Y" })
        };
    }

    private static bool IsSame(Clue clue, int categoryA, int indexA, int categoryB, int indexB)
    {
        return clue is SameClue &&
               clue.FirstItem.CategoryIndex == categoryA &&
               clue.FirstItem.Index == indexA &&
               clue.SecondItem.CategoryIndex == categoryB &&
               clue.SecondItem.Index == indexB;
    }

    private static bool IsDifferent(Clue clue, int categoryA, int indexA, int categoryB, int indexB)
    {
        return clue is DifferentClue &&
               clue.FirstItem.CategoryIndex == categoryA &&
               clue.FirstItem.Index == indexA &&
               clue.SecondItem.CategoryIndex == categoryB &&
               clue.SecondItem.Index == indexB;
    }
}