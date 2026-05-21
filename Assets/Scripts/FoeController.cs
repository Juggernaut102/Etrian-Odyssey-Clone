using UnityEngine;

public class FoeController : MonoBehaviour
{
    [Header("FOE Configuration")]
    [SerializeField] private FoeMovement foe;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private EnemyProfile profile;

    private void OnEnable()
    {
        GameManager.OnGlobalTurnTick += TakeTurnAction;
        GameManager.OnBattleEnd += HandleBattleResolution;
    }

    private void OnDisable()
    {
        GameManager.OnGlobalTurnTick -= TakeTurnAction;
        GameManager.OnBattleEnd -= HandleBattleResolution;

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
            GameManager.Instance.EnterBattle(profile, foe.CurrentGridPosition);
        }
    }

    private void HandleBattleResolution(bool playerVictory)
    {
        // if (GameManager.Instance.LastBattleResult == BattleManager.BattleResult.PlayerVictory)
        if (foe.CurrentGridPosition == GameManager.Instance.LastBattlePosition)
        {
            if (playerVictory)
            {
                Die();
            }
            else
            {
                Debug.Log("Player fled! Respawning visual mesh and backing away.");
                StepAwayFromPlayer(); // a little iffy if i want to make foe step away or player step away, or depends on who attack who?
            }
        }
    }

    private void StepAwayFromPlayer()
    {
        // logic for moving 1 step away
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
