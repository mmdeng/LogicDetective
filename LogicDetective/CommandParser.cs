namespace LogicDetective;

internal static class CommandParser
{
    public static bool TryParseItem(IReadOnlyList<Category> categories, string token, out Item? item)
    {
        item = null;

        var parts = token.Split(':', StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return false;
        }
        if (!int.TryParse(parts[0], out var categoryIndex) || !int.TryParse(parts[1], out var itemIndex))
        {
            return false;
        }
        if (categoryIndex < 0 || categoryIndex >= categories.Count)
        {
            return false;
        }
        var category = categories[categoryIndex];
        if (itemIndex < 0 || itemIndex >= category.Items.Count)
        {
            return false;
        }
        item = new Item(categoryIndex, itemIndex, category.Items[itemIndex]);
        return true;
    }

    public static bool TryParseAction(IReadOnlyList<Category> categories, string[] tokens, out PlayerAction? action)
    {
        action = null;
        if (tokens.Length != 3) return false;

        var actionType = tokens[0].ToUpperInvariant() switch
        {
            "Y" => PlayerActionType.SetYes,
            "N" => PlayerActionType.SetNo,
            "C" => PlayerActionType.Clear,
            _ => (PlayerActionType?)null
        };

        if (actionType is null) return false;
        if (!TryParseItem(categories, tokens[1], out var firstItem)) return false;
        if (!TryParseItem(categories, tokens[2], out var secondItem)) return false;

        action = new PlayerAction(actionType.Value, firstItem!, secondItem!);
        return true;
    }
}