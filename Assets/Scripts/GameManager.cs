using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


// Singleton class used to manage game states, such as the title screen, gameplay, pause menu, and game over screen. This class can be expanded to include methods for handling transitions between states, managing game flow, and coordinating with other systems like the AudioManager.

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

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

    private GameState prevGameState; // Used to store the previous game state when pausing

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
        if (UiManager.Instance != null) UiManager.Instance.SetExplorationHUD();
        Debug.Log("Beginnning exploration!");

    }

    private void HandleBattle()
    {
        Time.timeScale = 1f; // Ensure game time is running(?)
        if (UiManager.Instance != null) UiManager.Instance.SetBattleHUD();
        Debug.Log("Entering battle!");
    }

    private void HandlePaused()
    {
        Time.timeScale = 0f; // Pause game time when paused
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
        UpdateGameState(GameState.Explore);
        SceneManager.LoadScene(firstLevelName);
    }

    /// <summary>
    /// Sends the player completely back to the main title screen.
    /// </summary>
    public void ReturnToTitle()
    {
        UpdateGameState(GameState.TitleScreen);
        SceneManager.LoadScene(titleSceneName);
    }

    /// <summary>
    /// Returns to game state before pausing.
    /// </summary>
    public void ResumeGame()
    {
        UpdateGameState(prevGameState);
    }
}
