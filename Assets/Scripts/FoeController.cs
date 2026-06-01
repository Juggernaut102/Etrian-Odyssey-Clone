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
        if (foe.CurrentGridPosition == player.CurrentGridPosition ||
            (foe.PreviousGridPosition == player.CurrentGridPosition && player.PreviousGridPosition == foe.CurrentGridPosition))
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
