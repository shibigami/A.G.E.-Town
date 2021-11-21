using UnityEngine;

public class Node
{
    public Vector2 location { get; private set; }
    public float g { get; private set; }  //distance from starting node
    public float h { get; private set; }  //distance from end node
    public float f { get; private set; }
    public Node parentNode { get; private set; }


    public Node() { }

    public Node(Vector2 position)
    {
        location = position;
        g = 0;
        h = 0;
        f = 0;
    }
    public Node(float x, float y)
    {
        location = new Vector2(x, y);
        g = 0;
        h = 0;
        f = 0;
    }

    public void SetStartEndNodes(Vector2 start, Vector2 end, float costToMove)
    {
        g = Mathf.Sqrt(Mathf.Pow((location.x - start.x), 2)) + Mathf.Sqrt(Mathf.Pow((location.y - start.y), 2)) + costToMove;
        h = Mathf.Sqrt(Mathf.Pow((location.x - end.x), 2)) + Mathf.Sqrt(Mathf.Pow((location.y - end.y), 2));

        f = g + h;
    }

    public void SetParent(Node parent)
    {
        parentNode = parent;
    }

    public void SetLocation(Vector2 locationPoint)
    {
        location = locationPoint;
    }
}