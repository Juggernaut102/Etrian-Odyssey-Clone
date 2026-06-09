using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class CombatRuntimeData
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

    public event Action OnDeath; // Event triggered when an entity dies

    public bool IsAlive => currentHealth > 0;

    public virtual void Initialize(CombatProfile profile)
    {
        entityName = profile.EntityName;
        maxHealth = profile.MaxHealth;
        currentHealth = profile.MaxHealth; // Start at full HP!
        attackPower = profile.AttackPower;
        speed = profile.Speed;
    }

    public void TakeDamage(int dmg)
    {
        if (!IsAlive) return;

        currentHealth = Mathf.Max(0, currentHealth - dmg);
        Debug.Log($"Damage taken by {entityName}: {dmg}");

        if (!IsAlive)
        {
            Debug.Log($"{entityName} has died.");
            OnDeath?.Invoke();
        }
    }
}
