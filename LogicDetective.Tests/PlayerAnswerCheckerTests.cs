namespace LogicDetective.Tests;

public class PlayerAnswerCheckerTests
{
    [Fact]
    public void InitialState_IsIncomplete()
    {
        var puzzle = CreatePuzzle();
        var playerState = new PlayerState();
        var result = PlayerAnswerChecker.Check(puzzle, playerState);
        Assert.Equal(AnswerResult.Incomplete, result);
    }

    [Fact]
    public void CorrectYesOnlyInput_IsIncomplete()
    {
        var fixture = CreateFixture();
        fixture.PlayerState.SetState(fixture.A, fixture.X, PlayerPairState.Yes);
        var result = PlayerAnswerChecker.Check(fixture.Puzzle, fixture.PlayerState);
        Assert.Equal(AnswerResult.Incomplete, result);
    }

    [Fact]
    public void CorrectNoOnlyInput_IsIncomplete()
    {
        var fixture = CreateFixture();
        fixture.PlayerState.SetState(fixture.A, fixture.Y, PlayerPairState.No);
        var result = PlayerAnswerChecker.Check(fixture.Puzzle, fixture.PlayerState);
        Assert.Equal(AnswerResult.Incomplete, result);
    }

    [Fact]
    public void WrongYes_ReturnsIncorrect()
    {
        var fixture = CreateFixture();
        fixture.PlayerState.SetState(fixture.A, fixture.Y, PlayerPairState.Yes);
        var result = PlayerAnswerChecker.Check(fixture.Puzzle, fixture.PlayerState);
        Assert.Equal(AnswerResult.Incorrect, result);
    }

    [Fact]
    public void WrongNo_ReturnsIncorrect()
    {
        var fixture = CreateFixture();
        fixture.PlayerState.SetState(fixture.A, fixture.X, PlayerPairState.No);
        var result = PlayerAnswerChecker.Check(fixture.Puzzle, fixture.PlayerState);
        Assert.Equal(AnswerResult.Incorrect, result);
    }

    [Fact]
    public void AllCorrectInputs_ReturnsCorrect()
    {
        var fixture = CreateFixture();
        SetAllPairsFromSolution(fixture.Puzzle, fixture.PlayerState);
        var result = PlayerAnswerChecker.Check(fixture.Puzzle, fixture.PlayerState);
        Assert.Equal(AnswerResult.Correct, result);
    }

    [Fact]
    public void PartialInputs_ReturnsIncomplete()
    {
        var fixture = CreateFixture();
        SetAllPairsFromSolution(fixture.Puzzle, fixture.PlayerState);
        fixture.PlayerState.SetState(fixture.A, fixture.X, PlayerPairState.Unknown);
        var result = PlayerAnswerChecker.Check(fixture.Puzzle, fixture.PlayerState);
        Assert.Equal(AnswerResult.Incomplete, result);
    }

    [Fact]
    public void OneWrongAmongMultipleInputs_ReturnsIncorrect()
    {
        var fixture = CreateFixture();
        SetAllPairsFromSolution(fixture.Puzzle, fixture.PlayerState);
        fixture.PlayerState.SetState(fixture.A, fixture.Y, PlayerPairState.Yes);
        var result = PlayerAnswerChecker.Check(fixture.Puzzle, fixture.PlayerState);
        Assert.Equal(AnswerResult.Incorrect, result);
    }

    private static void SetAllPairsFromSolution(Puzzle puzzle, PlayerState playerState)
    {
        var categories = puzzle.Categories;
        var groupCount = puzzle.Solution.GroupCount;

        for (var categoryA = 0; categoryA < categories.Count; categoryA++)
        {
            for (var categoryB = categoryA + 1; categoryB < categories.Count; categoryB++)
            {
                for (var itemA = 0; itemA < groupCount; itemA++)
                {
                    for (var itemB = 0; itemB < groupCount; itemB++)
                    {
                        var firstItem = new Item(categoryA, itemA, categories[categoryA].Items[itemA]);
                        var secondItem = new Item(categoryB, itemB, categories[categoryB].Items[itemB]);

                        var sameGroup = puzzle.Solution.AreSameGroup(categoryA, itemA, categoryB, itemB);
                        var state = sameGroup ? PlayerPairState.Yes : PlayerPairState.No;
                        playerState.SetState(firstItem, secondItem, state);
                    }
                }
            }
        }
    }

    private static Puzzle CreatePuzzle()
    {
        return CreateFixture().Puzzle;
    }

