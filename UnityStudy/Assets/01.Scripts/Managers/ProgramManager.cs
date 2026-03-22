using UnityEngine;

public class ProgramManager : MonoBehaviour
{
    private MapGenerator mapGenerator;
    private PathfindManager pathfindManager;

    private void Awake()
    {
        mapGenerator = FindAnyObjectByType<MapGenerator>();
        pathfindManager = FindAnyObjectByType<PathfindManager>();
    }

    private void Start()
    {
        mapGenerator.BuildTilemap();
        pathfindManager.Initialize();
        mapGenerator.GenerateMap(pathfindManager.SearchStartPos, pathfindManager.SearchTargetPos);
        pathfindManager.PostProcess();
    }

    public void StartPathfinding()
    {
        pathfindManager.StartPathfinding();
    }
}
