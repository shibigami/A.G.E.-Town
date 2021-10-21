using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    private WorldMapNodes worldMapNodes;

    public PathFinder()
    {
        worldMapNodes = WorldMapNodes.Instance;
    }

    public Node[] FindPath(Node startNode, Node endNode)
    {
        if (worldMapNodes.getNodes().Count <= 0) return null;

        var start = startNode;
        start.SetLocation(new Vector2(Mathf.FloorToInt(start.location.x), Mathf.FloorToInt(start.location.y)));
        var end = endNode;
        end.SetLocation(new Vector2(Mathf.FloorToInt(end.location.x), Mathf.FloorToInt(end.location.y)));
        start.SetStartEndNodes(start.location, end.location, WorldMapNodes.DEFAULTMOVECOST);
        end.SetStartEndNodes(start.location, end.location, WorldMapNodes.DEFAULTMOVECOST);

        var OpenList = new List<Node>();
        var ClosedList = new List<Node>();
        var maxIterations = worldMapNodes.getNodes().Count - 1;
        OpenList.Add(start);
        OpenList[0].SetStartEndNodes(start.location, end.location, WorldMapNodes.DEFAULTMOVECOST);
        bool pathFound = false;

        while (!pathFound)
        {
            if (maxIterations <= 0 || OpenList.Count <= 0) break;
            maxIterations--;

            //get lowest cost node
            var currentNode = new Node(0, 0);
            var lowestfCost = float.MaxValue;
            var index = -1;
            for (int i = 0; i < OpenList.Count; i++)
            {
                if (OpenList[i].f < lowestfCost)
                {
                    currentNode = OpenList[i];
                    lowestfCost = OpenList[i].f;
                    index = i;
                }
            }

            //checking, add it to closed list
            ClosedList.Add(currentNode);
            OpenList.RemoveAt(index);

            //reached end node
            if (currentNode.location == end.location)
            {
                pathFound = true;
            }

            //check neighbours
            Vector2[] directions = new Vector2[4] {
                new Vector2(WorldMapNodes.NODEDISTANCE,0),
                new Vector2(-WorldMapNodes.NODEDISTANCE,0),
                new Vector2(0,WorldMapNodes.NODEDISTANCE),
                new Vector2(0,-WorldMapNodes.NODEDISTANCE)
            };

            for (int i = 0; i < directions.Length; i++)
            {
                //get the location
                var neighborLocation = new Vector2(
                    currentNode.location.x + directions[i].x,
                    currentNode.location.y + directions[i].y);

                if (worldMapNodes.getNodeAt(neighborLocation) == null) continue;

                //update node costs
                try
                {
                    worldMapNodes.UpdateNode(neighborLocation, start, end, WorldMapNodes.DEFAULTMOVECOST);
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
                        if (worldMapNodes.getNodes()[neighborLocation].g < OpenList[z].g)
                        {
                            OpenList[z].SetParent(worldMapNodes.getNodes()[neighborLocation]);
                        }
                        inOpenList = true;
                        break;
                    }
                }

                //if in open list, ignore it
                if (inOpenList) continue;

                //got here, add it to the open list, nodes to be checked
                OpenList.Add(worldMapNodes.getNodes()[neighborLocation]);
                OpenList[OpenList.Count - 1].SetStartEndNodes(start.location, end.location, WorldMapNodes.DEFAULTMOVECOST);
                OpenList[OpenList.Count - 1].SetParent(currentNode);
            }
        }

        //did not reach the end node
        if (end.location != ClosedList[ClosedList.Count - 1].location) return null;

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
