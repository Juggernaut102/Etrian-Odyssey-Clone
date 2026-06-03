using UnityEngine;
using static GameManager;

public class GameOverController : MonoBehaviour
{
    void Awake()
    {
        GameManager gameManager = GameManager.Instance; // Ensure the GameManager is initialized before we try to use it
    }

    public void ReturnToTitle()
    {
        if (GameManager.Instance != null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScreen");
            GameManager.Instance.UpdateGameState(GameState.TitleScreen);
        }
    }
}
