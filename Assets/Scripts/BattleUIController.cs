using UnityEngine;
using UnityEngine.InputSystem;

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
    private PlayerInput globalInput;
    private Camera battleCam;

    public UIState CurrentUiState => currentUiState;

    private void Awake()
    {
        globalInput = GetComponent<PlayerInput>();
        if (BattleManager.Instance == null || BattleManager.Instance.BattleCamera == null)
        {
            Debug.LogWarning("BattleUIcontroller cannot find BattleManager or its BattleCamera reference! Enemy click detection will not work without this. Please ensure BattleManager is present in the scene and has a valid reference to the battle camera.");
            return;
        }
        battleCam = BattleManager.Instance.BattleCamera;
    }

    private void OnEnable()
    {
        if (globalInput != null)
            globalInput.actions["UI/Click"].performed += OnGlobalClick;
    }

    private void OnDisable()
    {
        if (globalInput != null)
            globalInput.actions["UI/Click"].performed -= OnGlobalClick;
    }

    private void OnGlobalClick(InputAction.CallbackContext context)
    {
        if (currentUiState != UIState.SelectingTarget) return;

        if (battleCam == null)
        {
            Debug.LogWarning("Battlecam reference in BattleUiController is missing!");
            return;
        }

        // Grab the "Point" action (which automatically tracks mouse, touch, or gamepad virtual cursors)
        Vector2 screenPos = globalInput.actions["UI/Point"].ReadValue<Vector2>();

        // Raycast to detect if an enemy sprite was clicked and call OnClickEnemySprite with the corresponding BattleEntity
        Ray ray = battleCam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            BattleEntity targetedEnemy = hit.collider.GetComponent<BattleEntity>();
            if (targetedEnemy != null)
            {
                OnClickEnemySprite(targetedEnemy);
            }
        }
    }

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

        EnemyVisuals[] clickables = FindObjectsByType<EnemyVisuals>(FindObjectsSortMode.None);
        foreach (EnemyVisuals enemyUI in clickables)
        {
            enemyUI.ToggleHighlight(true);
        }
    }

    private void AdvanceToNextPartyMember() 
    {
        currentUiState = UIState.SelectingAction;
        currentAttacker = null;
        currentTarget = null;

        // Get the list of clickable enemy sprites from EnemyVisuals components in the scene and disable their click detection
        EnemyVisuals[] clickables = FindObjectsByType<EnemyVisuals>(FindObjectsSortMode.None);
        foreach (EnemyVisuals enemyUI in clickables)
        {
            enemyUI.ToggleHighlight(false);
        }

        BattleManager.Instance.NextAlly();
    }
}
