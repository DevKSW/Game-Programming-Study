using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPS : PathfindingBase
{
    [SerializeField] private HeuristicBase heuristic;

    [SerializeField] private float heuristicMultiplier = 1;
    public float HeuristicMultiplier => heuristicMultiplier;

    private AstarNode[,] jpsMap;

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
        jpsMap = new AstarNode[_mapWidth, _mapHeight];

        for(int x = 0; x < _mapWidth; x++)
        {
            for(int y = 0; y < _mapHeight; y++)
            {
                AstarNode newNode = new AstarNode(new BoardPos(x, y));
                jpsMap[x, y] = newNode;
            }
        }
    }

    public override IEnumerator ProcessPathfinding(BoardPos _startPos, BoardPos _targetPos, float _searchDelay)
    {
        MarkNodeWithColor(_startPos, _targetPos, _startPos, Color.blue, true);
        MarkNodeWithColor(_startPos, _targetPos, _targetPos, Color.blue, true);

        List<AstarNode> openList = new List<AstarNode>();
        List<AstarNode> closedList = new List<AstarNode>();

        AstarNode startNode = jpsMap[_startPos.XPos, _startPos.YPos];
        startNode.GCost = 0;

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            yield return new WaitForSeconds(_searchDelay);

            AstarNode currentNode = FindHighestPriorityNode(openList);

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            ++visitCount;

            if(currentNode.position.Equals(_targetPos) == true)
            {
                VisualizePath(_startPos, _targetPos);
                yield break;
            }

            //foreach (Vector2Int dir in directions)
            foreach (Vector2Int dir in GetPrunedDirections(currentNode))
            {
                BoardPos? jumpPoint = null;
                yield return StartCoroutine(ProcessJumpPointFinding(currentNode.position, _targetPos, dir, _searchDelay, result => jumpPoint = result));

                if(jumpPoint == null || closedList.Contains(BoardPosToNode(jumpPoint.Value)) == true)
                {
                    continue;
                }

                AstarNode neighborNode = BoardPosToNode(jumpPoint.Value);

                float hCost = heuristic.GetHeuristic(jumpPoint.Value, _targetPos);
                float gCost = GetDistanceBetweenBoardPos(currentNode.position, jumpPoint.Value);
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
                }
            }
        }

        Debug.Log("Search Ended");
    }

    public AstarNode BoardPosToNode(BoardPos _boardPos)
    {
        return tilemap.IsPositionValid(_boardPos) == true ? jpsMap[_boardPos.XPos, _boardPos.YPos] : null;
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

    private List<Vector2Int> GetPrunedDirections(AstarNode _node)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        if (_node.ParentNode == null)
        {
            result.AddRange(directions);
            return result;
        }

        BoardPos pos = _node.position;
        BoardPos parent = _node.ParentNode.position;

        int dx = Mathf.Clamp(pos.XPos - parent.XPos, -1, 1);
        int dy = Mathf.Clamp(pos.YPos - parent.YPos, -1, 1);

        int x = pos.XPos;
        int y = pos.YPos;

        if (dx != 0 && dy != 0)
        {
            result.Add(new Vector2Int(dx, dy));
            result.Add(new Vector2Int(dx, 0));
            result.Add(new Vector2Int(0, dy));

            if (!tilemap.IsTilePassable(new BoardPos(x - dx, y)) &&
                tilemap.IsTilePassable(new BoardPos(x - dx, y + dy)))
            {
                result.Add(new Vector2Int(-dx, dy));
            }

            if (!tilemap.IsTilePassable(new BoardPos(x, y - dy)) &&
                tilemap.IsTilePassable(new BoardPos(x + dx, y - dy)))
            {
                result.Add(new Vector2Int(dx, -dy));
            }
        }
        else if (dx != 0)
        {
            result.Add(new Vector2Int(dx, 0));

            if (!tilemap.IsTilePassable(new BoardPos(x, y + 1)) &&
                tilemap.IsTilePassable(new BoardPos(x + dx, y + 1)))
            {
                result.Add(new Vector2Int(dx, 1));
            }

            if (!tilemap.IsTilePassable(new BoardPos(x, y - 1)) &&
                tilemap.IsTilePassable(new BoardPos(x + dx, y - 1)))
            {
                result.Add(new Vector2Int(dx, -1));
            }
        }
        else if (dy != 0)
        {
            result.Add(new Vector2Int(0, dy));

            if (!tilemap.IsTilePassable(new BoardPos(x + 1, y)) &&
                tilemap.IsTilePassable(new BoardPos(x + 1, y + dy)))
            {
                result.Add(new Vector2Int(1, dy));
            }

            if (!tilemap.IsTilePassable(new BoardPos(x - 1, y)) &&
                tilemap.IsTilePassable(new BoardPos(x - 1, y + dy)))
            {
                result.Add(new Vector2Int(-1, dy));
            }
        }

        return result;
    }

    private IEnumerator ProcessJumpPointFinding(BoardPos _start, BoardPos _target, Vector2Int _direction, float _searchDelay, Action<BoardPos?> _onComplete)
    {
        BoardPos current = _start;
        BoardPos? jumpPoint = null;

        while(true)
        {
            yield return new WaitForSeconds(_searchDelay);

            if(current.Equals(_start) == false)
            {
                MarkNodeWithColor(_start, _target, current, Color.yellow);
            }

            current = new BoardPos(current.XPos + _direction.x, current.YPos + _direction.y);

            if(tilemap.IsTilePassable(current) == false)
            {
                break;
            }

            MarkNodeWithColor(_start, _target, current, Color.orange);

            if(current.Equals(_target) == true || IsForcedNeighborFounded(current, _direction) == true)
            {
                jumpPoint = current;
                MarkNodeWithColor(_start, _target, current, Color.red);
                break;
            }

            if(Mathf.Abs(_direction.x) + Mathf.Abs(_direction.y) == 2)
            {
                /*if (!tilemap.IsTilePassable(new BoardPos(current.XPos - _direction.x, current.YPos)) ||
                !tilemap.IsTilePassable(new BoardPos(current.XPos, current.YPos - _direction.y)))
                {
                    break;
                }*/

                BoardPos? xMoveJumpPoint = null;
                BoardPos? yMoveJumpPoint = null;

                yield return StartCoroutine(ProcessJumpPointFinding(current, _target, new Vector2Int(_direction.x, 0), _searchDelay, (result => xMoveJumpPoint = result)));
                yield return StartCoroutine(ProcessJumpPointFinding(current, _target, new Vector2Int(0, _direction.y), _searchDelay, (result => yMoveJumpPoint = result)));

                if(xMoveJumpPoint != null || yMoveJumpPoint != null)
                {
                    jumpPoint = current;
                    MarkNodeWithColor(_start, _target, current, Color.red);
                    break;
                }
            }
        }

        _onComplete(jumpPoint);
    }

    private bool IsForcedNeighborFounded(BoardPos _boardPos, Vector2Int _direction)
    {
        int dx = _direction.x;
        int dy = _direction.y;

        if (dx != 0 && dy == 0)
        {
            if (tilemap.IsTilePassable(new BoardPos(_boardPos.XPos, _boardPos.YPos + 1)) == false && 
                tilemap.IsTilePassable(new BoardPos(_boardPos.XPos + dx, _boardPos.YPos + 1)) == true)
            {
                return true;
            }
                
            if (tilemap.IsTilePassable(new BoardPos(_boardPos.XPos, _boardPos.YPos - 1)) == false && 
                tilemap.IsTilePassable(new BoardPos(_boardPos.XPos + dx, _boardPos.YPos - 1)) == true)
            {
                return true;
            } 
        }
        else if (dx == 0 && dy != 0)
        {
            if (tilemap.IsTilePassable(new BoardPos(_boardPos.XPos + 1, _boardPos.YPos)) == false && 
                tilemap.IsTilePassable(new BoardPos(_boardPos.XPos + 1, _boardPos.YPos + dy)) == true)
            {
                return true;
            }

            if (tilemap.IsTilePassable(new BoardPos(_boardPos.XPos - 1, _boardPos.YPos)) == false &&
                tilemap.IsTilePassable(new BoardPos(_boardPos.XPos - 1, _boardPos.YPos + dy)) == true)
            {
                return true;
            }
        }
        else if (dx != 0 && dy != 0)
        {
            if (tilemap.IsTilePassable(new BoardPos(_boardPos.XPos - dx, _boardPos.YPos)) == false && 
                tilemap.IsTilePassable(new BoardPos(_boardPos.XPos - dx, _boardPos.YPos + dy)) == true)
            {
                return true;
            }
                
            if (tilemap.IsTilePassable(new BoardPos(_boardPos.XPos, _boardPos.YPos - dy)) == false && 
                tilemap.IsTilePassable(new BoardPos(_boardPos.XPos + dx, _boardPos.YPos - dy)) == true)
            {
                return true;
            }   
        }

        return false;
    }

    private float GetDistanceBetweenBoardPos(BoardPos _posA, BoardPos _posB)
    {
        int dx = Mathf.Abs(_posA.XPos - _posB.XPos);
        int dy = Mathf.Abs(_posA.YPos - _posB.YPos);

        int diagonal = Mathf.Min(dx, dy);
        int straight = Mathf.Abs(dx - dy);

        return diagonal * 1.4f + straight * 1.0f;
    }

    private List<BoardPos> GetPathBetweenBoardPos(BoardPos from, BoardPos to)
    {
        List<BoardPos> path = new List<BoardPos>();

        int dx = Mathf.Clamp(to.XPos - from.XPos, -1, 1);
        int dy = Mathf.Clamp(to.YPos - from.YPos, -1, 1);

        BoardPos current = from;

        while (!current.Equals(to))
        {
            current = new BoardPos(current.XPos + dx, current.YPos + dy);
            path.Add(current);
        }

        return path;
    }

    private void VisualizePath(BoardPos _startPos, BoardPos _targetPos)
    {
        AstarNode currentNode = BoardPosToNode(_targetPos);

        while (currentNode.ParentNode != null)
        {
            List<BoardPos> segment = GetPathBetweenBoardPos(currentNode.ParentNode.position, currentNode.position);
            AstarNode prevNode = currentNode;
            currentNode = currentNode.ParentNode;

            foreach(BoardPos boardPos in segment)
            {
                if(BoardPosToNode(boardPos) != prevNode && BoardPosToNode(boardPos) != currentNode)
                {
                    MarkNodeWithColor(_startPos, _targetPos, boardPos, Color.green, false);
                }
            }
        }
    }
}
