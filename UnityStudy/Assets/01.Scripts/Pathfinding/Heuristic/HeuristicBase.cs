using UnityEngine;

public abstract class HeuristicBase : MonoBehaviour, IHeuristic
{
    public abstract float GetHeuristic(BoardPos _startPos, BoardPos _endPos);
}
