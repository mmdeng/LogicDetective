namespace LogicDetective.Tests;

public class GameSessionTests
{
    [Fact]
    public void CanCreateGameSessionWithPuzzle()
    {
        var puzzle = CreateFixture().Puzzle;
        var session = new GameSession(puzzle);
        Assert.Same(puzzle, session.Puzzle);
        Assert.NotNull(session.PlayerState);
    }

    [Fact]
    public void InitialPlayerState_IsUnknown()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);
        Assert.Equal(PlayerPairState.Unknown, session.PlayerState.GetState(fixture.A, fixture.X));
    }

    [Fact]
    public void ApplyAction_ChangesPlayerState()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);
        session.ApplyAction(new PlayerAction(PlayerActionType.SetYes, fixture.A, fixture.X));
        Assert.Equal(PlayerPairState.Yes, session.PlayerState.GetState(fixture.A, fixture.X));
    }

    [Fact]
    public void GetCertainRelations_ReturnsRelationsForcedByPlayerState()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);
        session.ApplyAction(new PlayerAction(PlayerActionType.SetYes, fixture.A, fixture.X));
        var relations = session.GetCertainRelations();

        Assert.Contains(relations, clue =>
            clue is SameClue
            && clue.FirstItem.CategoryIndex == fixture.A.CategoryIndex
            && clue.FirstItem.Index == fixture.A.Index
            && clue.SecondItem.CategoryIndex == fixture.X.CategoryIndex
            && clue.SecondItem.Index == fixture.X.Index);

        Assert.Contains(relations, clue =>
            clue is DifferentClue
            && clue.FirstItem.CategoryIndex == fixture.A.CategoryIndex
            && clue.FirstItem.Index == fixture.A.Index
            && clue.SecondItem.CategoryIndex == fixture.Y.CategoryIndex
            && clue.SecondItem.Index == fixture.Y.Index);
    }

    [Fact]
    public void GetCertainRelations_ReturnsRelationsForcedByPuzzleClues()
    {
        var fixture = CreateFixtureWithClue();
        var session = new GameSession(fixture.Puzzle);
        var relations = session.GetCertainRelations();

        Assert.Contains(relations, clue =>
            clue is SameClue
            && clue.FirstItem.CategoryIndex == fixture.A.CategoryIndex
            && clue.FirstItem.Index == fixture.A.Index
            && clue.SecondItem.CategoryIndex == fixture.X.CategoryIndex
            && clue.SecondItem.Index == fixture.X.Index);

        Assert.Contains(relations, clue =>
            clue is DifferentClue
            && clue.FirstItem.CategoryIndex == fixture.A.CategoryIndex
            && clue.FirstItem.Index == fixture.A.Index
            && clue.SecondItem.CategoryIndex == fixture.Y.CategoryIndex
            && clue.SecondItem.Index == fixture.Y.Index);
    }
    [Fact]
    public void GetHintRelations_ExcludesOriginalPuzzleClues()
    {
        var fixture = CreateFixtureWithClue();
        var session = new GameSession(fixture.Puzzle);
        var relations = session.GetHintRelations();
        Assert.DoesNotContain(relations, clue =>
            clue is SameClue
            && clue.FirstItem.CategoryIndex == fixture.A.CategoryIndex
            && clue.FirstItem.Index == fixture.A.Index
            && clue.SecondItem.CategoryIndex == fixture.X.CategoryIndex
            && clue.SecondItem.Index == fixture.X.Index);
    }

    [Fact]
    public void GetHintRelations_ExcludesPlayerAnswers()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);
        session.ApplyAction(new PlayerAction(PlayerActionType.SetYes, fixture.A, fixture.X));
        var relations = session.GetHintRelations();
        Assert.DoesNotContain(relations, clue =>
            clue is SameClue
            && clue.FirstItem.CategoryIndex == fixture.A.CategoryIndex
            && clue.FirstItem.Index == fixture.A.Index
            && clue.SecondItem.CategoryIndex == fixture.X.CategoryIndex
            && clue.SecondItem.Index == fixture.X.Index);
    }

    [Fact]
    public void GetHintRelations_IncludesRelationDerivedFromOriginalPuzzleClue()
    {
        var fixture = CreateFixtureWithClue();
        var session = new GameSession(fixture.Puzzle);
        var relations = session.GetHintRelations();
        Assert.Contains(relations, clue =>
            clue is DifferentClue
            && clue.FirstItem.CategoryIndex == fixture.A.CategoryIndex
            && clue.FirstItem.Index == fixture.A.Index
            && clue.SecondItem.CategoryIndex == fixture.Y.CategoryIndex
            && clue.SecondItem.Index == fixture.Y.Index);
    }

    [Fact]
    public void GetHintRelations_ExcludesOriginalCluesAndPlayerAnswers()
    {
        var fixture = CreateFixtureWithClue();
        var session = new GameSession(fixture.Puzzle);
        session.ApplyAction(new PlayerAction(PlayerActionType.SetYes, fixture.A, fixture.Y));
        var relations = session.GetHintRelations();
        Assert.DoesNotContain(relations, clue =>
            clue is SameClue
            && clue.FirstItem.CategoryIndex == fixture.A.CategoryIndex
            && clue.FirstItem.Index == fixture.A.Index
            && clue.SecondItem.CategoryIndex == fixture.X.CategoryIndex
            && clue.SecondItem.Index == fixture.X.Index);

        Assert.DoesNotContain(relations, clue =>
            clue is SameClue
            && clue.FirstItem.CategoryIndex == fixture.A.CategoryIndex
            && clue.FirstItem.Index == fixture.A.Index
            && clue.SecondItem.CategoryIndex == fixture.Y.CategoryIndex
            && clue.SecondItem.Index == fixture.Y.Index);
    }

    [Fact]
    public void CanGetCheckAnswerResult()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);
        var result = session.CheckAnswer();
        Assert.Equal(AnswerResult.Incomplete, result);
    }

    [Fact]
    public void InitialState_IsIncomplete()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);
        Assert.Equal(AnswerResult.Incomplete, session.CheckAnswer());
        Assert.False(session.IsCompleted);
    }

    [Fact]
    public void AllCorrectAnswers_ReturnsCorrect()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);
        ApplyAllCorrectActions(session, fixture.Puzzle);
        Assert.Equal(AnswerResult.Correct, session.CheckAnswer());
    }

    [Fact]
    public void AnyWrongAnswer_ReturnsIncorrect()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);
        session.ApplyAction(new PlayerAction(PlayerActionType.SetYes, fixture.A, fixture.Y));
        Assert.Equal(AnswerResult.Incorrect, session.CheckAnswer());
        Assert.False(session.IsCompleted);
    }

    [Fact]
    public void GetIncorrectAnswers_ReturnsWrongAnswers()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);
        session.ApplyAction(new PlayerAction(PlayerActionType.SetYes, fixture.A, fixture.Y));
        var incorrectAnswers = session.GetIncorrectAnswers();
        var answer = Assert.Single(incorrectAnswers);
        Assert.IsType<SameClue>(answer);
        Assert.Equal(fixture.A.CategoryIndex, answer.FirstItem.CategoryIndex);
        Assert.Equal(fixture.A.Index, answer.FirstItem.Index);
        Assert.Equal(fixture.Y.CategoryIndex, answer.SecondItem.CategoryIndex);
        Assert.Equal(fixture.Y.Index, answer.SecondItem.Index);
    }

    [Fact]
    public void CanFixIncorrectToCorrect()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);

        session.ApplyAction(new PlayerAction(PlayerActionType.SetYes, fixture.A, fixture.Y));
        Assert.Equal(AnswerResult.Incorrect, session.CheckAnswer());

        session.ApplyAction(new PlayerAction(PlayerActionType.Clear, fixture.A, fixture.Y));
        ApplyAllCorrectActions(session, fixture.Puzzle);

        Assert.Equal(AnswerResult.Correct, session.CheckAnswer());
    }

    [Fact]
    public void IsCompleted_TrueOnlyAfterCorrect()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);
        Assert.False(session.IsCompleted);

        ApplyAllCorrectActions(session, fixture.Puzzle);
        Assert.Equal(AnswerResult.Correct, session.CheckAnswer());
        Assert.True(session.IsCompleted);
    }

    [Fact]
    public void ApplyAction_AfterCorrect_IsRejected()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);

        ApplyAllCorrectActions(session, fixture.Puzzle);
        Assert.Equal(AnswerResult.Correct, session.CheckAnswer());

        Assert.Throws<InvalidOperationException>(() =>
            session.ApplyAction(new PlayerAction(PlayerActionType.Clear, fixture.A, fixture.X)));
    }

    [Fact]
    public void CheckAnswer_CanBeCalledRepeatedly_WithoutBreakingState()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);

        ApplyAllCorrectActions(session, fixture.Puzzle);

        var first = session.CheckAnswer();
        var second = session.CheckAnswer();
        Assert.Equal(AnswerResult.Correct, first);
        Assert.Equal(AnswerResult.Correct, second);
        Assert.True(session.IsCompleted);
        Assert.Equal(PlayerPairState.Yes, session.PlayerState.GetState(fixture.A, fixture.X));
    }

    [Fact]
    public void GetNextHint_DoesNotRepeatWithinSession()
    {
        var fixture = CreateFixtureWithClue();
        var session = new GameSession(fixture.Puzzle);

        var first = session.GetNextHint();
        var second = session.GetNextHint();

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.NotEqual(first.ToString(), second.ToString());
    }
    [Fact]
    public void GetNextHint_HistoryIsIndependentBetweenSessions()
    {
        var fixture = CreateFixtureWithClue();
        var firstSession = new GameSession(fixture.Puzzle);
        var secondSession = new GameSession(fixture.Puzzle);

        var firstHint = firstSession.GetNextHint();
        var secondHint = secondSession.GetNextHint();

        Assert.NotNull(firstHint);
        Assert.NotNull(secondHint);
        Assert.Equal(firstHint.ToString(), secondHint.ToString());
    }

    private static void ApplyAllCorrectActions(GameSession session, Puzzle puzzle)
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
                        var actionType = sameGroup ? PlayerActionType.SetYes : PlayerActionType.SetNo;
                        session.ApplyAction(new PlayerAction(actionType, firstItem, secondItem));
                    }
                }
            }
        }
    }

    private static (Puzzle Puzzle, Item A, Item X, Item Y) CreateFixture()
    {
        var categories = new[]
        {
            new Category(0, "People", new[] { "A", "B" }),
            new Category(1, "Pets", new[] { "X", "Y" }),
            new Category(2, "Drinks", new[] { "T", "U" })
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
        return (puzzle, a, x, y);
    }
    [Fact]
    public void GetUnknownAnswerCount_WithNoAnswers_ReturnsAllRelations()
    {
        var fixture = CreateFixture();
        var session = new GameSession(fixture.Puzzle);
        var unknownCount = session.GetUnknownAnswerCount();
        Assert.Equal(12, unknownCount);
    }
    private static (Puzzle Puzzle, Item A, Item X, Item Y) CreateFixtureWithClue()
    {
        var fixture = CreateFixture();
        var clue = new SameClue(fixture.A, fixture.X);
        var puzzle = new Puzzle(fixture.Puzzle.Categories, fixture.Puzzle.Solution, [clue]);
        return (puzzle, fixture.A, fixture.X, fixture.Y);
    }
}
