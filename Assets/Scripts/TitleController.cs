using System.Security.Cryptography;
using UnityEngine;
using static GameManager;

public class TitleController : MonoBehaviour
{
    void Awake()
    {
        GameManager gameManager = GameManager.Instance; // Ensure the GameManager is initialized before we try to use it

    }
    public void StartGame()
    {
        // Because the manager bootstrapped itself at boot time, 
        // Instance is guaranteed to be awake and waiting for us!
        if (GameManager.Instance != null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("FirstLevel");
            GameManager.Instance.UpdateGameState(GameState.Explore);
        }
    }

    public void QuitGame()
    {
        // 1. If we are testing inside the Unity Editor, stop playing
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in the editor
        
        // 2. If we are playing a compiled build, shut down the application
    #else
            Application.Quit();
    #endif
    }
}
