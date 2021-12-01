using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    private Dictionary<Vector2, Node> worldMapNodesCopy;

    public PathFinder()
    {
        worldMapNodesCopy = WorldMapNodes.Instance.getNodes();
    }

    public Node[] FindPath(Node startNode, Node endNode)
    {
        worldMapNodesCopy = WorldMapNodes.Instance.getNodes();
        if (worldMapNodesCopy.Count <= 0) return null;

        var start = startNode;
        ////set the start to the closest node of the start node
        start.SetLocation(new Vector2(Mathf.FloorToInt(startNode.location.x / WorldMapNodes.NODEDISTANCE) * WorldMapNodes.NODEDISTANCE,
            Mathf.FloorToInt(startNode.location.y / WorldMapNodes.NODEDISTANCE) * WorldMapNodes.NODEDISTANCE));
        var end = endNode;
        ////same thing for the end node
        end.SetLocation(new Vector2(Mathf.FloorToInt(endNode.location.x / WorldMapNodes.NODEDISTANCE) * WorldMapNodes.NODEDISTANCE,
            Mathf.FloorToInt(endNode.location.y / WorldMapNodes.NODEDISTANCE) * WorldMapNodes.NODEDISTANCE));

        bool startDone = false;
        bool endDone = false;
        foreach (var location in worldMapNodesCopy.Keys)
        {
            if (!startDone && (location - startNode.location).magnitude <= WorldMapNodes.NODEDISTANCE)
            {
                start = worldMapNodesCopy[location];
                startDone = true;
            }
            if (!endDone && (location - endNode.location).magnitude <= WorldMapNodes.NODEDISTANCE)
            {
                end = worldMapNodesCopy[location];
                endDone = true;
            }
            if (startDone && endDone) break;
        }

        start.SetStartEndNodes(start.location, end.location);
        end.SetStartEndNodes(start.location, end.location);

        var OpenList = new List<Node>();
        var ClosedList = new List<Node>();
        var maxIterations = worldMapNodesCopy.Count - 1;
        OpenList.Add(start);
        OpenList[0].SetStartEndNodes(start.location, end.location);
        bool pathFound = false;

        while (!pathFound)
        {
            if (maxIterations <= 0 || OpenList.Count <= 0) break;
            maxIterations--;

            //get lowest h cost node
            var currentNode = new Node(0, 0);
            var lowesthCost = float.MaxValue;
            var index = -1;
            for (int i = 0; i < OpenList.Count; i++)
            {
                if (OpenList[i].h < lowesthCost)
                {
                    currentNode = OpenList[i];
                    lowesthCost = OpenList[i].h;
                    index = i;
                }
            }

            //checking, move it to closed list
            ClosedList.Add(currentNode);
            OpenList.RemoveAt(index);

            //reached end node
            if ((end.location - currentNode.location).magnitude < WorldMapNodes.NODEDISTANCE || currentNode.location == end.location)
            {
                pathFound = true;
                break;
            }

            //check neighbours in 8 directions
            Vector2[] directions = new Vector2[8] {
                new Vector2(WorldMapNodes.NODEDISTANCE,0),
                new Vector2(-WorldMapNodes.NODEDISTANCE,0),
                new Vector2(0,WorldMapNodes.NODEDISTANCE),
                new Vector2(0,-WorldMapNodes.NODEDISTANCE),
                new Vector2(WorldMapNodes.NODEDISTANCE,WorldMapNodes.NODEDISTANCE),
                new Vector2(WorldMapNodes.NODEDISTANCE,-WorldMapNodes.NODEDISTANCE),
                new Vector2(-WorldMapNodes.NODEDISTANCE,WorldMapNodes.NODEDISTANCE),
                new Vector2(-WorldMapNodes.NODEDISTANCE,-WorldMapNodes.NODEDISTANCE)
            };

            for (int i = 0; i < directions.Length; i++)
            {
                //get the location
                var neighborLocation = new Vector2(
                    currentNode.location.x + directions[i].x,
                    currentNode.location.y + directions[i].y);

                //update node costs
                try
                {
                    worldMapNodesCopy[neighborLocation].SetStartEndNodes(start.location, end.location);
                }
                catch
                {
                    continue;
                }

                //check if in closed list
                bool inClosedList = false;
                for (int z = 0; z < ClosedList.Count; z++)
                {
                    if (ClosedList[z].location == neighborLocation)
                    {
                        inClosedList = true;
                    }
                }

                if (inClosedList) continue;

                bool inOpenList = false;
                //check if node in open list
                for (int z = 0; z < OpenList.Count; z++)
                {
                    if (OpenList[z].location == neighborLocation)
                    {
                        //check g cost to see which one is closer
                        if (worldMapNodesCopy[neighborLocation].g < OpenList[z].g)
                        {
                            //updae parent
                            OpenList[z].SetParent(worldMapNodesCopy[neighborLocation]);
                        }
                        inOpenList = true;
                        break;
                    }
                }

                //if in open list, ignore it
                if (inOpenList) continue;

                //got here, add it to the open list, nodes to be checked
                OpenList.Add(worldMapNodesCopy[neighborLocation]);
                OpenList[OpenList.Count - 1].SetStartEndNodes(start.location, end.location);
                OpenList[OpenList.Count - 1].SetParent(currentNode);
            }
        }

        //did not reach the end node
        //if (!pathFound) return null;

        List<Node> solutionPath = new List<Node>();
        var parentNode = ClosedList[ClosedList.Count - 1];

        //reverse the path
        bool complete = false;
        maxIterations = ClosedList.Count;
        while (!complete)
        {
            if (maxIterations <= 0) break;
            maxIterations--;
            if (parentNode.parentNode == null) break;
            //parentNode.AddToMoveCost();
            parentNode = parentNode.parentNode;
            solutionPath.Add(parentNode);
            if (parentNode.location == start.location)
            {
                complete = true;
            }
        }

        solutionPath.Reverse();

        return solutionPath.ToArray();
    }
}
