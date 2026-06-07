using UnityEngine;
using UnityEngine.InputSystem;

// This class defines the player's interaction behavior.
// It listens for the interact input and checks for interactable objects in front of the player or on the floor beneath them.
// If an interactable object is found, it calls its Interact method.
public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 4f;
    [SerializeField] private LayerMask interactLayer;

    private PlayerMovement playerMovement;
    private PlayerInputHandler inputHandler;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    private void Start()
    {
        if (inputHandler != null) 
        {
            inputHandler.Controls.Player.Interact.performed += TryInteract;
        }
    }

    private void OnDestroy()
    {
        if (inputHandler != null)
        {
            inputHandler.Controls.Player.Interact.performed -= TryInteract;
        }
    }

    private void TryInteract(InputAction.CallbackContext context)
    {
        if (playerMovement.IsMoving || GameManager.Instance.CurrentState != GameManager.GameState.Explore)
        {
            Debug.LogWarning($"Interaction blocked! IsMoving: {playerMovement.IsMoving}, State: {GameManager.Instance.CurrentState}");
            return;
        }

        Vector3 origin = transform.position + Vector3.up * 0.5f; // Adjust the origin to be slightly above floor

        // for floor events like gathering spots that require players to stand on a tile
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit floorHit, interactRange, interactLayer))
        {
            Debug.Log("Trying to interact with: " + floorHit.collider.name);
            if (floorHit.collider.TryGetComponent(out IInteractable floorObj))
            {
                floorObj.Interact();
                return;
            }
        }

        // for map events that take up space like doors, chests, npcs, etc. Raycast forward in the direction the player is facing to check for interactable objects within range
        if (Physics.Raycast(origin, transform.forward, out RaycastHit forwardHit, interactRange, interactLayer))
        {
            Debug.Log("Trying to interact with: " + forwardHit.collider.name);
            if (forwardHit.collider.TryGetComponent(out IInteractable forwardObj))
            {
                forwardObj.Interact();
                return;
            }
        }
    }
}
