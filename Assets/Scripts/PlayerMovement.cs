using UnityEngine;

public class PlayerMovement : GridMovement
{
    [Header("Player Movement Settings")]
    [SerializeField] private float rotateSpeed = 180f;

    private InputSystem_Actions controls;
    private Vector2 inputVector;

    protected override void Awake()
    {
        base.Awake(); // Call the base class Awake to initialize grid position
        controls = new InputSystem_Actions(); // Initialize the input actions
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
        OnMove();
    }

    private void OnMove()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Explore) return; // Only allow movement when in exploring state

        if (isMoving) return; // Prevent starting a new move while already moving

        if (Mathf.Abs(inputVector.y) > 0.5f)
        {
            Vector3 direction = transform.forward * Mathf.Sign(inputVector.y) * gridSize; // Move forward/backward based on input
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + direction;

            // Collision detection
            // We lift the start/end points up slightly (Vector3.up * 0.5f) so the line doesn't scrape the floor
            Vector3 lineStart = startPosition + (Vector3.up * 0.5f);
            Vector3 lineEnd = targetPosition + (Vector3.up * 0.5f);
            if (Physics.Linecast(lineStart, lineEnd, wallLayer))
            {
                AudioManager.Instance.PlayWallThud(gridSize / moveSpeed);
            }
            else
            {
                StartCoroutine(MoveEntity(direction));
                OnMovementComplete();
            }
        }
        else if (Mathf.Abs(inputVector.x) > 0.5f)
        {
            float angle = Mathf.Sign(inputVector.x) * 90f; // Rotate left/right based on input
            StartCoroutine(RotatePlayer(angle));
        }
    }

    System.Collections.IEnumerator RotatePlayer(float angle)
    {
        isMoving = true;

        AudioManager.Instance.PlayFootsteps();

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

    protected override void OnMovementComplete()
    {
        base.OnMovementComplete();
        GameManager.Instance.ProcessGlobalTurnTick(); // Notify GameManager to advance the turn after each move
        // GameManager.Instance.CheckForRandomEncounters(); // Check for random encounters after each move
    }
}
