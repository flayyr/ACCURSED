using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private Grid<PathNode> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;

    public PathNode cameFromNode;

    public PathNode(Grid<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public bool CheckWalkability(LayerMask obstacleLayers, bool squareDetection)
    {
        if (squareDetection)
            isWalkable = !Physics2D.OverlapBox(grid.GetWorldPosition(x, y) + (grid.cellSize * 0.5f * Vector3.one), new Vector2(grid.cellSize, grid.cellSize), 0, obstacleLayers);
        else
            isWalkable = !Physics2D.OverlapPoint(grid.GetWorldPosition(x, y) + (grid.cellSize * 0.5f * Vector3.one),obstacleLayers);
        return isWalkable;
    }

    public  void ChangeWalkable(bool change)
    {
        isWalkable = change;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}
