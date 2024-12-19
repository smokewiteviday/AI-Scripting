using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 1f;
    public GameObject cellPrefab;
    public GameObject turretPrefab;
    public int turretRange = 2; 
    private Cell[,] grid;
    public Vector2 start;
    public Vector2 end;
    public Pathfinding pathfinding;
    public GameObject enemyPrefab;

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
        StartCoroutine(PlaceTurretsOverTime());
        StartToEndPoint();
        StartCoroutine(SpawnEnemies());
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
    private IEnumerator PlaceTurretsOverTime()
    {
        // List to store all unwalkable cells and their coverage
        List<(Cell cell, int coveredCells)> unwalkableCells = new List<(Cell, int)>();

        // Loop through all cells in the grid
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Cell cell = grid[x, y];

                // Only consider unwalkable cells for turret placement
                if (!cell.IsWalkable)
                {
                    // Calculate how many walkable cells are within range of this cell
                    int coveredCells = CalculateCoveredWalkableCells(cell);

                    // Add the unwalkable cell and its coverage to the list
                    unwalkableCells.Add((cell, coveredCells));
                }
            }
        }

        // Sort the unwalkable cells by the number of covered walkable cells in descending order
        unwalkableCells.Sort((a, b) => b.coveredCells.CompareTo(a.coveredCells));

        int turretsPlaced = 0; // Counter for placed turrets

        while (turretsPlaced < 10) // Adjust the number of turrets as needed
        {
            if (turretsPlaced < unwalkableCells.Count)
            {
                Cell bestCell = unwalkableCells[turretsPlaced].cell;
                Vector3 turretPosition = GetWorldPosition(bestCell.GridX, bestCell.GridY);
              
                Instantiate(turretPrefab, turretPosition, Quaternion.identity);

                Debug.Log($"Turret {turretsPlaced + 1} placed at: {bestCell.GridX}, {bestCell.GridY}, covering {unwalkableCells[turretsPlaced].coveredCells} walkable cells.");

                turretsPlaced++;
            }
            else
            {
                Debug.LogWarning("No more suitable unwalkable cells for turret placement!");
                break; 
            }

            
            yield return new WaitForSeconds(5f);
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
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy(); 
            yield return new WaitForSeconds(2f);
        }
    }
    public void SpawnEnemy()
    {
        // Get the spawn position from the grid
        Vector2 spawnPosition = GetWorldPosition((int)start.x, (int)start.y);
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        var path = pathfinding.FindPath(start, end);
        if (path != null && path.Count > 0)
        {
            // Initialize the enemy with the path and the grid system
            EnemyAI enemyMovement = enemy.GetComponent<EnemyAI>();
            if (enemyMovement != null)
            {
                enemyMovement.Initialize(path, this); 
            }
            
        }
        else
        {
            Debug.LogError("No valid path found for the enemy to follow");
            Destroy(enemy);
        }
    }


}
