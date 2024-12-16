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

    public List<Cell> FindPath(Vector3 startWorldPosition, Vector3 targetWorldPosition)
    {
        Cell[,] grid = gridSystem.GetGrid();
        Cell startCell = gridSystem.GetCellFromWorldPosition(startWorldPosition);
        Cell targetCell = gridSystem.GetCellFromWorldPosition(targetWorldPosition);

        if (startCell == null || targetCell == null || !targetCell.IsWalkable) return null;

        List<Cell> openSet = new List<Cell>();
        HashSet<Cell> closedSet = new HashSet<Cell>();
        openSet.Add(startCell);

        while (openSet.Count > 0)
        {
            Cell currentCell = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentCell.FCost || (openSet[i].FCost == currentCell.FCost && openSet[i].HCost < currentCell.HCost))
                {
                    currentCell = openSet[i];
                }
            }

            openSet.Remove(currentCell);
            closedSet.Add(currentCell);

            if (currentCell == targetCell)
            {
                return RetracePath(startCell, targetCell);
            }

            foreach (Cell neighbor in gridSystem.GetNeighbors(currentCell))
            {
                if (!neighbor.IsWalkable || closedSet.Contains(neighbor)) continue;

                int newCostToNeighbor = currentCell.GCost + GetDistance(currentCell, neighbor);
                if (newCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    neighbor.GCost = newCostToNeighbor;
                    neighbor.HCost = GetDistance(neighbor, targetCell);
                    neighbor.Parent = currentCell;

                    if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }
        return null;
    }

    List<Cell> RetracePath(Cell startCell, Cell endCell)
    {
        List<Cell> path = new List<Cell>();
        Cell currentCell = endCell;

        while (currentCell != startCell)
        {
            path.Add(currentCell);
            currentCell = currentCell.Parent;
        }
        path.Reverse();
        return path;
    }

    int GetDistance(Cell a, Cell b)
    {
        int dstX = Mathf.Abs(a.GridX - b.GridX);
        int dstY = Mathf.Abs(a.GridY - b.GridY);

        return dstX + dstY;
    }
}
