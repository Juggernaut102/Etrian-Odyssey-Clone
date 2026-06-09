using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterRuntimeData : CombatRuntimeData
{
    [SerializeField] private int currentLevel;
    [SerializeField] private int currentXP;
    public int CurrentLevel => currentLevel;
    public int CurrentXP => currentXP;

    public override void Initialize(CombatProfile profile)
    {
        base.Initialize(profile);
        if (profile is CharacterProfile character)
        {
            currentLevel = character.StartingLevel;
            currentXP = character.StartingXP;
        }

    }
}
