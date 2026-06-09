using System.Collections.Generic;
using UnityEngine;

public class HarvestEvent : MonoBehaviour, IInteractable
{
    [Header("Visuals")]
    [SerializeField] private GameObject visualVFX;

    [Header("Harvesting configuration")]
    [SerializeField] private int minItemsPerHarvest = 1;
    [SerializeField] private int maxItemsPerHarvest = 3;
    [SerializeField] private int maxNumOfHavests = 3;
    // [SerializeField] private List<Item> possibleDrops = new List<Item>(); // This can be expanded to a more complex loot table with drop rates, rarities, etc.
    private int harvestCount = 0;

    public void Interact()
    {
        if (harvestCount >= maxNumOfHavests) 
        {
            // show notice to player that the spot is depleted and will respawn after a certain amount of time
            Debug.Log("This spot is now depleted. Come back later!");
            return;
        }

        // Simulate harvesting logic here. Something like randomly select items from the possibleDrops list and add them to the player's inventory.
        int itemsPerHarvest = Random.Range(minItemsPerHarvest, maxItemsPerHarvest + 1);
        // for each item, randomly select from possibleDrops and add to inventory
        // play harvesting audio

        Debug.Log("Harvesting! You gathered " + itemsPerHarvest + " items!");
        harvestCount++;
        Debug.Log($"This spot has been harvested {harvestCount} / {maxNumOfHavests} times");

        if (harvestCount == 3)
        {
            visualVFX.SetActive(false);
            // turn off visual indicator for harvestable spot here (e.g. change material, disable particle effects, etc.)

            // start a respawn coroutine here that will reset harvestCount to 0 after a certain amount of time, and turn the visual indicator back on to show that the spot is harvestable again
        }
    }
}
