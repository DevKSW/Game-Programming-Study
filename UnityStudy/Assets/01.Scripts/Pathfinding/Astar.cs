using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar : PathfindingBase
{
    [SerializeField] private HeuristicBase heuristic;

    [SerializeField] private float heuristicMultiplier = 1;
    public float HeuristicMultiplier => heuristicMultiplier;

    private AstarNode[,] astarMap;

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

    public override void PostProcess()
    {
        if(heuristic is IPostProcessable)
        {
            IPostProcessable postProcessable = heuristic as IPostProcessable;
            postProcessable.PostProcess();
        }
    }

    public override void InitNodes(int _mapWidth, int _mapHeight)
    {
        astarMap = new AstarNode[_mapWidth, _mapHeight];

        for(int x = 0; x < _mapWidth; x++)
        {
            for(int y = 0; y < _mapHeight; y++)
            {
                AstarNode newNode = new AstarNode(new BoardPos(x, y));
                astarMap[x, y] = newNode;
            }
        }
    }

    public override IEnumerator ProcessPathfinding(BoardPos _startPos, BoardPos _targetPos, float _searchDelay)
    {
        MarkNodeWithColor(_startPos, _targetPos, _startPos, Color.blue, true);
        MarkNodeWithColor(_startPos, _targetPos, _targetPos, Color.blue, true);

        List<AstarNode> openList = new List<AstarNode>();
        List<AstarNode> closedList = new List<AstarNode>();

        AstarNode startNode = astarMap[_startPos.XPos, _startPos.YPos];
        startNode.GCost = 0;

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            yield return new WaitForSeconds(_searchDelay);

            AstarNode currentNode = FindHighestPriorityNode(openList);
            Debug.Log($"Current Node : ({currentNode.position.XPos}, {currentNode.position.YPos})");
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            MarkNodeWithColor(_startPos, _targetPos, currentNode.position, Color.orange);
            ++visitCount;
            
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

            foreach (var dir in directions)
            {
                BoardPos newPos = new BoardPos(currentNode.position.XPos + dir.x, currentNode.position.YPos + dir.y);

                if(tilemap.IsTilePassable(newPos) == false ||
                    closedList.Contains(BoardPosToNode(newPos)) == true)
                {
                    continue;
                }

                AstarNode neighborNode = BoardPosToNode(newPos);

                float hCost = heuristic.GetHeuristic(newPos, _targetPos);
                float gCost = (dir.x == 0 || dir.y == 0) ? 1.0f : 1.4f;
                gCost += currentNode.GCost;

                if (hCost + gCost < neighborNode.FCost)
                {
                    neighborNode.GCost = gCost;
                    neighborNode.HCost = hCost;
                    neighborNode.ParentNode = currentNode;

                    if(openList.Contains(neighborNode) == false)
                    {
                        openList.Add(neighborNode);
                    }

                    MarkNodeWithColor(_startPos, _targetPos, neighborNode.position, Color.yellow);
                }
            }
        }

        Debug.Log("Search Ended");
    }

    public AstarNode BoardPosToNode(BoardPos _boardPos)
    {
        return tilemap.IsPositionValid(_boardPos) == true ? astarMap[_boardPos.XPos, _boardPos.YPos] : null;
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
}
