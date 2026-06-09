using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfile", menuName = "Scriptable Objects/CombatProfile/PlayerProfile")]
public class CharacterProfile : CombatProfile
{
    [Header("Character-Specific Data")]
    [SerializeField] private int startingLevel;
    [SerializeField] private int startingXP;

    public int StartingLevel => startingLevel;
    public int StartingXP => startingXP;
}
