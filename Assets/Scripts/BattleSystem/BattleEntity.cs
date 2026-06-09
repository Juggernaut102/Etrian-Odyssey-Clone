using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// This class is a concrete instantiation of the CombatProfile class, which is used to represent both the player and enemies in battle. It contains all the necessary stats and methods for calculating actions during combat.
// By making it abstract, we can create specific implementations for the player and enemies while still sharing common functionality.
public abstract class BattleEntity : MonoBehaviour
{
    [SerializeReference] private CombatRuntimeData combatData;
    public CombatRuntimeData CombatData => combatData;

    private void OnDisable()
    {
        if (combatData != null)
        {
            combatData.OnDeath -= HandleDeath;
        }
    }

    // The BattleManager calls this to inject the data when the scene loads
    public void SetUpEntity(CombatRuntimeData data)
    {
        combatData = data;
        combatData.OnDeath += HandleDeath;
    }

    public bool IsAlive() => combatData != null && combatData.IsAlive;

    protected virtual void HandleDeath()
    {
        // Additional death logic can be added here, such as playing an animation, dropping loot, etc.
        Debug.Log($"[VISUAL] {combatData.EntityName}'s body is dying.");
    }

    public void TakeDamage(int damage)
    {
        combatData.TakeDamage(damage);
    }
}
