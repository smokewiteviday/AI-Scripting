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
    public List<Vector2> startPoints; // Made public to allow EnemyAI to access
    public Vector2 end; // Made public to allow EnemyAI to access
    public Pathfinding pathfinding;
    public GameObject enemyPrefab;

    private int[,] randomMap;
    private float walkableProbability = 0.6f; // Adjust this to control walkable tile percentage
    private HashSet<Vector2Int> turretPositions; // Track positions where turrets are placed
    private int turretsPlacedCount; // Count the number of turrets placed
    private int endEdgeChoice; // Track which edge the end point is on

    // Enemy spawning variables
    private int enemiesPerSpawn = 1; // Number of enemies to spawn per start point
    private float timePerEnemyIncrease = 30f; // Increase every 30 seconds
    private GameManager gameManager; // Reference to GameManager to access gameTime

    private void Start()
    {
        turretPositions = new HashSet<Vector2Int>(); // Initialize the set to track turret positions
        startPoints = new List<Vector2>(); // Initialize the list of start points
        turretsPlacedCount = 0; // Initialize turret count

        // Find the GameManager in the scene
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }

        do
        {
            GenerateRandomMap(); // Generate a new random map
            GenerateGrid();      // Create the tilemap based on the generated map
            // Add the first start point at the beginning
            AddNewStartPoint();
        }
        while (!ValidatePaths()); // Ensure valid paths exist from all start points to end

        StartCoroutine(PlaceTurretsOverTime());
        StartCoroutine(SpawnEnemies());
    }

    // Generates a randomized walkable/unwalkable map.
    private void GenerateRandomMap()
    {
        randomMap = new int[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                randomMap[x, y] = (Random.value < walkableProbability) ? 1 : 0; // Randomly decide if the tile is walkable
            }
        }

        // Randomly select the single end point on an outer edge
        endEdgeChoice = Random.Range(0, 4); // 0: left, 1: right, 2: top, 3: bottom
        if (endEdgeChoice == 0) // Left edge (x = 0)
        {
            int endY = Random.Range(0, gridHeight);
            end = new Vector2(0, endY);
        }
        else if (endEdgeChoice == 1) // Right edge (x = gridWidth - 1)
        {
            int endY = Random.Range(0, gridHeight);
            end = new Vector2(gridWidth - 1, endY);
        }
        else if (endEdgeChoice == 2) // Top edge (y = gridHeight - 1)
        {
            int endX = Random.Range(0, gridWidth);
            end = new Vector2(endX, gridHeight - 1);
        }
        else // Bottom edge (y = 0)
        {
            int endX = Random.Range(0, gridWidth);
            end = new Vector2(endX, 0);
        }

        // Ensure the end point is walkable
        randomMap[(int)end.x, (int)end.y] = 1;

        // Ensure all start points are walkable
        foreach (var start in startPoints)
        {
            randomMap[(int)start.x, (int)start.y] = 1;
        }
    }

    // Generates the grid using the randomized map.
    private void GenerateGrid()
    {
        // Destroy old grid cells if they exist
        if (grid != null)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[x, y] != null)
                    {
                        Destroy(grid[x, y].gameObject);
                    }
                }
            }
        }

        grid = new Cell[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPosition = GetWorldPosition(x, y);
                GameObject cellObj = Instantiate(cellPrefab, worldPosition, Quaternion.identity);
                Cell cell = cellObj.GetComponent<Cell>();
                bool isWalkable = randomMap[x, y] == 1;
                cell.Initialize(x, y, isWalkable);
                grid[x, y] = cell;
            }
        }

        // Reapply yellow color to all start points after regenerating the grid
        foreach (var start in startPoints)
        {
            Cell startCell = grid[(int)start.x, (int)start.y];
            if (startCell != null)
            {
                startCell.SetColor(Color.yellow);
            }
        }

        // Color the end point cell red
        Cell endCell = grid[(int)end.x, (int)end.y];
        if (endCell != null)
        {
            endCell.SetColor(Color.red);
        }
    }

    // Ensures there are walkable paths from all start points to the end.
    private bool ValidatePaths()
    {
        foreach (var start in startPoints)
        {
            var path = pathfinding.FindPath(start, end);
            if (path == null)
            {
                return false;
            }
        }
        return true;
    }

    // Adds a new start point on the edge furthest from the end point
    private void AddNewStartPoint()
    {
        Vector2 newStart;
        bool validStart = false;
        int attempts = 0;
        const int maxAttempts = 50; // Prevent infinite loops

        do
        {
            // Choose the edge opposite to the end point to maximize distance
            int startEdgeChoice;
            if (endEdgeChoice == 0) // End is on left edge, prefer right edge
            {
                startEdgeChoice = 1; // Right edge
            }
            else if (endEdgeChoice == 1) // End is on right edge, prefer left edge
            {
                startEdgeChoice = 0; // Left edge
            }
            else if (endEdgeChoice == 2) // End is on top edge, prefer bottom edge
            {
                startEdgeChoice = 3; // Bottom edge
            }
            else // End is on bottom edge, prefer top edge
            {
                startEdgeChoice = 2; // Top edge
            }

            // If the preferred edge doesn't work after a few attempts, try other edges
            if (attempts > 10)
            {
                startEdgeChoice = Random.Range(0, 4); // Fallback to random edge
            }

            if (startEdgeChoice == 0) // Left edge (x = 0)
            {
                int startY = Random.Range(0, gridHeight);
                newStart = new Vector2(0, startY);
            }
            else if (startEdgeChoice == 1) // Right edge (x = gridWidth - 1)
            {
                int startY = Random.Range(0, gridHeight);
                newStart = new Vector2(gridWidth - 1, startY);
            }
            else if (startEdgeChoice == 2) // Top edge (y = gridHeight - 1)
            {
                int startX = Random.Range(0, gridWidth);
                newStart = new Vector2(startX, gridHeight - 1);
            }
            else // Bottom edge (y = 0)
            {
                int startX = Random.Range(0, gridWidth);
                newStart = new Vector2(startX, 0);
            }

            // Ensure the new start point is not the same as the end point
            if (newStart != end && !startPoints.Contains(newStart))
            {
                // Temporarily set the new start point as walkable to test the path
                randomMap[(int)newStart.x, (int)newStart.y] = 1;
                GenerateGrid(); // Regenerate grid to apply the change
                var path = pathfinding.FindPath(newStart, end);
                if (path != null)
                {
                    validStart = true;
                    startPoints.Add(newStart);

                    // Color the start point cell yellow (already handled in GenerateGrid)
                    Debug.Log($"New start point added at: {newStart}, opposite to end at: {end}");
                }
                else
                {
                    // If no path exists, reset the cell and try again
                    randomMap[(int)newStart.x, (int)newStart.y] = (Random.value < walkableProbability) ? 1 : 0;
                }
            }

            attempts++;
        } while (!validStart && attempts < maxAttempts);

        if (!validStart)
        {
            Debug.LogWarning("Could not find a valid new start point after maximum attempts!");
        }
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(x * cellSize, y * cellSize);
    }

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

    public List<Cell> GetNeighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

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
        List<(Cell cell, int coveredCells)> unwalkableCells = new List<(Cell, int)>();

        // Collect all unwalkable cells
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Cell cell = grid[x, y];
                if (!cell.IsWalkable)
                {
                    int coveredCells = CalculateCoveredWalkableCells(cell);
                    unwalkableCells.Add((cell, coveredCells));
                }
            }
        }

        unwalkableCells.Sort((a, b) => b.coveredCells.CompareTo(a.coveredCells));

        while (unwalkableCells.Count > 0)
        {
            Cell bestCell = unwalkableCells[0].cell;
            Vector2Int cellPos = new Vector2Int(bestCell.GridX, bestCell.GridY);

            // Place turret only if no turret exists at this position
            if (!turretPositions.Contains(cellPos))
            {
                Vector3 turretPosition = GetWorldPosition(bestCell.GridX, bestCell.GridY);
                Instantiate(turretPrefab, turretPosition, Quaternion.identity);
                turretPositions.Add(cellPos); // Mark this position as occupied
                turretsPlacedCount++; // Increment turret count

                Debug.Log($"Turret placed at: {bestCell.GridX}, {bestCell.GridY}, covering {unwalkableCells[0].coveredCells} walkable cells. Total turrets: {turretsPlacedCount}");

                // Add a new start point every 3 turrets
                if (turretsPlacedCount % 3 == 0)
                {
                    AddNewStartPoint();
                }
            }

            // Remove the cell from the list after attempting to place a turret
            unwalkableCells.RemoveAt(0);

            // Stop if there are no more unwalkable cells without turrets
            if (!HasUnoccupiedUnwalkableCells(unwalkableCells))
            {
                Debug.Log("No more unwalkable cells without turrets to place turrets on!");
                break;
            }

            yield return new WaitForSeconds(5f);
        }
    }

    // Check if there are any unwalkable cells without turrets
    private bool HasUnoccupiedUnwalkableCells(List<(Cell cell, int coveredCells)> unwalkableCells)
    {
        foreach (var (cell, _) in unwalkableCells)
        {
            Vector2Int cellPos = new Vector2Int(cell.GridX, cell.GridY);
            if (!turretPositions.Contains(cellPos))
            {
                return true; // Found an unwalkable cell without a turret
            }
        }
        return false;
    }

    private int CalculateCoveredWalkableCells(Cell centerCell)
    {
        int coveredCells = 0;
        for (int dx = -turretRange; dx <= turretRange; dx++)
        {
            for (int dy = -turretRange; dy <= turretRange; dy++)
            {
                int x = centerCell.GridX + dx;
                int y = centerCell.GridY + dy;

                if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
                {
                    if (grid[x, y].IsWalkable)
                    {
                        coveredCells++;
                    }
                }
            }
        }
        return coveredCells;
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (startPoints.Count == 0)
            {
                yield return new WaitForSeconds(2f);
                continue;
            }

            // Calculate enemiesPerSpawn based on game time
            if (gameManager != null)
            {
                float currentTime = gameManager.GetGameTime();
                enemiesPerSpawn = 1 + Mathf.FloorToInt(currentTime / timePerEnemyIncrease);
                Debug.Log($"Current time: {currentTime}, Enemies per spawn: {enemiesPerSpawn}");
            }

            // Create a copy of startPoints to avoid modification issues during iteration
            List<Vector2> startPointsCopy = new List<Vector2>(startPoints);

            // Spawn enemies from all start points simultaneously
            foreach (var start in startPointsCopy)
            {
                // Spawn the calculated number of enemies from each start point
                for (int i = 0; i < enemiesPerSpawn; i++)
                {
                    SpawnEnemy(start, end);
                    // Small delay between spawning multiple enemies from the same start point to avoid overlap
                    yield return new WaitForSeconds(0.1f);
                }
            }

            // Wait before spawning the next wave
            yield return new WaitForSeconds(2f);
        }
    }

    public void SpawnEnemy(Vector2 start, Vector2 end)
    {
        Vector2 spawnPosition = GetWorldPosition((int)start.x, (int)start.y);
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        var path = pathfinding.FindPath(start, end);
        if (path != null && path.Count > 0)
        {
            EnemyAI enemyMovement = enemy.GetComponent<EnemyAI>();
            if (enemyMovement != null)
            {
                enemyMovement.Initialize(path, this);
            }
        }
        else
        {
            Debug.LogError($"No valid path found for the enemy to follow from {start} to {end}");
            Destroy(enemy);
        }
    }
}