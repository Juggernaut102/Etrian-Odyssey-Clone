using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyClickTarget : MonoBehaviour
{
    private BattleEntity entity;

    [Header("Visual Feedback Layout")]
    // the gameObject which will be used to highlight the enemy when selected (can be arrow, outline etc)
    [SerializeField] private GameObject selectionHighlightGraphic;

    private void Awake()
    {
       entity = GetComponent<BattleEntity>();
       selectionHighlightGraphic.SetActive(false);
    }

    private void Update()
    {
        // Debug.Log($"Update is running on {gameObject.name} at position {transform.position}");
        // Check if a mouse or pointer device exists and if the left button was pressed THIS frame
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            ExecuteNewInputRaycast();
        }
        // Support for Touch Screens / Mobile / Steam Deck Trackpads:
        else if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            ExecuteNewInputRaycast();
        }
    }

    private void ExecuteNewInputRaycast()
    {
        if (BattleManager.Instance == null || BattleManager.Instance.BattleCamera == null) return;
        Camera battleCam = BattleManager.Instance.BattleCamera;

        if (battleCam == null) return;

        // Get the exact screen pixel coordinate from the active pointer device
        Vector2 screenPosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Pointer.current.position.ReadValue();

        // Shoot a physical ray through the modern input coordinates
        Ray ray = battleCam.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Did the ray physically hit THIS enemy object's root collider?
            if (hit.transform == this.transform)
            {
                ProcessClickSprite();
            }
        }
    }

    /// <summary>
    /// Executes when player clicks on enemy sprite. Possible extension to allow for controller controls?
    /// </summary>
    private void ProcessClickSprite()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Battle) return; // Only allow clicking when in battle state
        BattleUIController uiController = FindFirstObjectByType<BattleUIController>();
        if (uiController != null && entity != null)
        {
            if (uiController.CurrentUiState != BattleUIController.UIState.SelectingTarget) return; // Only allow clicking when in target selection mode)
            uiController.OnClickEnemySprite(entity);
        }
        else
        {
            Debug.LogWarning("Clicked enemy, but could not find BattleUIController or BattleEntity reference.");
        }
    }

    public void ToggleHighlight(bool isEnabled)
    {
        if (selectionHighlightGraphic != null) selectionHighlightGraphic.SetActive(isEnabled);
    }
}
