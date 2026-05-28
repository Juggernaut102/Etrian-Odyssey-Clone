using System.Collections.Generic;
using UnityEngine;

public class FoeMovement : GridMovement
{
    [Header("Patrol Pathing")]
    [SerializeField] private List<Vector2Int> pathWaypoints;
    private int currentWaypointIndex = 0;

    public void Move()
    {
        if (isMoving || pathWaypoints == null || pathWaypoints.Count == 0) return; // Don't move if already moving or if no waypoints are set
        Vector2Int targetWaypoint = pathWaypoints[currentWaypointIndex];
        Vector3 targetWorldPosition = new Vector3(targetWaypoint.x * gridSize, transform.position.y, targetWaypoint.y * gridSize); // Convert grid coordinates to world position
        StartCoroutine(MoveEntity(targetWorldPosition)); // Move towards the current waypoint
        currentWaypointIndex = (currentWaypointIndex + 1) % pathWaypoints.Count; // Loop to the next waypoint, wrapping around to the start
        OnMovementComplete(); // Update grid position immediately after starting movement (since we won't be checking it every frame during movement)
    }

    // Only called if player cannot step away from foe when retreating
    public void StepAwayFromPlayer()
    {
        // logic for moving 1 step away
    }
}
