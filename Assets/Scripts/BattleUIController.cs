using UnityEngine;

public class BattleUIController : MonoBehaviour
{
    // Temporary slots to remember the current selection state
    private BattleEntity currentAttacker;
    private BattleEntity currentTarget;

    [SerializeField] private Canvas battleHud;      // Is this even necessary?

    /// <summary>
    /// The BattleManager tells the UI which character is currently choosing their move.
    /// </summary>
    public void SetActiveAttacker(BattleEntity ally)
    {
        currentAttacker = ally;
        // Pop up the action menu next to this character's portrait!
        Debug.Log($"UI: Now selecting a move for {currentAttacker.EntityName}");
        // battleHud.SetActive(true);
    }

    /// <summary>
    /// Hook this up to the physical UI "Attack" Button asset.
    /// </summary>
    public void OnClickAttackButton()
    {
        Debug.Log($"UI: {currentAttacker.EntityName} chose Attack! Now click on a target enemy...");

        // Open the targeting system! Enable click detection on enemy sprites.
        EnterTargetSelectionMode();
    }

    /// <summary>
    /// Call when the player physically clicks on an enemy's 3D model or 2D sprite.
    /// Sends the selected target back to the BattleManager to be added to the turn queue, then advances to the next party member's turn.
    /// </summary>
    public void OnClickEnemySprite(BattleEntity clickedEnemy)
    {
        currentTarget = clickedEnemy;
        Debug.Log($"UI: Target selected: {currentTarget.EntityName}");

        BattleManager.Instance.CommandPlayerAttack(currentAttacker, currentTarget);

        AdvanceToNextPartyMember();
    }

    private void EnterTargetSelectionMode() { /* Highlight enemy sprites */ }
    private void AdvanceToNextPartyMember() { /* Reset slots for next character */ }
}
