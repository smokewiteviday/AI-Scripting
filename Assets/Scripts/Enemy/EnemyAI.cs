using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    enum EnemyState { Idle, Run, Upgrade, Die }
    private EnemyState state = EnemyState.Idle;
    private bool stateComplete = true;

    [Header("Attributes")]
    public float baseMoveSpeed = 2f;
    public int baseHealth = 50;
    private float currentMoveSpeed;
    public int currentHealth;

    private List<Cell> path;
    private int currentPathIndex;
    private bool isMoving = false;

    public GridSystem gridSystem;

    private static float healthMultiplier = 1f;
    private static float speedMultiplier = 1f;

    private GameManager gameManager;

    private void Start()
    {
        currentMoveSpeed = baseMoveSpeed * speedMultiplier;
        currentHealth = Mathf.RoundToInt(baseHealth * healthMultiplier);

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }

        // Start in Idle state initially
        ChangeState(EnemyState.Idle);
    }

    private void Update()
    {
        if (stateComplete)
        {
            SelectState();
        }
        UpdateState();
    }

    public void Initialize(List<Cell> pathToFollow, GridSystem grid)
    {
        if (pathToFollow == null || pathToFollow.Count == 0)
        {
            Debug.LogError("No valid path provided to the enemy!");
            return;
        }

        path = pathToFollow;
        gridSystem = grid;
        currentPathIndex = 0;
        isMoving = true;

        // Color the path green, but skip start points and end point to preserve their colors
        foreach (Cell cell in path)
        {
            Vector2 cellPosition = new Vector2(cell.GridX, cell.GridY);
            if (!gridSystem.startPoints.Contains(cellPosition) && cellPosition != gridSystem.end)
            {
                cell.SetColor(Color.green);
            }
        }

        // Start in Run state when initialized
        ChangeState(EnemyState.Run);
    }

    private void SelectState()
    {
        // This can be expanded to make decisions based on certain conditions
        if (state == EnemyState.Idle)
        {
            ChangeState(EnemyState.Run);
        }
    }

    private void UpdateState()
    {
        switch (state)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;
            case EnemyState.Run:
                UpdateRun();
                break;
            case EnemyState.Upgrade:
                UpdateUpgrade();
                break;
            case EnemyState.Die:
                UpdateDie();
                break;
        }
    }

    private void ChangeState(EnemyState newState)
    {
        state = newState;
        stateComplete = false;

        switch (state)
        {
            case EnemyState.Idle:
                StartIdle();
                break;
            case EnemyState.Run:
                StartRun();
                break;
            case EnemyState.Upgrade:
                StartUpgrade();
                break;
            case EnemyState.Die:
                StartDie();
                break;
        }
    }

    // ===== STATE: IDLE =====
    private void StartIdle()
    {
        stateComplete = true;
    }

    private void UpdateIdle()
    {
    }

    // ===== STATE: RUN =====
    private void StartRun()
    {
        Debug.Log("Enemy started running.");
        isMoving = true;
    }

    private void UpdateRun()
    {
        if (isMoving)
        {
            MoveAlongPath();
        }
    }

    private void MoveAlongPath()
    {
        if (path == null || currentPathIndex >= path.Count)
        {
            OnReachEnd();
            return;
        }

        Vector2 targetPosition = gridSystem.GetWorldPosition(
            path[currentPathIndex].GridX,
            path[currentPathIndex].GridY
        );

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentMoveSpeed * Time.deltaTime);

        if ((Vector2)transform.position == targetPosition)
        {
            currentPathIndex++;
        }
    }

    private void OnReachEnd()
    {
        isMoving = false;
        if (gameManager != null)
        {
            gameManager.EnemyReachedEndpoint();
        }

        ChangeState(EnemyState.Die);
    }

    // ===== STATE: UPGRADE =====
    private void StartUpgrade()
    {
        stateComplete = true;
    }

    private void UpdateUpgrade()
    {
        // Add behavior for upgrading if needed
    }

    // ===== STATE: DIE =====
    private void StartDie()
    {
        Debug.Log("Enemy is dying.");
        Die();
    }

    private void UpdateDie()
    {
        // This state completes immediately after starting
        stateComplete = true;
    }

    // ===== UTILITIES =====
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            ChangeState(EnemyState.Die);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public static void IncreaseBoosts(float healthIncrease, float speedIncrease)
    {
        healthMultiplier += healthIncrease;
        speedMultiplier += speedIncrease;
        Debug.Log($"Boosts applied: HealthMultiplier={healthMultiplier}, SpeedMultiplier={speedMultiplier}");
    }
}