using System.Collections;
using UnityEngine;

public abstract class PathfindingBase : MonoBehaviour, IPathfinding
{
    protected TileMap tilemap;

    protected int visitCount = 0;
    public int VisitCount => visitCount;

    public void Initialize(TileMap _tilemap)
    {
        tilemap = _tilemap;
    }

    public abstract void PostProcess();

    public abstract void InitNodes(int _mapWidth, int _mapHeight);

    public void StartPathfinding(BoardPos _startPos, BoardPos _targetPos, float _searchDelay)
    {
        StartCoroutine(ProcessPathfinding(_startPos, _targetPos, _searchDelay));
    }

    public abstract IEnumerator ProcessPathfinding(BoardPos _startPos, BoardPos _targetPos, float _searchDelay);

    protected void MarkNodeWithColor(BoardPos _startPos, BoardPos _endPos, BoardPos _targetPos, Color _color, bool _isOverlapWithStartEndAllowed = false)
    {
        if(_isOverlapWithStartEndAllowed == true)
        {
            tilemap.GetTile(_targetPos).ChangeTileColor(_color);
        }
        else if(_targetPos.Equals(_startPos) == false && _targetPos.Equals(_endPos) == false)
        {
            tilemap.GetTile(_targetPos).ChangeTileColor(_color);
        }
    }
}
