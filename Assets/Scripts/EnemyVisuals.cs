using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyVisuals : MonoBehaviour
{
    [Header("Visual Feedback Layout")]
    // the gameObject which will be used to highlight the enemy when selected (can be arrow, outline etc)
    [SerializeField] private GameObject selectionHighlightGraphic;

    private void Awake()
    {
       selectionHighlightGraphic.SetActive(false);
    }

    public void ToggleHighlight(bool isEnabled)
    {
        if (selectionHighlightGraphic != null) selectionHighlightGraphic.SetActive(isEnabled);
    }
}
