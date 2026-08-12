using System.Text;

namespace LogicDetective;

internal static class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        if (args.Length > 0 && args[0].Equals("--diagnose-minimizer", StringComparison.OrdinalIgnoreCase))
        {
            Console.Write(Command.RunMinimizerDiagnostics(args));
            return;
        }
        if (args.Length > 0 && args[0].Equals("--diagnose-hint", StringComparison.OrdinalIgnoreCase))
        {
            Console.Write(Command.RunHintDifficultyDiagnostics(args));
            return;
        }
        var categories = Command.CreateDefaultCategories3();
        var puzzle = PuzzleLogic.GeneratePuzzle(categories, false);
        var session = new GameSession(puzzle);

        Console.Write(PuzzleRenderer.GetGameSession(session));
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
            if (!TryExecuteCommand(session, input)) break;
        }
    }
    static bool TryExecuteCommand(GameSession session, string input)
    {
        var tokens = Command.GetTokens(input);
        switch (tokens[0])
        {
            case "show":
                Console.Write(PuzzleRenderer.GetGameSession(session));
                return true;

            case "help":
                Console.Write(Command.GetHelp());
                return true;

            case "s":
                Console.Write(Command.TryApplyStateCommand(session, tokens));
                Console.Write(PuzzleRenderer.GetGameSession(session));
                return true;

            case "hint":
                Console.Write(Command.GetNextHint(session));
                return true;

            case "solve":
                Console.Write(Command.GetAnswer(session.Puzzle));
                return true;

            case "check":
                var result = session.PlayerState.CheckAnswer(session.Puzzle);
                Console.Write(Command.GetResult(session, result));
                return result != AnswerResult.Correct;

            case "quit":
                Console.WriteLine("ゲームを終了します。");
                return false;

            default:
                Console.WriteLine("不正なコマンドです。'help' を入力して使い方を確認してください。");
                return true;
        }
    }
}