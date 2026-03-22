using UnityEngine;

public class EuclideanDistance : HeuristicBase
{
    public override float GetHeuristic(BoardPos _startPos, BoardPos _endPos)
    {
        return Mathf.Sqrt(Mathf.Pow(_startPos.XPos - _endPos.XPos, 2) + Mathf.Pow(_startPos.YPos - _endPos.YPos, 2));
    }
}
