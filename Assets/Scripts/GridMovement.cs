using UnityEngine;
using UnityEngine.InputSystem;

public abstract class GridMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] protected float gridSize = 4f;
    [SerializeField] protected LayerMask wallLayer;
    public Vector2Int PreviousGridPosition { get; protected set; }
    [SerializeField] protected Vector2Int currentGridPosition; // SerializeField for debugging purposes, can be hidden in final version
    public Vector2Int CurrentGridPosition => currentGridPosition;

    public bool IsMoving { get; protected set;  }

    protected virtual void Awake()
    {
        UpdateCurrentGridPosition(new Vector2Int(
            Mathf.RoundToInt(transform.position.x / 4),
            Mathf.RoundToInt(transform.position.z / 4))); // Set the initial grid position based on the starting transform
    }

    // Coroutine definitions for smooth movement to the target position
    protected virtual System.Collections.IEnumerator MoveEntity(Vector3 targetPosition)
    {
        IsMoving = true;

        Vector3 startPosition = transform.position;

        float elapsedTime = 0f;
        float moveDuration = gridSize / moveSpeed;
        
        while (elapsedTime < moveDuration)
        {
            float linearProgress = elapsedTime / moveDuration; // Calculate linear progress (0 to 1)
            float easedProgress = Mathf.SmoothStep(0f, 1f, linearProgress);  // Reshape it into an S-Curve for acceleration & deceleration
            transform.position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.position = targetPosition; // Snap perfectly to grid anchor
        IsMoving = false;
    }

    private void UpdateCurrentGridPosition(Vector2Int pos)
    {
        currentGridPosition = pos;
    }

    // Triggers after every step taken (not rotation, just forward/backward movement)
    protected virtual void OnMovementComplete(Vector2Int pos)
    {
        UpdateCurrentGridPosition(pos); // Update the current grid position after movement is complete
    }
}
