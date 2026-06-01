using UnityEngine;

// This class represents a combat action that can be taken by either the player or an enemy during a battle.
// It encapsulates all the necessary information about the action, such as the type of action (attack, defend, use item, etc.), the target of the action, and any relevant parameters (damage amount, status effects, etc.).
// This class can be expanded to include additional properties and methods as needed to support various types of combat actions and their effects on the game state.
[System.Serializable]
public class CombatAction
{
    public string actionName;
    public int speed; // Determines the order of actions during the resolving phase (higher speed goes first)
    public BattleEntity user;
    public BattleEntity target;
    public System.Action ExecuteActionLogic; // Delegate to hold the logic for executing this action, which can be defined when creating the CombatAction instance
}
