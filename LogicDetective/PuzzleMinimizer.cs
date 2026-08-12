namespace LogicDetective;

internal static class PuzzleMinimizer
{
    /// <summary>
    /// 手掛かりを最小限に減らすための処理
    /// </summary>
    /// <param name="puzzle"></param>
    /// <returns></returns>
    public static Puzzle Minimize(Puzzle puzzle)
    {
        var solver = new PuzzleSolver(puzzle.Categories);
        var clues = puzzle.Clues.ToList();
        var index = 0;
        while (index < clues.Count)
        {
            if (clues[index] is not SameClue && clues[index] is not DifferentClue)
            {
                index++;
                continue;
            }
            // 手掛かりを一つ減らす。
            var reduced = clues.Where((m, i) => i != index).ToList();

            // 手掛かりを一つ減らしても一意解が保たれるかをチェック
            if (solver.CountAnswers(reduced) == 1)
            {
                // 減らしても問題ないので減らす。
                clues = reduced;
                continue;
            }
            else
            {
                // 減らすと問題があるので減らさずに次へ。
                index++;
            }
        }
        return new Puzzle(puzzle.Categories, puzzle.Answer, clues);
    }
}
