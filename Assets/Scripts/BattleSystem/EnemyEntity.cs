using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyEntity : BattleEntity, IAIControlled
{
    protected override void HandleDeath()
    {
        base.HandleDeath();
        Destroy(gameObject); // Destroy the enemy game object when it dies
    }

    public CombatAction CalculateTurnAction(IEnumerable<BattleEntity> players)
    {
        if (players == null || players.Count() == 0) return null;

        // Placeholder for more complex AI decision making, for now just basic attack a random player in the list
        BattleEntity target = players.ElementAt(UnityEngine.Random.Range(0, players.Count()));

        CombatAction plannedMove = new CombatAction();
        plannedMove.actionName = "Basic Strike";
        plannedMove.user = this;
        plannedMove.target = target;
        plannedMove.speed = CombatData.Speed;

        plannedMove.ExecuteActionLogic = () => target.TakeDamage(CombatData.AttackPower);

        return plannedMove;
    }
}
