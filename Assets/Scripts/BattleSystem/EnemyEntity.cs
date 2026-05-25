using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyEntity : BattleEntity
{
    public void Initialize(EnemyProfile profile)
    {
        entityName = profile.EnemyName;
        maxHealth = profile.MaxHealth;
        currentHealth = maxHealth; // Start at full health
        attackPower = profile.AttackPower;
        speed = profile.Speed;
    }

    public override CombatAction CalculateTurnAction(List<BattleEntity> players)
    {
        if (players == null || players.Count == 0) return null;

        // Placeholder for more complex AI decision making, for now just basic attack a random player in the list
        BattleEntity target = players[UnityEngine.Random.Range(0, players.Count)];

        CombatAction plannedMove = new CombatAction();
        plannedMove.actionName = "Basic Strike";
        plannedMove.user = this;
        plannedMove.target = target;
        plannedMove.speed = this.speed;

        plannedMove.ExecuteActionLogic = () => target.TakeDamage(attackPower);

        return plannedMove;
    }
}
