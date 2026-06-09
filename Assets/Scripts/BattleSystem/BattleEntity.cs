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
    protected CombatProfile combatProfile;

    public string EntityName => entityName;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int AttackPower => attackPower;
    public int Speed => speed;

    private void OnEnable()
    {
        combatProfile.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        combatProfile.OnDeath -= HandleDeath;
    }

    public virtual void Initialize(CombatProfile profile)
    {
        combatProfile = profile;
    }

    public bool IsAlive() => combatProfile != null && combatProfile.IsAlive;

    protected virtual void HandleDeath()
    {
        // Additional death logic can be added here, such as playing an animation, dropping loot, etc.
        Debug.Log($"[VISUAL] {entityName}'s body is dying.");
    }

    public void TakeDamage(int damage)
    {
        combatProfile.TakeDamage(damage);
    }

    // We use IEnumerable here to allow for flexible input of opponents, whether it is PlayerEntity or EnemyEntity, as long as they are BattleEntities
    // Only for read-only methods
    public abstract CombatAction CalculateTurnAction(IEnumerable<BattleEntity> opponents);
}
