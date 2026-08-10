namespace LogicDetective;

internal sealed class Solution
{
    private readonly int[][] _groups;

    public int GroupCount { get; }
    public int CategoryCount { get; }

    public Solution(int groupCount, int categoryCount)
    {
        GroupCount = groupCount;
        CategoryCount = categoryCount;
        _groups = new int[groupCount][];

        for (var group = 0; group < groupCount; group++)
        {
            _groups[group] = new int[categoryCount];
        }
    }

    public int GetItemIndex(int group, int category)
    {
        return _groups[group][category];
    }

    public void SetItemIndex(int group, int category, int itemIndex)
    {
        _groups[group][category] = itemIndex;
    }

    public bool AreSameGroup(int categoryA, int itemA, int categoryB, int itemB)
    {
        for (var group = 0; group < GroupCount; group++)
        {
            if (_groups[group][categoryA] == itemA)
            {
                return _groups[group][categoryB] == itemB;
            }
        }
        throw new InvalidOperationException("指定された項目が解に存在しません。");
    }
}