namespace LogicDetective;

internal sealed class GameSession
{
    public Puzzle Puzzle { get; }
    public PlayerState PlayerState { get; }
    public bool IsCompleted { get; private set; }
    private readonly HintTracker _hintTracker = new();

    public GameSession(Puzzle puzzle)
    {
        Puzzle = puzzle ?? throw new ArgumentNullException(nameof(puzzle));
        PlayerState = new PlayerState();
    }

    public void ApplyAction(PlayerAction action)
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("完了したセッションは変更できません。");
        }
        PlayerActionProcessor.Apply(PlayerState, action);
    }

    public IReadOnlyList<Clue> GetCertainRelations()
    {
        var clues = Puzzle.Clues.Concat(PlayerState.ToClues(Puzzle.Categories)).ToList();
        return PuzzleSolver.FindCertainRelations(Puzzle.Categories, clues);
    }

    public IReadOnlyList<Clue> GetHintRelations()
    {
        var certainRelations = GetCertainRelations();
        var knownRelations = Puzzle.Clues.Concat(PlayerState.ToClues(Puzzle.Categories)).ToList();
        return certainRelations.Where(relation => !knownRelations.Any(known => AreSameRelation(known, relation))).ToList();
    }

    public Clue? GetNextHint()
    {
        var relations = GetHintRelations();
        if (relations.Count == 0)
        {
            return null;
        }
        return _hintTracker.GetNext(relations);
    }

    private static bool AreSameRelation(Clue first, Clue second)
    {
        if (first.GetType() != second.GetType())
        {
            return false;
        }
        return AreSameItems(first.FirstItem, second.FirstItem) && AreSameItems(first.SecondItem, second.SecondItem);
    }

    private static bool AreSameItems(Item first, Item second)
    {
        return first.CategoryIndex == second.CategoryIndex && first.Index == second.Index;
    }

    public IReadOnlyList<Clue> GetIncorrectAnswers()
    {
        return PlayerAnswerChecker.FindIncorrectAnswers(Puzzle, PlayerState);
    }

    public AnswerResult CheckAnswer()
    {
        var result = PlayerAnswerChecker.Check(Puzzle, PlayerState);
        if (result == AnswerResult.Correct) IsCompleted = true;
        return result;
    }

    public int GetUnknownAnswerCount()
    {
        var count = 0;
        for (var categoryA = 0; categoryA < Puzzle.Categories.Count; categoryA++)
        {
            for (var categoryB = categoryA + 1; categoryB < Puzzle.Categories.Count; categoryB++)
            {
                for (var itemA = 0; itemA < Puzzle.Categories[categoryA].Items.Count; itemA++)
                {
                    for (var itemB = 0; itemB < Puzzle.Categories[categoryB].Items.Count; itemB++)
                    {
                        var firstItem = new Item(categoryA, itemA, Puzzle.Categories[categoryA].Items[itemA]);
                        var secondItem = new Item(categoryB, itemB, Puzzle.Categories[categoryB].Items[itemB]);
                        if (PlayerState.GetState(firstItem, secondItem) == PlayerPairState.Unknown)
                        {
                            count++;
                        }
                    }
                }
            }
        }
        return count;
    }
}