    private static (Puzzle Puzzle, PlayerState PlayerState, Item A, Item X, Item Y) CreateFixture()
    {
        var categories = new[]
        {
            new Category(0, "People", ["A", "B"]),
            new Category(1, "Pets", ["X", "Y"]),
            new Category(2, "Drinks", ["T", "U"])
        };

        var solution = new Solution(groupCount: 2, categoryCount: 3);
        solution.SetItemIndex(0, 0, 0);
        solution.SetItemIndex(0, 1, 0);
        solution.SetItemIndex(0, 2, 0);
        solution.SetItemIndex(1, 0, 1);
        solution.SetItemIndex(1, 1, 1);
        solution.SetItemIndex(1, 2, 1);

        var puzzle = new Puzzle(categories, solution, clues: Array.Empty<Clue>());
        var a = new Item(0, 0, "A");
        var x = new Item(1, 0, "X");
        var y = new Item(1, 1, "Y");

        return (puzzle, new PlayerState(), a, x, y);
    }

    [Fact]
    public void FindIncorrectAnswers_WithWrongYes_ReturnsWrongAnswer()
    {
        var fixture = CreateFixture();
        fixture.PlayerState.SetState(fixture.A, fixture.Y, PlayerPairState.Yes);
        var incorrectAnswers = PlayerAnswerChecker.FindIncorrectAnswers(fixture.Puzzle, fixture.PlayerState);
        var answer = Assert.Single(incorrectAnswers);
        Assert.IsType<SameClue>(answer);
        Assert.Equal(fixture.A.CategoryIndex, answer.FirstItem.CategoryIndex);
        Assert.Equal(fixture.A.Index, answer.FirstItem.Index);
        Assert.Equal(fixture.Y.CategoryIndex, answer.SecondItem.CategoryIndex);
        Assert.Equal(fixture.Y.Index, answer.SecondItem.Index);
    }

    [Fact]
    public void FindIncorrectAnswers_WithWrongNo_ReturnsWrongAnswer()
    {
        var fixture = CreateFixture();
        fixture.PlayerState.SetState(fixture.A, fixture.X, PlayerPairState.No);
        var incorrectAnswers = PlayerAnswerChecker.FindIncorrectAnswers(fixture.Puzzle, fixture.PlayerState);
        var answer = Assert.Single(incorrectAnswers);
        Assert.IsType<DifferentClue>(answer);
        Assert.Equal(fixture.A.CategoryIndex, answer.FirstItem.CategoryIndex);
        Assert.Equal(fixture.A.Index, answer.FirstItem.Index);
        Assert.Equal(fixture.X.CategoryIndex, answer.SecondItem.CategoryIndex);
        Assert.Equal(fixture.X.Index, answer.SecondItem.Index);
    }

    [Fact]
    public void FindIncorrectAnswers_WithNoWrongAnswers_ReturnsEmpty()
    {
        var fixture = CreateFixture();
        fixture.PlayerState.SetState(fixture.A, fixture.X, PlayerPairState.Yes);
        var incorrectAnswers = PlayerAnswerChecker.FindIncorrectAnswers(fixture.Puzzle, fixture.PlayerState);
        Assert.Empty(incorrectAnswers);
    }

    [Fact]
    public void FindIncorrectAnswers_WithMultipleWrongAnswers_ReturnsAllWrongAnswers()
    {
        var fixture = CreateFixture();
        fixture.PlayerState.SetState(fixture.A, fixture.Y, PlayerPairState.Yes);
        fixture.PlayerState.SetState(fixture.A, fixture.X, PlayerPairState.No);

        var incorrectAnswers = PlayerAnswerChecker.FindIncorrectAnswers(fixture.Puzzle, fixture.PlayerState);

        Assert.Equal(2, incorrectAnswers.Count);
        Assert.Contains(incorrectAnswers, clue => clue is SameClue
            && clue.FirstItem.CategoryIndex == fixture.A.CategoryIndex
            && clue.FirstItem.Index == fixture.A.Index
            && clue.SecondItem.CategoryIndex == fixture.Y.CategoryIndex
            && clue.SecondItem.Index == fixture.Y.Index);
        Assert.Contains(incorrectAnswers, clue => clue is DifferentClue
            && clue.FirstItem.CategoryIndex == fixture.A.CategoryIndex
            && clue.FirstItem.Index == fixture.A.Index
            && clue.SecondItem.CategoryIndex == fixture.X.CategoryIndex
            && clue.SecondItem.Index == fixture.X.Index);
    }
}
