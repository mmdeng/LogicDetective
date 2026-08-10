namespace LogicDetective;

internal static class PuzzleRenderer
{
    private const int LabelWidth = 12;
    private const int CellWidth = 8;

    public static void Print(Puzzle puzzle)
    {
        PrintClues(puzzle);
        Console.WriteLine();
        PrintGrid(puzzle);
        Console.WriteLine();
        PrintAnswer(puzzle);
        Console.WriteLine();
        Console.WriteLine($"解の数: {PuzzleSolver.CountSolutions(puzzle.Categories, puzzle.Clues)}");
    }

    private static void PrintClues(Puzzle puzzle)
    {
        Console.WriteLine("===== ヒント =====");
        for (var index = 0; index < puzzle.Clues.Count; index++)
        {
            Console.WriteLine($"{index + 1}. {puzzle.Clues[index]}");
        }
    }

    private static void PrintGrid(Puzzle puzzle)
    {
        var categoryA = puzzle.Categories[0];
        var categoryB = puzzle.Categories[1];
        var categoryC = puzzle.Categories[2];

        Console.WriteLine("===== ○×グリッド =====");
        Console.WriteLine();
        PrintHeader(categoryB);

        for (var itemA = 0; itemA < categoryA.Items.Count; itemA++)
        {
            Console.Write(categoryA.Items[itemA].PadRight(LabelWidth));

            for (var itemB = 0; itemB < categoryB.Items.Count; itemB++)
            {
                Console.Write(GetMark(puzzle, 0, itemA, 1, itemB).PadRight(CellWidth));
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        PrintHeader(categoryC);

        for (var itemA = 0; itemA < categoryA.Items.Count; itemA++)
        {
            Console.Write(categoryA.Items[itemA].PadRight(LabelWidth));

            for (var itemC = 0; itemC < categoryC.Items.Count; itemC++)
            {
                Console.Write(GetMark(puzzle, 0, itemA, 2, itemC).PadRight(CellWidth));
            }
            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine(categoryC.Name);
        Console.Write("".PadRight(LabelWidth));

        for (var itemB = 0; itemB < categoryB.Items.Count; itemB++)
        {
            Console.Write(categoryB.Items[itemB].PadRight(CellWidth));
        }
        Console.WriteLine();

        for (var itemC = 0; itemC < categoryC.Items.Count; itemC++)
        {
            Console.Write(categoryC.Items[itemC].PadRight(LabelWidth));

            for (var itemB = 0; itemB < categoryB.Items.Count; itemB++)
            {
                Console.Write(GetMark(puzzle, 2, itemC, 1, itemB).PadRight(CellWidth));
            }
            Console.WriteLine();
        }
    }

    private static void PrintHeader(Category category)
    {
        Console.Write("".PadRight(LabelWidth));
        foreach (var item in category.Items)
        {
            Console.Write(item.PadRight(CellWidth));
        }
        Console.WriteLine();
    }

    private static string GetMark(Puzzle puzzle, int categoryA, int itemA, int categoryB, int itemB)
    {
        return puzzle.Solution.AreSameGroup(categoryA, itemA, categoryB, itemB) ? "○" : "×";
    }

    private static void PrintAnswer(Puzzle puzzle)
    {
        Console.WriteLine("===== 正解 =====");
        for (var group = 0; group < puzzle.Solution.GroupCount; group++)
        {
            var items = new List<string>();

            for (var category = 0; category < puzzle.Categories.Count; category++)
            {
                var itemIndex = puzzle.Solution.GetItemIndex(group, category);
                items.Add(puzzle.Categories[category].Items[itemIndex]);
            }
            Console.WriteLine(string.Join(" / ", items));
        }
    }
}