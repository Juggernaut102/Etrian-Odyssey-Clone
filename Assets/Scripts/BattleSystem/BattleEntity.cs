using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class BattleEntity : MonoBehaviour
{
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

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth = Mathf.Max(0, currentHealth - dmg);
        Debug.Log($"Damage taken by {entityName}: {dmg}");
    }

    public void PerformBasicAttack(BattleEntity target)
    {
        Debug.Log($"{entityName} attacks {target.EntityName} for {attackPower} damage!");
        target.TakeDamage(attackPower);
    }

    public abstract CombatAction CalculateTurnAction(List<BattleEntity> opponents);
}
