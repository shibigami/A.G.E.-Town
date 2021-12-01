using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapNodes
{
    public const float NODEDISTANCE = 0.5f;
    public const float MAXDISTANCEFORSEARCH = 200f;
    public const float DEFAULTMOVECOST = 1.0f;

    private static Dictionary<Vector2, Node> nodes;
    private static bool mapComplete;

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
        mapComplete = false;
    }

    public void CreateNodes()
    {
        nodes.Clear();
        //commented out code places sphere in node so that blocked nodes can be visualized
        //var pri = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //pri.transform.localScale = Vector3.one / 10;
        for (float x = -MAXDISTANCEFORSEARCH; x < MAXDISTANCEFORSEARCH; x += NODEDISTANCE)
        {
            for (float y = -MAXDISTANCEFORSEARCH; y < MAXDISTANCEFORSEARCH; y += NODEDISTANCE)
            {
                var nodePosition = new Vector2(x, y);

                ////check if can move here
                var blocked = false;
                var spherePosition = new Vector3(nodePosition.x, 0, nodePosition.y);
                var existingObjects = Physics.OverlapSphere(spherePosition, WorldMapNodes.NODEDISTANCE / 2.0f);
                for (int z = 0; z < existingObjects.Length; z++)
                {
                    if (existingObjects[z].gameObject.name == "Terrain" || existingObjects[z].gameObject.tag == "Door") continue;

                    if (existingObjects[z].gameObject.tag != Constants.Tags.Player.ToString() &&
                        existingObjects[z].gameObject.GetComponent<BoxCollider>())
                    {
                        //var sphere = GameObject.Instantiate(pri, new Vector3(x, 0, y), pri.transform.rotation);
                        //GameObject.Destroy(sphere.GetComponent<SphereCollider>());
                        //sphere.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        blocked = true;
                        break;
                    }
                }

                if (blocked) continue;

                nodes.Add(nodePosition, new Node(x, y));
            }
        }
        mapComplete = true;
    }

    public Dictionary<Vector2, Node> getNodes()
    {
        return nodes;
    }

    public Node getNodeAt(Vector2 position)
    {
        var result = new Node();

        try
        {
            nodes.TryGetValue(position, out result);

            if (result == null)
            {
                var fixedPosition = new Vector2(Mathf.FloorToInt(position.x / WorldMapNodes.NODEDISTANCE) * WorldMapNodes.NODEDISTANCE,
                Mathf.FloorToInt(position.y / WorldMapNodes.NODEDISTANCE) * WorldMapNodes.NODEDISTANCE);

                result = nodes[fixedPosition];
            }

            return result;
        }
        catch
        {
            return null;
        }
    }

    public Node getAvailableNodeBetween(Node firstNode, Node secondNode)
    {
        var midpoint = firstNode.location + (secondNode.location - firstNode.location) / 2.0f;
        var midpointNode = getNodeAt(midpoint);

        if (midpointNode == null)
        {
            return null;
        }

        var direction = new Vector2(NODEDISTANCE, 0);
        var range = 1;
        while (midpointNode == null)
        {
            //search around
            midpointNode = getNodeAt(midpointNode.location + direction);

            direction = direction.x > 0 ? new Vector2(0, NODEDISTANCE * range) :
                direction.y > 0 ? new Vector2(NODEDISTANCE * range, 0) : new Vector2(NODEDISTANCE * range, NODEDISTANCE * range);
            range++;
        }

        return midpointNode;
    }

    public void UpdateNode(Vector2 nodeLocation, Node start, Node end)
    {
        nodes[nodeLocation].SetStartEndNodes(start.location, end.location);
    }

    public void DecreaseNodeCosts()
    {
        foreach (var node in nodes.Values)
        {
            node.RemoveFromCost();
        }
    }

    public bool isMapComplete()
    {
        return mapComplete;
    }
}
