using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Pathfinding pathfinding;

    private void Start()
    {
        Vector2 start = new Vector2(0, 5);
        Vector2 target = new Vector2(9, 6);

        var path = pathfinding.FindPath(start, target);

        if (path != null)
        {
            foreach (var cell in path)
            {
                cell.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }
}
