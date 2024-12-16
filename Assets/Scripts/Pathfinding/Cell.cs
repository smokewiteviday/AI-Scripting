using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public bool IsWalkable { get; private set; } = true;
    public int GridX { get; private set; }
    public int GridY { get; private set; }

    public int GCost { get; set; }
    public int HCost { get; set; }
    public Cell Parent { get; set; }

    public int FCost => GCost + HCost;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int gridX, int gridY, bool isWalkable)
    {
        GridX = gridX;
        GridY = gridY;
        SetWalkable(isWalkable);
    }

    public void SetWalkable(bool isWalkable)
    {
        IsWalkable = isWalkable;
        spriteRenderer.color = isWalkable ? Color.white : Color.red;
    }
}

