using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

// Singleton class used to manage battle-related logic, such as initiating battles, handling turn order, and coordinating with the GameManager and UiManager to update the game state and UI accordingly.
// This class can be expanded to include methods for managing enemy AI, player actions during battle, and transitioning back to exploration mode after a battle is resolved.


// How this class works is as follows: During player turn, the player will choose actions for each character in the party. BattleUIController will tell BattleManager what action and which enemy the player clicked.
// Then, each enemy will also add their planned Combat Action to turn queue using some sort of AI logic.
// Then during resolving phase, BattleManager will sort the actions by speed, and invoke them from fastest to slowest. Then end turn.
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

    [Header("Initializing")]
    [SerializeField] private BattleState currentBattleState;
    [SerializeField] private Camera battleCamera;
    public Camera BattleCamera => battleCamera; // Public getter for the battle camera, so other scripts can raycast against it when player clicks on enemies during battle
    private Camera dungeonCamera; // Reference to the main camera to turn it off during battle
    private AudioListener dungeonListener; // Reference to the main audio listener to turn it off during battle

    [Header("Roster")]
    [SerializeField] private List<CharacterEntity> activeAllies = new List<CharacterEntity>();
    [SerializeField] private List<EnemyEntity> activeEnemies = new List<EnemyEntity>();

    [Header("Spawning Anchors")]
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Turn Variables")]
    private int currentTurn = 0; // Variable to track (and display?) battle turn order
    private int currentAllyIndex = 0; // Variable to track which party member is currently selecting their action during player turn
    private BattleEntity currentAlly;
    private List<CombatAction> actionTurnQueue = new List<CombatAction>(); // List to hold all combat actions chosen by player and enemies during the turn, which will be sorted and resolved at the end of the turn>

    public int Damage = 10; // Placeholder variable for damage amount, to be replaced with actual calculations based on character stats and action types.


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

    public void InitializeBattle(EncounterProfile enemyTroop)
    {
        currentBattleState = BattleState.Initializing;

        InitializeBattleCameraAndAudio();

        PopulatePlayerParty();
        PopulateEnemyForces(enemyTroop);

        Debug.Log($"Battle started! {activeAllies.Count} heroes fighting {activeEnemies.Count} monsters!");
        EnterPlayerTurn();
    }

    public void ReinforceBattle(EncounterProfile enemyTroop)
    {
        int slotsLeft = enemySpawnPoints.Length - activeEnemies.Count;
        SpawnEnemy(enemyTroop);

        Debug.Log($"Reinforcements have arrived! {activeAllies.Count} heroes fighting {activeEnemies.Count} monsters!");
    }

    private void PopulatePlayerParty()
    {
        activeAllies.Clear();

        // FUTURE-PROOFING: Replace this placeholder loop with a call to
        // PartyManager/GameManager when building persistent navigation systems!
        // e.g., List<PlayerProfile> currentParty = PartyManager.Instance.GetActiveParty();

        // For now, let's find any existing PlayerEntity scripts already sitting in combat scene asset:
        CharacterEntity[] sceneHeroes = FindObjectsByType<CharacterEntity>(FindObjectsSortMode.None);
        foreach (CharacterEntity hero in sceneHeroes)
        {
            // If they are alive, register them to act this turn round!
            if (hero.IsAlive())
            {
                activeAllies.Add(hero);
            }
        }
    }

    private void PopulateEnemyForces(EncounterProfile enemyTroop)
    {
        activeEnemies.Clear();

        SpawnEnemy(enemyTroop);
    }

    private void SpawnEnemy(EncounterProfile enemyTroop)
    {
        // Find current empty slots by checking each anchor for children. If it has no children, it's empty. If it has a child but that child is dead/dying, we can also consider it empty (see bulletproofing comment below).
        List<Transform> freeAnchors = new List<Transform>();
        foreach (Transform anchor in enemySpawnPoints)
        {
            if (anchor.childCount == 0)
            {
                freeAnchors.Add(anchor);
            }
            else
            {
                // BULLETPROOFING: If the anchor has a child, but that child is dead/dying this exact frame, 
                // we should still consider the slot free!
                EnemyEntity occupant = anchor.GetComponentInChildren<EnemyEntity>();
                if (occupant == null || !occupant.IsAlive())
                {
                    freeAnchors.Add(anchor);
                }
            }
        }

        if (freeAnchors.Count == 0)
        {
            Debug.LogWarning("No slots available! Reinforcements must queue. (not implemented yet oops)");
            return;
        }

        int spawnCount = Mathf.Min(enemyTroop.EnemiesInTroop.Count, freeAnchors.Count);

        if (spawnCount < enemyTroop.EnemiesInTroop.Count)
        {
            Debug.LogWarning($"Only spawned {spawnCount} out of {enemyTroop.EnemiesInTroop.Count} enemies due to lack of space.");
        }

        // Execute spawning
        for (int i = 0; i < spawnCount; i++)
        {
            EnemyProfile currentEnemyProfile = enemyTroop.EnemiesInTroop[i];

            Transform anchor = freeAnchors[i];
            Vector3 spawnPosition = anchor.position + (Vector3.down * 1f);

            GameObject newEnemyObj = Instantiate(currentEnemyProfile.EnemyPrefabLayout, spawnPosition, anchor.rotation, anchor);
            EnemyEntity enemyActor = newEnemyObj.GetComponent<EnemyEntity>();

            if (enemyActor == null)
            {
                Debug.LogError($"Missing enemy entity for {currentEnemyProfile.EntityName} in {currentEnemyProfile.EntityName}'s EnemyProfile's Prefab!");
                continue;
            }

            enemyActor.Initialize(currentEnemyProfile);
            activeEnemies.Add(enemyActor);
        }
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

        currentAlly = activeAllies[currentAllyIndex];

        // Skip this character if they are knocked out!
        if (!currentAlly.IsAlive())
        {
            currentAllyIndex++;
            StartMenuInputForCurrentAlly(); // Recursively check the next person
            return;
        }
    }

    // Called by BattleUiController after player selects an action and target for the current ally, to advance to the next ally's turn.
    public void NextAlly()
    {
        currentAllyIndex++;
        StartMenuInputForCurrentAlly();
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
            CombatAction enemyAction = enemy.CalculateTurnAction(activeAllies);
            if (enemyAction != null)
            {
                actionTurnQueue.Add(enemyAction);
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

            // Sweeps the list and removes any element where the Unity object has been destroyed
            activeAllies.RemoveAll(ally => ally == null || !ally.IsAlive());
            activeEnemies.RemoveAll(enemy => enemy == null || !enemy.IsAlive());

            if (IsPlayerDefeated())
            {
                OnBattleLoss();
                return;
            }

            if (HasPlayerWon())
            {
                Win();
                return;
            }
        }
        actionTurnQueue.Clear();

        currentTurn++;
        GameManager.Instance.ProcessGlobalTurnTick();

        Debug.Log("Combat turn resolved. Current combat turn: " + currentTurn);
    }

    public void Win()
    {
        Debug.Log("Player has won the battle! Exiting Battle Scene!");
        CleanUpBatleScene();
        GameManager.Instance.ExitBattle(true); // Notify GameManager to return to exploration mode
    }

    // flee logic not implemented in battle system yet, but this is where it would go when player clicks "Flee" button in UI
    public void Flee()
    {
        Debug.Log("Player has fled the battle! Exiting Battle Scene!");
        CleanUpBatleScene();
        GameManager.Instance.ExitBattle(false); // Notify GameManager to return to exploration mode
    }

    public void OnBattleLoss()
    {
        Debug.Log("Player has lost the battle! What a noob! Exiting Battle Scene!");
        CleanUpBatleScene();
        GameManager.Instance.GameOver(); // Notify GameManager to trigger game over state
    }

    private bool IsPlayerDefeated() => activeAllies.Count == 0;

    private bool HasPlayerWon() => activeEnemies.Count == 0;

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

        plannedMove.ExecuteActionLogic = () => plannedMove.target.TakeDamage(Damage);

        RegisterPlayerAction(plannedMove);
    }

    private void InitializeBattleCameraAndAudio()
    {
        dungeonCamera = Camera.main;
        if (dungeonCamera != null)
        {
            dungeonListener = dungeonCamera.GetComponent<AudioListener>();
            dungeonCamera.gameObject.SetActive(false);
        }
        if (dungeonListener != null) dungeonListener.enabled = false;
    }

    private void CleanUpBatleScene()
    {
        if (dungeonCamera != null) dungeonCamera.gameObject.SetActive(true); // Turn the camera back on when battle is finished
        if (dungeonListener != null) dungeonListener.enabled = true; // Turn the audio listener back on when battle is finished
    }

    public BattleEntity GetCurrentAttacker() => currentAlly;
}
