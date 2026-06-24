using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    
    public bool MOVE_DIAGONAL = true;
    public bool SQUARE_WALKABLITY_DETECTION = true;

    public LayerMask obstacleLayers;

    public static Pathfinding Instance { get; private set; }

    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    public Pathfinding(int width, int height, float cellsize, Vector3 origin, bool moveDiagonal) 
    {
        MOVE_DIAGONAL = moveDiagonal;
        Instance = this;
        grid = new Grid<PathNode>(width, height, cellsize, origin, (Grid<PathNode> g,int x,int y) => new PathNode(g,x,y));
    }

    public Grid<PathNode> GetGrid()
    {
        return grid;
    }

    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        grid.GetXY(startWorldPos, out int startX, out int startY);
        grid.GetXY(endWorldPos, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if(path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach(PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.cellSize + Vector3.one * grid.cellSize * 0.5f + grid.originPosition);
            }
            return vectorPath;
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        //Debug.Log("STARTX: " + startX);
        //Debug.Log("STARTY: " + startY);
        //Debug.Log("STARTNODE: " + startNode);

        //Debug.Log("ENDX: " + endX);
        //Debug.Log("ENDY: " + endY);
        //Debug.Log("ENDNODE: " + endNode);

        if(!endNode.CheckWalkability(obstacleLayers, SQUARE_WALKABLITY_DETECTION))
            return null;

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        { 
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.CheckWalkability(obstacleLayers,SQUARE_WALKABLITY_DETECTION);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode,endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if(currentNode == endNode)
            {
                //Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neightbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neightbourNode)) continue;

                if (!neightbourNode.isWalkable)
                {
                    closedList.Add(neightbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neightbourNode);
                if(tentativeGCost < neightbourNode.gCost)
                {
                    neightbourNode.cameFromNode = currentNode;
                    neightbourNode.gCost = tentativeGCost;
                    neightbourNode.hCost = CalculateDistanceCost(neightbourNode, endNode);
                    neightbourNode.CalculateFCost();

                    if(!openList.Contains(neightbourNode))
                    {
                        openList.Add(neightbourNode);
                    }
                }
            }
        }

        //Out of Nodes on the openList
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode > neighbourList = new List<PathNode>();

        if(currentNode.x - 1 >= 0)
        {
            //Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            if (MOVE_DIAGONAL)
            {
                //Left Down
                if (currentNode.y - 1 >= 0) 
                    neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
                //Left Up
                if (currentNode.y + 1 < grid.GetHeight()) 
                    neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
            }
        }
        if(currentNode.x + 1 < grid.GetWidth())
        {
            //Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            if (MOVE_DIAGONAL)
            {
                //Right Down
                if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
                //Right Up
                if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
            }
        }
        //Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        //Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    private PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode> ();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null) { 
            path.Add (currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(PathNode a,PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for(int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode; 
    }
}
