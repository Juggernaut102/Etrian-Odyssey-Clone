using System.Collections.Generic;
using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [Header("Encounter Configurations")]
    [SerializeField] private int meterLimit = 100;
    [SerializeField] private int maxIncrease = 15;
    [SerializeField] private int minIncrease = 5;
    [SerializeField] private List<EncounterProfile> enemyTroops = new List<EncounterProfile>();

    [SerializeField] private int currentMeter = 0; // SerializeField for debugging purposes, can be hidden in final version

    private void OnEnable()
    {
        GameManager.OnGlobalTurnTick += AddToEncounterMeter;
    }

    private void OnDisable()
    {
        GameManager.OnGlobalTurnTick -= AddToEncounterMeter;
    }

    private void AddToEncounterMeter()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Explore) return;

        int value = Random.Range(minIncrease, maxIncrease + 1);
        currentMeter += value;
        Debug.Log($"Current encounter meter increased by {value}, now at {currentMeter}/{meterLimit}");

        if (currentMeter >= meterLimit)
        {
            TriggerRandomEncounter();
        }
    }

    private void TriggerRandomEncounter()
    {
        currentMeter = 0;
        EncounterProfile enemyTroop = enemyTroops[Random.Range(0, enemyTroops.Count)];
        GameManager.Instance.EnterBattle(enemyTroop);
    }

}
