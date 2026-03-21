using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarMap : MonoBehaviour
{
    private TileMap tilemap;

    [SerializeField] private EHeuristicType heuristicType;
    [SerializeField] private float heuristicMultiplier = 1;

    [SerializeField] private BoardPos searchStartPos;
    public BoardPos SearchStartPos => searchStartPos;

    [SerializeField] private BoardPos searchTargetPos;
    public BoardPos SearchTargetPos => searchTargetPos;

    [SerializeField] private float searchDelay;

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
        MarkNodeWithColor(_startPos, _targetPos, _startPos, Color.blue, true);
        MarkNodeWithColor(_startPos, _targetPos, _targetPos, Color.blue, true);

        List<AstarNode> openList = new List<AstarNode>();
        List<AstarNode> closedList = new List<AstarNode>();

        AstarNode startNode = astarNodes[_startPos.XPos, _startPos.YPos];
        startNode.GCost = 0;

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            yield return new WaitForSeconds(_searchDelay);

            AstarNode currentNode = FindHighestPriorityNode(openList);
            openList.Remove(currentNode);

            MarkNodeWithColor(_startPos, _targetPos, currentNode.position, Color.orange);
            
            if (_targetPos.Equals(currentNode.position))
            {
                while(currentNode.ParentNode != null)
                {
                    currentNode = currentNode.ParentNode;

                    if(currentNode.position.Equals(_startPos) == false)
                    {
                        MarkNodeWithColor(_startPos, _targetPos, currentNode.position, Color.green);
                    }
                }

                break;             
            }
            
            closedList.Add(currentNode);

            foreach (var dir in directions)
            {
                BoardPos newPos = new BoardPos(currentNode.position.XPos + dir.x, currentNode.position.YPos + dir.y);

                if(tilemap.IsTilePassable(newPos) == false ||
                    closedList.Contains(astarNodes[newPos.XPos, newPos.YPos]))
                {
                    continue;
                }

                AstarNode neighborNode = astarNodes[newPos.XPos, newPos.YPos];

                float hCost = GetHeuristic(newPos, _targetPos);
                float gCost = (dir.x == 0 || dir.y == 0) ? 1.0f : 1.4f;
                gCost += currentNode.GCost;

                if (hCost + gCost < neighborNode.FCost)
                {
                    if (openList.Contains(neighborNode))
                    {
                        openList.Remove(neighborNode);
                    }
                
                    neighborNode.GCost = gCost;
                    neighborNode.HCost = hCost;
                    neighborNode.ParentNode = currentNode;

                    openList.Add(neighborNode);

                    MarkNodeWithColor(_startPos, _targetPos, neighborNode.position, Color.yellow);
                }
            }
        }

        Debug.Log("Search Ended");
    }

    private AstarNode FindHighestPriorityNode(List<AstarNode> _nodes)
    {
        AstarNode highestPriorityNode = null;

        foreach(AstarNode node in _nodes)
        {
            if(highestPriorityNode == null || highestPriorityNode.FCost > node.FCost)
            {
                highestPriorityNode = node;
            }
        }

        return highestPriorityNode;
    }

    private float GetHeuristic(BoardPos _startPos, BoardPos _endPos)
    {
        float result = 0;

        switch(heuristicType)
        {
            case EHeuristicType.MANHATTAN_DISTANCE:
                result = Mathf.Abs(_startPos.XPos - _endPos.XPos) + Mathf.Abs(_startPos.YPos - _endPos.YPos);
                break;

            case EHeuristicType.EUCLIDEAN_DISTANCE:
                result = Mathf.Sqrt(Mathf.Pow(_startPos.XPos - _endPos.XPos, 2) + Mathf.Pow(_startPos.YPos - _endPos.YPos, 2));
                break;

            case EHeuristicType.CHEBYSHEV_DISTANCE:
                result = Mathf.Abs(_startPos.XPos - _endPos.XPos) > Mathf.Abs(_startPos.YPos - _endPos.YPos)?
                    Mathf.Abs(_startPos.XPos - _endPos.XPos) : Mathf.Abs(_startPos.YPos - _endPos.YPos);
                break;

            default:
                break;
        }

        return result * heuristicMultiplier;
    }

    private void MarkNodeWithColor(BoardPos _startPos, BoardPos _endPos, BoardPos _targetPos, Color _color, bool _isOverlapWithStartEndAllowed = false)
    {
        if(_isOverlapWithStartEndAllowed == true)
        {
            tilemap.GetTile(_targetPos).ChangeTileColor(_color);
        }
        else if(_targetPos.Equals(_startPos) == false && _targetPos.Equals(_endPos) == false)
        {
            tilemap.GetTile(_targetPos).ChangeTileColor(_color);
        }
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
