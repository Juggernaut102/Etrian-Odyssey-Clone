using UnityEngine;

[CreateAssetMenu(fileName = "EnemyProfile", menuName = "Scriptable Objects/EnemyProfile")]
public class EnemyProfile : CombatProfile
{
    [Header("Enemy-Specific Data")]
    [SerializeField] private int xpReward;
    [SerializeField] private int goldReward;

    public int XpReward => xpReward;
    public int GoldReward => goldReward;
}
