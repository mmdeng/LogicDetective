namespace LogicDetective;

internal sealed class PlayerState
{
    private readonly Dictionary<PairKey, ReasoningState> _states = [];

    public int GetAnsweredCount()
    {
        return _states.Count;
    }
    public ReasoningState GetState(Item item1, Item item2)
    {
        var key = CreateKey(item1, item2);
        return _states.TryGetValue(key, out var state) ? state : ReasoningState.Unknown;
    }

    public ReasoningState GetState(ItemPair itemPair)
    {
        return GetState(itemPair.Item1, itemPair.Item2);
    }

    public void SetState(Item item1, Item item2, ReasoningState state)
    {
        var key = CreateKey(item1, item2);
        if (state == ReasoningState.Unknown)
        {
            _states.Remove(key);
            return;
        }
        _states[key] = state;
    }

    public void SetState(ItemPair itemPair, ReasoningState state)
    {
        SetState(itemPair.Item1, itemPair.Item2, state);
    }

    public void SetState(PlayerAction action)
    {
        SetState(action.Pair, action.Type);
    }

    public void SetAllCorrectStates(CategoryList categories, Answer answer)
    {
        foreach (var itemPair in categories.EnumerateAllItemPairs())
        {
            var sameGroup = answer.AreSameGroup(itemPair);
            var actionType = sameGroup ? ReasoningState.Positive : ReasoningState.Negative;
            SetState(new PlayerAction(actionType, itemPair));
        }
    }

    /// <summary>
    /// 現在のプレイヤーによる推理済みのセルの状態から手掛かりを作成する。
    /// </summary>
    /// <param name="categories"></param>
    /// <returns></returns>
    public IEnumerable<Clue> GetCluesWithPlayerState(CategoryList categories)
    {
        foreach (var pair in _states)
        {
            var itemPair = categories.GetItemPair(pair.Key);
            if (pair.Value == ReasoningState.Positive)
            {
                yield return new SameClue(itemPair);
            }
            else if (pair.Value == ReasoningState.Negative)
            {
                yield return new DifferentClue(itemPair);
            }
        }
    }

    private static PairKey CreateKey(Item item1, Item item2)
    {
        if (item1.IsSame(item2))
        {
            throw new ArgumentException("同一項目の組み合わせは指定できません。");
        }
        if (item1.Compare(item2) <= 0)
        {
            return new PairKey(item1, item2);
        }
        else
        {
            return new PairKey(item2, item1);
        }
    }

    /// <summary>
    /// 全カテゴリペアのグリッドについて、Positive周辺のそれ以外のセルをNegativeにする。
    /// </summary>
    /// <param name="categories"></param>
    public void SetNegativesOtherThanPositive(CategoryList categories)
    {
        foreach (var categoryPair in categories.EnumerateAllCategoryPairs())
        {
            SetNegativesOtherThanPositive(categoryPair.Category1, categoryPair.Category2);
            SetNegativesOtherThanPositive(categoryPair.Category2, categoryPair.Category1);
        }
    }

    /// <summary>
    /// 指定カテゴリペアのグリッドについて、プレーヤーがPositiveをつけたセル周辺について、それ以外のセルをNegativeにする。
    /// </summary>
    /// <param name="category1"></param>
    /// <param name="category2"></param>
    void SetNegativesOtherThanPositive(Category category1, Category category2)
    {
        foreach (var item1 in category1.Items)
        {
            // 1行からPositiveを検索する。
            var positive = category2.Items.FirstOrDefault(m => GetState(item1, m) == ReasoningState.Positive);

            // Positiveがついていないところは除外。
            if (positive is null) continue;

            // PositiveがついているところはUnknownをNegativeに変える。
            foreach (var item2 in category2.Items)
            {
                if (GetState(item1, item2) == ReasoningState.Unknown)
                {
                    SetState(item1, item2, ReasoningState.Negative);
                }
            }
        }
    }

    /// <summary>
    /// 全盤面について正解であることを確認する。
    /// </summary>
    /// <param name="puzzle"></param>
    /// <returns></returns>
    public AnswerResult CheckAnswer(Puzzle puzzle)
    {
        var hasUnknown = false;
        foreach (var itemPair in puzzle.Categories.EnumerateAllItemPairs())
        {
            var state = GetState(itemPair);
            if (state == ReasoningState.Unknown)
            {
                hasUnknown = true;
                continue; // Incorrectも見つけたいので継続する。
            }
            var sameGroup = puzzle.Answer.AreSameGroup(itemPair);

            if (state == ReasoningState.Positive && !sameGroup) return AnswerResult.Incorrect;
            if (state == ReasoningState.Negative && sameGroup) return AnswerResult.Incorrect;
        }
        return hasUnknown ? AnswerResult.Incomplete : AnswerResult.Correct;
    }

