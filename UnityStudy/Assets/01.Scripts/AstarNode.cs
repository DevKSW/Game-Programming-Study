public class AstarNode
{
    private BoardPos boardPos;
    public BoardPos position => boardPos;

    private AstarNode parentNode;
    public AstarNode ParentNode;

    private float gCost;
    public float GCost {get => gCost; set => gCost = value;}

    private float hCost;
    public float HCost {get => hCost; set => hCost = value;}

    public float FCost => gCost + hCost;

    public AstarNode(BoardPos _boardPos)
    {
        boardPos = _boardPos;
        gCost = 9999;
    }
}
