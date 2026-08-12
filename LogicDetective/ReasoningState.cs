namespace LogicDetective;

internal enum ReasoningState
{
    Unknown = 0,
    Positive = 1,
    Negative = 2
}

internal static class ReasoningStateExtension
{
    public static string GetMark(this ReasoningState state)
    {
        return state switch
        {
            ReasoningState.Unknown => "?",
            ReasoningState.Positive => "○",
            ReasoningState.Negative => "×",
            _ => throw new InvalidOperationException($"不正な推論状態: {state}"),
        };
    }
}
