namespace LogicDetective;

internal static class PlayerActionProcessor
{
    public static void Apply(PlayerState playerState, PlayerAction action)
    {
        ArgumentNullException.ThrowIfNull(playerState);
        ArgumentNullException.ThrowIfNull(action);

        switch (action.Type)
        {
            case PlayerActionType.SetYes:
                playerState.SetState(action.FirstItem, action.SecondItem, PlayerPairState.Yes);
                return;
            case PlayerActionType.SetNo:
                playerState.SetState(action.FirstItem, action.SecondItem, PlayerPairState.No);
                return;
            case PlayerActionType.Clear:
                playerState.SetState(action.FirstItem, action.SecondItem, PlayerPairState.Unknown);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action.Type, "不正な操作種別です。");
        }
    }
}
