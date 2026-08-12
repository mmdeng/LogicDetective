using System.Text;

namespace LogicDetective;

internal sealed class CategoryList : List<Category>
{
    public void Add(string category, string[] items)
    {
        Add(new Category(Count, category, items));
    }

    public int GetItemCount()
    {
        return this[0].Items.Count;
    }

    public int GetGridCount()
    {
        return Mathmatics.Combination(Count, 2);
    }

    public int GetCellCountInOneGrid()
    {
        return this[0].Items.Count * this[1].Items.Count; // this[0].Items.Countの2乗でも同じ。
    }

    public int GetCellCountInWholeGrid()
    {
        return GetCellCountInOneGrid() * GetGridCount();
    }

    public Item GetItem(string categoryName, string itemName)
    {
        var category = this.SingleOrDefault(m => m.Name == categoryName) ?? throw new Exception();
        return category.GetItem(itemName);
    }

    public ItemPair GetItemPair(string categoryName1, string itemName1, string categoryName2, string itemName2)
    {
        var item1 = GetItem(categoryName1, itemName1);
        var item2 = GetItem(categoryName2, itemName2);
        return new ItemPair(item1, item2);
    }

    public Item GetItem(int categoryIndex, int itemIndex)
    {
        return this[categoryIndex].Items[itemIndex];
    }

    public ItemPair GetItemPair(int categoryIndex1, int itemIndex1, int categoryIndex2, int itemIndex2)
    {
        var item1 = GetItem(categoryIndex1, itemIndex1);
        var item2 = GetItem(categoryIndex2, itemIndex2);
        return new ItemPair(item1, item2);
    }

    public ItemPair GetItemPair(PairKey pairKey)
    {
        var item1 = GetItem(pairKey.CategoryIndex1, pairKey.ItemIndex1);
        var item2 = GetItem(pairKey.CategoryIndex2, pairKey.ItemIndex2);
        return new ItemPair(item1, item2);
    }

    public IEnumerable<CategoryPair> EnumerateAllCategoryPairs()
    {
        for (var i = 0; i < Count; i++)
        {
            for (var j = i + 1; j < Count; j++)
            {
                if (i == 0) yield return new CategoryPair(this[i], this[j]);
                else yield return new CategoryPair(this[j], this[i]);
            }
        }
    }

    public IEnumerable<ItemPair> EnumerateAllItemPairs()
    {
        var itemCount = GetItemCount();
        foreach (var categoryPair in EnumerateAllCategoryPairs())
        {
            for (var i = 0; i < itemCount; i++)
            {
                var item1 = categoryPair.Category1.Items[i];
                for (var j = 0; j < itemCount; j++)
                {
                    var item2 = categoryPair.Category2.Items[j];
                    yield return new ItemPair(item1, item2);
                }
            }
        }
    }

    public void Validate()
    {
        if (Count < 3)
        {
            throw new ArgumentException("カテゴリは3個以上必要です。");
        }
        if (GetItemCount() < 2)
        {
            throw new ArgumentException("各カテゴリには2個以上の項目が必要です。");
        }
        foreach (var category in this)
        {
            if (category.Items.Count != GetItemCount())
            {
                throw new ArgumentException("全カテゴリの項目数を同じにしてください。");
            }
        }
    }
}

// public IReadOnlyList<Answer> Solve(IReadOnlyList<Clue> clues)
// {
//     var answers = new List<Answer>();
//     foreach (var answer in GetAllAnswers())
//     {
//         if (answer.SatisfiesAllClues(clues))
//         {
//             answers.Add(answer);
//         }
//     }
//     return answers;
// }

// /// <summary>
// /// 指定の手掛かりリストで解ける解の数を取得
// /// </summary>
// /// <param name="categories">カテゴリリスト</param>
// /// <param name="clues">指定の手掛かりリスト</param>
// /// <returns>解ける解の数</returns>
// public int CountAnswers(IReadOnlyList<Clue> clues)
// {
//     var answerCount = 0;
//     var answers = GetAllAnswers();
//     foreach (var answer in answers)
//     {
//         if (!answer.SatisfiesAllClues(clues)) continue;

//         answerCount++;
//         if (answerCount >= 2) return answerCount;
//     }
//     return answerCount;
// }
// List<Answer> GetAllAnswers()
// {
//     var permutations = new int[Count][];
//     permutations[0] = CreatePermutation();

//     var answers = new List<Answer>();
//     GetAllAnswersRecursive(answers, permutations, 1);
//     return answers;
// }

// void GetAllAnswersRecursive(List<Answer> answers, int[][] permutations, int categoryIndex)
// {
//     var permutation = CreatePermutation();
//     do
//     {
//         permutations[categoryIndex] = permutation;
//         if (categoryIndex < Count - 1)
//         {
//             GetAllAnswersRecursive(answers, permutations, categoryIndex + 1);
//         }
//         else
//         {
//             var answer = new Answer(this);
//             answer.SetPermutations(permutations);
//             answers.Add(answer);
//         }
//     }
//     while (Mathmatics.NextPermutation(permutation));
// }

// public int[] CreatePermutation()
// {
//     return Enumerable.Range(0, GetItemCount()).ToArray();
// }

// public int[][] CreatePermutations(bool isRandom = true)
// {
//     var permutations = new int[Count][];
//     permutations[0] = CreatePermutation();
//     for (var i = 1; i < Count; i++)
//     {
//         // 0番目のカテゴリは順番を固定する。正解は以下の様に出すが、田中、鈴木、井上、高橋の順番は変えない。
//         // 問題を作るために年齢、ペットの順番はランダムにする。
//         // ロジック的には別に0番目のカテゴリもランダムにしていいんだけど、
//         // プレーヤーにわかりやすいようにこうしている。
//         // ===== 正解 =====
//         // グループ1: 田中 / 53歳 / 魚
//         // グループ2: 鈴木 / 35歳 / 猫
//         // グループ3: 井上 / 42歳 / 犬
//         // グループ4: 高橋 / 24歳 / 鳥
//         var permutation = CreatePermutation();
//         if (isRandom) Mathmatics.Shuffle(permutation);
//         permutations[i] = permutation;
//     }
//     return permutations;
// }
