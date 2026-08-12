namespace LogicDetective.Tests;

public class GameSessionTests
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
        return new Puzzle(categories, answer, []);
    }

    [Fact]
    public void GetHintRelations_ExcludesOriginalPuzzleClues()
    {
        var puzzle = CreatePuzzle();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "10");
        var clue = new SameClue(itemPair);
        puzzle = new Puzzle(puzzle.Categories, puzzle.Answer, [clue]);
        var session = new GameSession(puzzle);

        var hintGenerator = new HintGenrator(puzzle.Categories);
        var hints = hintGenerator.GetHintRelations(session.PlayerState, puzzle.Clues);
        Assert.DoesNotContain(hints, m => m is SameClue && m.Pair.Equal(itemPair));
    }

    [Fact]
    public void GetHintRelations_ExcludesPlayerAnswers()
    {
        var puzzle = CreatePuzzle();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "10");
        var session = new GameSession(puzzle);
        session.PlayerState.SetState(new PlayerAction(ReasoningState.Positive, itemPair));

        var hintGenerator = new HintGenrator(puzzle.Categories);
        var hints = hintGenerator.GetHintRelations(session.PlayerState, puzzle.Clues);
        Assert.DoesNotContain(hints, m => m is SameClue && m.Pair.Equal(itemPair));
    }

    [Fact]
    public void GetHintRelations_IncludesRelationDerivedFromOriginalPuzzleClue()
    {
        var puzzle = CreatePuzzle();
        var itemPair0010 = puzzle.Categories.GetItemPair("a", "00", "b", "10");
        var itemPair0011 = puzzle.Categories.GetItemPair("a", "00", "b", "11");
        var clue = new SameClue(itemPair0010);
        puzzle = new Puzzle(puzzle.Categories, puzzle.Answer, [clue]);
        var session = new GameSession(puzzle);

        var hintGenerator = new HintGenrator(puzzle.Categories);
        var hints = hintGenerator.GetHintRelations(session.PlayerState, puzzle.Clues);
        //var hints = session.GetHintRelations();
        Assert.Contains(hints, m => m is DifferentClue && m.Pair.Equal(itemPair0011));
    }

    [Fact]
    public void GetHintRelations_ExcludesOriginalCluesAndPlayerAnswers()
    {
        var puzzle = CreatePuzzle();
        var itemPair0010 = puzzle.Categories.GetItemPair("a", "00", "b", "10");
        var itemPair0011 = puzzle.Categories.GetItemPair("a", "00", "b", "11");
        var clue = new SameClue(itemPair0010);
        puzzle = new Puzzle(puzzle.Categories, puzzle.Answer, [clue]);
        var session = new GameSession(puzzle);
        session.PlayerState.SetState(new PlayerAction(ReasoningState.Positive, itemPair0011));

        var hintGenerator = new HintGenrator(puzzle.Categories);
        var hints = hintGenerator.GetHintRelations(session.PlayerState, puzzle.Clues);
        Assert.DoesNotContain(hints, m => m is SameClue && m.Pair.Equal(itemPair0010));
        Assert.DoesNotContain(hints, m => m is SameClue && m.Pair.Equal(itemPair0011));
    }

    [Fact]
    public void CanGetCheckAnswerResult()
    {
        var puzzle = CreatePuzzle();
        var session = new GameSession(puzzle);
        var result = session.PlayerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incomplete, result);
    }

    [Fact]
    public void InitialState_IsIncomplete()
    {
        var puzzle = CreatePuzzle();
        var session = new GameSession(puzzle);
        var result = session.PlayerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incomplete, result);
    }

    [Fact]
    public void AllCorrectAnswers_ReturnsCorrect()
    {
        var puzzle = CreatePuzzle();
        var session = new GameSession(puzzle);
        session.PlayerState.SetAllCorrectStates(puzzle.Categories, puzzle.Answer);
        var result = session.PlayerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Correct, result);
    }

    [Fact]
    public void AnyWrongAnswer_ReturnsIncorrect()
    {
        var puzzle = CreatePuzzle();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "11");
        var session = new GameSession(puzzle);
        session.PlayerState.SetState(new PlayerAction(ReasoningState.Positive, itemPair));
        var result = session.PlayerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incorrect, result);
    }

    [Fact]
    public void GetIncorrectAnswers_ReturnsWrongAnswers()
    {
        var puzzle = CreatePuzzle();
        var session = new GameSession(puzzle);
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "11");
        session.PlayerState.SetState(new PlayerAction(ReasoningState.Positive, itemPair));
        var incorrectAnswers = session.PlayerState.FindIncorrectAnswers(puzzle);
        var answer = Assert.Single(incorrectAnswers);
        Assert.IsType<SameClue>(answer);
        Assert.True(answer.Pair.Equal(itemPair));
    }

    [Fact]
    public void CanFixIncorrectToCorrect()
    {
        var puzzle = CreatePuzzle();
        var session = new GameSession(puzzle);
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "11");

        session.PlayerState.SetState(new PlayerAction(ReasoningState.Positive, itemPair));
        var result = session.PlayerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incorrect, result);

        session.PlayerState.SetState(new PlayerAction(ReasoningState.Unknown, itemPair));
        session.PlayerState.SetAllCorrectStates(puzzle.Categories, puzzle.Answer);

        result = session.PlayerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Correct, result);
    }

    [Fact]
    public void IsCompleted_TrueOnlyAfterCorrect()
    {
        var puzzle = CreatePuzzle();
        var session = new GameSession(puzzle);
        session.PlayerState.SetAllCorrectStates(puzzle.Categories, puzzle.Answer);
        Assert.Equal(AnswerResult.Correct, session.PlayerState.CheckAnswer(puzzle));
    }

    [Fact]
    public void CheckAnswer_CanBeCalledRepeatedly_WithoutBreakingState()
    {
        var puzzle = CreatePuzzle();
        var session = new GameSession(puzzle);
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "10");

        session.PlayerState.SetAllCorrectStates(puzzle.Categories, puzzle.Answer);

        Assert.Equal(AnswerResult.Correct, session.PlayerState.CheckAnswer(puzzle));
        Assert.Equal(AnswerResult.Correct, session.PlayerState.CheckAnswer(puzzle));
        Assert.Equal(ReasoningState.Positive, session.PlayerState.GetState(itemPair));
    }

    [Fact]
    public void GetNextHint_DoesNotRepeatWithinSession()
    {
        var puzzle = CreatePuzzle();
        var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "10");
        var clue = new SameClue(itemPair);
        puzzle = new Puzzle(puzzle.Categories, puzzle.Answer, [clue]);
        var session = new GameSession(puzzle);

        var hintGenerator = new HintGenrator(puzzle.Categories);

        var hint1 = hintGenerator.GetNextHint(session.PlayerState, session.Puzzle.Clues);
        Assert.NotNull(hint1);
        var hint2 = hintGenerator.GetNextHint(session.PlayerState, session.Puzzle.Clues);
        Assert.NotNull(hint2);

        Assert.NotEqual(hint1.ToString(), hint2.ToString());
    }
}

// [Fact]
// public void GetNextHint_HistoryIsIndependentBetweenSessions()
// {
//     var puzzle = CreatePuzzle();
//     var itemPair = puzzle.Categories.GetItemPair("a", "00", "b", "10");
//     var clue = new SameClue(itemPair);
//     puzzle = new Puzzle(puzzle.Categories, puzzle.Answer, [clue]);
//     var session1 = new GameSession(puzzle);
//     var session2 = new GameSession(puzzle);

//     var hintGenerator1 = new HintGenrator(session1.Puzzle.Categories);
//     var hint = hintGenerator.GetNextHint(session.PlayerState, session.Puzzle.Clues);

//     var hint1 = session1.GetNextHint();
//     var hint2 = session2.GetNextHint();

//     Assert.NotNull(hint1);
//     Assert.NotNull(hint2);
//     Assert.Equal(hint1.ToString(), hint2.ToString());
// }