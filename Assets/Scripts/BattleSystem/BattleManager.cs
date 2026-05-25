using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

// Singleton class used to manage battle-related logic, such as initiating battles, handling turn order, and coordinating with the GameManager and UiManager to update the game state and UI accordingly.
// This class can be expanded to include methods for managing enemy AI, player actions during battle, and transitioning back to exploration mode after a battle is resolved.


// How this class works is as follows: During player turn, the player will choose actions for each character in the party.
// BattleUIController will then encapsulate these actions into a CombatAction and pass it to BattleManager.
// Then, same thing for enemy turn, but for enemy units.    
// Then during resolving phase, BattleManager will sort the actions by speed, and resolve them by invoking them. Then end turn.
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

    public enum BattleState
    {
        Initializing,
        PlayerTurn,
        EnemyTurn,
        Resolving,
        PlayerVictory,
        PlayerFled,
        PlayerDied
    }

    [Header("Current Phase")]
    [SerializeField] private BattleState currentBattleState;
    [SerializeField] private BattleUIController battleUiController;

    // Placeholder, will be placed in player profile once that is created
    [Header("Player Stats")]
    private int playerMaxHP = 100;
    private int playerCurrentHP = 100;

    private Camera dungeonCamera; // Reference to the main camera to turn it off during battle
    private AudioListener dungeonListener; // Reference to the main audio listener to turn it off during battle

    [Header("Battle Variable")]
    private int currentTurn = 0; // Variable to track (and display?) battle turn order
    [SerializeField] private List<BattleEntity> activeAllies = new List<BattleEntity>();
    [SerializeField] private List<BattleEntity> activeEnemies = new List<BattleEntity>();

    [Header("Turn Variables")]
    private int currentAllyIndex = 0; // Variable to track which party member is currently selecting their action during player turn
    private EnemyProfile currentEnemy; // Reference to the current enemy being fought, can be used to access stats and update UI
    private List<CombatAction> actionTurnQueue = new List<CombatAction>(); // List to hold all combat actions chosen by player and enemies during the turn, which will be sorted and resolved at the end of the turn>


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void InitializeBattle(EnemyProfile enemy)
    {
        currentBattleState = BattleState.Initializing;

        dungeonCamera = Camera.main;
        if (dungeonCamera != null)
        {
            dungeonListener = dungeonCamera.GetComponent<AudioListener>();
            dungeonCamera.gameObject.SetActive(false);
        }
        if (dungeonListener != null) dungeonListener.enabled = false;

        // ADD LOGIC to fill in activeEnemies and activeAllies
        currentEnemy = enemy;
        Debug.Log($"Combat Scene Initializing! Fighting: {currentEnemy.EnemyName}");
        EnterPlayerTurn();
    }

    // ─── BEGINNING OF TURN ───────────────────────────────────
    public void EnterPlayerTurn()
    {
        currentBattleState = BattleState.PlayerTurn;
        actionTurnQueue.Clear(); // Clean slate for the new round (called at turn end but just call again in case)
        currentAllyIndex = 0;    // Reset to the first party member

        Debug.Log("Player Selection Phase Started.");
        StartMenuInputForCurrentAlly();
    }

    private void StartMenuInputForCurrentAlly()
    {
        // Safety check: Have all allies finished inputting their actions?
        if (currentAllyIndex >= activeAllies.Count)
        {
            Debug.Log("All allies have submitted actions! Moving to enemy phase...");
            EnterEnemyTurn();
            return;
        }

        BattleEntity currentWorker = activeAllies[currentAllyIndex];

        // Skip this character if they are knocked out!
        if (!currentWorker.IsAlive())
        {
            currentAllyIndex++;
            StartMenuInputForCurrentAlly(); // Recursively check the next person
            return;
        }

        // 🎯 THE HANDOFF: Push the active character directly to the UI!
        // (Assuming you have a reference to your BattleUIController script instance)
        battleUiController.SetActiveAttacker(currentWorker);
    }

    /// <summary>
    /// Hook this up to a physical "End Turn" button in UI canvas layout.
    /// </summary>
    public void OnPlayerFinishedAllInputs()
    {
        if (currentBattleState != BattleState.PlayerTurn) return;

        Debug.Log("Player submitted all actions. Moving to Enemy phase...");
        EnterEnemyTurn();
    }

    // This method is called by the BattleUIController when the player selects an action and clicks "End Turn". It adds the player's chosen action to the turn queue, which will be resolved at the end of the turn.
    public void RegisterPlayerAction(CombatAction action)
    {
        actionTurnQueue.Add(action);
        Debug.Log($"Action saved: { action.user.name} will use { action.actionName}.");
    }

    /// <summary>
    /// Once the player clicks "End Turn", the enemy AI calculates its moves.
    /// </summary>
    public void EnterEnemyTurn()
    {
        currentBattleState = BattleState.EnemyTurn;
        foreach (BattleEntity enemy in activeEnemies)
        {
            CombatAction enemyAction = enemy.CalculateEnemyAction(activeAllies); // Enemy calculates its action based on the current state of the battle and the player's party
            if (enemyAction != null)
            {
                actionTurnQueue.Add(enemyAction); // Add the enemy's action to the turn queue
                Debug.Log($"Enemy Action: {enemy.name} will use {enemyAction.actionName}.");
            }
        }
        ResolveBattleTurn();
    }

    /// <summary>
    /// Calls at the end of every turn in battle.
    /// </summary>
    public void ResolveBattleTurn()
    {
        // ... Calculate player damage, enemy damage, check for deaths ...

        Debug.Log("Resolving Phase started! Sorting actions by agility...");
        actionTurnQueue = actionTurnQueue.OrderByDescending(action => action.speed).ToList(); // Sort actions by speed (descending order, so higher speed goes first)
        foreach (CombatAction action in actionTurnQueue)
        {
            if (action.user == null || !action.user.IsAlive())
            {
                Debug.Log("Action skipped: " + action.user?.name + " is already defeated.");
                continue;
            }
            Debug.Log($"Executing action: {action.user.name} uses {action.actionName} on {action.target.name}.");
            action.ExecuteActionLogic?.Invoke(); // Execute the logic for this action, which will apply its effects to the game state (damage, status effects, etc.)

            if (IsBattleOver()) return;
        }
        actionTurnQueue.Clear();

        currentTurn++;
        GameManager.Instance.ProcessGlobalTurnTick();

        Debug.Log("Combat turn resolved. Current combat turn: " + currentTurn);
    }

    public void Win()
    {
        if (dungeonCamera != null) dungeonCamera.gameObject.SetActive(true); // Turn the camera back on when battle is finished
        if (dungeonListener != null) dungeonListener.enabled = true; // Turn the audio listener back on when battle is finished
        GameManager.Instance.ExitBattle(true); // Notify GameManager to return to exploration mode
    }

    public void Flee()
    {
        if (dungeonCamera != null) dungeonCamera.gameObject.SetActive(true); // Turn the camera back on when battle is finished
        if (dungeonListener != null) dungeonListener.enabled = true; // Turn the audio listener back on when battle is finished
        GameManager.Instance.ExitBattle(false); // Notify GameManager to return to exploration mode
    }

    public void OnBattleLoss()
    {
        if (dungeonCamera != null) dungeonCamera.gameObject.SetActive(true); // Turn the camera back on when battle is finished
        if (dungeonListener != null) dungeonListener.enabled = true; // Turn the audio listener back on when battle is finished
        GameManager.Instance.GameOver(); // Notify GameManager to trigger game over state
    }

    private bool IsBattleOver() => false; // placeholder

    /// <summary>
    /// The UI Controller will call this method when the player clicks "Attack"
    /// </summary>
    public void CommandPlayerAttack(BattleEntity attacker, BattleEntity target)
    {
        CombatAction plannedMove = new CombatAction();
        plannedMove.actionName = "Basic Strike";
        plannedMove.user = attacker;
        plannedMove.target = target;
        plannedMove.speed = attacker.Speed;

        plannedMove.ExecuteActionLogic = () => plannedMove.target.TakeDamage(15);

        RegisterPlayerAction(plannedMove);
    }
}
