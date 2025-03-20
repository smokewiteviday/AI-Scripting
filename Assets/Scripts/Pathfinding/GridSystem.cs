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

    private int[,] randomMap;
    private float walkableProbability = 0.6f; // Adjust this to control walkable tile percentage

    private void Start()
    {
        do
        {
            GenerateRandomMap(); // Generate a new random map
            GenerateGrid();      // Create the tilemap based on the generated map
        }
        while (!ValidatePath()); // Ensure a valid path exists from start to end

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

        // Ensure start and end points are walkable
        start = new Vector2(0, 5);
        end = new Vector2(gridWidth - 1, gridHeight / 2);
        randomMap[(int)start.x, (int)start.y] = 1;
        randomMap[(int)end.x, (int)end.y] = 1;
    }

    // Generates the grid using the randomized map.
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
                bool isWalkable = randomMap[x, y] == 1;
                cell.Initialize(x, y, isWalkable);
                grid[x, y] = cell;
            }
        }
    }

    // Ensures there's a walkable path from start to end.
    private bool ValidatePath()
    {
        var path = pathfinding.FindPath(start, end);
        return path != null;
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
        int turretsPlaced = 0;

        while (turretsPlaced < 10)
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
            SpawnEnemy();
            yield return new WaitForSeconds(2f);
        }
    }

    public void SpawnEnemy()
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
            Debug.LogError("No valid path found for the enemy to follow");
            Destroy(enemy);
        }
    }
}
