using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// This class is a concrete instantiation of the CombatProfile class, which is used to represent both the player and enemies in battle. It contains all the necessary stats and methods for calculating actions during combat.
// By making it abstract, we can create specific implementations for the player and enemies while still sharing common functionality.
public abstract class BattleEntity : MonoBehaviour
{
    // SerializeField not needed?
    [SerializeField] protected string entityName;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int attackPower;
    [SerializeField] protected int speed;

    public string EntityName => entityName;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int AttackPower => attackPower;
    public int Speed => speed;

    public abstract void Initialize(CombatProfile profile);

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth = Mathf.Max(0, currentHealth - dmg);
        Debug.Log($"Damage taken by {entityName}: {dmg}");
        if (!IsAlive())
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log($"{entityName} has died.");
        // Additional death logic can be added here, such as playing an animation, dropping loot, etc.
    }

    // We use IEnumerable here to allow for flexible input of opponents, whether it is PlayerEntity or EnemyEntity, as long as they are BattleEntities
    // Only for read-only methods
    public abstract CombatAction CalculateTurnAction(IEnumerable<BattleEntity> opponents);
}
