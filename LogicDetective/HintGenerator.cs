namespace LogicDetective;

internal class HintGenrator
{
    public CategoryList Categories { get; }
    private readonly HintTracker _hintTracker = new();

    public HintGenrator(CategoryList categories)
    {
        Categories = categories;
    }

    public Clue? GetNextHint(PlayerState playerState, IEnumerable<Clue> givenClues)
    {
        var relations = GetHintRelations(playerState, givenClues);
        if (relations.Count == 0) return null;

        return _hintTracker.GetNext(relations);
    }

    public IReadOnlyList<Clue> GetHintRelations(PlayerState playerState, IEnumerable<Clue> givenClues)
    {
        // 現在のプレイヤーによる推理済みのセルの状態から手掛かりを作成する。
        var clues = playerState.GetCluesWithPlayerState(Categories).ToList();

        // 最初から与えられている手掛かりを含める。
        var knownRelations = givenClues.Concat(clues).ToList();

        // まずは現在のプレイヤーが知っている手掛かりを使って、パズルを最後まで解く。
        // プレイヤーの推理が間違っている場合は複数解になる可能性がある。
        var solver = new PuzzleSolver(Categories);
        var answers = solver.Solve(knownRelations);
        if (answers.Count == 0) return [];

        // パズルを解いた後の全てのセルについて、手掛かりを作成する。
        var certainRelations = FindCertainRelations(answers).ToList();
        if (certainRelations.Count == 0) return [];

        // 現在のプレイヤーが知っている手掛かりを除外する。
        return certainRelations.Where(relation => !knownRelations.Any(known => AreSameRelation(known, relation))).ToList();
    }

    private static bool AreSameRelation(Clue clue1, Clue clue2)
    {
        if (clue1.GetType() != clue2.GetType())
        {
            return false;
        }
        return AreSameItems(clue1.Pair.Item1, clue2.Pair.Item1) && AreSameItems(clue1.Pair.Item2, clue2.Pair.Item2);
    }

    private static bool AreSameItems(Item item1, Item item2)
    {
        return item1.CategoryIndex == item2.CategoryIndex && item1.ItemIndex == item2.ItemIndex;
    }

    /// <summary>
    /// ヒントを作成するためのロジック
    /// 現在のプレイヤーによる推理済みのセルの状態から一旦パズルを解き、全てのアイテムペアについて、手掛かりを作成する。
    /// </summary>
    /// <param name="categories"></param>
    /// <param name="cluesWithPlayerState">現在のプレイヤーによる推理済みのセルの状態から作成した手掛かり</param>
    /// <returns></returns>
    public IEnumerable<Clue> FindCertainRelations(IReadOnlyList<Answer> answers)
    {
        if (answers.Count == 0) yield break;

        // パズルを最後まで解いた状態から、全てのアイテムペアについて、手掛かりを作成する。
        // プレーヤーにより推理済みセルも、未推理のセルも両方とも作成する。
        // 複数解になっている場合は、全ての解で同じ関係になるアイテムペアのみを手掛かりとして作成する。
        foreach (var itemPair in Categories.EnumerateAllItemPairs())
        {
            var isSame = answers[0].AreSameGroup(itemPair);
            var isCertain = true;

            for (var answerIndex = 1; answerIndex < answers.Count; answerIndex++)
            {
                var relation = answers[answerIndex].AreSameGroup(itemPair);

                if (relation != isSame)
                {
                    isCertain = false;
                    break;
                }
            }
            if (!isCertain) continue;
            if (isSame)
            {
                yield return new SameClue(itemPair);
            }
            else
            {
                yield return new DifferentClue(itemPair);
            }
        }
    }
}