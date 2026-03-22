using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //[SerializeField] private TextMeshProUGUI heuristicType;
    //[SerializeField] private TextMeshProUGUI heuristicMultiplier;
    [SerializeField] private TextMeshProUGUI visitCounter;

    private PathfindManager pathfindManager;

    private void Awake()
    {
        pathfindManager = FindAnyObjectByType<PathfindManager>();

        //RefreshHeuristicType();
        //heuristicMultiplier.text = $"Heuristic multiplier : {astarMap.HeuristicMultiplier}";
        visitCounter.text = "";
    }

    public void Update()
    {
        visitCounter.text = $"Visited node count : {pathfindManager.VisitCount}";
    }

    /*private void RefreshHeuristicType()
    {
        heuristicType.text = "";

        switch(astarMap.HeuristicType)
        {
            case EHeuristicType.ZERO_HEURISTIC:
                heuristicType.text = "Heuristic : Zero Heuristic(Dijkstra)";
                break;

            case EHeuristicType.MANHATTAN_DISTANCE:
                heuristicType.text = "Heuristic : Manhattan";
                break;

            case EHeuristicType.EUCLIDEAN_DISTANCE:
                heuristicType.text = "Heuristic : Euclidean";
                break;

            case EHeuristicType.CHEBYSHEV_DISTANCE:
                heuristicType.text = "Heuristic : Chebyshev";
                break;

            case EHeuristicType.ALT:
                heuristicType.text = "Heuristic : ALT";
                break;

            default:
                break;
        }
    }*/
}
