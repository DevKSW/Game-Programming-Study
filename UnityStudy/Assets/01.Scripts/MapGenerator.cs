using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float tileSpace;

    private TileMap tileMap;

    private void Start()
    {
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

        GenerateMaze(new BoardPos(0, 1), new BoardPos(mapWidth - 1, mapHeight - 2));

        AdjustCameraCenter();
    }

    public void AdjustCameraCenter()
    {
        float centerX = (mapWidth - 1) * 0.5f;
        float centerY = (mapHeight - 1) * 0.5f;

        Camera.main.transform.position = new Vector3(centerX + (centerX * tileSpace), centerY + (centerY * tileSpace), -10f);

        int longestSize = Mathf.Max(mapWidth, mapHeight);
        Camera.main.orthographicSize = longestSize / 2 + longestSize / 2 * 0.1f + longestSize * tileSpace;
    }

    private void GenerateMaze(BoardPos _startPos, BoardPos _endPos)
    {
        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                BoardPos currentBoardPos = new BoardPos(x, y);
                Tile currentTile = tileMap.GetTile(currentBoardPos);

                if(currentBoardPos.Equals(_startPos) || currentBoardPos.Equals(_endPos))
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
