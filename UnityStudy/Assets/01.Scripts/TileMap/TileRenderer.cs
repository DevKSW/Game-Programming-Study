using UnityEngine;

public class TileRenderer : MonoBehaviour
{
    [SerializeField] private Color normalTileColor;
    [SerializeField] private Color obstacleTileColor;
    private Color storedColor;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RenderTile(ETileType _tileType)
    {
        switch(_tileType)
        {
            case ETileType.TILE:
                storedColor = normalTileColor;
                spriteRenderer.color = normalTileColor;
                break;

            case ETileType.OBSTACLE:
                storedColor = obstacleTileColor;
                spriteRenderer.color = obstacleTileColor;
                break;

            default:
                spriteRenderer.color = new Color(0f, 0f, 0f, 0f);
                break;
        }
    }

    public void ChangeColor(Color _color)
    {
        spriteRenderer.color = _color;
    }

    public void RevertColor()
    {
        
    }
}
