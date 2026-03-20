using UnityEngine;

public class TileMap : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;

    private Tile[,] tiles;
    private int mapWidth;
    private int mapHeight;

    public void InitTiles(int _mapWidth, int _mapHeight)
    {
        if(tiles != null)
        {
            for(int i = 0; i < mapWidth; i++)
            {
                for(int j = 0; j < mapHeight; j++)
                {
                    Tile destroyTarget = tiles[i, j];
                    tiles[i, j] = null;
                    Destroy(destroyTarget);
                }
            }
        }

        mapWidth = _mapWidth;
        mapHeight = _mapHeight;

        tiles = new Tile[_mapWidth, _mapHeight];

        for(int i = 0; i < mapWidth; i++)
        {
            for(int j = 0; j < mapHeight; j++)
            {
                Tile newTile = Instantiate(tilePrefab, this.transform).GetComponent<Tile>();
                newTile.SetTile(new BoardPos(i, j), ETileType.NONE);
                tiles[i, j] = newTile;
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

        return xPos >= 0 || yPos >= 0 || xPos < mapWidth || yPos < mapHeight;
    }

    public Tile GetTile(BoardPos _boardPos)
    {
        return IsPositionValid(_boardPos) == true ? tiles[_boardPos.XPos, _boardPos.YPos] : null;
    }
}
