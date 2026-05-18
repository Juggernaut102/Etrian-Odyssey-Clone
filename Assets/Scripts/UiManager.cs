using UnityEngine;

// Singleton class used to manage UI elements, such as the exploration panel, battle panel, and pause menu. This class can be expanded to include methods for updating UI elements based on game state changes, handling user interactions with the UI, and coordinating with other systems like the GameManager and AudioManager.
public class UiManager : MonoBehaviour
{
    private static UiManager instance;

    // Property to access the singleton instance of UiManager. If it doesn't exist, it will attempt to find one in the scene or create a new one from a prefab.

    public static UiManager Instance
    {        
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<UiManager>();
                if (instance == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("GameRoot");
                    if (prefab != null)
                    {
                        GameObject obj = Instantiate(prefab);
                        instance = obj.GetComponent<UiManager>();
                    }
                    else
                    {
                        Debug.Log("Can't find GameRoot prefab in Resources folder! Please create one and add the UiManager component to it.");
                    }
                }
            }
            return instance;
        }
    }

    [Header("UI Panels")]
    [SerializeField] private GameObject explorationPanel;
    [SerializeField] private GameObject battlePanel;
    [SerializeField] private GameObject menuPanel;

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

    public void SetTitleUI()
    {
        explorationPanel.SetActive(false);
        battlePanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    public void SetExplorationHUD()
    {
        explorationPanel.SetActive(true);
        battlePanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    public void SetBattleHUD()
    {
        explorationPanel.SetActive(false);
        battlePanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void SetMenuUI()
    {
        explorationPanel.SetActive(false);
        battlePanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void SetGameOverUI()
    {
        explorationPanel.SetActive(false);
        battlePanel.SetActive(false);
        menuPanel.SetActive(false);
    }
}
