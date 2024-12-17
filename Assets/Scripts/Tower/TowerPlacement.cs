using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    public GameObject turretPrefab; // The turret prefab to be placed on the grid.
    public float turretRange = 3f; // The range of the turret in grid units (cells).

    private GridSystem gridSystem; // Reference to the GridSystem script.

    private void Start()
    {
        // Find the GridSystem component in the scene.
        gridSystem = FindObjectOfType<GridSystem>();
        // Place the turret automatically when the game starts.
        PlaceTurret();
    }

    private void PlaceTurret()
    {
        // Check if necessary components are assigned.
        if (gridSystem == null || turretPrefab == null)
        {
            Debug.LogError("GridSystem or TurretPrefab is not assigned!");
            return;
        }

        Cell bestCell = null; // To store the cell with the best coverage.
        int maxWalkableCellsInRange = 0; // Tracks the maximum walkable cells a turret can cover.

        // Loop through all unwalkable cells in the grid.
        foreach (Cell cell in GetUnwalkableCells())
        {
            // Count the number of walkable cells within this cell's range.
            int walkableCellsInRange = GetWalkableCellsInRange(cell).Count;

            // If this cell covers more walkable cells, update the bestCell.
            if (walkableCellsInRange > maxWalkableCellsInRange)
            {
                maxWalkableCellsInRange = walkableCellsInRange;
                bestCell = cell;
            }
        }

        // If a suitable cell is found, place the turret at that location.
        if (bestCell != null)
        {
            // Convert the grid coordinates of the best cell to world position.
            Vector2 turretPosition = gridSystem.GetWorldPosition(bestCell.GridX, bestCell.GridY);
            // Instantiate the turret prefab at the calculated position.
            Instantiate(turretPrefab, turretPosition, Quaternion.identity);
            Debug.Log($"Turret placed at ({bestCell.GridX}, {bestCell.GridY}) covering {maxWalkableCellsInRange} walkable cells.");
        }
        else
        {
            // If no suitable cell is found, log a warning.
            Debug.LogWarning("No suitable unwalkable cell found for turret placement.");
        }
    }

    // Retrieves all unwalkable cells from the grid.
    private List<Cell> GetUnwalkableCells()
    {
        List<Cell> unwalkableCells = new List<Cell>();

        // Loop through all grid cells.
        for (int x = 0; x < gridSystem.gridWidth; x++)
        {
            for (int y = 0; y < gridSystem.gridHeight; y++)
            {
                // Get the cell at the current coordinates.
                Cell cell = gridSystem.GetCellFromWorldPosition(gridSystem.GetWorldPosition(x, y));
                // If the cell exists and is unwalkable, add it to the list.
                if (cell != null && !cell.IsWalkable)
                {
                    unwalkableCells.Add(cell);
                }
            }
        }
        return unwalkableCells; // Return the list of unwalkable cells.
    }

    // Retrieves all walkable cells within a turret's range of a given cell.
    private List<Cell> GetWalkableCellsInRange(Cell centerCell)
    {
        List<Cell> walkableCells = new List<Cell>();
        int rangeInCells = Mathf.CeilToInt(turretRange); // Convert turret range to an integer.

        // Loop through a square of cells around the center cell within the turret's range.
        for (int dx = -rangeInCells; dx <= rangeInCells; dx++)
        {
            for (int dy = -rangeInCells; dy <= rangeInCells; dy++)
            {
                // Calculate the coordinates of the neighboring cell.
                int x = centerCell.GridX + dx;
                int y = centerCell.GridY + dy;

                // Check if the coordinates are within grid bounds.
                if (x >= 0 && x < gridSystem.gridWidth && y >= 0 && y < gridSystem.gridHeight)
                {
                    // Get the cell at the current coordinates.
                    Cell cell = gridSystem.GetCellFromWorldPosition(gridSystem.GetWorldPosition(x, y));
                    // If the cell exists and is walkable, and within range, add it to the list.
                    if (cell != null && cell.IsWalkable)
                    {
                        float distance = Vector2.Distance(
                            gridSystem.GetWorldPosition(centerCell.GridX, centerCell.GridY),
                            gridSystem.GetWorldPosition(cell.GridX, cell.GridY)
                        );

                        if (distance <= turretRange * gridSystem.cellSize)
                        {
                            walkableCells.Add(cell);
                        }
                    }
                }
            }
        }

        return walkableCells; // Return the list of walkable cells within range.
    }
}
