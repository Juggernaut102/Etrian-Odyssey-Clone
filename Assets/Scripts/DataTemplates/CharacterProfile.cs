using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfile", menuName = "Scriptable Objects/CombatProfile/PlayerProfile")]
public class CharacterProfile : CombatProfile
{
    [Header("Character-Specific Data")]
    [SerializeField] private int currentLevel;
    [SerializeField] private int currentXP;

    public int CurrentLevel => currentLevel;
    public int CurrentXP => currentXP;
}
