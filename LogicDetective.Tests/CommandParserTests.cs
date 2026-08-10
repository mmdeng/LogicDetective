namespace LogicDetective.Tests;

public class CommandParserTests
{
    [Fact]
    public void TryParseItem_ValidToken_ReturnsItem()
    {
        var categories = new[] { new Category(0, "People", ["A", "B"]), new Category(1, "Pets", ["X", "Y"]) };
        var result = CommandParser.TryParseItem(categories, "1:0", out var item);
        Assert.True(result);
        Assert.NotNull(item);
        Assert.Equal(1, item.CategoryIndex);
        Assert.Equal(0, item.Index);
        Assert.Equal("X", item.Name);
    }

    [Fact]
    public void TryParseItem_InvalidItemIndex_ReturnsFalse()
    {
        var categories = new[] { new Category(0, "People", ["A", "B"]), new Category(1, "Pets", ["X", "Y"]) };
        var result = CommandParser.TryParseItem(categories, "1:99", out var item);
        Assert.False(result);
        Assert.Null(item);
    }

    [Fact]
    public void TryParseItem_InvalidCategoryIndex_ReturnsFalse()
    {
        var categories = new[] { new Category(0, "People", ["A", "B"]), new Category(1, "Pets", ["X", "Y"]) };
        var result = CommandParser.TryParseItem(categories, "99:0", out var item);
        Assert.False(result);
        Assert.Null(item);
    }

    [Fact]
    public void TryParseItem_InvalidTokenFormat_ReturnsFalse()
    {
        var categories = new[] { new Category(0, "People", ["A", "B"]), new Category(1, "Pets", ["X", "Y"]) };
        var result = CommandParser.TryParseItem(categories, "0-1", out var item);
        Assert.False(result);
        Assert.Null(item);
    }

    [Fact]
    public void TryParseItem_NonNumericToken_ReturnsFalse()
    {
        var categories = new[] { new Category(0, "People", ["A", "B"]), new Category(1, "Pets", ["X", "Y"]) };
        var result = CommandParser.TryParseItem(categories, "a:b", out var item);
        Assert.False(result); Assert.Null(item);
    }

    [Fact]
    public void TryParseItem_TooManyParts_ReturnsFalse()
    {
        var categories = new[] { new Category(0, "People", ["A", "B"]), new Category(1, "Pets", ["X", "Y"]) };
        var result = CommandParser.TryParseItem(categories, "0:1:2", out var item);
        Assert.False(result); Assert.Null(item);
    }

    [Fact]
    public void TryParseItem_EmptyToken_ReturnsFalse()
    {
        var categories = new[] { new Category(0, "People", ["A", "B"]), new Category(1, "Pets", ["X", "Y"]) };
        var result = CommandParser.TryParseItem(categories, "", out var item);
        Assert.False(result);
        Assert.Null(item);
    }

    [Fact]
    public void TryParseItem_WhitespaceToken_ReturnsFalse()
    {
        var categories = new[] { new Category(0, "People", ["A", "B"]), new Category(1, "Pets", ["X", "Y"]) };
        var result = CommandParser.TryParseItem(categories, "   ", out var item);
        Assert.False(result);
        Assert.Null(item);
    }

    [Fact]
    public void TryParseAction_Yes_ReturnsSetYes()
    {
        var categories = new[] { new Category(0, "People", ["A", "B"]), new Category(1, "Pets", ["X", "Y"]) };
        var result = CommandParser.TryParseAction(categories, ["Y", "0:0", "1:0"], out var action);
        Assert.True(result);
        Assert.NotNull(action);
        Assert.Equal(PlayerActionType.SetYes, action.Type);
        Assert.Equal(0, action.FirstItem.CategoryIndex);
        Assert.Equal(0, action.FirstItem.Index);
        Assert.Equal(1, action.SecondItem.CategoryIndex);
        Assert.Equal(0, action.SecondItem.Index);
    }

    [Fact]
    public void TryParseAction_No_ReturnsSetNo()
    {
        var categories = new[] { new Category(0, "People", ["A", "B"]), new Category(1, "Pets", ["X", "Y"]) };
        var result = CommandParser.TryParseAction(categories, ["N", "0:0", "1:0"], out var action);
        Assert.True(result);
        Assert.NotNull(action);
        Assert.Equal(PlayerActionType.SetNo, action.Type);
    }

    [Fact]
    public void TryParseAction_Clear_ReturnsClear()
    {
        var categories = new[] { new Category(0, "People", ["A", "B"]), new Category(1, "Pets", ["X", "Y"]) };
        var result = CommandParser.TryParseAction(categories, ["C", "0:0", "1:0"], out var action);
        Assert.True(result);
        Assert.NotNull(action);
        Assert.Equal(PlayerActionType.Clear, action.Type);
    }

    [Fact]
    public void TryParseAction_InvalidCommandOrArguments_ReturnsFalse()
    {
        var categories = new[] { new Category(0, "People", ["A", "B"]), new Category(1, "Pets", ["X", "Y"]) };
        Assert.False(CommandParser.TryParseAction(categories, ["Q", "0:0", "1:0"], out var action1));
        Assert.Null(action1);
        Assert.False(CommandParser.TryParseAction(categories, ["Y", "0:0"], out var action2));
        Assert.Null(action2);
        Assert.False(CommandParser.TryParseAction(categories, ["Y", "0:0", "1:0", "extra"], out var action3));
        Assert.Null(action3);
        Assert.False(CommandParser.TryParseAction(categories, ["Y", "0:0", "9:9"], out var action4));
        Assert.Null(action4);
    }
}
