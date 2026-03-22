using System.Collections.Generic;
using UnityEngine;

public class ALT : HeuristicBase, IPostProcessable
{
    private TileMap tilemap;

    private List<BoardPos> landmarks;
    private Dictionary<BoardPos, float[,]> costMaps;

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

    public void PostProcess()
    {
        landmarks = new List<BoardPos>();
        costMaps = new Dictionary<BoardPos, float[,]>();

        landmarks.Add(CreateLandmark(new BoardPos(0, 0)));
        landmarks.Add(CreateLandmark(new BoardPos(0, tilemap.MapHeight - 1)));
        landmarks.Add(CreateLandmark(new BoardPos(tilemap.MapWidth - 1, 0)));
        landmarks.Add(CreateLandmark(new BoardPos(tilemap.MapWidth - 1, tilemap.MapHeight - 1)));

        foreach(BoardPos landmark in landmarks)
        {
            //tilemap.GetTile(landmark).ChangeTileColor(Color.red);
            costMaps.Add(landmark, CalculateCostMap(landmark));
        }
    }

    public BoardPos CreateLandmark(BoardPos _tryStartPos)
    {
        BoardPos currentPos = _tryStartPos;

        Queue<BoardPos> checkQueue = new Queue<BoardPos>();
        List<BoardPos> visitedList = new List<BoardPos>();
        
        while(tilemap.IsTilePassable(currentPos) == false)
        {
            foreach (var dir in directions)
            {
                BoardPos newPos = new BoardPos(currentPos.XPos + dir.x, currentPos.YPos + dir.y);

                if(tilemap.IsPositionValid(newPos) == true && visitedList.Contains(newPos) == false)
                {
                    checkQueue.Enqueue(newPos);
                    visitedList.Add(newPos);
                }
            }

            currentPos = checkQueue.Dequeue();
        }
        
        return currentPos;
    }

    public float[,] CalculateCostMap(BoardPos _calculateStartPos)
    {
        float[,] costMap = new float[tilemap.MapWidth, tilemap.MapHeight];

        for(int x = 0; x < tilemap.MapWidth; x++)
        {
            for(int y = 0; y < tilemap.MapHeight; y++)
            {
                costMap[x, y] = int.MaxValue;
            }
        }

        costMap[_calculateStartPos.XPos, _calculateStartPos.YPos] = 0f;

        List<BoardPos> openList = new List<BoardPos>();
        List<BoardPos> closedList = new List<BoardPos>();

        openList.Add(_calculateStartPos);

        while (openList.Count > 0)
        {
            BoardPos currentPos = FindHighestPriorityNodeInCostMap(openList, costMap).Value;

            openList.Remove(currentPos);
            closedList.Add(currentPos);

            foreach (var dir in directions)
            {
                BoardPos newPos = new BoardPos(currentPos.XPos + dir.x, currentPos.YPos + dir.y);

                if(tilemap.IsTilePassable(newPos) == false || closedList.Contains(newPos) == true)
                {
                    continue;
                }

                float moveCost = currentPos.XPos != newPos.XPos && currentPos.YPos != newPos.YPos ? 1.4f : 1.0f;

                if(costMap[currentPos.XPos, currentPos.YPos] + moveCost < costMap[newPos.XPos, newPos.YPos])
                {
                    costMap[newPos.XPos, newPos.YPos] = costMap[currentPos.XPos, currentPos.YPos] + moveCost;

                    if(openList.Contains(newPos) == false)
                    {
                        openList.Add(newPos);
                    }
                }
            }
        }

        return costMap;
    }

    private BoardPos? FindHighestPriorityNodeInCostMap(List<BoardPos> _nodes, float[,] _costMap)
    {
        BoardPos? highestPriorityNode = null;

        foreach(BoardPos node in _nodes)
        {
            if(highestPriorityNode == null || 
                _costMap[highestPriorityNode.Value.XPos, highestPriorityNode.Value.YPos] > _costMap[node.XPos, node.YPos])
            {
                highestPriorityNode = node;
            }
        }

        return highestPriorityNode;
    }

    public override float GetHeuristic(BoardPos _startPos, BoardPos _endPos)
    {
        float biggestHeuristic = 0;

        foreach(BoardPos landmark in landmarks)
        {
            float landmarkToStartPos = costMaps[landmark][_startPos.XPos, _startPos.YPos];
            float landmarkToEndPos = costMaps[landmark][_endPos.XPos, _endPos.YPos];

            float heuristic = Mathf.Abs(landmarkToStartPos - landmarkToEndPos);

            if(heuristic > biggestHeuristic)
            {
                biggestHeuristic = heuristic;
            }
        }

        return biggestHeuristic;
    }
}
