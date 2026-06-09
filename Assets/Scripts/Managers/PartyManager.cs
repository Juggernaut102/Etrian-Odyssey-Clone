using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Singleton manager responsible for handling the player's party of characters, including their stats, inventory, and other related data.
// It ensures that the party information persists across scenes and can be accessed globally throughout the game.
public class PartyManager : MonoBehaviour
{
    private static PartyManager instance;

    // Property to access the singleton instance of PartyManager. If it doesn't exist, it will attempt to find one in the scene or create a new one from a prefab.
    public static PartyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PartyManager>();
                if (instance == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("GameRoot");
                    if (prefab != null)
                    {
                        GameObject obj = Instantiate(prefab);
                        instance = obj.GetComponent<PartyManager>();
                    }
                    else
                    {
                        Debug.Log("Can't find GameRoot prefab in Resources folder! Please create one and add the PartyManager component to it.");
                    }
                }
            }
            return instance;
        }
    }

    [Header("Party Configuration")]
    [SerializeField] private CharacterProfile[] startingPartyBlueprints;    // assign in inspector with the base character profiles for the starting party members
    [SerializeField] private int maxPartySize = 5; // Maximum number of characters allowed in the party
    [SerializeField] private List<CharacterRuntimeData> livePartyData;
    public List<CharacterRuntimeData> LivePartyData => livePartyData;
    public int AlivePartyCount => livePartyData.Count(data => data.IsAlive);
    private int currentGold = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes

            InitializeParty();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeParty()
    {
        // Initialize the empty list
        livePartyData = new List<CharacterRuntimeData>();

        for (int i = 0; i < startingPartyBlueprints.Length; i++)
        {
            // Safety check: Don't exceed max party size during setup!
            if (startingPartyBlueprints[i] != null && livePartyData.Count < maxPartySize)
            {
                CharacterRuntimeData newData = new CharacterRuntimeData();
                newData.Initialize(startingPartyBlueprints[i]);
                newData.OnDeath += CheckForPartyWipe;
                livePartyData.Add(newData);
            }
        }
    }

    public void RecruitCharacter(CharacterProfile newRecruitBlueprint)
    {
        if (livePartyData.Count < maxPartySize)
        {
            CharacterRuntimeData newData = new CharacterRuntimeData();
            newData.Initialize(newRecruitBlueprint);
            newData.OnDeath += CheckForPartyWipe;
            livePartyData.Add(newData);
            Debug.Log($"{newData.EntityName} joined the party!");
        }
        else
        {
            Debug.Log("The party is full!");
        }
    }

    public void AssignXP(int totalXPAmount)
    {
        int aliveCount = AlivePartyCount;

        if (aliveCount == 0) return;
        
        int splitXP = totalXPAmount / aliveCount;
        foreach (CharacterRuntimeData heroData in livePartyData)
        {
            if (heroData != null && heroData.IsAlive)
            {
                heroData.GainXP(splitXP);
                Debug.Log($"{heroData.EntityName} earned {splitXP} XP!");
            }
        }
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        Debug.Log($"Party found {amount} Gold.");
    }

    private void CheckForPartyWipe()
    {
        if (AlivePartyCount == 0)
        {
            Debug.Log("All party members have fallen! Game Over.");
            GameManager.Instance.GameOver();
        }
    }
}
