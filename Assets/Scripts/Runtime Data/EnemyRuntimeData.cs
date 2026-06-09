using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnemyRuntimeData : CombatRuntimeData
{
    [Header("Enemy-Specific Data")]
    [SerializeField] private int xpReward;
    [SerializeField] private int goldReward;

    public int XpReward => xpReward;
    public int GoldReward => goldReward;
    public override void Initialize(CombatProfile profile)
    {
        base.Initialize(profile);
        if (profile is EnemyProfile monster)
        {
            xpReward = monster.XpReward;
            goldReward = monster.GoldReward;
        }
    }
}
