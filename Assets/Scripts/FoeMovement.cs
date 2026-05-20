using UnityEngine;

public class FoeMovement : MonoBehaviour
{
    private Vector2Int currentGridPosition; // Starting grid position
    public Vector2Int CurrentGridPosition => currentGridPosition; // Public getter for current grid position

    private void Awake()
    {
        UpdateCurrentGridPosition();                    // Set the initial grid position based on the starting transform
    }

    private void UpdateCurrentGridPosition()
    {
        currentGridPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z)); // Update grid position based on new transform
    }

    public void Move()
    {
        // do movement... possible to set up predefined paths for foes to follow?
    }
}
