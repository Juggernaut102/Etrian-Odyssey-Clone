using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

// Singleton class used to manage battle-related logic, such as initiating battles, handling turn order, and coordinating with the GameManager and UiManager to update the game state and UI accordingly.
// This class can be expanded to include methods for managing enemy AI, player actions during battle, and transitioning back to exploration mode after a battle is resolved.

public class BattleManager : MonoBehaviour
{
    private static BattleManager instance;

    // Property to access the singleton instance of BattleManager. If it doesn't exist, it will attempt to find one in the scene or create a new one from a prefab.
    public static BattleManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<BattleManager>();
                if (instance == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("GameRoot");
                    if (prefab != null)
                    {
                        GameObject obj = Instantiate(prefab);
                        instance = obj.GetComponent<BattleManager>();
                    }
                    else
                    {
                        Debug.Log("Can't find GameRoot prefab in Resources folder! Please create one and add the BattleManager component to it.");
                    }
                }
            }
            return instance;
        }
    }

    private Camera dungeonCamera; // Reference to the main camera to turn it off during battle
    private int currentTurn = 0; // Variable to track (and display?) battle turn order

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }

        dungeonCamera = Camera.main;
        if (dungeonCamera != null) dungeonCamera.gameObject.SetActive(false);
    }

    /// <summary>
    /// Calls at the end of every turn in battle.
    /// </summary>
    public void ResolveBattleTurn()
    {
        // ... Calculate player damage, enemy damage, check for deaths ...

        currentTurn++;
        GameManager.Instance.ProcessGlobalTurnTick();

        Debug.Log("Combat turn resolved. Current combat turn: " + currentTurn);
    }

    public void OnBattleFinished()
    {
        if (dungeonCamera != null) dungeonCamera.gameObject.SetActive(true); // Turn the camera back on when battle is finished
        GameManager.Instance.ExitBattle(); // Notify GameManager to return to exploration mode
    }

    public void OnBattleLoss()
    {
        if (dungeonCamera != null) dungeonCamera.gameObject.SetActive(true); // Turn the camera back on when battle is finished
        GameManager.Instance.GameOver(); // Notify GameManager to trigger game over state
    }
}
