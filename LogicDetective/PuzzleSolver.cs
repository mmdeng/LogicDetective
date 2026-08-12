namespace LogicDetective;

/// <summary>
/// このロジックが不要かもしれない。
/// 最初に総当たりで複数の正解を作成し、そこから手掛かりの一覧を作成し、正解が一つになるように手掛かりを減らしていく手法。
/// この手法、SameClue、DifferentClueでしか使えないのでは。
/// 正解が一つになるまで手掛かりを増やしていく手法の方がいいのでは？
/// </summary>
/// <returns></returns>
internal sealed class PuzzleSolver
{
    public CategoryList Categories { get; }

    public PuzzleSolver(CategoryList categories)
    {
        Categories = categories;
    }

    public IReadOnlyList<Answer> Solve(IReadOnlyList<Clue> clues)
    {
        var answers = new List<Answer>();
        foreach (var answer in GetAllAnswers())
        {
            if (answer.SatisfiesAllClues(clues))
            {
                answers.Add(answer);
            }
        }
        return answers;
    }

    List<Answer> GetAllAnswers()
    {
        var permutations = new int[Categories.Count][];
        permutations[0] = CreatePermutation();

        var answers = new List<Answer>();
        GetAllAnswersRecursive(answers, permutations, 1);
        return answers;
    }

    void GetAllAnswersRecursive(List<Answer> answers, int[][] permutations, int categoryIndex)
    {
        var permutation = CreatePermutation();
        do
        {
            permutations[categoryIndex] = permutation;
            if (categoryIndex < Categories.Count - 1)
            {
                GetAllAnswersRecursive(answers, permutations, categoryIndex + 1);
            }
            else
            {
                var answer = new Answer(Categories);
                answer.SetPermutations(permutations);
                answers.Add(answer);
            }
        }
        while (Mathmatics.NextPermutation(permutation));
    }

    public int[] CreatePermutation()
    {
        return Enumerable.Range(0, Categories.GetItemCount()).ToArray();
    }

    public int[][] CreatePermutations(bool isRandom = true)
    {
        var permutations = new int[Categories.Count][];
        permutations[0] = CreatePermutation();
        for (var i = 1; i < Categories.Count; i++)
        {
            // 0番目のカテゴリは順番を固定する。正解は以下の様に出すが、田中、鈴木、井上、高橋の順番は変えない。
            // 問題を作るために年齢、ペットの順番はランダムにする。
            // ロジック的には別に0番目のカテゴリもランダムにしていいんだけど、
            // プレーヤーにわかりやすいようにこうしている。
            // ===== 正解 =====
            // グループ1: 田中 / 53歳 / 魚
            // グループ2: 鈴木 / 35歳 / 猫
            // グループ3: 井上 / 42歳 / 犬
            // グループ4: 高橋 / 24歳 / 鳥
            var permutation = CreatePermutation();
            if (isRandom) Mathmatics.Shuffle(permutation);
            permutations[i] = permutation;
        }
        return permutations;
    }

    /// <summary>
    /// 指定の手掛かりリストで解ける解の数を取得
    /// </summary>
    /// <param name="categories">カテゴリリスト</param>
    /// <param name="clues">指定の手掛かりリスト</param>
    /// <returns>解ける解の数</returns>
    public int CountAnswers(IReadOnlyList<Clue> clues)
    {
        var answerCount = 0;
        var answers = GetAllAnswers();
        foreach (var answer in answers)
        {
            if (!answer.SatisfiesAllClues(clues)) continue;

            answerCount++;
            if (answerCount >= 2) return answerCount;
        }
        return answerCount;
    }
}
