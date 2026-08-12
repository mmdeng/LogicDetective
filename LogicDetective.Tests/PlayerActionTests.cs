namespace LogicDetective.Tests;

public class PlayerActionTests
{
    [Fact]
    public void PairOrder_IsHandledAsSamePair()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");
        state.SetState(new PlayerAction(ReasoningState.Negative, a, b));
        Assert.Equal(ReasoningState.Negative, state.GetState(b, a));
    }

    [Fact]
    public void DifferentPairs_AreIndependent()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");
        var c = new Item(2, 0, "C");
        state.SetState(new PlayerAction(ReasoningState.Positive, a, b));
        Assert.Equal(ReasoningState.Positive, state.GetState(a, b));
        Assert.Equal(ReasoningState.Unknown, state.GetState(a, c));
    }

    [Fact]
    public void MultipleActions_CanBeAppliedInSequence()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");
        state.SetState(new PlayerAction(ReasoningState.Positive, a, b));
        state.SetState(new PlayerAction(ReasoningState.Negative, a, b));
        state.SetState(new PlayerAction(ReasoningState.Unknown, a, b));
        Assert.Equal(ReasoningState.Unknown, state.GetState(a, b));
    }

}
