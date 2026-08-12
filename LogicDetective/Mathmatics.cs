namespace LogicDetective;

public static class Mathmatics
{
    /// <summary>
    /// 順列 nPr を計算します。
    /// </summary>
    public static int Permutation(int n, int r)
    {
        if (n < 0 || r < 0 || r > n) return 0;

        int result = 1;
        for (int i = 0; i < r; i++)
        {
            result *= (n - i);
        }
        return result;
    }

    /// <summary>
    /// 階乗 n! を計算します。
    /// </summary>
    public static int Factorial(int n)
    {
        return Permutation(n, n);
    }

    /// <summary>
    /// 組み合わせ nCr を Permutation (nPr) を使って計算します。
    /// nCr = nPr / r!
    /// </summary>
    public static int Combination(int n, int r)
    {
        if (n < 0 || r < 0 || r > n) return 0;
        if (r == 0 || r == n) return 1;

        // 計算量を減らすために nCr = nC(n-r) の性質を適用
        if (r > n / 2) r = n - r;

        // nCr = nPr / r!
        return Permutation(n, r) / Factorial(r);
    }

    private static readonly Random random = new();
    public static void Shuffle<T>(IList<T> items)
    {
        for (var i = items.Count - 1; i > 0; i--)
        {
            var randomIndex = random.Next(i + 1);
            (items[i], items[randomIndex]) = (items[randomIndex], items[i]);
        }
    }

    public static bool NextPermutation(int[] values)
    {
        var pivotIndex = values.Length - 2;

        while (pivotIndex >= 0 && values[pivotIndex] >= values[pivotIndex + 1])
        {
            pivotIndex--;
        }
        if (pivotIndex < 0)
        {
            return false;
        }
        var swapIndex = values.Length - 1;
        while (values[swapIndex] <= values[pivotIndex])
        {
            swapIndex--;
        }
        (values[pivotIndex], values[swapIndex]) = (values[swapIndex], values[pivotIndex]);
        Array.Reverse(values, pivotIndex + 1, values.Length - pivotIndex - 1);
        return true;
    }
}
