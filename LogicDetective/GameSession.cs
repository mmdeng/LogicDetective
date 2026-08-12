namespace LogicDetective;

internal sealed class GameSession
{
    public Puzzle Puzzle { get; }
    public PlayerState PlayerState { get; }
    public HintGenrator HintGenerator { get; }

    public GameSession(Puzzle puzzle)
    {
        Puzzle = puzzle ?? throw new ArgumentNullException(nameof(puzzle));
        PlayerState = new PlayerState();
        HintGenerator = new HintGenrator(Puzzle.Categories);
    }

    public int GetUnknownAnswerCount()
    {
        var pairs = Puzzle.Categories.EnumerateAllItemPairs();
        pairs = pairs.Where(m => PlayerState.GetState(m) == ReasoningState.Unknown);
        return pairs.Count();
    }

    // /// <summary>
    // /// 全カテゴリペアのグリッドについて、プレーヤーがPositiveをつけたセル周辺について、それ以外のセルをNegativeにする。
    // /// </summary>
    // /// <param name="session"></param>
    // public void SetNegativeWithPositive()
    // {
    //     PlayerState.SetNegativesOtherThanPositive(Puzzle.Categories);
    // }
}
