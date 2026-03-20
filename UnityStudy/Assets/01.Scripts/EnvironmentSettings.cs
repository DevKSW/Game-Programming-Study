public static class EnvironmentSettings
{
    private const bool IS_DIAGONAL_MOVE_ALLOWED = true;
    public static bool IsDiagonalMoveAllowed => IS_DIAGONAL_MOVE_ALLOWED;

    private const float DIAGONAL_MOVE_COST_MULTIPLIER = 1.4f;
    public static float DiagonalMoveCostMultiplier => DIAGONAL_MOVE_COST_MULTIPLIER;
}
