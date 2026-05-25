using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class PlayerEntity : BattleEntity
{
    // to implement when Playerprofile is ready
    /*
    public void Initialize(PlayerProfile profile)
    {
        entityName = profile.EnemyName;
        maxHealth = profile.MaxHealth;
        currentHealth = maxHealth; // Start at full health
        attackPower = profile.AttackPower;
        speed = profile.Speed;
    }
    */

    public override CombatAction CalculateTurnAction(List<BattleEntity> enemies)
    {
        return null; // Player action is determined by player input, so this method can return null or be left unimplemented
    }
}
