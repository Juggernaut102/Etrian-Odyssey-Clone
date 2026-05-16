using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] private float gridSize = 2f;

    private InputSystem_Actions controls;
    private bool isMoving = false;

    private void Awake()
    {
        controls = new InputSystem_Actions();           // Initialize the input action asset
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Move.performed += OnMoveInput;   // Subscribe custom method to the event signature
    }

    private void OnDisable()
    {
        controls.Player.Disable();
        controls.Player.Move.performed -= OnMoveInput;   // Unsubscribe to prevent memory leaks
    }

    // Beginning of custom method that handles movement input
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        if (isMoving) return; // Prevent starting a new move while already moving

        Vector2 input = context.ReadValue<Vector2>(); 

        if (input == null) return;

        if (Mathf.Abs(input.y) > 0.5f)
        {
            Vector3 direction = transform.forward * Mathf.Sign(input.y) * gridSize; // Move forward/backward based on input
            StartCoroutine(MovePlayer(direction));  // Coroutines allow for smooth movement over time without blocking the main thread
        }
        else if (Mathf.Abs(input.x) > 0.5f)
        {
            float angle = Mathf.Sign(input.x) * 90f; // Rotate left/right based on input
            StartCoroutine(RotatePlayer(angle));
        }
    }

    System.Collections.IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + direction;
        float elapsedTime = 0f;
        float moveDuration = gridSize / moveSpeed;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.position = targetPosition; // Snap perfectly to grid anchor
        isMoving = false;
    }

    System.Collections.IEnumerator RotatePlayer(float angle)
    {
        isMoving = true;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, angle, 0);
        float elapsedTime = 0f;
        float rotateDuration = Mathf.Abs(angle) / rotateSpeed;

        while (elapsedTime < rotateDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotateDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.rotation = targetRotation; // Snap perfectly to 90-degree rotation
        isMoving = false;
    }
}
