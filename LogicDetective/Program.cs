using System.Text;

namespace LogicDetective;

internal static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        if (args.Length > 0 && args[0].Equals("--diagnose-minimizer", StringComparison.OrdinalIgnoreCase))
        {
            RunMinimizerDiagnostics(args);
            return;
        }
        if (args.Length > 0 && args[0].Equals("--diagnose-hint", StringComparison.OrdinalIgnoreCase))
        {
            RunHintDifficultyDiagnostics(args);
            return;
        }
        var categories = new List<Category>
        {
            new(0, "曜日", ["月曜日", "火曜日", "水曜日", "木曜日"]),
            new(1, "人物", ["田中さん", "佐藤さん", "鈴木さん", "高橋さん"]),
            new(2, "料理", ["カレー", "ラーメン", "寿司", "ピザ"])
        };
        var puzzle = PuzzleLogic.Generate(categories);
        var session = new GameSession(puzzle);

        PrintPuzzleOverview(puzzle);
        while (true)
        {
            Console.WriteLine();
            Console.Write("Command> ");
            var input = Console.ReadLine();
            if (input is null)
            {
                Console.WriteLine("入力を終了します。");
                return;
            }
            if (!TryExecuteCommand(session, input))
            {
                break;
            }
        }
    }

    private static void RunMinimizerDiagnostics(string[] args)
    {
        var sampleCount = 100;
        if (args.Length >= 2 && !int.TryParse(args[1], out sampleCount))
        {
            Console.WriteLine("サンプル数は整数で指定してください。例: --diagnose-minimizer 100");
            return;
        }
        if (sampleCount <= 0)
        {
            Console.WriteLine("サンプル数は1以上で指定してください。");
            return;
        }
        var categories = CreateDefaultCategories();
        var report = PuzzleMinimizerDiagnostics.Run(categories, sampleCount);

        Console.WriteLine("===== PuzzleMinimizer Diagnostics =====");
        Console.WriteLine("Columns: No, Before, After, Reduced, SolutionsAfter");

        for (var index = 0; index < report.Measurements.Count; index++)
        {
            var measurement = report.Measurements[index];
            Console.WriteLine($"{index + 1},{measurement.OriginalClueCount},{measurement.MinimizedClueCount},{measurement.ReducedClueCount},{measurement.MinimizedSolutionCount}");
        }
        Console.WriteLine();
        Console.WriteLine("----- Summary -----");
        Console.WriteLine($"Samples: {report.SampleCount}");
        Console.WriteLine($"Average Before: {report.AverageBefore:F2}");
        Console.WriteLine($"Average After: {report.AverageAfter:F2}");
        Console.WriteLine($"Average Reduced: {report.AverageReduced:F2}");
        Console.WriteLine($"Median Before: {report.MedianBefore:F2}");
        Console.WriteLine($"Median After: {report.MedianAfter:F2}");
        Console.WriteLine($"Median Reduced: {report.MedianReduced:F2}");
        Console.WriteLine($"Min After: {report.MinAfter}");
        Console.WriteLine($"Max After: {report.MaxAfter}");
        Console.WriteLine($"All Unique After: {report.AllUniqueAfter}");
    }
    private static void RunHintDifficultyDiagnostics(string[] args)
    {
        var sampleCount = 100;
        if (args.Length >= 2 && !int.TryParse(args[1], out sampleCount))
        {
            Console.WriteLine("サンプル数は整数で指定してください。例: --diagnose-hint 100");
            return;
        }
        if (sampleCount <= 0)
        {
            Console.WriteLine("サンプル数は1以上で指定してください。");
            return;
        }
        var categories = CreateDefaultCategories();
        var report = HintDifficultyDiagnostics.Run(categories, sampleCount);

        Console.WriteLine("===== Puzzle Difficulty Diagnostics =====");
        Console.WriteLine("Columns: No, Clues, MinimumClues, RedundantClues");

        for (var index = 0; index < report.Measurements.Count; index++)
        {
            var measurement = report.Measurements[index];
            Console.WriteLine(
                $"{index + 1}," +
                $"{measurement.ClueCount}," +
                $"{measurement.MinimumClueCount}," +
                $"{measurement.RedundantClueCount}");
        }
        Console.WriteLine();
        Console.WriteLine("----- Summary -----");
        Console.WriteLine($"Samples: {report.SampleCount}");
        Console.WriteLine($"Average Clues: {report.AverageClueCount:F2}");
        Console.WriteLine($"Average Minimum Clues: {report.AverageMinimumClueCount:F2}");
        Console.WriteLine($"Average Redundant Clues: {report.AverageRedundantClueCount:F2}");
        Console.WriteLine($"Median Clues: {report.MedianClueCount:F2}");
        Console.WriteLine($"Median Minimum Clues: {report.MedianMinimumClueCount:F2}");
        Console.WriteLine($"Median Redundant Clues: {report.MedianRedundantClueCount:F2}");
        Console.WriteLine($"Min Minimum Clues: {report.MinMinimumClueCount}");
        Console.WriteLine($"Max Minimum Clues: {report.MaxMinimumClueCount}");
    }
    private static List<Category> CreateDefaultCategories()
    {
        return
        [
            new(0, "曜日", ["月曜日", "火曜日", "水曜日", "木曜日"]),
            new(1, "人物", ["田中さん", "佐藤さん", "鈴木さん", "高橋さん"]),
            new(2, "料理", ["カレー", "ラーメン", "寿司", "ピザ"])
        ];
    }

    private static bool TryExecuteCommand(GameSession session, string input)
    {
        var tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0) return true;

        var command = tokens[0].ToUpperInvariant();
        switch (command)
        {
            case "HELP":
                PrintCommands();
                return true;
            case "SHOW":
                PrintPlayerState(session);
                return true;
            case "HINT":
                PrintCertainRelations(session);
                return true;
            case "CHECK":
                var result = session.CheckAnswer();

                switch (result)
                {
                    case AnswerResult.Incomplete:
                        Console.WriteLine("まだ回答が完成していません。");
                        Console.WriteLine($"未回答: {session.GetUnknownAnswerCount()}件");
                        break;
                    case AnswerResult.Incorrect:
                        Console.WriteLine("回答に誤りがあります。");
                        var incorrectAnswers = session.GetIncorrectAnswers();
                        Console.WriteLine("誤っている回答:");
                        for (var index = 0; index < incorrectAnswers.Count; index++)
                        {
                            Console.WriteLine($"{index + 1}. {incorrectAnswers[index]}");
                        }
                        break;
                    case AnswerResult.Correct:
                        Console.WriteLine("正解です。ゲームクリア！");
                        return false;
                }

                return true;
            case "QUIT":
                Console.WriteLine("ゲームを終了します。");
                return false;
            case "Y":
            case "N":
            case "C":
                return TryApplyAction(session, tokens);
            default:
                Console.WriteLine("不正なコマンドです。HELPを入力して使い方を確認してください。");
                return true;
        }
    }
    private static bool TryApplyAction(GameSession session, string[] tokens)
    {
        if (!CommandParser.TryParseAction(session.Puzzle.Categories, tokens, out var action))
        {
            Console.WriteLine("形式が不正です。例: Y 0:1 2:3");
            return true;
        }
        try
        {
            session.ApplyAction(action!);
            Console.WriteLine("操作を反映しました。");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"操作を適用できません: {ex.Message}");
        }
        return true;
    }

    private static void PrintPuzzleOverview(Puzzle puzzle)
    {
        Console.WriteLine("===== LogicDetective =====");
        Console.WriteLine("カテゴリと項目:");

        for (var categoryIndex = 0; categoryIndex < puzzle.Categories.Count; categoryIndex++)
        {
            var category = puzzle.Categories[categoryIndex];
            Console.WriteLine($"[{categoryIndex}] {category.Name}");

            for (var itemIndex = 0; itemIndex < category.Items.Count; itemIndex++)
            {
                Console.WriteLine($"  - {categoryIndex}:{itemIndex} {category.Items[itemIndex]}");
            }
        }
        Console.WriteLine();
        Console.WriteLine("ヒント:");

        for (var index = 0; index < puzzle.Clues.Count; index++)
        {
            Console.WriteLine($"{index + 1}. {puzzle.Clues[index]}");
        }
        Console.WriteLine();
        PrintCommands();
    }

    private static void PrintCommands()
    {
        Console.WriteLine("コマンド一覧:");
        Console.WriteLine("  Y <cat:item> <cat:item>  : 2つの項目を同じ組(Yes)に設定");
        Console.WriteLine("  N <cat:item> <cat:item>  : 2つの項目を異なる組(No)に設定");
        Console.WriteLine("  C <cat:item> <cat:item>  : 2つの項目の判断をクリア(Unknown)");
        Console.WriteLine("  SHOW                      : 現在の回答状態を表示");
        Console.WriteLine("  HINT                      : 確定している推論結果を1件表示");
        Console.WriteLine("  CHECK                     : 回答を判定");
        Console.WriteLine("  HELP                      : コマンドを再表示");
        Console.WriteLine("  QUIT                      : 終了");
    }

    private static void PrintPlayerState(GameSession session)
    {
        var puzzle = session.Puzzle;
        Console.WriteLine("現在の回答状態 (?=Unknown, O=Yes, X=No)");

        for (var categoryA = 0; categoryA < puzzle.Categories.Count; categoryA++)
        {
            for (var categoryB = categoryA + 1; categoryB < puzzle.Categories.Count; categoryB++)
            {
                var firstCategory = puzzle.Categories[categoryA];
                var secondCategory = puzzle.Categories[categoryB];

                Console.WriteLine();
                Console.WriteLine($"[{categoryA}] {firstCategory.Name} × [{categoryB}] {secondCategory.Name}");

                Console.Write("            ");
                for (var itemB = 0; itemB < secondCategory.Items.Count; itemB++)
                {
                    Console.Write($"{itemB}:{secondCategory.Items[itemB],-8}");
                }
                Console.WriteLine();

                for (var itemA = 0; itemA < firstCategory.Items.Count; itemA++)
                {
                    Console.Write($"{itemA}:{firstCategory.Items[itemA],-8}");

                    for (var itemB = 0; itemB < secondCategory.Items.Count; itemB++)
                    {
                        var firstItem = new Item(categoryA, itemA, firstCategory.Items[itemA]);
                        var secondItem = new Item(categoryB, itemB, secondCategory.Items[itemB]);
                        var state = session.PlayerState.GetState(firstItem, secondItem);
                        var mark = state switch
                        {
                            PlayerPairState.Yes => "O",
                            PlayerPairState.No => "X",
                            _ => "?"
                        };
                        Console.Write($"{mark,-10}");
                    }
                    Console.WriteLine();
                }
            }
        }
    }

    private static void PrintCertainRelations(GameSession session)
    {
        var relation = session.GetNextHint();
        if (relation is not null)
        {
            Console.WriteLine("ヒント:");
            Console.WriteLine(relation);
            return;
        }
        Console.WriteLine("これ以上の新しいヒントはありません。");
    }
}