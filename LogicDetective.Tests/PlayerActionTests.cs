namespace LogicDetective.Tests;

public class PlayerActionTests
{
    [Fact]
    public void SetYes_SetsYesInPlayerState()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");
        var action = new PlayerAction(PlayerActionType.SetYes, a, b);
        PlayerActionProcessor.Apply(state, action);
        Assert.Equal(PlayerPairState.Yes, state.GetState(a, b));
    }

    [Fact]
    public void SetNo_SetsNoInPlayerState()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");
        var action = new PlayerAction(PlayerActionType.SetNo, a, b);
        PlayerActionProcessor.Apply(state, action);
        Assert.Equal(PlayerPairState.No, state.GetState(a, b));
    }

    [Fact]
    public void Clear_SetsUnknownInPlayerState()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");
        PlayerActionProcessor.Apply(state, new PlayerAction(PlayerActionType.SetYes, a, b));
        PlayerActionProcessor.Apply(state, new PlayerAction(PlayerActionType.Clear, a, b));
        Assert.Equal(PlayerPairState.Unknown, state.GetState(a, b));
    }

    [Fact]
    public void PairOrder_IsHandledAsSamePair()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");
        PlayerActionProcessor.Apply(state, new PlayerAction(PlayerActionType.SetNo, a, b));
        Assert.Equal(PlayerPairState.No, state.GetState(b, a));
    }

    [Fact]
    public void DifferentPairs_AreIndependent()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");
        var c = new Item(2, 0, "C");
        PlayerActionProcessor.Apply(state, new PlayerAction(PlayerActionType.SetYes, a, b));
        Assert.Equal(PlayerPairState.Yes, state.GetState(a, b));
        Assert.Equal(PlayerPairState.Unknown, state.GetState(a, c));
    }

    [Fact]
    public void MultipleActions_CanBeAppliedInSequence()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");
        PlayerActionProcessor.Apply(state, new PlayerAction(PlayerActionType.SetYes, a, b));
        PlayerActionProcessor.Apply(state, new PlayerAction(PlayerActionType.SetNo, a, b));
        PlayerActionProcessor.Apply(state, new PlayerAction(PlayerActionType.Clear, a, b));
        Assert.Equal(PlayerPairState.Unknown, state.GetState(a, b));
    }

    [Fact]
    public void InvalidAction_IsRejected()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");

        Assert.Throws<ArgumentException>(() =>
            PlayerActionProcessor.Apply(state, new PlayerAction(PlayerActionType.SetYes, a, a)));

        Assert.Throws<ArgumentNullException>(() =>
            new PlayerAction(PlayerActionType.SetYes, null!, a));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PlayerActionProcessor.Apply(state, new PlayerAction((PlayerActionType)999, a, new Item(1, 0, "B"))));
    }
}
