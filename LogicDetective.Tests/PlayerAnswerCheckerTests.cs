namespace LogicDetective.Tests;

public class PlayerAnswerCheckerTests
{
    private static Puzzle CreatePuzzle()
    {
        var categories = new CategoryList
        {
            { "a", ["00", "01"] },
            { "b", ["10", "11"] },
            { "c", ["20", "21"] }
        };
        var answer = new Answer(categories);
        answer.SetItemIndex(0, 0, 0);
        answer.SetItemIndex(0, 1, 0);
        answer.SetItemIndex(0, 2, 0);
        answer.SetItemIndex(1, 0, 1);
        answer.SetItemIndex(1, 1, 1);
        answer.SetItemIndex(1, 2, 1);
        return new Puzzle(categories, answer, clues: []);
    }

    [Fact]
    public void InitialState_IsIncomplete()
    {
        var puzzle = CreatePuzzle();
        var playerState = new PlayerState();
        var result = playerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incomplete, result);
    }

    [Fact]
    public void CorrectYesOnlyInput_IsIncomplete()
    {
        var puzzle = CreatePuzzle();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "10");
        var playerState = new PlayerState();
        playerState.SetState(itemPair, ReasoningState.Positive);
        var result = playerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incomplete, result);
    }

    [Fact]
    public void CorrectNoOnlyInput_IsIncomplete()
    {
        var puzzle = CreatePuzzle();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "11");
        var playerState = new PlayerState();
        playerState.SetState(itemPair, ReasoningState.Negative);
        var result = playerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incomplete, result);
    }

    [Fact]
    public void WrongYes_ReturnsIncorrect()
    {
        var puzzle = CreatePuzzle();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "11");
        var playerState = new PlayerState();
        playerState.SetState(itemPair, ReasoningState.Positive);
        var result = playerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incorrect, result);
    }

    [Fact]
    public void WrongNo_ReturnsIncorrect()
    {
        var puzzle = CreatePuzzle();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "10");
        var playerState = new PlayerState();
        playerState.SetState(itemPair, ReasoningState.Negative);
        var result = playerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incorrect, result);
    }

    [Fact]
    public void AllCorrectInputs_ReturnsCorrect()
    {
        var puzzle = CreatePuzzle();
        var playerState = new PlayerState();
        playerState.SetAllCorrectStates(puzzle.Categories, puzzle.Answer);
        var result = playerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Correct, result);
    }

    [Fact]
    public void PartialInputs_ReturnsIncomplete()
    {
        var puzzle = CreatePuzzle();
        var playerState = new PlayerState();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "10");
        playerState.SetAllCorrectStates(puzzle.Categories, puzzle.Answer);
        playerState.SetState(itemPair, ReasoningState.Unknown);
        var result = playerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incomplete, result);
    }

    [Fact]
    public void OneWrongAmongMultipleInputs_ReturnsIncorrect()
    {
        var puzzle = CreatePuzzle();
        var playerState = new PlayerState();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "11");
        playerState.SetAllCorrectStates(puzzle.Categories, puzzle.Answer);
        playerState.SetState(itemPair, ReasoningState.Positive);
        var result = playerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incorrect, result);
    }

    [Fact]
    public void FindIncorrectAnswers_WithWrongYes_ReturnsWrongAnswer()
    {
        var puzzle = CreatePuzzle();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "11");
        var playerState = new PlayerState();
        playerState.SetState(itemPair, ReasoningState.Positive);
        var incorrectAnswers = playerState.FindIncorrectAnswers(puzzle);
        var answer = Assert.Single(incorrectAnswers);
        Assert.IsType<SameClue>(answer);
        Assert.True(itemPair.Equal(answer.Pair));
    }

    [Fact]
    public void FindIncorrectAnswers_WithWrongNo_ReturnsWrongAnswer()
    {
        var puzzle = CreatePuzzle();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "10");
        var playerState = new PlayerState();
        playerState.SetState(itemPair, ReasoningState.Negative);
        var incorrectAnswers = playerState.FindIncorrectAnswers(puzzle);
        var answer = Assert.Single(incorrectAnswers);
        Assert.IsType<DifferentClue>(answer);
        Assert.True(itemPair.Equal(answer.Pair));
    }

    [Fact]
    public void FindIncorrectAnswers_WithNoWrongAnswers_ReturnsEmpty()
    {
        var puzzle = CreatePuzzle();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "10");
        var playerState = new PlayerState();
        playerState.SetState(itemPair, ReasoningState.Positive);
        var incorrectAnswers = playerState.FindIncorrectAnswers(puzzle);
        Assert.Empty(incorrectAnswers);
    }

    [Fact]
    public void FindIncorrectAnswers_WithMultipleWrongAnswers_ReturnsAllWrongAnswers()
    {
        var puzzle = CreatePuzzle();
        var itemPair0010 = puzzle.Categories.GetItemPair("a", "00", "b", "10");
        var itemPair0011 = puzzle.Categories.GetItemPair("a", "00", "b", "11");
        var playerState = new PlayerState();
        playerState.SetState(itemPair0011, ReasoningState.Positive);
        playerState.SetState(itemPair0010, ReasoningState.Negative);

        var incorrectAnswers = playerState.FindIncorrectAnswers(puzzle);

        Assert.Equal(2, incorrectAnswers.Count);
        Assert.Contains(incorrectAnswers, m => m is SameClue && m.Pair.Equal(itemPair0011));
        Assert.Contains(incorrectAnswers, m => m is DifferentClue && m.Pair.Equal(itemPair0010));
    }
}
