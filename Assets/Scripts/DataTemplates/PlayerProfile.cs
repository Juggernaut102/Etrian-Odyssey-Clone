using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfile", menuName = "Scriptable Objects/PlayerProfile")]
public class PlayerProfile : CombatProfile
{
    [Header("Player-Specific Data")]
    [SerializeField] private int currentLevel;
    [SerializeField] private int currentXP;

    public int CurrentLevel => currentLevel;
    public int CurrentXP => currentXP;
}
