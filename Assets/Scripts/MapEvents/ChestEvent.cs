using UnityEngine;

public class ChestEvent : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOpen = false;
    // [SerializeField] private Item loot; // This can be expanded to a list of items for more complex loot tables
    public void Interact()
    {
        if (isOpen)
        {
            Debug.Log("The chest is already open. There's nothing left inside.");
            return;
        }

        isOpen = true;
        Debug.Log("Opening chest! Found some loot inside!");
        // Debug.Log($"You found: {loot.ItemName}"); // Assuming Item has an ItemName property

        // play open animation here
        // play sound effect here   
        // Send loot to inventory system here
    }
}