    public IReadOnlyList<Clue> FindIncorrectAnswers(Puzzle puzzle)
    {
        var incorrectAnswers = new List<Clue>();
        foreach (var itemPair in puzzle.Categories.EnumerateAllItemPairs())
        {
            var state = GetState(itemPair);
            if (state == ReasoningState.Unknown)
            {
                continue;
            }
            var sameGroup = puzzle.Answer.AreSameGroup(itemPair);

            if (state == ReasoningState.Positive && !sameGroup)
            {
                incorrectAnswers.Add(new SameClue(itemPair));
            }
            else if (state == ReasoningState.Negative && sameGroup)
            {
                incorrectAnswers.Add(new DifferentClue(itemPair));
            }
        }
        return incorrectAnswers;
    }

    public void AutoSolve(CategoryList categories)
    {
        SetNegativesOtherThanPositive(categories);
        ChainReaction(categories);
        SetPositiveIfOthersNegative(categories);
    }
    public void ChainReaction(CategoryList categories)
    {
        foreach (var itemPairA in categories.EnumerateAllItemPairs())
        {
            var stateA = GetState(itemPairA);
            if (stateA != ReasoningState.Positive) continue;

            foreach (var categoryPairB in categories.EnumerateAllCategoryPairs())
            {
                ChainReaction(categories, itemPairA, categoryPairB, true);
                ChainReaction(categories, itemPairA, categoryPairB, false);
            }
        }
        SetNegativesOtherThanPositive(categories);
    }
    void ChainReaction(CategoryList categories, ItemPair itemPairA, CategoryPair categoryPairB, bool first)
    {
        if (categoryPairB.Get1(first).Index == itemPairA.Get1(first).CategoryIndex) return;

        var itemB2 = categoryPairB.Get2(first).Items[itemPairA.Get2(first).ItemIndex];
        foreach (var itemB1 in categoryPairB.Get1(first).Items)
        {
            var stateB = GetState(itemB1, itemB2);
            if (stateB != ReasoningState.Positive) continue;

            foreach (var categoryPairC in categories.EnumerateAllCategoryPairs())
            {
                if (categoryPairC.Get1(first).Index != categoryPairB.Get1(first).Index) continue;
                if (categoryPairC.Get2(first).Index != itemPairA.Get1(first).CategoryIndex) continue;

                var itemC1 = categoryPairC.Get1(first).Items[itemB1.ItemIndex];
                var itemC2 = categoryPairC.Get2(first).Items[itemPairA.Get1(first).ItemIndex];

                if (GetState(itemC1, itemC2) == ReasoningState.Unknown)
                {
                    SetState(itemC1, itemC2, ReasoningState.Positive);
                }
            }
        }
    }

    /// <summary>
    /// 全カテゴリペアのグリッドについて、Positive周辺のそれ以外のセルをNegativeにする。
    /// </summary>
    /// <param name="categories"></param>
    public void SetPositiveIfOthersNegative(CategoryList categories)
    {
        foreach (var categoryPair in categories.EnumerateAllCategoryPairs())
        {
            SetPositiveIfOthersNegative(categoryPair.Category1, categoryPair.Category2);
            SetPositiveIfOthersNegative(categoryPair.Category2, categoryPair.Category1);
        }
        SetNegativesOtherThanPositive(categories);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="category1"></param>
    /// <param name="category2"></param>
    void SetPositiveIfOthersNegative(Category category1, Category category2)
    {
        foreach (var item1 in category1.Items)
        {
            // 1行からPositiveを検索する。
            var positive = category2.Items.FirstOrDefault(m => GetState(item1, m) == ReasoningState.Positive);

            // Positiveがついているところは除外。
            if (positive is not null) continue;

            // 1行からUnknownを検索する。
            var unknownItems = category2.Items.Where(m => GetState(item1, m) == ReasoningState.Unknown);
            if (unknownItems.Count() != 1) continue;

            // Unknownがひとつだけで、それ以外は全てNegative
            var unknownItem = unknownItems.Single();

            // Positive確定
            SetState(item1, unknownItem, ReasoningState.Positive);
        }
    }
}