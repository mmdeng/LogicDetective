namespace LogicDetective.Tests;

public class CommandTest
{
    [Fact]
    public void Check()
    {
        var categories = Command.CreateDefaultCategories3();
        var puzzle = PuzzleLogic.GeneratePuzzle(categories, false);
        var session = new GameSession(puzzle);

        var geme = PuzzleRenderer.GetGameSession(session);
        Assert.Contains("グループ1: 田中 / 24歳 / 猫", geme);
        Assert.Contains("グループ2: 鈴木 / 53歳 / 犬", geme);
        Assert.Contains("グループ3: 井上 / 35歳 / 鳥", geme);
        Assert.Contains("グループ4: 高橋 / 42歳 / 魚", geme);

        var result = session.PlayerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incomplete, result);

        var solver = new PuzzleSolver(categories);
        var answer = solver.Solve(puzzle.Clues.ToList()).Single();
        {
            // 田中 / 24歳 は同じグループに属するので、×を入力すると不正解になる。
            var itemPair = categories.GetItemPair("人物", "田中", "年齢", "24歳");
            Assert.True(answer.AreSameGroup(itemPair));
            session.PlayerState.SetState(itemPair, ReasoningState.Negative);
            Assert.Equal(AnswerResult.Incorrect, session.PlayerState.CheckAnswer(puzzle));
        }
        // 全ての〇を入力。
        {
            {
                var itemPair = categories.GetItemPair("人物", "田中", "年齢", "24歳");
                Assert.True(answer.AreSameGroup(itemPair));
                session.PlayerState.SetState(itemPair, ReasoningState.Positive);
                Assert.Equal(AnswerResult.Incomplete, session.PlayerState.CheckAnswer(puzzle));
            }
            {
                var itemPair = categories.GetItemPair("年齢", "24歳", "ペット", "猫");
                Assert.True(answer.AreSameGroup(itemPair));
                session.PlayerState.SetState(itemPair, ReasoningState.Positive);
            }
            {
                var itemPair = categories.GetItemPair("人物", "田中", "ペット", "猫");
                Assert.True(answer.AreSameGroup(itemPair));
                session.PlayerState.SetState(itemPair, ReasoningState.Positive);
            }
        }
        {
            {
                var itemPair = categories.GetItemPair("人物", "鈴木", "年齢", "53歳");
                Assert.True(answer.AreSameGroup(itemPair));
                session.PlayerState.SetState(itemPair, ReasoningState.Positive);
            }
            {
                var itemPair = categories.GetItemPair("年齢", "53歳", "ペット", "犬");
                Assert.True(answer.AreSameGroup(itemPair));
                session.PlayerState.SetState(itemPair, ReasoningState.Positive);
            }
            {
                var itemPair = categories.GetItemPair("人物", "鈴木", "ペット", "犬");
                Assert.True(answer.AreSameGroup(itemPair));
                session.PlayerState.SetState(itemPair, ReasoningState.Positive);
            }
        }
        {
            {
                var itemPair = categories.GetItemPair("人物", "井上", "年齢", "35歳");
                Assert.True(answer.AreSameGroup(itemPair));
                session.PlayerState.SetState(itemPair, ReasoningState.Positive);
            }
            {
                var itemPair = categories.GetItemPair("年齢", "35歳", "ペット", "鳥");
                Assert.True(answer.AreSameGroup(itemPair));
                session.PlayerState.SetState(itemPair, ReasoningState.Positive);
            }
            {
                var itemPair = categories.GetItemPair("人物", "井上", "ペット", "鳥");
                Assert.True(answer.AreSameGroup(itemPair));
                session.PlayerState.SetState(itemPair, ReasoningState.Positive);
            }
        }
        {
            {
                var itemPair = categories.GetItemPair("人物", "高橋", "年齢", "42歳");
                Assert.True(answer.AreSameGroup(itemPair));
                session.PlayerState.SetState(itemPair, ReasoningState.Positive);
            }
            {
                var itemPair = categories.GetItemPair("年齢", "42歳", "ペット", "魚");
                Assert.True(answer.AreSameGroup(itemPair));
                session.PlayerState.SetState(itemPair, ReasoningState.Positive);
            }
            {
                var itemPair = categories.GetItemPair("人物", "高橋", "ペット", "魚");
                Assert.True(answer.AreSameGroup(itemPair));
                session.PlayerState.SetState(itemPair, ReasoningState.Positive);
            }
        }
        Assert.Equal(AnswerResult.Incomplete, session.PlayerState.CheckAnswer(puzzle));
        // {
        //     // 全ての〇を入力したが、×は入力していない。
        //     // ×の分だけヒントが出てくる。未入力のセル数から、
        //     // 最初に与えたDifferentClueの手掛かりの数を引いた分だけヒントが出てくるはず。
        //     Assert.Equal(48, categories.GetCellCountInWholeGrid());
        //     Assert.Equal(12, session.PlayerState.GetAnsweredCount());

