using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatProfile", menuName = "Scriptable Objects/CombatProfile")]
public abstract class CombatProfile : ScriptableObject
{
    [Header("Base Identity")]
    [SerializeField] protected string entityName;

    [Header("Shared Combat Stats")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected int attackPower;
    [SerializeField] protected int speed;   // This can be used to determine turn order in battle

    public event Action OnDeath; // Event triggered when an entity dies

    // Public properties to access the private fields
    public string EntityName => entityName;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int AttackPower => attackPower;
    public int Speed => speed;

    public bool IsAlive => currentHealth > 0;

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
