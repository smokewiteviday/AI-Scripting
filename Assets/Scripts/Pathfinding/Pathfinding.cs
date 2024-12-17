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

    // Finds the path from start to target.
    public List<Cell> FindPath(Vector2 startWorldPosition, Vector2 targetWorldPosition)
    {
        Cell startCell = gridSystem.GetCellFromWorldPosition(startWorldPosition);
        Cell targetCell = gridSystem.GetCellFromWorldPosition(targetWorldPosition);

        if (startCell == null || targetCell == null || !targetCell.IsWalkable) return null;

        List<Cell> openList = new List<Cell> { startCell };
        List<Cell> closedSet = new List<Cell>();

        while (openList.Count > 0)
        {
            Cell currentCell = openList[0];
            foreach (Cell cell in openList)
            {
                if (cell.FCost < currentCell.FCost || (cell.FCost == currentCell.FCost && cell.HCost < currentCell.HCost))
                {
                    currentCell = cell;
                }
            }

            openList.Remove(currentCell);
            closedSet.Add(currentCell);

            if (currentCell == targetCell)
            {
                return RetracePath(startCell, targetCell);
            }

            foreach (Cell neighbor in gridSystem.GetNeighbors(currentCell))
            {
                if (!neighbor.IsWalkable || closedSet.Contains(neighbor)) continue;

                int newCostToNeighbor = currentCell.GCost + GetDistance(currentCell, neighbor);

                if (newCostToNeighbor < neighbor.GCost || !openList.Contains(neighbor))
                {
                    neighbor.GCost = newCostToNeighbor;
                    neighbor.HCost = GetDistance(neighbor, targetCell);
                    neighbor.Parent = currentCell;

                    if (!openList.Contains(neighbor)) openList.Add(neighbor);
                }
            }
        }
        return null;
    }

    // Retraces the path by following parent links.
    private List<Cell> RetracePath(Cell startCell, Cell targetCell)
    {
        List<Cell> path = new List<Cell>();
        Cell currentCell = targetCell;

        while (currentCell != startCell)
        {
            path.Add(currentCell);
            currentCell = currentCell.Parent;
        }

        path.Reverse();
        return path;
    }

    // Calculates Manhattan distance.
    private int GetDistance(Cell a, Cell b)
    {
        int dx = Mathf.Abs(a.GridX - b.GridX);
        int dy = Mathf.Abs(a.GridY - b.GridY);
        return dx + dy;
    }
}
