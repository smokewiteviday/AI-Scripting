using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Pathfinding pathfinding;

    private void Start()
    {
        Vector3 start = new Vector3(0, 0, 0);
        Vector3 target = new Vector3(5, 5, 0);

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
