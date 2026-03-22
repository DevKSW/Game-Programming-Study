using System.Collections;

public interface IPathfinding
{
    public int VisitCount { get; }

    public void Initialize(TileMap _tilemap);
    public void PostProcess();
    public void InitNodes(int _mapWidth, int _mapHeight);
    public void StartPathfinding(BoardPos _startPos, BoardPos _targetPos, float _searchDelay);
    public IEnumerator ProcessPathfinding(BoardPos _startPos, BoardPos _targetPos, float _searchDelay);
}
