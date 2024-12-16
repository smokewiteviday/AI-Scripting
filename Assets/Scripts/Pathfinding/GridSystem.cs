using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 1f;
    public GameObject cellPrefab;

    private Cell[,] grid;

    private void Start()
    {
        GenerateGrid();
    }

    // Generates the grid by instantiating cells.
    private void GenerateGrid()
    {
        grid = new Cell[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPosition = GetWorldPosition(x, y);
                GameObject cellObj = Instantiate(cellPrefab, worldPosition, Quaternion.identity);
                Cell cell = cellObj.GetComponent<Cell>();
                cell.Initialize(x, y, true);
                grid[x, y] = cell;
            }
        }
    }

    // Converts grid coordinates to world position.
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * cellSize, y * cellSize, 0);
    }

    // Retrieves the cell at a specific world position.
    public Cell GetCellFromWorldPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int y = Mathf.RoundToInt(worldPosition.y / cellSize);

        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
        {
            return grid[x, y];
        }
        return null;
    }

    // Returns the neighboring cells of a given cell.
    public List<Cell> GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue; // Skip the cell itself.

                int x = cell.GridX + dx;
                int y = cell.GridY + dy;

                if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
                {
                    neighbors.Add(grid[x, y]);
                }
            }
        }
        return neighbors;
    }
}
