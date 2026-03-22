using UnityEngine;

public class ManhattanDistance : HeuristicBase
{
    public override float GetHeuristic(BoardPos _startPos, BoardPos _endPos)
    {
        return Mathf.Abs(_startPos.XPos - _endPos.XPos) + Mathf.Abs(_startPos.YPos - _endPos.YPos);
    }
}
