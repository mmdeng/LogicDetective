namespace LogicDetective.Tests;

public class PuzzleSolverTests
{
    /// <summary>
    /// 2カテゴリ : 2項目
    /// </summary>
    [Fact]
    public void Solve22()
    {
        var categories = new CategoryList
        {
            { "a", ["00", "01"] },
            { "b", ["10", "11"] }
        };
        var solver = new PuzzleSolver(categories);

        // 手掛かりなしなら正解は2個。
        {
            var answers = solver.Solve([]);
            Assert.Equal(2, answers.Count);
            {
                Assert.True(answers[0].AreSameGroup(categories.GetItemPair("a", "00", "b", "10")));
                Assert.False(answers[0].AreSameGroup(categories.GetItemPair("a", "00", "b", "11")));
                Assert.False(answers[0].AreSameGroup(categories.GetItemPair("a", "01", "b", "10")));
                Assert.True(answers[0].AreSameGroup(categories.GetItemPair("a", "01", "b", "11")));
            }
            {
                Assert.False(answers[1].AreSameGroup(categories.GetItemPair("a", "00", "b", "10")));
                Assert.True(answers[1].AreSameGroup(categories.GetItemPair("a", "00", "b", "11")));
                Assert.True(answers[1].AreSameGroup(categories.GetItemPair("a", "01", "b", "10")));
                Assert.False(answers[1].AreSameGroup(categories.GetItemPair("a", "01", "b", "11")));
            }
        }
        // 一つ決まった瞬間に全てが決まる。
        var itemPair = categories.GetItemPair("a", "00", "b", "10");
        {
            var clues = new Clue[] { new SameClue(itemPair) };
            var answer = solver.Solve(clues).Single();
            Assert.True(answer.AreSameGroup(itemPair));
            Assert.True(answer.AreSameGroup(categories.GetItemPair("a", "00", "b", "10")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("a", "00", "b", "11")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("a", "01", "b", "10")));
            Assert.True(answer.AreSameGroup(categories.GetItemPair("a", "01", "b", "11")));
        }
        {
            var clues = new Clue[] { new DifferentClue(itemPair) };
            var answer = solver.Solve(clues).Single();
            Assert.False(answer.AreSameGroup(itemPair));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("a", "00", "b", "10")));
            Assert.True(answer.AreSameGroup(categories.GetItemPair("a", "00", "b", "11")));
            Assert.True(answer.AreSameGroup(categories.GetItemPair("a", "01", "b", "10")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("a", "01", "b", "11")));
        }
        // 手掛かりが矛盾しているケース
        {
            var clues = new Clue[] { new SameClue(itemPair), new DifferentClue(itemPair) };
            var answers = solver.Solve(clues);
            Assert.Empty(answers);
        }
    }
    /// <summary>
    /// 3カテゴリ : 2項目
    /// </summary>
    [Fact]
    public void Solve32()
    {
        var categories = new CategoryList
        {
            { "a", ["00", "01"] },
            { "b", ["10", "11"] },
            { "c", ["20", "21"] }
        };
        var solver = new PuzzleSolver(categories);
        {
            // Answer1
            //    b     c
            // a  10 11 20 21
            // 00 ○  ×  ○  ×
            // 01 ×  ○  ×  ○
            // 20 ○  ×
            // 21 ×  ○

            // Answer1
            // 00 10 ○     00 20 ○
            // 00 11 ×     00 21 ×
            // 01 10 ×     01 20 ×
            // 01 11 ○     01 21 ○

            // ○だけ収集
            //                    Answer1, Answer2, Answer3, Answer4
            // Group0, Category0  000      000      000      000
            // Group0, Category1  010      010      011      011
            // Group0, Category2  020      021      020      021
            // Group1, Category0  101      101      101      101
            // Group1, Category1  111      111      110      110
            // Group1, Category2  121      120      121      120

            // Permutation 01,10
            //                    Answer1, Answer2, Answer3, Answer4
            // Category0          01       01       01       01         ここだけ固定
            // Category1          01       01       10       10
            // Category2          01       10       01       10

            //                    Answer1, Answer2, Answer3, Answer4
            // Category0          000      000      000      000
            // Category0          101      101      101      101
            // Category1          010      010      011      011
            // Category1          111      111      110      110
            // Category2          020      021      020      021
            // Category2          121      120      121      120
            var answers = solver.Solve([]);
            Assert.Equal(4, answers.Count);
        }
        var itemPair1 = categories.GetItemPair("a", "00", "b", "10");
        var itemPair2 = categories.GetItemPair("a", "00", "c", "20");
        {
            var clues = new Clue[] { new SameClue(itemPair1), new SameClue(itemPair2) };
            var answer = solver.Solve(clues).Single();
            Assert.True(answer.AreSameGroup(itemPair1));
            Assert.True(answer.AreSameGroup(itemPair2));

            Assert.True(answer.AreSameGroup(categories.GetItemPair("a", "00", "b", "10")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("a", "00", "b", "11")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("a", "01", "b", "10")));
            Assert.True(answer.AreSameGroup(categories.GetItemPair("a", "01", "b", "11")));

            Assert.True(answer.AreSameGroup(categories.GetItemPair("a", "00", "c", "20")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("a", "00", "c", "21")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("a", "01", "c", "20")));
            Assert.True(answer.AreSameGroup(categories.GetItemPair("a", "01", "c", "21")));

            Assert.True(answer.AreSameGroup(categories.GetItemPair("b", "10", "c", "20")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("b", "10", "c", "21")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("b", "11", "c", "20")));
            Assert.True(answer.AreSameGroup(categories.GetItemPair("b", "11", "c", "21")));
        }
        {
            var clues = new Clue[] { new SameClue(itemPair1), new DifferentClue(itemPair2) };
            var answer = solver.Solve(clues).Single();
            Assert.True(answer.AreSameGroup(itemPair1));
            Assert.False(answer.AreSameGroup(itemPair2));

            Assert.True(answer.AreSameGroup(categories.GetItemPair("a", "00", "b", "10")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("a", "00", "b", "11")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("a", "01", "b", "10")));
            Assert.True(answer.AreSameGroup(categories.GetItemPair("a", "01", "b", "11")));

            Assert.False(answer.AreSameGroup(categories.GetItemPair("a", "00", "c", "20")));
            Assert.True(answer.AreSameGroup(categories.GetItemPair("a", "00", "c", "21")));
            Assert.True(answer.AreSameGroup(categories.GetItemPair("a", "01", "c", "20")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("a", "01", "c", "21")));

            Assert.False(answer.AreSameGroup(categories.GetItemPair("b", "10", "c", "20")));
            Assert.True(answer.AreSameGroup(categories.GetItemPair("b", "10", "c", "21")));
            Assert.True(answer.AreSameGroup(categories.GetItemPair("b", "11", "c", "20")));
            Assert.False(answer.AreSameGroup(categories.GetItemPair("b", "11", "c", "21")));
        }
    }
    /// <summary>
    /// 2カテゴリ : 3項目
    /// </summary>
    [Fact]
    public void Solve23()
    {
        var categories = new CategoryList
        {
            { "a", ["00", "01", "02"] },
            { "b", ["10", "11", "12"] },
        };
        var solver = new PuzzleSolver(categories);
        {
            var answers = solver.Solve([]);
            Assert.Equal(6, answers.Count);
        }
        var itemPair1 = categories.GetItemPair("a", "00", "b", "10");
        {
            var clues = new Clue[] { new SameClue(itemPair1) };
            var answers = solver.Solve(clues);
            Assert.Equal(2, answers.Count);
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair1));
        }
        var itemPair2 = categories.GetItemPair("a", "01", "b", "11");
        {
            var clues = new Clue[] { new SameClue(itemPair1), new SameClue(itemPair2) };
            var answers = solver.Solve(clues);
            Assert.Single(answers);
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair1));
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair2));
        }
    }
    /// <summary>
    /// 3カテゴリ : 3項目
    /// </summary>
    [Fact]
    public void Solve33()
    {
        var categories = new CategoryList
        {
            { "a", ["00", "01", "02"] },
            { "b", ["10", "11", "12"] },
            { "c", ["20", "21", "22"] }
        };
        var solver = new PuzzleSolver(categories);
        {
            var answers = solver.Solve([]);
            Assert.Equal(36, answers.Count);
        }
        var itemPair1 = categories.GetItemPair("a", "00", "b", "10");
        {
            var clues = new Clue[] { new SameClue(itemPair1) };
            var answers = solver.Solve(clues);
            Assert.Equal(12, answers.Count);
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair1));
        }
        var itemPair2 = categories.GetItemPair("a", "01", "b", "11");
        {
            var clues = new Clue[] { new SameClue(itemPair1), new SameClue(itemPair2) };
            var answers = solver.Solve(clues);
            Assert.Equal(6, answers.Count);
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair1));
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair2));
        }
        var itemPair3 = categories.GetItemPair("a", "00", "c", "20");
        {
            var clues = new Clue[]
            {
                new SameClue(itemPair1),
                new SameClue(itemPair2),
                new SameClue(itemPair3)
            };
            var answers = solver.Solve(clues);
            Assert.Equal(2, answers.Count);
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair1));
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair2));
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair3));
        }
        var itemPair4 = categories.GetItemPair("a", "01", "c", "21");
        {
            var clues = new Clue[]
            {
                new SameClue(itemPair1),
                new SameClue(itemPair2),
                new SameClue(itemPair3),
                new SameClue(itemPair4)
            };
            var answers = solver.Solve(clues);
            Assert.Single(answers);
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair1));
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair2));
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair3));
            answers.ToList().ForEach(m => m.AreSameGroup(itemPair4));
        }
    }

    [Fact]
    public void Permutation()
    {
        const int CATEGORY_COUNT = 4;
        for (var categoryIndex = 1; categoryIndex <= CATEGORY_COUNT; categoryIndex++)
        {
            var count = 0;
            var permutation = Enumerable.Range(0, categoryIndex).ToArray();
            do
            {
                count++;
                //Console.WriteLine(string.Join(",", permutation));
            }
            while (Mathmatics.NextPermutation(permutation));
            Assert.Equal(Mathmatics.Factorial(categoryIndex), count);
        }
        //var matrix = PuzzleSolver3.CreateMatrix(3, 2);

        // var categories = new CategoryList
        // {
        //     { "a", ["00", "01"] },
        //     { "b", ["10", "11"] },
        //     { "c", ["20", "21"] }
        // };
        // var s = PuzzleSolver3.CreateAnswer(categories, matrix);
    }
}
