using UnityEngine;

public class FoeController : MonoBehaviour
{
    [Header("FOE Configuration")]
    [SerializeField] private FoeMovement foe;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private EncounterProfile profile;

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
            this.enabled = false;
            this.gameObject.SetActive(false);

            if (GameManager.Instance.CurrentState != GameManager.GameState.Battle)
            {
                GameManager.Instance.EnterBattle(profile, this);
            }
            else
            {
                GameManager.Instance.Reinforce(profile, this);
            }
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
