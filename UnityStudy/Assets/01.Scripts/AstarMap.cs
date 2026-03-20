using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AstarMap : MonoBehaviour
{
    private TileMap tilemap;

    [SerializeField] private float searchDelay;

    [SerializeField] private BoardPos searchStartPos;
    public BoardPos SearchStartPos => searchStartPos;

    [SerializeField] private BoardPos searchTargetPos;
    public BoardPos SearchTargetPos => searchTargetPos;

    private AstarNode[,] astarNodes;
    private int mapWidth = 0;
    private int mapHeight = 0;

    private Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right, 
        Vector2Int.right + Vector2Int.up,
        Vector2Int.right + Vector2Int.down,
        Vector2Int.left + Vector2Int.up,
        Vector2Int.left + Vector2Int.down
    };

    private void Awake()
    {
        tilemap = FindAnyObjectByType<TileMap>();
    }

    public void Initialize()
    {
        mapWidth = tilemap.MapWidth;
        mapHeight = tilemap.MapHeight;

        ValidateSearchPosition();
    }

    private void InitNodes()
    {
        astarNodes = new AstarNode[mapWidth, mapHeight];

        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                AstarNode newNode = new AstarNode(new BoardPos(x, y));
                astarNodes[x, y] = newNode;
            }
        }
    }

    public void StartPathfinding()
    {
        InitNodes();
        StartCoroutine(ProcessPathfinding(searchStartPos, searchTargetPos, searchDelay));
    }

    private IEnumerator ProcessPathfinding(BoardPos _startPos, BoardPos _targetPos, float _searchDelay)
    {
        tilemap.GetTile(_startPos).ChangeTileColor(Color.blue);
        tilemap.GetTile(_targetPos).ChangeTileColor(Color.blue);

        SortedSet<AstarNode> openSet = new SortedSet<AstarNode>(Comparer<AstarNode>.Create((a, b) => 
            a.FCost == b.FCost ? a.HCost.CompareTo(b.HCost) : a.FCost.CompareTo(b.FCost)));

        HashSet<AstarNode> closedSet = new HashSet<AstarNode>();

        AstarNode startNode = astarNodes[_startPos.XPos, _startPos.YPos];
        startNode.GCost = 0;

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            yield return new WaitForSeconds(_searchDelay);

            AstarNode currentNode = openSet.First();
            openSet.Remove(currentNode);
            
            if (_targetPos.Equals(currentNode.position))
            {
                while(currentNode.ParentNode != null)
                {
                    currentNode = currentNode.ParentNode;

                    if(currentNode.position.Equals(_startPos) == false)
                    {
                        tilemap.GetTile(currentNode.position).ChangeTileColor(Color.green);
                    }
                }

                break;             
            }
            
            closedSet.Add(currentNode);

            foreach (var dir in directions)
            {
                BoardPos newPos = new BoardPos(currentNode.position.XPos + dir.x, currentNode.position.YPos + dir.y);

                if(tilemap.IsTilePassable(newPos) == false)
                {
                    continue;
                }
                
                if (closedSet.Contains(astarNodes[newPos.XPos, newPos.YPos]))
                {
                    continue;
                }

                AstarNode neighborNode = astarNodes[newPos.XPos, newPos.YPos];

                if(neighborNode.position.Equals(_startPos) == false && neighborNode.position.Equals(_targetPos) == false)
                {
                    tilemap.GetTile(neighborNode.position).ChangeTileColor(Color.yellow);
                }

                float hCost = GetHeuristic(newPos, _targetPos);
                float gCost = (dir.x == 0 || dir.y == 0) ? 1.0f : 1.4f;

                if (hCost + gCost < neighborNode.FCost)
                {
                    if (openSet.Contains(neighborNode))
                    {
                        openSet.Remove(neighborNode);
                    }
                
                    neighborNode.GCost = gCost;
                    neighborNode.HCost = hCost;
                    neighborNode.ParentNode = currentNode;

                    openSet.Add(neighborNode);
                }
            }
        }

        Debug.Log("Search Ended");
    }

    private float GetHeuristic(BoardPos _startPos, BoardPos _endPos)
    {
        float result = 0;
        
        // 유클리드
        result = Mathf.Sqrt(Mathf.Pow(_startPos.XPos - _endPos.XPos, 2) + Mathf.Pow(_startPos.YPos - _endPos.YPos, 2));

        
        // 체비쇼프
        /*result = Mathf.Abs(_startPos.XPos - _endPos.XPos) > Mathf.Abs(_startPos.YPos - _endPos.YPos)?
            Mathf.Abs(_startPos.XPos - _endPos.XPos) : Mathf.Abs(_startPos.YPos - _endPos.YPos);*/
        
        
        // 맨하탄
        /*result = Mathf.Abs(_startPos.XPos - _endPos.XPos) + Mathf.Abs(_startPos.YPos - _endPos.YPos);*/
        
        return result;
    }

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

    public AstarNode BoardPosToNode(BoardPos _boardPos)
    {
        return tilemap.IsPositionValid(_boardPos) == true ? astarNodes[_boardPos.XPos, _boardPos.YPos] : null;
    }
}
