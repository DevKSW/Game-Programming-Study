using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float tileSpace;

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        TileMap tileMap = FindAnyObjectByType<TileMap>();

        tileMap.InitTiles(mapWidth, mapHeight);

        for(int i = 0; i < mapWidth; i++)
        {
            for(int j = 0; j < mapHeight; j++)
            {
                BoardPos currentBoardPos = new BoardPos(i, j);
                Vector3 tileWorldPosition = new Vector3(i + (i * tileSpace), j + (j * tileSpace), 0f);
                tileMap.SetTile(currentBoardPos, tileWorldPosition, ETileType.TILE);
            }
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
}
