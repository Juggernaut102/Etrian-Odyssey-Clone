using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


// Singleton class used to manage game states, such as the title screen, gameplay, pause menu, and game over screen.
// This class can be expanded to include methods for handling transitions between states, managing game flow, and coordinating with other systems like the AudioManager.

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static int CurrentTurn = 0; // Used to track turns in battle and trigger turn-based events


    // Property to access the singleton instance of GameManager. If it doesn't exist, it will attempt to find one in the scene or create a new one from a prefab.
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
                if (instance == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("GameRoot");
                    if (prefab != null)
                    {
                        GameObject obj = Instantiate(prefab);
                        instance = obj.GetComponent<GameManager>();
                    }
                    else
                    {
                        Debug.Log("Can't find GameRoot prefab in Resources folder! Please create one and add the GameManager component to it.");
                    }
                }
            }
            return instance;
        }
    }

    public enum GameState
    {
        TitleScreen,
        Explore,
        Battle,
        Paused,
        GameOver
    }

    [Header("Current GameState")]
    [SerializeField] private GameState currentState = GameState.TitleScreen;
    public GameState CurrentState => currentState;

    [Header("Scene Configuration")]
    [SerializeField] private string titleSceneName; // Remember to assign in editor!!
    [SerializeField] private string firstLevelName; // Remember to assign in editor!!
    [SerializeField] private string battleSceneName; // Remember to assign in editor!!

    private GameState prevGameState; // Used to store the previous game state when pausing

    [Header("Battle info")]
    public Vector2Int LastBattlePosition { get; private set; }


    /// <summary>
    /// Fires when the player takes a step in the dungeon, or a turn passes in battle
    /// </summary>  
    public static event Action OnGlobalTurnTick;

    public static event Action<bool> OnBattleEnd;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        string activeScene = SceneManager.GetActiveScene().name;
        if (activeScene == titleSceneName)
        {
            UpdateGameState(GameState.TitleScreen);
        }
        else
        {
            UpdateGameState(GameState.Explore);
        }
    }

    public void OnPauseInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return; // Only trigger on button press, not release

        if (currentState == GameState.Paused)
        {
            ResumeGame();
        }
        else if (currentState == GameState.Explore || currentState == GameState.Battle)
        {
            prevGameState = currentState; // Store the current state before pausing
            UpdateGameState(GameState.Paused);
        }
    }


    /// <summary>
    /// Invokes the global turn tick event, which can be used to trigger any logic that should happen on each turn, such as enemy actions, status effect updates, or environmental changes.
    /// </summary>
    public void ProcessGlobalTurnTick()
    {
        CurrentTurn++;
        OnGlobalTurnTick?.Invoke();
        Debug.Log("One turn passed! Current turn: " + CurrentTurn);
    }

    /// <summary>
    /// Safely changes the game state and handles any background logic needed for that state.
    /// </summary>
    public void UpdateGameState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.TitleScreen:
                HandleTitleScreen();
                break;
            case GameState.Explore:
                HandleExplore();
                break;
            case GameState.Battle:
                HandleBattle();
                break;
            case GameState.Paused:
                HandlePaused();
                break;
            case GameState.GameOver:
                HandleGameOver();
                break;
            default:
                Debug.LogWarning("Unhandled GameState: " + currentState);
                break;
        }
    }

    // --- State Handler Methods ---
    private void HandleTitleScreen()
    {
        Time.timeScale = 0f; // Pause game time on title screen
        if (UiManager.Instance != null) UiManager.Instance.SetTitleUI();
        Debug.Log("Going to title screen!");
    }

    private void HandleExplore()
    {
        Time.timeScale = 1f; // Ensure game time is running
        GetComponent<PlayerInput>().SwitchCurrentActionMap("Player"); // Switch to player controls
        if (UiManager.Instance != null) UiManager.Instance.SetExplorationHUD();
        Debug.Log("Beginnning exploration!");

    }

    private void HandleBattle()
    {
        Time.timeScale = 1f;    // Leave game time running for dungeon enemies to join battle
        GetComponent<PlayerInput>().SwitchCurrentActionMap("UI"); // Switch to UI controls for battle
        if (UiManager.Instance != null) UiManager.Instance.SetBattleHUD();
        Debug.Log("Entering battle!");
    }

    private void HandlePaused()
    {
        Time.timeScale = 0f; // Pause game time when paused
        GetComponent<PlayerInput>().SwitchCurrentActionMap("UI"); // Switch to UI controls
        if (UiManager.Instance != null) UiManager.Instance.SetMenuUI();
        Debug.Log("Menu opened!");
    }

    private void HandleGameOver()
    {
        Time.timeScale = 0f; // Pause game time on game over
        if (UiManager.Instance != null) UiManager.Instance.SetGameOverUI();
        Debug.Log("Player Died! Game Over.");
    }


    // --- Scene Navigation Methods ---

    /// <summary>
    /// Commands the scene manager to boot up the actual game level.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(firstLevelName);
        UpdateGameState(GameState.Explore);
    }

    /// <summary>
    /// Sends the player completely back to the main title screen.
    /// </summary>
    public void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
        UpdateGameState(GameState.TitleScreen);
    }

    /// <summary>
    /// Returns to game state before pausing.
    /// </summary>
    public void ResumeGame()
    {
        UpdateGameState(prevGameState);
    }

    /// <summary>
    /// Sends the player to the battle scene when battle starts. 
    /// This should be called by the BattleManager when the battle is initiated.
    /// </summary>
    public void EnterBattle(EnemyProfile enemy, Vector2Int combatTile)
    {
        UpdateGameState(GameState.Battle);

        LastBattlePosition = combatTile;

        // Load battle scene additively and initialize battle with the given enemy once loading is complete
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(battleSceneName, LoadSceneMode.Additive);
        loadOp.completed += (_) =>
        {
            BattleManager battleManager = FindFirstObjectByType<BattleManager>();
            if (battleManager != null) battleManager.InitializeBattle(enemy);
            else Debug.LogError("BattleManager not found in scene after loading battle scene! Please ensure a BattleManager component is present in the battle scene.");
        };
    }

    /// <summary>
    ///  Returns player to same position in dungeon after battle ends. 
    ///  Battle considered end only when won or fled, not when party wiped (that is the game over state). 
    ///  This should be called by the BattleManager when the battle is concluded.
    /// </summary>
    public void ExitBattle(bool playerVictory)
    {
        OnBattleEnd?.Invoke(playerVictory);
        SceneManager.UnloadSceneAsync(battleSceneName);
        UpdateGameState(GameState.Explore);

        
    }

    /// <summary>
    /// Sends player to GameOver Scene after party wipe. 
    /// This should be called by any relevent script when party health hits zero.
    /// </summary>
    public void GameOver()
    {
        UpdateGameState(GameState.GameOver);
    }
}
