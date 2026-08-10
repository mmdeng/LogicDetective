namespace LogicDetective.Tests;

public class PuzzleSolverInferenceTests
{
    [Fact]
    public void FindCertainRelations_WithSameClue_FindsSameAndDifferentRelations()
    {
        var categories = CreateCategories();

        var firstItem = new Item(0, 0, "A");
        var secondItem = new Item(1, 0, "X");

        var clues = new List<Clue>
        {
            new SameClue(firstItem, secondItem)
        };

        var relations = PuzzleSolver.FindCertainRelations(
            categories,
            clues);

        Assert.Contains(
            relations,
            clue =>
                clue is SameClue &&
                clue.FirstItem.CategoryIndex == 0 &&
                clue.FirstItem.Index == 0 &&
                clue.SecondItem.CategoryIndex == 1 &&
                clue.SecondItem.Index == 0);

        Assert.Contains(
            relations,
            clue =>
                clue is DifferentClue &&
                clue.FirstItem.CategoryIndex == 0 &&
                clue.FirstItem.Index == 0 &&
                clue.SecondItem.CategoryIndex == 1 &&
                clue.SecondItem.Index == 1);
    }

    [Fact]
    public void FindCertainRelations_WithInsufficientInformation_DoesNotInventRelations()
    {
        var categories = CreateCategories();
        var relations = PuzzleSolver.FindCertainRelations(categories, Array.Empty<Clue>());
        Assert.Empty(relations);
    }

    [Fact]
    public void FindCertainRelations_WithContradictoryClues_ReturnsNoRelations()
    {
        var categories = CreateCategories();
        var firstItem = new Item(0, 0, "A");
        var secondItem = new Item(1, 0, "X");

        var clues = new List<Clue>
        {
            new SameClue(firstItem, secondItem),
            new DifferentClue(firstItem, secondItem)
        };
        var relations = PuzzleSolver.FindCertainRelations(categories, clues);
        Assert.Empty(relations);
    }

    [Fact]
    public void FindCertainRelations_DoesNotReturnRelationsWithinSameCategory()
    {
        var categories = CreateCategories();
        var relations = PuzzleSolver.FindCertainRelations(categories, Array.Empty<Clue>());
        Assert.All(
            relations,
            clue =>
            {
                Assert.NotEqual(
                    clue.FirstItem.CategoryIndex,
                    clue.SecondItem.CategoryIndex);
            });
    }

    private static IReadOnlyList<Category> CreateCategories()
    {
        return
        [
            new Category(0, "Category A", ["A", "B"]),
            new Category(1, "Category B", ["X", "Y"])
        ];
    }
}
