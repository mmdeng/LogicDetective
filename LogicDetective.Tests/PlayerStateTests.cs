namespace LogicDetective.Tests;

public class PlayerStateTests
{
    [Fact]
    public void PairOrder_IsSymmetric()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");
        state.SetState(a, b, ReasoningState.Positive);
        Assert.Equal(ReasoningState.Positive, state.GetState(a, b));
        Assert.Equal(ReasoningState.Positive, state.GetState(b, a));
    }

    [Fact]
    public void IndependentPairs_DoNotAffectEachOther()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");
        var c = new Item(2, 0, "C");
        state.SetState(a, b, ReasoningState.Positive);
        Assert.Equal(ReasoningState.Positive, state.GetState(a, b));
        state.SetState(a, b, ReasoningState.Negative);
        Assert.Equal(ReasoningState.Negative, state.GetState(a, b));
        state.SetState(a, b, ReasoningState.Unknown);
        Assert.Equal(ReasoningState.Unknown, state.GetState(a, b));

        Assert.Equal(ReasoningState.Unknown, state.GetState(a, c));
        Assert.Equal(ReasoningState.Unknown, state.GetState(b, c));
    }

    [Fact]
    public void InvalidAction_IsRejected()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        Assert.Throws<ArgumentException>(() => state.GetState(a, a));
        Assert.Throws<ArgumentException>(() => state.SetState(a, a, ReasoningState.Positive));
        Assert.Throws<ArgumentException>(() => state.SetState(new PlayerAction(ReasoningState.Positive, a, a)));
        Assert.Throws<ArgumentNullException>(() => state.SetState(new PlayerAction(ReasoningState.Positive, null!, a)));
    }
}
