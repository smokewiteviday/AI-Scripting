using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 1f;
    public GameObject cellPrefab;
    public GameObject turretPrefab; // Prefab for the turret to be placed on unwalkable cells.
    public int turretRange = 2; // The range of the turret in grid cells.
    public GameObject spawnerPrefab;
    private Cell[,] grid;
    public Vector2 start;
    public Vector2 end;
    public Pathfinding pathfinding;

    private int[,] testMap = new int[10, 10] // Test map: 1 = walkable, 0 = unwalkable
   {
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 1, 1, 1, 0, 0, 0 },
        { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0 },
        { 0, 0, 0, 0, 1, 0, 1, 0, 0, 0 },
        { 1, 1, 1, 1, 1, 0, 1, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
   };

    private void Start()
    {
        GenerateGrid();
        PlaceTurrets();
        StartToEndPoint();
        PlaceEnemySpawner();
    }

    // Generates the grid by instantiating cells.
    private void GenerateGrid()
    {
        grid = new Cell[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPosition = GetWorldPosition(x, y);
                GameObject cellObj = Instantiate(cellPrefab, worldPosition, Quaternion.identity);
                Cell cell = cellObj.GetComponent<Cell>();
                bool isWalkable = testMap[y, x] == 1;
                cell.Initialize(x, y, isWalkable);
                grid[x, y] = cell;
            }
        }
    }

    // Converts grid coordinates to world position.
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(x * cellSize, y * cellSize);
    }

    // Retrieves the cell at a specific world position.
    public Cell GetCellFromWorldPosition(Vector2 worldPosition)
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
    private void PlaceTurrets()
    {
        // List to store all unwalkable cells and their coverage.
        List<(Cell cell, int coveredCells)> unwalkableCells = new List<(Cell, int)>();

        // Loop through all cells in the grid.
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Cell cell = grid[x, y];

                // Only consider unwalkable cells for turret placement.
                if (!cell.IsWalkable)
                {
                    // Calculate how many walkable cells are within range of this cell.
                    int coveredCells = CalculateCoveredWalkableCells(cell);

                    // Add the unwalkable cell and its coverage to the list.
                    unwalkableCells.Add((cell, coveredCells));
                }
            }
        }

        // Sort the unwalkable cells by the number of covered walkable cells in descending order.
        unwalkableCells.Sort((a, b) => b.coveredCells.CompareTo(a.coveredCells));

        // Place up to three turrets on the best unwalkable cells.
        int turretsPlaced = 0;
        for (int i = 0; i < unwalkableCells.Count && turretsPlaced < 3; i++)
        {
            Cell bestCell = unwalkableCells[i].cell;

            // Get the world position of the cell.
            Vector3 turretPosition = GetWorldPosition(bestCell.GridX, bestCell.GridY);

            // Instantiate the turret prefab at the cell's position.
            Instantiate(turretPrefab, turretPosition, Quaternion.identity);

            // Log the placement for debugging.
            Debug.Log($"Turret {turretsPlaced + 1} placed at: {bestCell.GridX}, {bestCell.GridY}, covering {unwalkableCells[i].coveredCells} walkable cells.");

            turretsPlaced++;
        }

        // If no turrets could be placed, log a message.
        if (turretsPlaced == 0)
        {
            Debug.LogWarning("No suitable unwalkable cells found for turret placement!");
        }
    }

    // Calculates the number of walkable cells within the turret's range for a given center cell.
    private int CalculateCoveredWalkableCells(Cell centerCell)
    {
        int coveredCells = 0; // Counter for walkable cells within range.

        // Loop through all positions within the range.
        for (int dx = -turretRange; dx <= turretRange; dx++)
        {
            for (int dy = -turretRange; dy <= turretRange; dy++)
            {
                int x = centerCell.GridX + dx;
                int y = centerCell.GridY + dy;

                // Ensure the position is within grid bounds.
                if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
                {
                    // If the cell at this position is walkable, increment the counter.
                    if (grid[x, y].IsWalkable)
                    {
                        coveredCells++;
                    }
                }
            }
        }

        return coveredCells; // Return the total count of covered walkable cells.
    }
    // Places an enemy spawner at the starting walkable cell in the grid.
    private void PlaceEnemySpawner()
    {
        // Ensure the starting point is within the grid bounds.
        int startX = Mathf.RoundToInt(start.x);
        int startY = Mathf.RoundToInt(start.y);

        if (startX >= 0 && startX < gridWidth && startY >= 0 && startY < gridHeight)
        {
            Cell startCell = grid[startX, startY];

            // Check if the starting cell is walkable.
            if (startCell.IsWalkable)
            {
                // Get the world position of the starting cell.
                Vector2 spawnerPosition = GetWorldPosition(startCell.GridX, startCell.GridY);

                // Instantiate the spawner prefab at the cell's position.
                Instantiate(spawnerPrefab, spawnerPosition, Quaternion.identity);

                // Log the placement for debugging.
                Debug.Log($"Enemy spawner placed at the starting point: {startCell.GridX}, {startCell.GridY}");
            }
            else
            {
                Debug.LogError($"Starting cell at {startCell.GridX}, {startCell.GridY} is not walkable!");
            }
        }
        else
        {
            Debug.LogError("Starting point is out of grid bounds!");
        }
    }
    public void StartToEndPoint()
    {
        this.start = new Vector2(0, 5);
        this.end = new Vector2(9, 6);
        var path = pathfinding.FindPath(start, end);
        if (path != null)
        {
            foreach (var cell in path)
            {
                cell.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }
}
