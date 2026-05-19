using Unity.VisualScripting;
using UnityEngine;

public class FoeController : MonoBehaviour
{
    [Header("FOE Configuration")]
    // [SerializeField] private FoeMovement foeMovement;
    [SerializeField] private GameObject player;

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
        // foeMovement.Move();
        CheckForCombatIntersection();
    }

    private void CheckForCombatIntersection()
    {
        if (transform.position == player.transform.position)
        {
            GameManager.Instance.EnterBattle();
        }
    }
}
