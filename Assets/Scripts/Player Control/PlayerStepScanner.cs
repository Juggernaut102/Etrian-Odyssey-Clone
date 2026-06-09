using UnityEngine;

public class PlayerStepScanner : MonoBehaviour
{
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private float interactRange = 1f;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        playerMovement.OnStepComplete += CheckFloor;
    }

    private void OnDisable()
    {
        playerMovement.OnStepComplete -= CheckFloor;
    }

    private void CheckFloor()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f; // Adjust the origin to be slightly above floor
        Debug.Log("Checking floor!");
        // for floor events like gathering spots that require players to stand on a tile
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit floorHit, interactRange, interactLayer))
        {
            Debug.Log("Trying to interact with: " + floorHit.collider.name);
            if (floorHit.collider.TryGetComponent(out ISteppable floorObj))
            {
                floorObj.OnStep();
                return;
            }
        }
    }
}
