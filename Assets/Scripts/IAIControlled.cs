using System.Collections.Generic;

public interface IAIControlled
{
    CombatAction CalculateTurnAction(IEnumerable<BattleEntity> opponents);
}