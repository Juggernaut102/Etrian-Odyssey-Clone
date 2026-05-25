using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.GraphicsBuffer;

public class PlayerEntity : BattleEntity
{
    public override void Initialize(CombatProfile profile)
    {
        if (profile is PlayerProfile character)
        {
            entityName = character.EntityName;
            maxHealth = character.MaxHealth;
            currentHealth = maxHealth; // Start at full health
            attackPower = character.AttackPower;
            speed = character.Speed;
        }
        
    }

    public override CombatAction CalculateTurnAction(List<BattleEntity> enemies)
    {
        return null; // Player action is determined by player input, so this method can return null or be left unimplemented
    }
}
