using UnityEngine;
using UnityEngine.InputSystem;

public abstract class GridMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] protected float gridSize = 4f;
    [SerializeField] protected LayerMask wallLayer;

    protected Vector2Int currentGridPosition; // Starting grid position
    public Vector2Int CurrentGridPosition => currentGridPosition; // Public getter for current grid position

    protected bool isMoving = false;

    protected virtual void Awake()
    {
        UpdateCurrentGridPosition();                    // Set the initial grid position based on the starting transform
    }

    // Coroutine definitions for smooth movement to the target position
    protected System.Collections.IEnumerator MoveEntity(Vector3 targetPosition)
    {
        isMoving = true;

        AudioManager.Instance.PlayFootsteps();

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
        isMoving = false;
    }

    private void UpdateCurrentGridPosition()
    {
        currentGridPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)); // Update grid position based on new transform
    }

    // Triggers after every step taken (not rotation, just forward/backward movement)
    protected virtual void OnMovementComplete()
    {
        UpdateCurrentGridPosition(); // Update the current grid position after movement is complete
    }
}
