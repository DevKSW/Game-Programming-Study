using UnityEngine;

public class TileMap : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;

    private Tile[,] tiles;

    private int mapWidth = 0;
    public int MapWidth => mapWidth;

    private int mapHeight = 0;
    public int MapHeight => mapHeight;

    public void InitTiles(int _mapWidth, int _mapHeight)
    {
        if(tiles != null)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                for(int y = 0; y < mapHeight; y++)
                {
                    Tile destroyTarget = tiles[x, y];
                    tiles[x, y] = null;
                    Destroy(destroyTarget);
                }
            }
        }

        mapWidth = _mapWidth;
        mapHeight = _mapHeight;

        tiles = new Tile[_mapWidth, _mapHeight];

        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                Tile newTile = Instantiate(tilePrefab, this.transform).GetComponent<Tile>();
                newTile.SetTile(new BoardPos(x, y), ETileType.NONE);
                tiles[x, y] = newTile;
            }
        }
    }

    public void SetTile(BoardPos _boardPos, Vector3 _worldPosition, ETileType _tileType)
    {
        Tile targetTile = GetTile(_boardPos);

        if(targetTile != null)
        {
            targetTile.SetTile(_tileType);
            targetTile.transform.position = _worldPosition;
        }
    }

    public bool IsPositionValid(BoardPos _boardPos)
    {
        int xPos = _boardPos.XPos;
        int yPos = _boardPos.YPos;

        if(xPos >= 0 && yPos >= 0 && xPos < mapWidth && yPos < mapHeight)
        {
            return true;
        }
        else
        {
            Debug.LogWarning($"Position ({xPos}, {yPos}) is unvalid!");
            return false;
        }
    }

    public Tile GetTile(BoardPos _boardPos)
    {
        return IsPositionValid(_boardPos) == true ? tiles[_boardPos.XPos, _boardPos.YPos] : null;
    }

    public bool IsTilePassable(BoardPos _boardPos)
    {
        Tile targetTile = GetTile(_boardPos);

        return targetTile != null && targetTile.GetTileType() == ETileType.TILE;
    }
}
