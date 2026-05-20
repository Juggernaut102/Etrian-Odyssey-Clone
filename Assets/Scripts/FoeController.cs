using Unity.VisualScripting;
using UnityEngine;

public class FoeController : MonoBehaviour
{
    [Header("FOE Configuration")]
    [SerializeField] private FoeMovement foe;
    [SerializeField] private GridMovement player;

    private void OnEnable()
    {
        GameManager.OnGlobalTurnTick += TakeTurnAction;
    }

    private void OnDisable()
    {
        GameManager.OnGlobalTurnTick -= TakeTurnAction;
    }

    private void TakeTurnAction()
    {
        foe.Move();
        CheckForCombatIntersection();
    }

    private void CheckForCombatIntersection()
    {
        // Not complete, must check if player and foe cross paths during movement, not just if they end on the same tile. This is just a placeholder for now to trigger battle when they end on the same tile.
        if (foe.CurrentGridPosition == player.CurrentGridPosition)
        {
            GameManager.Instance.EnterBattle();
        }
    }
}
