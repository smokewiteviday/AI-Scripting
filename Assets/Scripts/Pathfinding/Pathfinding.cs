using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private GridSystem gridSystem;

    private void Start()
    {
        gridSystem = FindObjectOfType<GridSystem>();
    }

    // Finds the path from start to target
    public List<Cell> FindPath(Vector2 startWorldPosition, Vector2 targetWorldPosition)
    {
        // Convert world positions to grid cells
        Cell startCell = gridSystem.GetCellFromWorldPosition(startWorldPosition);
        Cell targetCell = gridSystem.GetCellFromWorldPosition(targetWorldPosition);

        //return null if the start or target cell is null or unwalkable
        if (startCell == null || targetCell == null || !targetCell.IsWalkable) return null;

        //cells that need to be checked
        List<Cell> openList = new List<Cell> { startCell };
        //cells already processed.
        List<Cell> closedSet = new List<Cell>();

        // Continue searching for a path while there are cells to check
        while (openList.Count > 0)
        {
            Cell currentCell = openList[0];
            //find the cell in the list with lowest F cost
            foreach (Cell cell in openList)
            {
                if (cell.FCost < currentCell.FCost || (cell.FCost == currentCell.FCost && cell.HCost < currentCell.HCost))
                {
                    currentCell = cell;
                }
            }
            //move the current cell to the closeSet
            openList.Remove(currentCell);
            closedSet.Add(currentCell);

            //if the current cell is the target, retrace the path
            if (currentCell == targetCell)
            {
                return RetracePath(startCell, targetCell);
            }
            // Check all neighboring cells of the current cell
            foreach (Cell neighbor in gridSystem.GetNeighbors(currentCell))
            {
                // skip if the neighbor is unwalkable or already processed
                if (!neighbor.IsWalkable || closedSet.Contains(neighbor)) continue;
                
                int newCostToNeighbor = currentCell.GCost + GetDistance(currentCell, neighbor);
                //update if the new cost is lower or the neighbor is not in the list
                if (newCostToNeighbor < neighbor.GCost || !openList.Contains(neighbor))
                {
                    // Update the neighbor's costs and set its parent to the current cell
                    neighbor.GCost = newCostToNeighbor;
                    neighbor.HCost = GetDistance(neighbor, targetCell);
                    neighbor.Parent = currentCell;

                    // Add the neighbor to the open list if it's not already there
                    if (!openList.Contains(neighbor)) openList.Add(neighbor);
                }
            }
        }
        return null;
    }

    // Retraces the path from target back to start
    private List<Cell> RetracePath(Cell startCell, Cell targetCell)
    {
        List<Cell> path = new List<Cell>();
        Cell currentCell = targetCell;

        while (currentCell != startCell)
        {
            path.Add(currentCell);
            currentCell = currentCell.Parent;
        }
        //Reverse the path to get it from start to target
        path.Reverse();
        return path;
    }

    // Calculates Manhattan distance
    private int GetDistance(Cell a, Cell b)
    {
        int dx = Mathf.Abs(a.GridX - b.GridX);
        int dy = Mathf.Abs(a.GridY - b.GridY);
        return dx + dy;
    }
}
