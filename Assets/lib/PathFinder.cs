using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    private const float NODEDISTANCE = 1f;
    private List<Node> nodes;
    public PathFinder()
    {
        nodes = new List<Node>();
    }

    public Node[] FindPath(Node start, Node end)
    {
        var pathList = new List<Node>();
        var OpenList = new List<Node>();
        var ClosedList = new List<Node>();
        var maxIterations = 500;
        OpenList.Add(start);
        OpenList[0].SetStartEndNodes(start.location, end.location);
        bool pathFound = false;

        while (!pathFound)
        {
            if (maxIterations <= 0) break;
            maxIterations--;

            //get lowest cost node
            var currentNode = new Node(0, 0);
            var lowestfCost = float.MaxValue;
            for (int i = 0; i < OpenList.Count; i++)
            {
                if (OpenList[i].f < lowestfCost)
                {
                    currentNode = OpenList[i];
                    lowestfCost = OpenList[i].f;
                }
            }

            //checking, add it to closed list
            ClosedList.Add(currentNode);

            //reached end node
            if (currentNode.location == end.location)
            {
                pathList.Add(currentNode);
                pathFound = true;
            }

            //check neighbours
            Vector2[] directions = new Vector2[4] {
                new Vector2(NODEDISTANCE,0),
                new Vector2(-NODEDISTANCE,0),
                new Vector2(0,NODEDISTANCE),
                new Vector2(0,-NODEDISTANCE)
            };

            for (int i = 0; i < directions.Length; i++)
            {
                //create node in world
                var neighbourNode = new Node(
                    currentNode.location.x + directions[i].x,
                    currentNode.location.y + directions[i].y);
                //check if in closed list or if cant move there
                bool inClosedListOrBlocked = false;
                for (int z = 0; z < ClosedList.Count; z++)
                {
                    if (ClosedList[z].location == neighbourNode.location)
                    {
                        inClosedListOrBlocked = true;
                    }
                }
                //check if can move here
                var existingObjects = Physics.OverlapSphere((Vector3)neighbourNode.location, NODEDISTANCE / 2.1f);
                for (int z = 0; z < existingObjects.Length; z++)
                {
                    if (existingObjects[z].gameObject.GetComponent<Terrain>() ||
                        existingObjects[z].gameObject.GetComponent<Collider>())
                    {
                        inClosedListOrBlocked = true;
                        break;
                    }
                }

                if (inClosedListOrBlocked) continue;

                bool inOpenList = false;
                //check if node in open list
                for (int z = 0; z < OpenList.Count; z++)
                {
                    if (OpenList[z].location == neighbourNode.location)
                    {
                        //check g cost to see which one is closer
                        if (neighbourNode.g < OpenList[z].g)
                        {
                            OpenList[z].SetParent(neighbourNode);
                        }
                        inOpenList = true;
                        break;
                    }
                }

                if (inOpenList) continue;

                //got here, add it to the open list, nodes to be checked
                OpenList.Add(neighbourNode);
                OpenList[OpenList.Count - 1].SetStartEndNodes(start.location, end.location);
                OpenList[OpenList.Count - 1].SetParent(currentNode);
            }

            //add current node to path
            pathList.Add(currentNode);
        }


        List<Node> solutionPath = new List<Node>();
        var parentNode = pathList[pathList.Count - 1];

        //reverse the path
        bool complete = false;
        maxIterations = 500;
        while (!complete || maxIterations > 0)
        {
            maxIterations--;
            if (parentNode.parentNode == null) break;
            parentNode = parentNode.parentNode;
            solutionPath.Add(parentNode);
            if (parentNode.location == start.location)
            {
                complete = true;
            }
        }

        return solutionPath.ToArray();
    }
}
