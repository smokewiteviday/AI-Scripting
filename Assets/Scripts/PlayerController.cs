using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;  // Speed at which the character moves
    private Rigidbody2D rb;       // Rigidbody2D component for physics-based movement

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float moveDirection = 0f;

        // Check for left and right key inputs
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            moveDirection = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            moveDirection = 1f;
        }

        // Apply movement to the Rigidbody2D
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }
}