        //     var givenDifferentClue = session.Puzzle.Clues.Count(m => m is DifferentClue);
        //     var count = categories.GetCellCountInWholeGrid() - session.PlayerState.GetAnsweredCount() - givenDifferentClue;

        //     // Differentしか残っていないはず。
        //     for (int i = 0; i < count; i++)
        //     {
        //         var nextHint = session.HintGenerator.GetNextHint(session.PlayerState, puzzle.Clues);
        //         Assert.NotNull(nextHint);
        //         //Assert.IsType<DifferentClue>(nextHint);
        //     }
        //     {
        //         // ヒントを出し尽くした。
        //         var nextHint = session.HintGenerator.GetNextHint(session.PlayerState, puzzle.Clues);
        //         //Assert.Null(nextHint);
        //         //var message = Command.GetNextHint(session);
        //         //Assert.Contains("これ以上の新しいヒントはありません。", message);
        //     }
        // }
        // 全ての×を入力。
        session.PlayerState.SetNegativesOtherThanPositive(puzzle.Categories);
        Assert.Equal(AnswerResult.Correct, session.PlayerState.CheckAnswer(puzzle));
    }
    [Fact]
    public void Check2()
    {
        var categories = Command.CreateDefaultCategories3();
        var puzzle = PuzzleLogic.GeneratePuzzle(categories, false);
        var session = new GameSession(puzzle);

        var geme = PuzzleRenderer.GetGameSession(session);
        Assert.Contains("グループ1: 田中 / 24歳 / 猫", geme);
        Assert.Contains("グループ2: 鈴木 / 53歳 / 犬", geme);
        Assert.Contains("グループ3: 井上 / 35歳 / 鳥", geme);
        Assert.Contains("グループ4: 高橋 / 42歳 / 魚", geme);

        var result = session.PlayerState.CheckAnswer(puzzle);
        Assert.Equal(AnswerResult.Incomplete, result);

        var resultText = Command.GetResult(session, result);
        Assert.Contains($"未回答: {puzzle.Categories.GetCellCountInWholeGrid()}件", resultText);

        // Positiveをつけると、周辺が勝手にNegativeになる。
        {
            Command.TryApplyStateCommand(session, ["s", "田中", "24歳", "1"]);
            var itemPair1 = categories.GetItemPair("人物", "田中", "年齢", "53歳");
            Assert.Equal(ReasoningState.Negative, session.PlayerState.GetState(itemPair1));
            var itemPair2 = categories.GetItemPair("人物", "鈴木", "年齢", "24歳");
            Assert.Equal(ReasoningState.Negative, session.PlayerState.GetState(itemPair2));
        }
        // 全ての〇を入力。
        {
            Command.TryApplyStateCommand(session, ["s", "24歳", "猫", "1"]);
            Assert.Equal(AnswerResult.Incomplete, session.PlayerState.CheckAnswer(puzzle));
            // Command.TryApplyStateCommand(session, ["s", "田中", "猫", "1"]);
            // Assert.Equal(AnswerResult.Incomplete, session.PlayerState.CheckAnswer(puzzle));
        }
        {
            Command.TryApplyStateCommand(session, ["s", "鈴木", "53歳", "1"]);
            Assert.Equal(AnswerResult.Incomplete, session.PlayerState.CheckAnswer(puzzle));
            Command.TryApplyStateCommand(session, ["s", "53歳", "犬", "1"]);
            // Assert.Equal(AnswerResult.Incomplete, session.PlayerState.CheckAnswer(puzzle));
            // Command.TryApplyStateCommand(session, ["s", "鈴木", "犬", "1"]);
            Assert.Equal(AnswerResult.Incomplete, session.PlayerState.CheckAnswer(puzzle));
        }
        {
            Command.TryApplyStateCommand(session, ["s", "井上", "35歳", "1"]);
            Assert.Equal(AnswerResult.Incomplete, session.PlayerState.CheckAnswer(puzzle));
            Command.TryApplyStateCommand(session, ["s", "35歳", "鳥", "1"]);
            Assert.Equal(AnswerResult.Correct, session.PlayerState.CheckAnswer(puzzle));
            // Command.TryApplyStateCommand(session, ["s", "井上", "鳥", "1"]);
            // Assert.Equal(AnswerResult.Incomplete, session.PlayerState.CheckAnswer(puzzle));
        }
        {
            // Command.TryApplyStateCommand(session, ["s", "高橋", "42歳", "1"]);
            // Assert.Equal(AnswerResult.Incomplete, session.PlayerState.CheckAnswer(puzzle));
            // Command.TryApplyStateCommand(session, ["s", "42歳", "魚", "1"]);
            // Assert.Equal(AnswerResult.Correct, session.PlayerState.CheckAnswer(puzzle));
            // Command.TryApplyStateCommand(session, ["s", "高橋", "魚", "1"]);
            // Assert.Equal(AnswerResult.Correct, session.PlayerState.CheckAnswer(puzzle));
        }
    }

    [Fact]
    public void AutoSolveBridgeTest1()
    {
        var categories = Command.CreateDefaultCategories4();
        var puzzle = PuzzleLogic.GeneratePuzzle(categories, false);
        var session = new GameSession(puzzle);

        var bridge = categories.GetItemPair("職業", "公務員", "年齢", "42歳");
        session.PlayerState.SetState(bridge, ReasoningState.Positive);
        session.PlayerState.AutoSolve(categories);

        var trigger = categories.GetItemPair("年齢", "42歳", "ペット", "魚");
        session.PlayerState.SetState(trigger, ReasoningState.Positive);
        session.PlayerState.AutoSolve(categories);

        var itemPair = categories.GetItemPair("職業", "公務員", "ペット", "魚");
        Assert.Equal(ReasoningState.Positive, session.PlayerState.GetState(itemPair));
    }
    [Fact]
    public void AutoSolveBridgeTest2()
    {
        var categories = Command.CreateDefaultCategories4();
        var puzzle = PuzzleLogic.GeneratePuzzle(categories, false);
        var session = new GameSession(puzzle);

        var bridge = categories.GetItemPair("ペット", "犬", "人物", "鈴木");
        session.PlayerState.SetState(bridge, ReasoningState.Positive);
        session.PlayerState.AutoSolve(categories);

        var trigger = categories.GetItemPair("人物", "鈴木", "職業", "作家");
        session.PlayerState.SetState(trigger, ReasoningState.Positive);
        session.PlayerState.AutoSolve(categories);

        var itemPair = categories.GetItemPair("ペット", "犬", "職業", "作家");
        Assert.Equal(ReasoningState.Positive, session.PlayerState.GetState(itemPair));
    }
    [Fact]
    public void SetNegativesOtherThanPositiveTest1()
    {
        var categories = Command.CreateDefaultCategories3();
        var puzzle = PuzzleLogic.GeneratePuzzle(categories, false);
        var session = new GameSession(puzzle);

        {
            var itemPair = categories.GetItemPair("人物", "田中", "年齢", "24歳");
            session.PlayerState.SetState(itemPair, ReasoningState.Negative);
        }
        {
            var itemPair = categories.GetItemPair("人物", "田中", "年齢", "53歳");
            session.PlayerState.SetState(itemPair, ReasoningState.Negative);
        }
        {
            var itemPair = categories.GetItemPair("人物", "田中", "年齢", "35歳");
            session.PlayerState.SetState(itemPair, ReasoningState.Negative);
        }
        session.PlayerState.SetPositiveIfOthersNegative(categories);
        {
            var itemPair = categories.GetItemPair("人物", "田中", "年齢", "42歳");
            Assert.Equal(ReasoningState.Positive, session.PlayerState.GetState(itemPair));
        }
    }
    [Fact]
    public void SetNegativesOtherThanPositiveTest2()
    {
        var categories = Command.CreateDefaultCategories3();
        var puzzle = PuzzleLogic.GeneratePuzzle(categories, false);
        var session = new GameSession(puzzle);

        {
            var itemPair = categories.GetItemPair("年齢", "42歳", "人物", "田中");
            session.PlayerState.SetState(itemPair, ReasoningState.Negative);
        }
        {
            var itemPair = categories.GetItemPair("年齢", "42歳", "人物", "鈴木");
            session.PlayerState.SetState(itemPair, ReasoningState.Negative);
        }
        {
            var itemPair = categories.GetItemPair("年齢", "42歳", "人物", "井上");
            session.PlayerState.SetState(itemPair, ReasoningState.Negative);
        }
        session.PlayerState.SetPositiveIfOthersNegative(categories);
        {
            var itemPair = categories.GetItemPair("年齢", "42歳", "人物", "高橋");
            Assert.Equal(ReasoningState.Positive, session.PlayerState.GetState(itemPair));
        }
    }
}
