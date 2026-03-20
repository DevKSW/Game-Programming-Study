using System;
using UnityEngine;

[Serializable]
public struct BoardPos
{
    [SerializeField] private int xPos;
    public int XPos => xPos;

    [SerializeField] private int yPos;
    public int YPos => yPos;

    public BoardPos(int _xPos, int _yPos)
    {
        xPos = _xPos;
        yPos = _yPos;
    }

    public readonly bool IsEquals(int _xPos, int _yPos)
    {
        return xPos == _xPos && yPos == _yPos;
    }    
}

public class Tile : MonoBehaviour
{
    private TileRenderer tileRenderer;

    private BoardPos boardPos;
    private ETileType tileType;

    private void Awake()
    {
        tileRenderer = GetComponent<TileRenderer>();
    }

    public void SetTile(ETileType _tileType)
    {
        tileType = _tileType;
        tileRenderer.RenderTile(tileType);
    }

    public void SetTile(BoardPos _boardPos, ETileType _tileType)
    {
        boardPos = _boardPos;
        tileType = _tileType;
        tileRenderer.RenderTile(tileType);
    }

    public void ChangeTileColor(Color _color)
    {
        tileRenderer.ChangeColor(_color);
    }

    public void RevertColor(Color _color)
    {
        tileRenderer.ChangeColor(_color);
    }

    public BoardPos GetBoardPos()
    {
        return boardPos;
    }

    public ETileType GetTileType()
    {
        return tileType;
    }
}
