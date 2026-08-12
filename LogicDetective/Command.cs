namespace LogicDetective;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class Command
{
    public static string[] GetTokens(string input)
    {
        var tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 0) return [string.Empty];

        tokens[0] = tokens[0].ToLowerInvariant();
        return tokens;
    }
    public static string RunMinimizerDiagnostics(string[] args)
    {
        var text = new StringBuilder();
        var sampleCount = 100;

        if (args.Length >= 2 && !int.TryParse(args[1], out sampleCount))
        {
            text.AppendLine("サンプル数は整数で指定してください。例: --diagnose-minimizer 100");
            return text.ToString();
        }
        if (sampleCount <= 0)
        {
            text.AppendLine("サンプル数は1以上で指定してください。");
            return text.ToString();
        }
        var categories = CreateDefaultCategories3();
        var report = PuzzleMinimizerDiagnostics.Run(categories, sampleCount);

        text.AppendLine("===== PuzzleMinimizer Diagnostics =====");
        text.AppendLine("Columns: No, Before, After, Reduced, AnswersAfter");

        for (var index = 0; index < report.Measurements.Count; index++)
        {
            var measurement = report.Measurements[index];
            text.AppendLine($"{index + 1},{measurement.OriginalClueCount},{measurement.MinimizedClueCount},{measurement.ReducedClueCount},{measurement.MinimizedAnswerCount}");
        }
        text.AppendLine();
        text.AppendLine("----- Summary -----");
        text.AppendLine($"Samples: {report.SampleCount}");
        text.AppendLine($"Average Before: {report.AverageBefore:F2}");
        text.AppendLine($"Average After: {report.AverageAfter:F2}");
        text.AppendLine($"Average Reduced: {report.AverageReduced:F2}");
        text.AppendLine($"Average Reduced Rate: {report.AverageReducedRate:F2}");
        text.AppendLine($"Median Before: {report.MedianBefore:F2}");
        text.AppendLine($"Median After: {report.MedianAfter:F2}");
        text.AppendLine($"Median Reduced: {report.MedianReduced:F2}");
        text.AppendLine($"Min After: {report.MinAfter}");
        text.AppendLine($"Max After: {report.MaxAfter}");
        text.AppendLine($"All Unique After: {report.AllUniqueAfter}");
        return text.ToString();
    }

    public static string RunHintDifficultyDiagnostics(string[] args)
    {
        var text = new StringBuilder();
        var sampleCount = 100;

        if (args.Length >= 2 && !int.TryParse(args[1], out sampleCount))
        {
            text.AppendLine("サンプル数は整数で指定してください。例: --diagnose-hint 100");
            return text.ToString();
        }
        if (sampleCount <= 0)
        {
            text.AppendLine("サンプル数は1以上で指定してください。");
            return text.ToString();
        }
        var categories = CreateDefaultCategories3();
        var report = HintDifficultyDiagnostics.Run(categories, sampleCount);

        text.AppendLine("===== Puzzle Difficulty Diagnostics =====");
        text.AppendLine("Columns: No, Clues, MinimumClues, RedundantClues");

        for (var index = 0; index < report.Measurements.Count; index++)
        {
            var measurement = report.Measurements[index];
            text.AppendLine(
                $"{index + 1}," +
                $"{measurement.ClueCount}," +
                $"{measurement.MinimumClueCount}," +
                $"{measurement.RedundantClueCount}");
        }
        text.AppendLine();
        text.AppendLine("----- Summary -----");
        text.AppendLine($"Samples: {report.SampleCount}");
        text.AppendLine($"Average Clues: {report.AverageClueCount:F2}");
        text.AppendLine($"Average Minimum Clues: {report.AverageMinimumClueCount:F2}");
        text.AppendLine($"Average Redundant Clues: {report.AverageRedundantClueCount:F2}");
        text.AppendLine($"Median Clues: {report.MedianClueCount:F2}");
        text.AppendLine($"Median Minimum Clues: {report.MedianMinimumClueCount:F2}");
        text.AppendLine($"Median Redundant Clues: {report.MedianRedundantClueCount:F2}");
        text.AppendLine($"Min Minimum Clues: {report.MinMinimumClueCount}");
        text.AppendLine($"Max Minimum Clues: {report.MaxMinimumClueCount}");
        return text.ToString();
    }

    // public static string GetPuzzleInfo(Puzzle puzzle)
    // {
    //     var text = new StringBuilder();
    //     text.AppendLine("===== パズル情報 =====");
    //     text.AppendLine($"カテゴリ数: {puzzle.Categories.Count}");
    //     text.AppendLine($"各カテゴリの項目数: {puzzle.Answer.ItemCount}");
    //     return text.ToString();
    // }

    public static string GetHelp()
    {
        var text = new StringBuilder();
        text.AppendLine("===== コマンド =====");
        text.AppendLine("show    - ゲーム状態を再表示");
        text.AppendLine("s       - 推理状態を設定（0=?, 1=○, 2=×）");
        text.AppendLine("hint    - ヒントを表示");
        text.AppendLine("solve   - 正解を表示（開発用）");
        text.AppendLine("check   - チェック");
        text.AppendLine("quit    - ゲームを終了");
        return text.ToString();
    }

    public static CategoryList CreateDefaultCategories3()
    {
        return
        [
            new(0, "人物", ["田中", "鈴木", "井上", "高橋"]),
            new(1, "年齢", ["24歳", "53歳", "35歳", "42歳"]),
            new(2, "ペット", ["猫", "犬", "鳥", "魚"])
            // new(0, "人物", ["田中", "鈴木"]),
            // new(1, "年齢", ["24歳", "53歳"]),
            // new(2, "ペット", ["猫", "犬"])
        ];
    }

    public static CategoryList CreateDefaultCategories4()
    {
        return
        [
            new(0, "人物", ["田中", "鈴木", "井上", "高橋"]),
            new(1, "年齢", ["24歳", "53歳", "35歳", "42歳"]),
            new(2, "ペット", ["猫", "犬", "鳥", "魚"]),
            new(3, "職業", ["教師", "作家", "会社員", "公務員"])
        ];
    }

    public static CategoryList CreateDefaultCategories5()
    {
        return
        [
            new(0, "人物", ["田中", "鈴木", "井上", "高橋"]),
            new(1, "年齢", ["24歳", "53歳", "35歳", "42歳"]),
            new(2, "ペット", ["猫", "犬", "鳥", "魚"]),
            new(3, "職業", ["教師", "作家", "会社員", "公務員"]),
            new(4, "出身", ["東京", "神奈川", "北海道", "大阪"])
        ];
    }

    public static string TryApplyStateCommand(GameSession session, string[] tokens)
    {
        var text = new StringBuilder();
        if (tokens.Length != 4)
        {
            text.AppendLine("形式が不正です。");
            return text.ToString();
        }
        var parsed1 = TryParseItem(session.Puzzle.Categories, tokens[1], out var item1);
        var parsed2 = TryParseItem(session.Puzzle.Categories, tokens[2], out var item2);

        if (!parsed1 || !parsed2 || item1 is null || item2 is null)
        {
            text.AppendLine("項目の指定が不正です。");
            return text.ToString();
        }
        if (!int.TryParse(tokens[3], out var stateValue) || stateValue < 0 || stateValue > 2)
        {
            text.AppendLine("状態値は 0（？）, 1（○）, 2（×）で指定してください。");
            return text.ToString();
        }

        var state = (ReasoningState)stateValue;
        try
        {
            session.PlayerState.SetState(item1, item2, state);
            if (state == ReasoningState.Positive)
            {
                session.PlayerState.AutoSolve(session.Puzzle.Categories);
            }
            text.AppendLine("操作を反映しました。");
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            text.AppendLine($"操作を適用できません: {ex.Message}");
        }
        return text.ToString();
    }

    static bool TryParseItem(IReadOnlyList<Category> categories, string token, out Item? item)
    {
        item = null;
        foreach (var category in categories)
        {
            item = category.Items.SingleOrDefault(m => m.Name == token);
            if (item is not null) return true;
        }
        return false;
    }

    public static string GetClues(Puzzle puzzle)
    {
        var text = new StringBuilder();
        text.AppendLine("===== 手掛かり =====");
        for (var index = 0; index < puzzle.Clues.Count; index++)
        {
            text.AppendLine($"{index + 1}. {puzzle.Clues[index]}");
        }
        return text.ToString();
    }

    public static string GetNextHint(GameSession session)
    {
        var hint = session.HintGenerator.GetNextHint(session.PlayerState, session.Puzzle.Clues);

        var text = new StringBuilder();
        if (hint is not null)
        {
            text.AppendLine("ヒント:");
            text.AppendLine($"  {hint}");
            return text.ToString();
        }
        text.AppendLine("これ以上の新しいヒントはありません。");
        // var certainRelations = session.GetCertainRelations();
        // if (certainRelations.Count > 0)
        // {
        //     text.AppendLine("確定している推論結果一覧:");
        //     for (var i = 0; i < certainRelations.Count; i++)
        //     {
        //         text.AppendLine($"  {i + 1}. {certainRelations[i]}");
        //     }
        // }
        return text.ToString();
    }

    public static string GetAnswer(Puzzle puzzle)
    {
        var text = new StringBuilder();
        text.AppendLine("===== 正解 =====");
        int groupIndex = 1;
        foreach (var group in puzzle.Answer.EnumerateGroups(puzzle.Categories))
        {
            text.AppendLine($"グループ{groupIndex++}: {string.Join(" / ", group.Select(item => item.Name))}");
        }
        return text.ToString();
    }

    public static string GetResult(GameSession session, AnswerResult result)
    {
        var text = new StringBuilder();
        switch (result)
        {
            case AnswerResult.Incomplete:
                text.AppendLine("まだ回答が完成していません。");
                text.AppendLine($"未回答: {session.GetUnknownAnswerCount()}件");
                break;
            case AnswerResult.Incorrect:
                text.AppendLine("回答に誤りがあります。");
                var incorrectAnswers = session.PlayerState.FindIncorrectAnswers(session.Puzzle);
                text.AppendLine("誤っている回答:");
                for (var index = 0; index < incorrectAnswers.Count; index++)
                {
                    text.AppendLine($"{index + 1}. {incorrectAnswers[index]}");
                }
                break;
            case AnswerResult.Correct:
                text.AppendLine("正解です。ゲームクリア！");
                break;
        }
        return text.ToString();
    }
}