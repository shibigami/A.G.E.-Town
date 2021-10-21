using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapNodes
{
    public const float NODEDISTANCE = 1f;
    public const float MAXDISTANCEFORSEARCH = 40f;
    public const float DEFAULTMOVECOST = 1f;

    private static Dictionary<Vector2, Node> nodes;

    private static WorldMapNodes _instance;

    public static WorldMapNodes Instance
    {
        get
        {
            if (nodes == null || nodes.Count <= 0) _instance = new WorldMapNodes();
            return _instance;
        }
    }

    private WorldMapNodes()
    {
        nodes = new Dictionary<Vector2, Node>();
    }

    public void CreateNodes()
    {
        nodes.Clear();

        for (float x = -MAXDISTANCEFORSEARCH; x < MAXDISTANCEFORSEARCH; x += NODEDISTANCE)
        {
            for (float y = -MAXDISTANCEFORSEARCH; y < MAXDISTANCEFORSEARCH; y += NODEDISTANCE)
            {
                var nodePosition = new Vector2(x, y);

                //check if can move here
                var blocked = false;
                var spherePosition = new Vector3(nodePosition.x, 0, nodePosition.y);
                var existingObjects = Physics.OverlapSphere(spherePosition, WorldMapNodes.NODEDISTANCE / 2.0f);
                for (int z = 0; z < existingObjects.Length; z++)
                {
                    if (existingObjects[z].gameObject.tag == "Player") continue;
                    if (!existingObjects[z].gameObject.GetComponent<Terrain>() &&
                        existingObjects[z].gameObject.tag != Constants.Tags.Player.ToString() &&
                        existingObjects[z].gameObject.GetComponent<Collider>())
                    {
                        blocked = true;
                        break;
                    }
                }

                if (blocked) continue;

                nodes.Add(nodePosition, new Node(x, y));
            }
        }
    }

    public Dictionary<Vector2, Node> getNodes()
    {
        return nodes;
    }

    public Node getNodeAt(Vector2 position)
    {
        try
        {
            return nodes[position];
        }
        catch
        {
            return null;
        }
    }

    public void UpdateNode(Vector2 nodeLocation, Node start, Node end, float moveCost)
    {
        nodes[nodeLocation].SetStartEndNodes(start.location, end.location, WorldMapNodes.DEFAULTMOVECOST);
    }
}
