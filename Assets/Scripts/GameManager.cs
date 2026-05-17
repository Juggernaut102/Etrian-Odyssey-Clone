using Unity.VisualScripting;
using UnityEngine;
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
        Playing,
        Paused,
        GameOver
    }

    [Header("Current GameState")]
    [SerializeField] private GameState currentState = GameState.TitleScreen;
    public GameState CurrentState => currentState;

    [Header("Scene Configuration")]
    [SerializeField] private string titleSceneName; // Remember to assign in editor!!
    [SerializeField] private string firstLevelName; // Remember to assign in editor!!

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
            UpdateGameState(GameState.Playing);
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
            case GameState.Playing:
                HandlePlaying();
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
    }

    private void HandlePlaying()
    {
        Time.timeScale = 1f; // Ensure game time is running
    }

    private void HandlePaused()
    {
        Time.timeScale = 0f; // Pause game time when paused
    }

    private void HandleGameOver()
    {
        Time.timeScale = 0f; // Pause game time on game over
        Debug.Log("Player Died! Game Over.");
    }


    // --- Scene Navigation Methods ---

    /// <summary>
    /// Commands the scene manager to boot up the actual game level.
    /// </summary>
    public void StartGame() 
    {
        UpdateGameState(GameState.Playing);
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
}
