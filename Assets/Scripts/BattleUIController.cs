using UnityEngine;

public class BattleUIController : MonoBehaviour
{
    public enum UIState
    {
        Idle,   // will this state be used?
        SelectingAction,
        SelectingTarget
    }

    [SerializeField] private UIState currentUiState = UIState.SelectingAction; // or selecting action?
    [SerializeField] private BattleEntity currentAttacker;
    private BattleEntity currentTarget;

    public UIState CurrentUiState => currentUiState;

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
        if (currentUiState != UIState.SelectingTarget) return;

        currentTarget = clickedEnemy;
        Debug.Log($"UI: Target selected: {currentTarget.EntityName}");

        BattleManager.Instance.CommandPlayerAttack(currentAttacker, currentTarget);

        AdvanceToNextPartyMember();
    }

    private void EnterTargetSelectionMode() 
    {
        currentUiState = UIState.SelectingTarget;

        // Get the list of clickable enemy sprites from EnemyClickTarget components in the scene and enable their click detection
        EnemyClickTarget[] clickables = FindObjectsByType<EnemyClickTarget>(FindObjectsSortMode.None);
        foreach (EnemyClickTarget enemyUI in clickables)
        {
            enemyUI.ToggleHighlight(true);
        }
    }

    private void AdvanceToNextPartyMember() 
    {
        currentUiState = UIState.SelectingAction;
        currentAttacker = null;
        currentTarget = null;

        // Get the list of clickable enemy sprites from EnemyClickTarget components in the scene and disable their click detection
        EnemyClickTarget[] clickables = FindObjectsByType<EnemyClickTarget>(FindObjectsSortMode.None);
        foreach (EnemyClickTarget enemyUI in clickables)
        {
            enemyUI.ToggleHighlight(false);
        }

        BattleManager.Instance.NextAlly();
    }
}
