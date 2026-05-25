using UnityEngine;

public class BattleUIController : MonoBehaviour
{
    // Temporary slots to remember the current selection state
    [SerializeField] private BattleEntity currentAttacker;
    private BattleEntity currentTarget;

    /// <summary>
    /// Hook this up to the physical UI "Attack" Button asset.
    /// </summary>
    public void OnClickAttackButton()
    {
        currentAttacker = BattleManager.Instance.GetCurrentAttacker();
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
