using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private EMapType mapType;
    [SerializeField] private int seed;
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float tileSpace;

    private TileMap tileMap;
    private PathfindManager pathfindManager;

    private void Start()
    {
        Random.InitState(seed);
        Generate();
    }

    public void Generate()
    {
        tileMap = FindAnyObjectByType<TileMap>();

        tileMap.InitTiles(mapWidth, mapHeight);

        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                BoardPos currentBoardPos = new BoardPos(x, y);
                Vector3 tileWorldPosition = new Vector3(x + (x * tileSpace), y + (y * tileSpace), 0f);
                tileMap.SetTile(currentBoardPos, tileWorldPosition, ETileType.TILE);
            }
        }

        pathfindManager = FindAnyObjectByType<PathfindManager>();
        pathfindManager.Initialize();

        switch(mapType)
        {
            case EMapType.WALL:
                GenerateWall();
                break;
            
            case EMapType.MAZE:
                GenerateMaze();
                break;

            default:
                break;
        }

        pathfindManager.PostProcess();
        AdjustCameraCenter();

        pathfindManager.StartPathfinding();
    }

    public void AdjustCameraCenter()
    {
        float centerX = (mapWidth - 1) * 0.5f;
        float centerY = (mapHeight - 1) * 0.5f;

        Camera.main.transform.position = new Vector3(centerX + (centerX * tileSpace), centerY + (centerY * tileSpace), -10f);

        int longestSize = Mathf.Max(mapWidth, mapHeight);
        Camera.main.orthographicSize = longestSize / 2 + longestSize / 2 * 0.1f + longestSize * tileSpace;
    }

    private void GenerateWall()
    {
        BoardPos startPos = pathfindManager.SearchStartPos;
        BoardPos endPos = pathfindManager.SearchTargetPos;

        int halfWidth = mapWidth / 2;
        int halfHeight = mapHeight / 2;

        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                BoardPos currentBoardPos = new BoardPos(x, y);
                Tile currentTile = tileMap.GetTile(currentBoardPos);

                if(currentBoardPos.Equals(startPos) || currentBoardPos.Equals(endPos))
                {
                    continue;
                }
                else if(x == halfWidth && y > halfHeight - halfHeight / 2 && y < halfHeight + halfHeight / 2 ||
                    y == halfHeight && x > halfWidth - halfWidth / 2 && x < halfWidth + halfWidth / 2)
                {
                    currentTile.SetTile(ETileType.OBSTACLE);
                }
            }
        }
    }

    private void GenerateMaze()
    {
        BoardPos startPos = pathfindManager.SearchStartPos;
        BoardPos endPos = pathfindManager.SearchTargetPos;

        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                BoardPos currentBoardPos = new BoardPos(x, y);
                Tile currentTile = tileMap.GetTile(currentBoardPos);

                if(currentBoardPos.Equals(startPos) || currentBoardPos.Equals(endPos))
                {
                    continue;
                }
                else if(x % 2 == 0 || y % 2 == 0 || x == 0 || y == 0 || x == mapWidth - 1 || y == mapHeight - 1)
                {
                    currentTile.SetTile(ETileType.OBSTACLE);
                }
            }
        }

        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                BoardPos targetBoardPos = new BoardPos(x, y);

                if(x % 2 == 0 || y % 2 == 0 || (x == mapWidth - 2 && y == mapHeight - 2))
                {
                    continue;
                }

                if(x == mapWidth - 2)
                {
                    targetBoardPos = new BoardPos(x, y + 1);
                }
                else if(y == mapHeight - 2)
                {
                    targetBoardPos = new BoardPos(x + 1, y);
                }
                else if(Random.Range(0, 2) == 0)
                {
                    targetBoardPos = new BoardPos(x, y + 1);
                }
                else
                {
                    targetBoardPos = new BoardPos(x + 1, y);
                }

                Tile targetTile = tileMap.GetTile(targetBoardPos);

                if(targetTile != null)
                {
                    targetTile.SetTile(ETileType.TILE);
                }
            }
        }
    }
}
