namespace LogicDetective.Tests;

public class PlayerStateTests
{
    [Fact]
    public void InitialState_IsUnknown()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");

        var value = state.GetState(a, b);

        Assert.Equal(PlayerPairState.Unknown, value);
    }

    [Fact]
    public void SetState_Yes_StoresYes()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");

        state.SetState(a, b, PlayerPairState.Yes);

        Assert.Equal(PlayerPairState.Yes, state.GetState(a, b));
    }

    [Fact]
    public void SetState_No_StoresNo()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");

        state.SetState(a, b, PlayerPairState.No);

        Assert.Equal(PlayerPairState.No, state.GetState(a, b));
    }

    [Fact]
    public void PairOrder_IsSymmetric()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");

        state.SetState(a, b, PlayerPairState.Yes);

        Assert.Equal(PlayerPairState.Yes, state.GetState(a, b));
        Assert.Equal(PlayerPairState.Yes, state.GetState(b, a));
    }

    [Fact]
    public void State_CanBeChanged()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");

        state.SetState(a, b, PlayerPairState.Yes);
        state.SetState(a, b, PlayerPairState.No);
        state.SetState(a, b, PlayerPairState.Unknown);

        Assert.Equal(PlayerPairState.Unknown, state.GetState(a, b));
    }

    [Fact]
    public void IndependentPairs_DoNotAffectEachOther()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");
        var b = new Item(1, 0, "B");
        var c = new Item(2, 0, "C");

        state.SetState(a, b, PlayerPairState.Yes);

        Assert.Equal(PlayerPairState.Yes, state.GetState(a, b));
        Assert.Equal(PlayerPairState.Unknown, state.GetState(a, c));
        Assert.Equal(PlayerPairState.Unknown, state.GetState(b, c));
    }

    [Fact]
    public void SelfPair_IsRejected()
    {
        var state = new PlayerState();
        var a = new Item(0, 0, "A");

        Assert.Throws<ArgumentException>(() => state.GetState(a, a));
        Assert.Throws<ArgumentException>(() => state.SetState(a, a, PlayerPairState.Yes));
    }
}
