namespace LogicDetective;

using System.Text;

internal static class PuzzleRenderer
{
    private const int LABEL_WIDTH = 12;
    private const int CELL_WIDTH = 8;
    private const int GRID_SPACING = 4;

    public static string GetGameSession(GameSession session)
    {
        var text = new StringBuilder();
        text.AppendLine(Command.GetAnswer(session.Puzzle));
        text.AppendLine();
        text.AppendLine(Command.GetHelp());
        text.AppendLine();
        text.Append(GetGrid(session));
        text.AppendLine();
        text.Append(Command.GetClues(session.Puzzle));
        return text.ToString();
    }

    static string GetGrid(GameSession session)
    {
        var text = new StringBuilder();
        text.AppendLine("===== 推理グリッド =====");
        text.AppendLine();
        foreach (var categoryPairs in EnumerateGridRows(session.Puzzle.Categories))
        {
            AppendGridRow(text, session.PlayerState, categoryPairs);
        }
        return text.ToString();
    }

    static IEnumerable<IReadOnlyList<CategoryPair>> EnumerateGridRows(CategoryList categories)
    {
        var count = categories.Count;
        if (count < 2) yield break;

        yield return Enumerable
            .Range(1, count - 1)
            .Select(index => new CategoryPair(categories[0], categories[index]))
            .ToList();

        for (var category1Index = count - 1; category1Index >= 2; category1Index--)
        {
            yield return Enumerable
                .Range(1, category1Index - 1)
                .Select(category2Index => new CategoryPair(categories[category1Index], categories[category2Index]))
                .ToList();
        }
    }

    static void AppendGridRow(StringBuilder text, PlayerState playerState, IReadOnlyList<CategoryPair> categoryPairs)
    {
        var grids = categoryPairs.Select(m => GetCategoryGridLines(playerState, m)).ToList();
        var lineCount = grids.Max(grid => grid.Count);
        for (var lineIndex = 0; lineIndex < lineCount; lineIndex++)
        {
            var gridIndex = 0;
            foreach (var grid in grids)
            {
                if (lineIndex < grid.Count)
                {
                    text.Append(grid[lineIndex]);
                }
                if (gridIndex < grids.Count - 1)
                {
                    text.Append(' ', GRID_SPACING);
                }
                gridIndex++;
            }
            text.AppendLine();
        }
        text.AppendLine();
    }
    static List<string> GetCategoryGridLines(PlayerState playerState, CategoryPair categoryPair)
    {
        var lines = new List<string>
        {
            CreatePaddedText(categoryPair.ToString(), GetGridWidth(categoryPair))
        };
        var header = CreatePaddedText(string.Empty, LABEL_WIDTH);
        foreach (var item2 in categoryPair.Category2.Items)
        {
            header += CreatePaddedText(item2.Name, CELL_WIDTH);
        }
        lines.Add(header);

        foreach (var item1 in categoryPair.Category1.Items)
        {
            var line = CreatePaddedText(item1.Name, LABEL_WIDTH);
            foreach (var item2 in categoryPair.Category2.Items)
            {
                var state = playerState.GetState(item1, item2);
                var mark = state.GetMark();
                line += CreatePaddedText(mark, CELL_WIDTH);
            }
            lines.Add(line);
        }
        return lines;
    }

    static int GetGridWidth(CategoryPair categoryPair)
    {
        return LABEL_WIDTH + categoryPair.Category2.Items.Count * CELL_WIDTH;
    }

    static string CreatePaddedText(string value, int width)
    {
        var displayWidth = GetDisplayWidth(value);
        var padding = Math.Max(0, width - displayWidth);
        return value + new string(' ', padding);
    }

    static int GetDisplayWidth(string value)
    {
        var width = 0;
        foreach (var character in value)
        {
            width += IsFullWidth(character) ? 2 : 1;
        }
        return width;
    }

    static bool IsFullWidth(char character)
    {
        return character >= '\u1100' && character <= '\u11FF'
            || character >= '\u3000' && character <= '\u303F'
            || character >= '\u3040' && character <= '\u309F'
            || character >= '\u30A0' && character <= '\u30FF'
            || character >= '\uFF00' && character <= '\uFFEF'
            || character >= '\u4E00' && character <= '\u9FFF';
    }
}