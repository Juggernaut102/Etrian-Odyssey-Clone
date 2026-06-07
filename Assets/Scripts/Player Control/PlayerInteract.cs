using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 1f;
    [SerializeField] private LayerMask interactLayer;

    private PlayerMovement playerMovement;
    private InputSystem_Actions controls;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        controls = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Interact.performed += TryInteract;
    }

    private void OnDisable()
    {
        controls.Player.Interact.performed -= TryInteract;
        controls.Player.Disable();
    }

    private void TryInteract(InputAction.CallbackContext context)
    {
        if (playerMovement.IsMoving || GameManager.Instance.CurrentState != GameManager.GameState.Explore) return;

        Vector3 origin = transform.position + Vector3.up * 0.5f; // Adjust the origin to be at the player's chest height

        // for floor events like gathering spots that require players to stand on a tile
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit floorHit, interactRange, interactLayer))
        {
            if (floorHit.collider.TryGetComponent(out IInteractable floorObj))
            {
                floorObj.Interact();
                return;
            }
        }

        if (Physics.Raycast(origin, transform.forward, out RaycastHit forwardHit, interactRange, interactLayer))
        {
            if (forwardHit.collider.TryGetComponent(out IInteractable forwardObj))
            {
                forwardObj.Interact();
                return;
            }
        }
    }
}
