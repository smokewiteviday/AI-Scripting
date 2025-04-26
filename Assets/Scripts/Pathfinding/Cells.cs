using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public bool IsWalkable { get; private set; } = true;
    public int GridX { get; private set; }
    public int GridY { get; private set; }

    public int GCost { get; set; } // Cost from start cell.
    public int HCost { get; set; } // Heuristic cost to target cell.
    public Cell Parent { get; set; } // Cell to trace the path back.

    public int FCost => GCost + HCost; // Total cost.

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Initializes the cell with grid coordinates and walkability.
    public void Initialize(int gridX, int gridY, bool isWalkable)
    {
        GridX = gridX;
        GridY = gridY;
        SetWalkable(isWalkable);
    }

    // Sets walkability and updates the visual appearance.
    public void SetWalkable(bool isWalkable)
    {
        IsWalkable = isWalkable;
        spriteRenderer.color = isWalkable ? Color.white : Color.red;
    }

    // Sets a custom color for the cell (e.g., for the path)
    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
}