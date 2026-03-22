using UnityEngine;

public class PathfindManager : MonoBehaviour
{
    private TileMap tilemap;

    [SerializeField] private PathfindingBase pathfindingAlgorithm;

    [SerializeField] private BoardPos searchStartPos;
    public BoardPos SearchStartPos => searchStartPos;

    [SerializeField] private BoardPos searchTargetPos;
    public BoardPos SearchTargetPos => searchTargetPos;

    [SerializeField] private float searchDelay;

    private int mapWidth = 0;
    private int mapHeight = 0;

    public int VisitCount => pathfindingAlgorithm.VisitCount;

    private void Awake()
    {
        tilemap = FindAnyObjectByType<TileMap>();
    }

    public void Initialize()
    {
        mapWidth = tilemap.MapWidth;
        mapHeight = tilemap.MapHeight;

        ValidateSearchPosition();

        pathfindingAlgorithm.Initialize(tilemap);
    }

    public void PostProcess()
    {
        pathfindingAlgorithm.PostProcess();
    }

    private void InitNodes()
    {
        pathfindingAlgorithm.InitNodes(mapWidth, mapHeight);
    }

    public void StartPathfinding()
    {
        InitNodes();
        pathfindingAlgorithm.StartPathfinding(searchStartPos, searchTargetPos, searchDelay);
    }

    /// <summary>
    /// 탐색 시작, 종료 지점 유효한 값으로 갱신하는 메소드
    /// </summary>
    public void ValidateSearchPosition()
    {
        int startX = searchStartPos.XPos;
        startX = startX < 0 ? mapWidth + startX : startX;

        int startY = searchStartPos.YPos;
        startY = startY < 0 ? mapHeight + startY : startY;

        int endX = searchTargetPos.XPos;
        endX = endX < 0 ? mapWidth + endX : endX;

        int endY = searchTargetPos.YPos;
        endY = endY < 0 ? mapHeight + endY : endY;

        searchStartPos = new BoardPos(startX, startY);
        searchTargetPos = new BoardPos(endX, endY);
    }
}
