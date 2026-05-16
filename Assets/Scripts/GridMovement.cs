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
    private Vector2 inputVector;

    private void Awake()
    {
        controls = new InputSystem_Actions();           // Initialize the input action asset
    }

    private void OnEnable()
    {
        if (controls != null)
        {
            controls.Player.Enable();
            controls.Player.Move.performed += ctx => inputVector = ctx.ReadValue<Vector2>();    // Subscribe custom method to the event signature
            controls.Player.Move.canceled += ctx => inputVector = Vector2.zero;                 // Reset input when movement is canceled
        }
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.Move.performed -= ctx => inputVector = ctx.ReadValue<Vector2>();   // Unsubscribe to prevent memory leaks
            controls.Player.Move.canceled -= ctx => inputVector = Vector2.zero;
            controls.Player.Disable();
        }        
    }

    // Beginning of custom method that handles movement input
    void Update()
    {
        if (isMoving) return; // Prevent starting a new move while already moving

        if (Mathf.Abs(inputVector.y) > 0.5f)
        {
            Vector3 direction = transform.forward * Mathf.Sign(inputVector.y) * gridSize; // Move forward/backward based on input
            StartCoroutine(MovePlayer(direction));
        }
        else if (Mathf.Abs(inputVector.x) > 0.5f)
        {
            float angle = Mathf.Sign(inputVector.x) * 90f; // Rotate left/right based on input
            StartCoroutine(RotatePlayer(angle));
        }
    }

    // Coroutine definitions for smooth movement to the target position
    System.Collections.IEnumerator MovePlayer(Vector3 direction)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + direction;

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

    System.Collections.IEnumerator RotatePlayer(float angle)
    {
        isMoving = true;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, angle, 0);
        float elapsedTime = 0f;
        float rotateDuration = Mathf.Abs(angle) / rotateSpeed;

        while (elapsedTime < rotateDuration)
        {
            float linearProgress = elapsedTime / rotateDuration;
            float easedProgress = Mathf.SmoothStep(0f, 1f, linearProgress);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, easedProgress);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.rotation = targetRotation; // Snap perfectly to 90-degree rotation
        isMoving = false;
    }
}
