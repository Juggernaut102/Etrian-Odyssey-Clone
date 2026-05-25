using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BattleEntity : MonoBehaviour
{
    [SerializeField] private string entityName;
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private int attackPower;
    [SerializeField] private int speed;

    public string EntityName => entityName;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int AttackPower => attackPower;
    public int Speed => speed;

    public void Initialize(EnemyProfile profile)
    {
        entityName = profile.EnemyName;
        maxHealth = profile.MaxHealth;
        currentHealth = maxHealth; // Start at full health
        attackPower = profile.AttackPower;
        speed = profile.Speed;
    }

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

    public CombatAction CalculateEnemyAction(List<BattleEntity> players)
    {
        if (players == null || players.Count == 0) return null;

        // Placeholder for more complex AI decision making, for now just basic attack a random player in the list
        BattleEntity player = players[UnityEngine.Random.Range(0, players.Count)];

        CombatAction plannedMove = new CombatAction();
        plannedMove.actionName = "Basic Strike";
        plannedMove.user = this;
        plannedMove.target = player;
        plannedMove.speed = this.speed;

        plannedMove.ExecuteActionLogic = () => plannedMove.target.TakeDamage(attackPower);

        return plannedMove;
    }
}
