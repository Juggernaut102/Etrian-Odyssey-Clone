using UnityEngine;

[CreateAssetMenu(fileName = "EnemyProfile", menuName = "Scriptable Objects/EnemyProfile")]
public class EnemyProfile : CombatProfile
{
    [Header("Visual Assets")]
    [SerializeField] private GameObject enemyPrefabLayout;

    [Header("Enemy-Specific Data")]
    [SerializeField] private int xpReward;
    [SerializeField] private int goldReward;

    public GameObject EnemyPrefabLayout => enemyPrefabLayout;
    public int XpReward => xpReward;
    public int GoldReward => goldReward;
}
