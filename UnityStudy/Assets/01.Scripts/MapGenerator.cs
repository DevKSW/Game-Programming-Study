using System.Collections.Generic;
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

    private void Awake()
    {
        Random.InitState(seed);
    }

    public void BuildTilemap()
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
    }

    public void GenerateMap(BoardPos _startPos, BoardPos _endPos)
    {
        switch(mapType)
        {
            case EMapType.WALL:
                GenerateWall(_startPos, _endPos);
                break;

            case EMapType.ROOM:
                GenerateRoom(_startPos, _endPos);
                break;
            
            case EMapType.MAZE:
                GenerateMaze(_startPos, _endPos);
                break;

            default:
                break;
        }

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

    private void GenerateWall(BoardPos _startPos, BoardPos _endPos)
    {
        int halfWidth = mapWidth / 2;
        int halfHeight = mapHeight / 2;

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
                else if(x == halfWidth && y > halfHeight - halfHeight / 2 && y < halfHeight + halfHeight / 2 ||
                    y == halfHeight && x > halfWidth - halfWidth / 2 && x < halfWidth + halfWidth / 2)
                {
                    currentTile.SetTile(ETileType.OBSTACLE);
                }
            }
        }
    }

    private void GenerateRoom(BoardPos _startPos, BoardPos _endPos)
    {
        int halfWidth = mapWidth / 2;
        int halfHeight = mapHeight / 2;

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
                else if(x == halfWidth || y == halfHeight)
                {
                    currentTile.SetTile(ETileType.OBSTACLE);
                }
            }
        }

        List<BoardPos> doorPositions = new List<BoardPos>();

        doorPositions.Add(new BoardPos(halfWidth, 2));
        doorPositions.Add(new BoardPos(halfWidth, mapHeight - 3));
        doorPositions.Add(new BoardPos(mapWidth - 3, halfHeight));
        doorPositions.Add(new BoardPos(2, halfHeight));

        doorPositions.RemoveAt(Random.Range(0, 4));

        foreach(BoardPos doorPosition in doorPositions)
        {
            Tile currentTile = tileMap.GetTile(doorPosition);
            currentTile.SetTile(ETileType.TILE);
        }
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
